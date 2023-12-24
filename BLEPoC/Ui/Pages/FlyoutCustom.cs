using Microsoft.Maui.Devices;
namespace BLEPoC.Ui.Pages;

internal class FlyoutCustom : FlyoutPage
{
	internal FlyoutCustom(string title = "")
	{
		_ = new LifeCycleTracing(this, title);

		Flyout = new MainPage(null, true, true, $"Flyout({title})");
		Detail = new SecondPage(true, true, $"Detail({title})");

		PropertyChanged += (_, propertyChangedEventHandler) =>
		{
			if (propertyChangedEventHandler.PropertyName == nameof(FlyoutLayoutBehavior))
			{
				if (DeviceInfo.Platform == DevicePlatform.WinUI && FlyoutLayoutBehavior is not (FlyoutLayoutBehavior.Popover or FlyoutLayoutBehavior.SplitOnPortrait))
					IsPresented = true;

				if (Detail is SecondPage secondPage)
					secondPage.UpdateLabel2($"{nameof(FlyoutLayoutBehavior)}:{FlyoutLayoutBehavior}{Environment.NewLine}{nameof(IsPresented)}:{IsPresented}");
			}
		};

		IsPresentedChanged += (_, _) =>
		{
			if (Detail is SecondPage secondPage)
				secondPage.UpdateLabel2($"{nameof(FlyoutLayoutBehavior)}:{FlyoutLayoutBehavior}{Environment.NewLine}{nameof(IsPresented)}:{IsPresented}");
		};

		Flyout.Focused += (_, __) =>
		{
			if (DeviceInfo.Platform == DevicePlatform.WinUI && FlyoutLayoutBehavior is FlyoutLayoutBehavior.Popover or FlyoutLayoutBehavior.SplitOnPortrait)
				IsPresented = true;
		};

		Flyout.Unfocused += (_, __) =>
		{
			if (DeviceInfo.Platform == DevicePlatform.WinUI && FlyoutLayoutBehavior is FlyoutLayoutBehavior.Popover or FlyoutLayoutBehavior.SplitOnPortrait)
				IsPresented = false;
		};
	}
}