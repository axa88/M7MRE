namespace BLEPoC.Ui.Pages;

internal class TabbedCustom : TabbedPage
{
    internal TabbedCustom(string title = null)
    {
        new LifeCycleTracing(this, title);
        Children.Add(new MainPage(new SecondPage(true, true, "TPage #2"), true, true, "TPage #0"));
    }
}
