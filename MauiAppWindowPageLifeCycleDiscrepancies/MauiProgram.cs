using Microsoft.Maui.LifecycleEvents;

using Microsoft.Extensions.Logging;


namespace MauiAppWindowPageLifeCycleDiscrepancies;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				// ReSharper disable StringLiteralTypo
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				// ReSharper restore StringLiteralTypo
			});

	#if ANDROID
		builder.ConfigureLifecycleEvents(lifecycleBuilder => lifecycleBuilder.AddAndroid(androidLifecycleBuilder => androidLifecycleBuilder.OnDestroy(activity => Permissions.PermissionsProcessor.OnWindowDestroying(activity))));
	#endif

	#if DEBUG
		builder.Logging.AddDebug();
	#endif

		_ = new PageTrace(builder);

		return builder.Build();
	}
}