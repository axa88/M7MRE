using BLEPoC.Permissions;
using BLEPoC.Utility;

using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;


namespace BLEPoC;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSans-Regular");
				// ReSharper disable StringLiteralTypo
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				// ReSharper restore StringLiteralTypo
			});

	#if ANDROID
		builder.ConfigureLifecycleEvents(lifecycleBuilder => lifecycleBuilder.AddAndroid(androidLifecycleBuilder => androidLifecycleBuilder.OnDestroy(activity => PermissionsProcessor.Instance.OnWindowDestroying(activity))));
	#endif

	#if DEBUG
		builder.Logging.AddDebug();
	#endif

		_ = new LifeCycleTracing(builder);

		return builder.Build();
	}
}