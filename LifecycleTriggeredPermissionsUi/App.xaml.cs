using System.Diagnostics;


namespace LifecycleTriggeredPermissionsUi;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		_ = new PageTrace(this, nameof(App));

		//MainPage = new MainPage { Title = "#0" }; // ContentPage
		//MainPage = new NavigationPage(new MainPage { Title = "#0" }); // NavigationPage
		//MainPage = new TabbedPage { Title = "#0", Children = { new MainPage { Title = "#0" } } }; // TabbedPage
	}

	#region Overrides of Application

	protected override Window CreateWindow(IActivationState activationState)
	{
		Trace.WriteLine("App.CreateWindow");
		//return new CustomWindow(new MainPage(true, true, "CPage #0"), "Window #0"); // test multi window using 2 windows with ContentPages
		//return new CustomWindow(new NavigationPage(new MainPage(true, true, "CPage #0")) { Title = "Nav" }, "Window #0"); // test NavigationPage using built in stacked ContentPages
		//return new CustomWindow(new TabbedPage { Title = "Tab #0", Children = { new MainPage(true, true, "CPage #0") } }, "Window #0"); // test TabbedPage using multiple tabs of ContentPages
		//return new CustomWindow(new FlyoutCustom(new MainPage(true, true, "Flyout"), new SecondPage(true, true, "Detail"), "FlyoutPage"), "Window #0" ); // test FlyoutPage using ContentPages for the Flyout and Detail pages
		return new CustomWindow(new SelectorPage(), "S.Window");
	}

	protected override void OnStart()
	{
		base.OnStart();
		Trace.WriteLine("App.OnStart");
	}

	protected override void OnResume()
	{
		base.OnResume();
		Trace.WriteLine("App.OnResume");
	}

	protected override void OnSleep()
	{
		base.OnSleep();
		Trace.WriteLine("App.OnSleep");
	}

	#endregion
}
