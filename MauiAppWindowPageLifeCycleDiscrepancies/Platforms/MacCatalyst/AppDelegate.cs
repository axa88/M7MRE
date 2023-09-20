using Foundation;


// ReSharper disable once CheckNamespace
namespace MauiAppWindowPageLifeCycleDiscrepancies;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}