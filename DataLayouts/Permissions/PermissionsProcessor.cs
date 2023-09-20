using System.Diagnostics;

using static Microsoft.Maui.ApplicationModel.Permissions;


namespace DataLayouts.Permissions;

internal static class PermissionsProcessor
{
	private static readonly Dictionary<Window, SubbedProperties> SubscribedWindows = new();

	internal static bool DiscretePopups { get; set; } = true;

	internal static void Request(Window window)
	{
		if (SubscribedWindows.TryAdd(window, new SubbedProperties(window, OnWindowResumedTimeout)))
		{
			window.Resumed += OnWindowResumed;
			window.Deactivated += OnWindowDeactivated;

			if (DeviceInfo.Platform != DevicePlatform.Android)
				window.Destroying += OnWindowDestroying;
		}

		OnWindowResumed(window);
	}

	internal static void Subscribe(Page page)
	{
		var window = page.Window;
		if (SubscribedWindows.TryGetValue(window, out var windowProperties))
		{
			if (!windowProperties.Pages.Contains(page))
				windowProperties.Pages.Add(page);
		}
		else if (SubscribedWindows.TryAdd(window, new SubbedProperties(page, OnWindowResumedTimeout)))
		{
			window.Resumed += OnWindowResumed;
			window.Deactivated += OnWindowDeactivated;

			if (DeviceInfo.Platform != DevicePlatform.Android)
				window.Destroying += OnWindowDestroying;
		}
	}

	internal static void Unsubscribe(Page page)
	{
		if (SubscribedWindows.TryGetValue(page.Window, out var windowProperties))
		{
			windowProperties.Pages.Remove(page);
			Unsubscribe(page.Window, windowProperties);
		}
	}

	internal static async ValueTask UnsubscribeAsync(Page page)
	{
		if (SubscribedWindows.TryGetValue(page.Window, out var windowProperties))
		{
			windowProperties.Pages.Remove(page);
			await UnsubscribeAsync(page.Window, windowProperties);
		}
	}

	private static void OnWindowResumed(object window, EventArgs _ = null)
	{
		SubbedProperties windowProperties;

		if (DiscretePopups)
		{
			// resumed window has a subbed page, and all pages on RESUMED window not in progress
			if (SubscribedWindows.TryGetValue((Window)window, out windowProperties) && !windowProperties.InProgress)
				windowProperties.ResumedTimer.Change(TimeSpan.FromSeconds(.5), Timeout.InfiniteTimeSpan);
		}
		else
		{
			// resumed window has a subbed page, and all pages on ALL windows not in progress
			if (SubscribedWindows.TryGetValue((Window)window, out windowProperties) && SubscribedWindows.Values.All(subscribedWindow => !subscribedWindow.InProgress))
				windowProperties.ResumedTimer.Change(TimeSpan.FromSeconds(.5), Timeout.InfiniteTimeSpan);
		}
	}

	private static void OnWindowDeactivated(object window, EventArgs _)
	{
		if (SubscribedWindows.TryGetValue((Window)window, out var windowProperties))
			windowProperties.ResumedTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
	}

	// ReSharper disable once MemberCanBePrivate.Global
	internal static void OnWindowDestroying(object sender, EventArgs _ = null)
	{
		// ReSharper disable once JoinDeclarationAndInitializer
		Window window;
	#if ANDROID
		window = SubscribedWindows.FirstOrDefault(pair => pair.Value.PlatformSpecificWindowIdentifier == sender).Key;
	#else
		window = (Window)sender;
	#endif

		if (window != null && SubscribedWindows.TryGetValue(window, out var windowProperties))
		{
			windowProperties.Pages.Clear();
			Unsubscribe(window, windowProperties);
		}
	}

	private static void Unsubscribe(Window window, SubbedProperties windowProperties)
	{
		if (!windowProperties.Pages.Any())
		{
			window.Resumed -= OnWindowResumed;

			windowProperties.ResumedTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
			windowProperties.ResumedTimer.Dispose();
			SubscribedWindows.Remove(window);
			windowProperties.Cts.Cancel();
			windowProperties.Cts.Dispose();

			window.Deactivated -= OnWindowDeactivated;
		}
	}

