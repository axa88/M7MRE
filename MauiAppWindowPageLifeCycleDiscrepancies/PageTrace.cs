using System.ComponentModel;
using System.Diagnostics;

using Microsoft.Maui.LifecycleEvents;
using Microsoft.Maui.Devices;


namespace MauiAppWindowPageLifeCycleDiscrepancies;

public class PageTrace
{
	private static readonly object _lock = new();

	public PageTrace(Page page, string id = null)
	{
		page.Title = string.IsNullOrWhiteSpace(id) ? page.GetType().ToString() : id;
		Trace.WriteLine($"{page.Title}.ctor");

		page.Appearing += (_, __) => TraceIt(page.Title, nameof(page.Appearing));
		page.Disappearing += (_, __) => TraceIt(page.Title, nameof(page.Disappearing));
		page.Focused += (_, focusEventArgs) => TraceIt(page.Title, nameof(page.Focused), focusEventArgs);
		page.Unfocused += (_, focusEventArgs) => TraceIt(page.Title, nameof(page.Unfocused), focusEventArgs);
		page.Loaded += (_, __) => TraceIt(page.Title, nameof(page.Loaded));
		page.Unloaded += (_, __) => TraceIt(page.Title, nameof(page.Unloaded));
	}

	public PageTrace(Window window, string id = null)
	{
		window.Title = string.IsNullOrWhiteSpace(id) ? nameof(CustomWindow) : id;
		Trace.WriteLine($"{window.Title}.ctor");

		window.Created += (_, __) => TraceIt(window.Title, nameof(window.Created));
		window.Resumed += (_, __) => TraceIt(window.Title, nameof(window.Resumed));
		window.Activated += (_, __) => TraceIt(window.Title, nameof(window.Activated));
		window.Deactivated += (_, __) => TraceIt(window.Title, nameof(window.Deactivated));
		window.Stopped += (_, __) => TraceIt(window.Title, nameof(window.Stopped));
		window.Destroying += (_, __) => TraceIt(window.Title, nameof(window.Destroying));
	}

	public PageTrace(Application application, string id = null)
	{
		var idd = string.IsNullOrWhiteSpace(id) ? application.GetType().ToString() : id;
		Trace.WriteLine($"{idd}.ctor");
		application.PageAppearing += (_, page) => TraceIt(idd, $"{nameof(application.PageAppearing)}:{page.Title}");
		application.PageDisappearing += (_, page) => TraceIt(idd, $"{nameof(application.PageDisappearing)}:{page.Title}");
	}

	public PageTrace(MauiAppBuilder builder, string id = null)
	{
		builder.ConfigureLifecycleEvents(events =>
		{
			var idd = string.IsNullOrWhiteSpace(id) ? DeviceInfo.Platform.ToString() : id;

			#if ANDROID
			events.AddAndroid(lifecycleBuilder => lifecycleBuilder
				.OnCreate((activity, bundle) =>TraceIt(idd, nameof(AndroidLifecycle.OnCreate)))
				.OnStart(activity => TraceIt(idd, nameof(AndroidLifecycle.OnStart)))
				.OnPause(activity => TraceIt(idd, nameof(AndroidLifecycle.OnPause)))
				.OnResume(activity => TraceIt(idd, nameof(AndroidLifecycle.OnResume)))
				.OnStop(activity => TraceIt(idd, nameof(AndroidLifecycle.OnStop)))
				.OnRestart(activity => TraceIt(idd, nameof(AndroidLifecycle.OnRestart)))
				.OnDestroy(activity => TraceIt(idd, nameof(AndroidLifecycle.OnDestroy)))
				.OnBackPressed(activity => TraceIt(idd, nameof(AndroidLifecycle.OnBackPressed)))
				.OnActivityResult((activity, requestCode, resultCode, data) => TraceIt(idd, nameof(AndroidLifecycle.OnActivityResult)))
				.OnRequestPermissionsResult((activity, requestCode, permissions, grantResults) => TraceIt(idd, nameof(AndroidLifecycle.OnActivityResult)))
			);
			#endif

			#if WINDOWS
			events.AddWindows(lifecycleBuilder => lifecycleBuilder
				.OnLaunching((application, launchActivatedEventArgs) => TraceIt(idd, nameof(WindowsLifecycle.OnLaunching)))
				.OnWindowCreated(window => TraceIt(idd, nameof(WindowsLifecycle.OnWindowCreated)))
				.OnActivated((window, windowActivatedEventArgs) => TraceIt(idd, $"{nameof(WindowsLifecycle.OnActivated)}:{windowActivatedEventArgs.WindowActivationState}"))
				.OnLaunched((application, launchActivatedEventArgs) => TraceIt(idd, nameof(WindowsLifecycle.OnLaunched)))
				.OnResumed(window => TraceIt(idd, nameof(WindowsLifecycle.OnResumed)))
				.OnClosed((window, windowEventArgs) => TraceIt(idd, nameof(WindowsLifecycle.OnClosed)))
				.OnPlatformMessage((window, windowsPlatformMessageEventArgs) =>
				{
					// https://www.autoitscript.com/autoit3/docs/appendix/WinMsgCodes.htm
					// https://learn.microsoft.com/en-us/windows/win32/winmsg/window-notifications
					var wm = $"{windowsPlatformMessageEventArgs.MessageId:0000}" switch
					{
						"0002" => "WM_DESTROY",
						"0010" => "WM_ClOSE",
						_ => null
					};

					if (wm != null)
						TraceIt(idd, $"{nameof(WindowsLifecycle.OnPlatformMessage)}:{wm}");
				}));
			#endif
		});
	}

	private static bool TraceIt(object title, object @event, EventArgs args = null)
	{
		lock (_lock)
		{
			Trace.Write($"{title}:{@event}");

			if (args != null)
			{
				foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(args))
				{
					var value = descriptor.GetValue(args);

					Trace.Write((value) switch
					{
						Page p => $" {p.Title}",
						Window w => $" {w.Title}",
						_ => $" {descriptor.Name}:{value}"
					});
				}
			}

			Trace.WriteLine("");

			return true;
		}
	}
}
