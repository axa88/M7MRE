using Android.App;
using Android.Runtime;


// ReSharper disable once CheckNamespace
namespace BLEPoC;

[Application]
public class MainApplication(IntPtr handle, JniHandleOwnership ownership) : MauiApplication(handle, ownership)
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