	private static async ValueTask UnsubscribeAsync(Window window, SubbedProperties windowProperties)
	{
		if (!windowProperties.Pages.Any())
		{
			window.Resumed -= OnWindowResumed;

			windowProperties.ResumedTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
			await windowProperties.ResumedTimer.DisposeAsync();
			SubscribedWindows.Remove(window);
			windowProperties.Cts.Cancel();
			windowProperties.Cts.Dispose();

			window.Deactivated -= OnWindowDeactivated;
		}
	}

	private static async void OnWindowResumedTimeout(object window)
	{
		if (window is Window w && SubscribedWindows.TryGetValue(w, out var windowProperties) && w.Page != null)
		{
			var page = w.Page switch
			{
				ContentPage contentPage when windowProperties.Pages.Contains(contentPage) => contentPage,
				NavigationPage navigationPage when windowProperties.Pages.Contains(navigationPage.CurrentPage) => navigationPage.CurrentPage,
				TabbedPage tabbedPage when windowProperties.Pages.Contains(tabbedPage.CurrentPage) => tabbedPage.CurrentPage,

				// cuz WinUI IsPresented is broken bug https://github.com/dotnet/maui/issues/11785
			#if WINDOWS
				FlyoutPage { Flyout.IsFocused: true } flyoutPage when windowProperties.Pages.Contains(flyoutPage.Flyout) => flyoutPage.Flyout,
				FlyoutPage { Detail.IsFocused: true } flyoutPage when windowProperties.Pages.Contains(flyoutPage.Detail) => flyoutPage.Detail,
				// https://github.com/dotnet/maui/issues/13496#issuecomment-1718331707
				FlyoutPage { Flyout.IsFocused: false, Detail.IsFocused: false } flyoutPage when windowProperties.Pages.Contains(flyoutPage.Detail) => flyoutPage.Detail,
			#else
				FlyoutPage { IsPresented: true } flyoutPage when windowProperties.Pages.Contains(flyoutPage.Flyout) => flyoutPage.Flyout,
				FlyoutPage { IsPresented: false } flyoutPage when windowProperties.Pages.Contains(flyoutPage.Detail) => flyoutPage.Detail,
			#endif
				_ => null
			};

			if (page != null)
			{
				SubscribedWindows[w].InProgress = true; // in progress

				try
				{
					await MainThread.InvokeOnMainThreadAsync(DeviceInfo.Platform == DevicePlatform.Android
						? DeviceInfo.Version.Major switch
						{
							>= 12 => () => RequestPermissions<BluetoothPermissions>(page, windowProperties.Cts.Token),
							> 9 => () => RequestPermissions<FineLocationPermissions>(page, windowProperties.Cts.Token),
							_ => () => RequestPermissions<CoarseLocationPermissions>(page, windowProperties.Cts.Token),
						}
						: () => RequestPermissions<LocationPermissions>(page, windowProperties.Cts.Token));
				}
				catch (OperationCanceledException exception) when (exception.CancellationToken.IsCancellationRequested)
				{
					Trace.WriteLine($"{w.Title}:{exception.Message}");
				}
				finally
				{
					if (SubscribedWindows.TryGetValue(w, out var subscribedWindow) && subscribedWindow.Pages.Contains(page))
						subscribedWindow.InProgress = false;
				}
			}
		}
	}

	private static async Task RequestPermissions<T>(Page page, CancellationToken cancellationToken) where T : BasePlatformPermission, IPermissionPrompts, new()
	{
		var permissions = new T();

		var status = await CheckStatusAsync<T>();
		if (status != PermissionStatus.Granted)
		{
			if (ShouldShowRationale<T>())
			{
				Trace.WriteLine($"{page.Title} Permissions.Rational");
				await permissions.DisplayRationalAlert(page);
			}

			status = await RequestAsync<T>();
			if (status != PermissionStatus.Granted)
			{
				Trace.WriteLine($"{page.Title} Permissions.Requesting");
				var choice = await permissions.DisplaySettingsAlert(page).WaitAsync(cancellationToken);
				if (choice)
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
			Pages = new List<Page>();
			#if ANDROID
			PlatformSpecificWindowIdentifier = Platform.CurrentActivity;
			#endif
		}
	}
}
