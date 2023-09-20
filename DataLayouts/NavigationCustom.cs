using System.Diagnostics;


namespace DataLayouts;

internal class NavigationCustom : NavigationPage
{
	public NavigationCustom(string title = null) : base(new MainPage(new SecondPage(true, true, "NPage #2"), true, true, "NPage #0"))
	{
		new PageTrace(this, title);
		Pushed += (_, navigationEventArgs) => Trace.WriteLine($"{Title}:{nameof(Pushed)}:{navigationEventArgs.Page.Title}");
		Popped += (_, navigationEventArgs) => Trace.WriteLine($"{Title}:{nameof(Popped)}:{navigationEventArgs.Page.Title}");
	}

	#region Overrides of NavigationPage

	protected override bool OnBackButtonPressed()
	{
		Trace.WriteLine($"{nameof(OnBackButtonPressed)}");
		return base.OnBackButtonPressed();
	}

	#endregion
}