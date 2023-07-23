namespace Win10TabbedPageSwitchCrash;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new TabbedPageCoded();
	}
}
