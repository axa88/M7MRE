using System.Diagnostics;

using static Microsoft.Maui.ApplicationModel.Permissions;


namespace BLEPoC.Permissions;

internal sealed class PermissionsProcessor
{
	private readonly Dictionary<Window, SubbedProperties> _subscribedWindows = [];
	private bool _permissionGranted;

	// Explicit static constructor tells compiler not to mark type as beforefieldinit https://csharpindepth.com/Articles/BeforeFieldInit
	// no more need? https://github.com/dotnet/runtime/issues/4346
	//static PermissionsProcessor() { }

	// Explicit private constructor prevents a new instance from being created anyplace else
	private PermissionsProcessor() { }

	internal event EventHandler<PermissionEventArgs> PermissionsChanged;

	internal static PermissionsProcessor Instance { get; } = new();

	internal bool DiscretePopups { get; set; } = true;

	internal bool PermissionGranted
	{
		get => _permissionGranted;
		private set
		{
			if (_permissionGranted != value)
			{
				_permissionGranted = value;
				PermissionsChanged?.Invoke(this, new PermissionEventArgs { PermissionGranted = _permissionGranted });
			}
		}
	}

	internal void Request(Window window)
	{
		if (_subscribedWindows.TryAdd(window, new SubbedProperties(window, OnWindowResumedTimeout)))
		{
			window.Resumed += OnWindowResumed;
			window.Deactivated += OnWindowDeactivated;

			if (DeviceInfo.Platform != DevicePlatform.Android)
				window.Destroying += OnWindowDestroying;
		}

		OnWindowResumed(window);
	}

	internal void Subscribe(Page page)
	{
		if (page.Window == null)
			throw new NullReferenceException($"Cannot {nameof(Subscribe)} {page.Title} to {nameof(PermissionsProcessor)}, as the page is not assigned to a {nameof(page.Window)}");

		var window = page.Window;
		if (_subscribedWindows.TryGetValue(window, out var windowProperties))
		{
			if (!windowProperties.Pages.Contains(page))
				windowProperties.Pages.Add(page);
		}
		else if (_subscribedWindows.TryAdd(window, new SubbedProperties(page, OnWindowResumedTimeout)))
		{
			window.Resumed += OnWindowResumed;
			window.Deactivated += OnWindowDeactivated;

			if (DeviceInfo.Platform != DevicePlatform.Android)
				window.Destroying += OnWindowDestroying;
		}
	}

	internal void Unsubscribe(Page page)
	{
		if (page.Window == null)
			throw new NullReferenceException($"Cannot {nameof(Unsubscribe)} {page.Title} from {nameof(PermissionsProcessor)}, as the page is not assigned to a {nameof(page.Window)}");

		if (_subscribedWindows.TryGetValue(page.Window, out var windowProperties))
		{
			windowProperties.Pages.Remove(page);
			Unsubscribe(page.Window, windowProperties);
		}
	}

	internal async ValueTask UnsubscribeAsync(Page page)
	{
		if (page.Window == null)
			throw new NullReferenceException($"Cannot {nameof(Unsubscribe)} {page.Title} from {nameof(PermissionsProcessor)}, as the page is not assigned to a {nameof(page.Window)}");

		if (_subscribedWindows.TryGetValue(page.Window, out var windowProperties))
		{
			windowProperties.Pages.Remove(page);
			await UnsubscribeAsync(page.Window, windowProperties);
		}
	}

	private void OnWindowResumed(object window, EventArgs _ = null)
	{
		SubbedProperties windowProperties;

		if (DiscretePopups)
		{
			// popup if resumed window has a subbed page, and all pages on RESUMED window are not in progress
			if (_subscribedWindows.TryGetValue((Window)window, out windowProperties) && !windowProperties.InProgress)
				windowProperties.ResumedTimer.Change(TimeSpan.FromSeconds(.5), Timeout.InfiniteTimeSpan);
		}
		else
		{
			// popup if resumed window has a subbed page, and all pages on ALL windows are not in progress
			if (_subscribedWindows.TryGetValue((Window)window, out windowProperties) && _subscribedWindows.Values.All(subscribedWindow => !subscribedWindow.InProgress))
				windowProperties.ResumedTimer.Change(TimeSpan.FromSeconds(.5), Timeout.InfiniteTimeSpan);
		}
	}

	private void OnWindowDeactivated(object window, EventArgs _)
	{
		if (_subscribedWindows.TryGetValue((Window)window, out var windowProperties))
			windowProperties.ResumedTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
	}

	// ReSharper disable once MemberCanBePrivate.Global
	internal void OnWindowDestroying(object sender, EventArgs _ = null)
	{
		// ReSharper disable once JoinDeclarationAndInitializer
		Window window;
		#if ANDROID
		window = _subscribedWindows.FirstOrDefault(pair => pair.Value.PlatformSpecificWindowIdentifier == sender).Key;
		#else
		window = (Window)sender;
		#endif

		if (window != null && _subscribedWindows.TryGetValue(window, out var windowProperties))
		{
			windowProperties.Pages.Clear();
			Unsubscribe(window, windowProperties);
		}
	}

	private void Unsubscribe(Window window, SubbedProperties windowProperties)
	{
		if (!windowProperties.Pages.Any())
		{
			window.Resumed -= OnWindowResumed;

			windowProperties.ResumedTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
			windowProperties.ResumedTimer.Dispose();
			_subscribedWindows.Remove(window);
			windowProperties.Cts.Cancel();
			windowProperties.Cts.Dispose();

			window.Deactivated -= OnWindowDeactivated;
		}
	}

