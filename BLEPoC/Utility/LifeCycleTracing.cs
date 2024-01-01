using System.ComponentModel;
using System.Diagnostics;

using BLEPoC.Ui.Pages.Basic;

using Microsoft.Maui.LifecycleEvents;


namespace BLEPoC.Utility;

internal class LifeCycleTracing
{
	private static readonly object _lock = new();

	internal LifeCycleTracing(Page page, string id = null)
	{
		page.Title = string.IsNullOrWhiteSpace(id) ? page.GetType().ToString() : id;
		Trace.WriteLine($"{page.Title}.ctor");

		page.Appearing += (_, _) => TraceIt(GetHeader(), nameof(page.Appearing));
		page.Disappearing += (_, _) => TraceIt(GetHeader(), nameof(page.Disappearing));
		page.Focused += (_, focusEventArgs) => TraceIt(GetHeader(), nameof(page.Focused), focusEventArgs);
		page.Unfocused += (_, focusEventArgs) => TraceIt(GetHeader(), nameof(page.Unfocused), focusEventArgs);
		page.Loaded += (_, _) => TraceIt(GetHeader(), nameof(page.Loaded));
		page.Unloaded += (_, _) => TraceIt(GetHeader(), nameof(page.Unloaded));
		page.NavigatedFrom += (_, _) => TraceIt(GetHeader(), nameof(page.NavigatedFrom));
		page.NavigatedTo += (_, _) => TraceIt(GetHeader(), nameof(page.NavigatedTo));
		page.NavigatingFrom += (_, _) => TraceIt(GetHeader(), nameof(page.NavigatingFrom));

		switch (page)
		{
			case NavigationPageCustom navigationPage:
				//navigationPage.Pushed += (_, navigationEventArgs) => TraceIt(GetHeader(), $"{nameof(navigationPage.Pushed)}:{navigationEventArgs.Page.Title}");
				navigationPage.Pushed += (_, navigationEventArgs) => TraceIt(GetHeader(), nameof(navigationPage.Pushed), navigationEventArgs);
				navigationPage.Popped += (_, navigationEventArgs) => TraceIt(GetHeader(), nameof(navigationPage.Popped), navigationEventArgs);
				navigationPage.PoppedToRoot += (_, navigationEventArgs) => TraceIt(GetHeader(), nameof(navigationPage.PoppedToRoot), navigationEventArgs);
				// protected overrides
				navigationPage.BackButtonPressing += (_, traceEventArgs) => TraceIt(GetHeader(), traceEventArgs.EventName);
				break;
			case TabbedPageCustom tabbedPage:
				tabbedPage.CurrentPageChanged += (_, _) => TraceIt(GetHeader(), nameof(tabbedPage.CurrentPageChanged));
				tabbedPage.PagesChanged += (_, notifyCollectionChangedEventArgs) => TraceIt(GetHeader(), nameof(tabbedPage.PagesChanged), notifyCollectionChangedEventArgs);
				// protected overrides
				tabbedPage.BackButtonPressing += (_, traceEventArgs) => TraceIt(GetHeader(), traceEventArgs.EventName);
				break;
			case FlyoutPageCustom flyoutPage:
				// protected overrides
				flyoutPage.BackButtonPressing += (_, traceEventArgs) => TraceIt(GetHeader(), traceEventArgs.EventName);
				break;
		}
		string GetHeader() => $"{nameof(Page)} {page.Title}";
	}

	internal LifeCycleTracing(Window window, string id = null)
	{
		window.Title = string.IsNullOrWhiteSpace(id) ? nameof(WindowCustom) : id;
		Trace.WriteLine($"{window.Title}.ctor");

		window.Created += (_, _) => TraceIt(GetHeader(), nameof(window.Created));
		window.Resumed += (_, _) => TraceIt(GetHeader(), nameof(window.Resumed));
		window.Activated += (_, _) => TraceIt(GetHeader(), nameof(window.Activated));
		window.Deactivated += (_, _) => TraceIt(GetHeader(), nameof(window.Deactivated));
		window.Stopped += (_, _) => TraceIt(GetHeader(), nameof(window.Stopped));
		window.Destroying += (_, _) => TraceIt(GetHeader(), nameof(window.Destroying));

		string GetHeader() => $"{nameof(Window)} {window.Title}";
	}

