using System.ComponentModel;
using System.Diagnostics;

using Microsoft.Maui.LifecycleEvents;


namespace BLEPoC.Utility;

internal class LifeCycleTracing
{
	private static readonly object _lock = new();

	internal LifeCycleTracing(Page page, string id = null)
	{
		page.Title = string.IsNullOrWhiteSpace(id) ? page.GetType().ToString() : id;
		Trace.WriteLine($"{page.Title}.ctor");

		page.Appearing += (_, _) => TraceIt(page.Title, nameof(page.Appearing));
		page.Disappearing += (_, _) => TraceIt(page.Title, nameof(page.Disappearing));
		page.Focused += (_, focusEventArgs) => TraceIt(page.Title, nameof(page.Focused), focusEventArgs);
		page.Unfocused += (_, focusEventArgs) => TraceIt(page.Title, nameof(page.Unfocused), focusEventArgs);
		page.Loaded += (_, _) => TraceIt(page.Title, nameof(page.Loaded));
		page.Unloaded += (_, _) => TraceIt(page.Title, nameof(page.Unloaded));
	}

	internal LifeCycleTracing(Window window, string id = null)
	{
		window.Title = string.IsNullOrWhiteSpace(id) ? nameof(CustomWindow) : id;
		Trace.WriteLine($"{window.Title}.ctor");

		window.Created += (_, _) => TraceIt(window.Title, nameof(window.Created));
		window.Resumed += (_, _) => TraceIt(window.Title, nameof(window.Resumed));
		window.Activated += (_, _) => TraceIt(window.Title, nameof(window.Activated));
		window.Deactivated += (_, _) => TraceIt(window.Title, nameof(window.Deactivated));
		window.Stopped += (_, _) => TraceIt(window.Title, nameof(window.Stopped));
		window.Destroying += (_, _) => TraceIt(window.Title, nameof(window.Destroying));
	}

	internal LifeCycleTracing(Application application, string id = null)
	{
		var idd = string.IsNullOrWhiteSpace(id) ? application.GetType().ToString() : id;
		Trace.WriteLine($"{idd}.ctor");
		application.PageAppearing += (_, page) => TraceIt(idd, $"{nameof(application.PageAppearing)}:{page.Title}");
		application.PageDisappearing += (_, page) => TraceIt(idd, $"{nameof(application.PageDisappearing)}:{page.Title}");
	}

	internal LifeCycleTracing(MauiAppBuilder builder, string id = null)
	{
		builder.ConfigureLifecycleEvents(events =>
		{
			var idd = string.IsNullOrWhiteSpace(id) ? DeviceInfo.Platform.ToString() : id;

			#if ANDROID
			events.AddAndroid(lifecycleBuilder => lifecycleBuilder
				.OnCreate((_, _) =>TraceIt(idd, nameof(AndroidLifecycle.OnCreate)))
				.OnStart(_ => TraceIt(idd, nameof(AndroidLifecycle.OnStart)))
				.OnPause(_ => TraceIt(idd, nameof(AndroidLifecycle.OnPause)))
				.OnResume(_ => TraceIt(idd, nameof(AndroidLifecycle.OnResume)))
				.OnStop(_ => TraceIt(idd, nameof(AndroidLifecycle.OnStop)))
				.OnRestart(_ => TraceIt(idd, nameof(AndroidLifecycle.OnRestart)))
				.OnDestroy(_ => TraceIt(idd, nameof(AndroidLifecycle.OnDestroy)))
				.OnBackPressed(_ => TraceIt(idd, nameof(AndroidLifecycle.OnBackPressed)))
				.OnActivityResult((_, _, _, _) => TraceIt(idd, nameof(AndroidLifecycle.OnActivityResult)))
				.OnRequestPermissionsResult((_, _, _, _) => TraceIt(idd, nameof(AndroidLifecycle.OnActivityResult)))
			);
			#endif

			#if WINDOWS
			events.AddWindows(lifecycleBuilder => lifecycleBuilder
				.OnLaunching((_, _) => TraceIt(idd, nameof(WindowsLifecycle.OnLaunching)))
				.OnWindowCreated(_ => TraceIt(idd, nameof(WindowsLifecycle.OnWindowCreated)))
				.OnActivated((_, windowActivatedEventArgs) => TraceIt(idd, $"{nameof(WindowsLifecycle.OnActivated)}:{windowActivatedEventArgs.WindowActivationState}"))
				.OnLaunched((_, _) => TraceIt(idd, nameof(WindowsLifecycle.OnLaunched)))
				.OnResumed(_ => TraceIt(idd, nameof(WindowsLifecycle.OnResumed)))
				.OnClosed((_, _) => TraceIt(idd, nameof(WindowsLifecycle.OnClosed)))
				.OnPlatformMessage((_, windowsPlatformMessageEventArgs) =>
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