	private async ValueTask UnsubscribeAsync(Window window, SubbedProperties windowProperties)
	{
		if (!windowProperties.Pages.Any())
		{
			window.Resumed -= OnWindowResumed;

			windowProperties.ResumedTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
			await windowProperties.ResumedTimer.DisposeAsync();
			_subscribedWindows.Remove(window);
			windowProperties.Cts.Cancel();
			windowProperties.Cts.Dispose();

			window.Deactivated -= OnWindowDeactivated;
		}
	}

	private async void OnWindowResumedTimeout(object window)
	{
		if (window is Window w && _subscribedWindows.TryGetValue(w, out var windowProperties) && w.Page != null)
		{
			var page = w.Page switch
			{
				ContentPage contentPage when windowProperties.Pages.Contains(contentPage) => contentPage,
				NavigationPage navigationPage when windowProperties.Pages.Contains(navigationPage.CurrentPage) => navigationPage.CurrentPage,
				TabbedPage tabbedPage when windowProperties.Pages.Contains(tabbedPage.CurrentPage) => tabbedPage.CurrentPage,

				// @formatter:off — disable formatter after this line
				// cuz WinUI IsPresented is broken bug https://github.com/dotnet/maui/issues/11785
				#if WINDOWS
				FlyoutPage { Flyout.IsFocused: true } flyoutPage when windowProperties.Pages.Contains(flyoutPage.Flyout) => flyoutPage.Flyout,
				FlyoutPage { Detail.IsFocused: true } flyoutPage when windowProperties.Pages.Contains(flyoutPage.Detail) => flyoutPage.Detail, // https://github.com/dotnet/maui/issues/13496#issuecomment-1718331707
				FlyoutPage { Flyout.IsFocused: false, Detail.IsFocused: false } flyoutPage when windowProperties.Pages.Contains(flyoutPage.Detail) => flyoutPage.Detail,
				#else
				FlyoutPage { IsPresented: true } flyoutPage when windowProperties.Pages.Contains(flyoutPage.Flyout) => flyoutPage.Flyout,
				FlyoutPage { IsPresented: false } flyoutPage when windowProperties.Pages.Contains(flyoutPage.Detail) => flyoutPage.Detail,
				#endif
				// @formatter:on — enable formatter after this line
				_ => null
			};

			if (page != null)
			{
				_subscribedWindows[w].InProgress = true; // in progress

				try
				{
					await MainThread.InvokeOnMainThreadAsync(DeviceInfo.Platform == DevicePlatform.Android
						? DeviceInfo.Version.Major switch
						{
							>= 12 => () => RequestPermissions<BluetoothPermissions>(page, windowProperties.Cts.Token),
							> 9 => () => RequestPermissions<FineLocationPermissions>(page, windowProperties.Cts.Token),
							_ => () => RequestPermissions<CoarseLocationPermissions>(page, windowProperties.Cts.Token)
						}
						: () => RequestPermissions<LocationPermissions>(page, windowProperties.Cts.Token));
				}
				catch (OperationCanceledException exception)
				{
					Trace.WriteLine($"{w.Title}:{exception.Message}");
				}
				catch (PermissionException exception)
				{
					Trace.WriteLine(exception.Message);
					var exit = await MainThread.InvokeOnMainThreadAsync(() => page.DisplayAlert("Critical Permission(s) Error", "Manifest missing permission definition(s)", "Exit", "Fail"));
					if (exit)
						Application.Current?.Quit();
				}
				finally
				{
					if (_subscribedWindows.TryGetValue(w, out var subscribedWindow) && subscribedWindow.Pages.Contains(page))
						subscribedWindow.InProgress = false;
				}
			}
		}
	}

	private async Task RequestPermissions<T>(Page page, CancellationToken cancellationToken) where T : BasePlatformPermission, IPermissionPrompts, new()
	{
		var permissions = new T();

		permissions.EnsureDeclared();

		PermissionGranted = await CheckStatusAsync<T>() == PermissionStatus.Granted;
		if (!PermissionGranted)
		{
			if (ShouldShowRationale<T>())
				await permissions.DisplayRationalAlert(page);

			PermissionGranted = await RequestAsync<T>() == PermissionStatus.Granted;
			if (!PermissionGranted)
			{
				if (await permissions.DisplaySettingsAlert(page).WaitAsync(cancellationToken))
					AppInfo.Current.ShowSettingsUI();
			}
		}
	}


	private class SubbedProperties
	{
		internal readonly Timer ResumedTimer;
		internal readonly CancellationTokenSource Cts;
		internal readonly List<Page> Pages;
		internal bool InProgress;
		#if ANDROID
		internal readonly object PlatformSpecificWindowIdentifier;
		#endif

		internal SubbedProperties(Page page, TimerCallback timerCallback)
		{
			ResumedTimer = new(timerCallback, page.Window, Timeout.Infinite, Timeout.Infinite);
			Cts = new CancellationTokenSource();
			Pages = new List<Page> { page };
			#if ANDROID
			PlatformSpecificWindowIdentifier = Platform.CurrentActivity;
			#endif
		}

		public SubbedProperties(Window window, TimerCallback timerCallback)
		{
			ResumedTimer = new(timerCallback, window, Timeout.Infinite, Timeout.Infinite);
			Cts = new CancellationTokenSource();
			Pages = [];
			#if ANDROID
			PlatformSpecificWindowIdentifier = Platform.CurrentActivity;
			#endif
		}
	}
}


public class PermissionEventArgs : EventArgs
{
	public bool PermissionGranted { get; set; }
}
