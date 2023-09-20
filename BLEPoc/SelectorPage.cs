namespace BLEPoc;

public class SelectorPage : ContentPage
{
	public SelectorPage()
	{
		var contentWindowButton = new Button { Text = "Content Page Windows" };
		contentWindowButton.Clicked += (_, __) =>
		{
			Application.Current?.OpenWindow(new CustomWindow(new MainPage(null, true, true, "CPage #0"), "MultiWin #0"));
			Application.Current?.OpenWindow(new CustomWindow(new SecondPage(true, true, "CPage #2"), "MultiWin #2"));

		}; // test multi window using 2 windows with ContentPages

		var navigationPageButton = new Button { Text = "Navigation Page Window" };
		navigationPageButton.Clicked += (_, __) => Application.Current?.OpenWindow(new CustomWindow(new NavigationCustom(), "NavWin #0")); // test NavigationPage using built in stacked ContentPages

		var tabbedPageButton = new Button { Text = "Tabbed Page Window" };
		tabbedPageButton.Clicked += (_, __) => Application.Current?.OpenWindow(new CustomWindow(new TabbedCustom(), "TabWin #0")); // test TabbedPage using multiple tabs of ContentPages)

		var flyoutPageButton = new Button { Text = "Flyout Page Window" };
		flyoutPageButton.Clicked += (_, __) => Application.Current?.OpenWindow(new CustomWindow(new FlyoutCustom(), "FlyWin #0")); // test FlyoutPage using ContentPages for the Flyout and Detail pages

		var exitButton = new Button { Text = "Exit" };
		exitButton.Clicked += (_, __) => Application.Current?.Quit();

		Content = new StackLayout
		{
			Children =
			{
				contentWindowButton,
				navigationPageButton,
				tabbedPageButton,
				flyoutPageButton,
				exitButton
			}
		};
	}
}