	internal LifeCycleTracing(App application, string id = null)
	{
		var idd = string.IsNullOrWhiteSpace(id) ? application.GetType().ToString() : id;
		Trace.WriteLine($"{idd}.ctor");
		application.PageAppearing += (_, page) => TraceIt(GetHeader(), $"{nameof(application.PageAppearing)}:{nameof(Page)} {page.Title}"); // ToDo why is page.Title blank  // ToDo change parameter to obj and handle page?
		application.PageDisappearing += (_, page) => TraceIt(GetHeader(), $"{nameof(application.PageDisappearing)}:{nameof(Page)} {page.Title}"); // ToDo why is page.Title blank  // ToDo change parameter to obj and handle page?

		// protected overrides
		application.Starting += (_, traceEventArgs) => TraceIt(GetHeader(), traceEventArgs.EventName);
		application.Resuming += (_, traceEventArgs) => TraceIt(GetHeader(), traceEventArgs.EventName);
		application.Sleeping += (_, traceEventArgs) => TraceIt(GetHeader(), traceEventArgs.EventName);
		application.CreatingWindow += (_, traceEventArgs) => TraceIt(GetHeader(), traceEventArgs.EventName);

		string GetHeader() => $"{nameof(Application)} {idd}";
	}

	internal LifeCycleTracing(MauiAppBuilder builder, string id = null)
	{
		builder.ConfigureLifecycleEvents(events =>
		{
			var idd = string.IsNullOrWhiteSpace(id) ? DeviceInfo.Platform.ToString() : id;

			#if ANDROID
			events.AddAndroid(lifecycleBuilder => lifecycleBuilder
				.OnCreate((_, _) =>TraceIt(GetHeader(), nameof(AndroidLifecycle.OnCreate)))
				.OnStart(_ => TraceIt(GetHeader(), nameof(AndroidLifecycle.OnStart)))
				.OnPause(_ => TraceIt(GetHeader(), nameof(AndroidLifecycle.OnPause)))
				.OnResume(_ => TraceIt(GetHeader(), nameof(AndroidLifecycle.OnResume)))
				.OnStop(_ => TraceIt(GetHeader(), nameof(AndroidLifecycle.OnStop)))
				.OnRestart(_ => TraceIt(GetHeader(), nameof(AndroidLifecycle.OnRestart)))
				.OnDestroy(_ => TraceIt(GetHeader(), nameof(AndroidLifecycle.OnDestroy)))
				.OnBackPressed(_ => TraceIt(GetHeader(), nameof(AndroidLifecycle.OnBackPressed)))
				.OnActivityResult((_, _, _, _) => TraceIt(GetHeader(), nameof(AndroidLifecycle.OnActivityResult)))
				.OnRequestPermissionsResult((_, _, _, _) => TraceIt(GetHeader(), nameof(AndroidLifecycle.OnActivityResult)))
			);
			#endif

			#if WINDOWS
			events.AddWindows(lifecycleBuilder => lifecycleBuilder
				.OnLaunching((_, _) => TraceIt(GetHeader(), nameof(WindowsLifecycle.OnLaunching)))
				.OnWindowCreated(_ => TraceIt(GetHeader(), nameof(WindowsLifecycle.OnWindowCreated)))
				.OnActivated((_, windowActivatedEventArgs) => TraceIt(GetHeader(), $"{nameof(WindowsLifecycle.OnActivated)}:{windowActivatedEventArgs.WindowActivationState}"))
				.OnLaunched((_, _) => TraceIt(GetHeader(), nameof(WindowsLifecycle.OnLaunched)))
				.OnResumed(_ => TraceIt(GetHeader(), nameof(WindowsLifecycle.OnResumed)))
				.OnClosed((_, _) => TraceIt(GetHeader(), nameof(WindowsLifecycle.OnClosed)))
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
						TraceIt(GetHeader(), $"{nameof(WindowsLifecycle.OnPlatformMessage)}:{wm}");
				}));
			#endif

			string GetHeader() => $"{nameof(MauiProgram)} {idd}";
		});
	}

	private static bool TraceIt(object title, string @event, EventArgs args = null)
	{
		lock (_lock)
		{
			Trace.Write($"{title}:{@event}");

			if (args != null)
			{
				foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(args))
				{
					var value = descriptor.GetValue(args);

					Trace.Write(value switch
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

internal class TraceEventArgs(string eventName) : EventArgs
{
	public string EventName { get; } = eventName;
}