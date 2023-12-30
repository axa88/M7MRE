using BLEPoC.Utility;

using static System.Reflection.MethodBase;


namespace BLEPoC.Ui.Pages.Basic;

internal class TabbedCustom : TabbedPage
{
    internal TabbedCustom(string title = null)
    {
		_ = new LifeCycleTracing(this, title);
		Children.Add(new MainPage(new SecondPage(true, true, "TPage #2"), true, true, "TPage #0"));
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
