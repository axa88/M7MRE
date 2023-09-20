namespace LifecycleTriggeredPermissionsUi;

public class TabbedCustom : TabbedPage
{
	public TabbedCustom(string title = null)
	{
		new PageTrace(this, title);
		Children.Add(new MainPage(new SecondPage(true, true, "TPage #2"), true, true, "TPage #0"));
	}
}
