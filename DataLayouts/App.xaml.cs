using DataLayouts.Ui;


namespace DataLayouts;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new CollectionPage();
	}
}
