using BLEPoC.Utility;

using static System.Reflection.MethodBase;


namespace BLEPoC.Ui.Pages.Basic;

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

		Flyout.Focused += (_, _) =>
		{
			if (DeviceInfo.Platform == DevicePlatform.WinUI && FlyoutLayoutBehavior is FlyoutLayoutBehavior.Popover or FlyoutLayoutBehavior.SplitOnPortrait)
				IsPresented = true;
		};

		Flyout.Unfocused += (_, _) =>
		{
			if (DeviceInfo.Platform == DevicePlatform.WinUI && FlyoutLayoutBehavior is FlyoutLayoutBehavior.Popover or FlyoutLayoutBehavior.SplitOnPortrait)
				IsPresented = false;
		};
	}

	internal event EventHandler<TraceEventArgs> BackButtonPressing;

	#region Overrides of NavigationPage

	protected override bool OnBackButtonPressed()
	{
		OnBackButtonPressing(GetCurrentMethod()?.Name);
		return base.OnBackButtonPressed();
	}

	#endregion

	private void OnBackButtonPressing(string originEvent) => BackButtonPressing?.Invoke(this, new TraceEventArgs(originEvent));
}