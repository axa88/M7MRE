using Microsoft.Extensions.Logging;


namespace DataLayouts;

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

	#if DEBUG
		builder.Logging.AddDebug();
	#endif

		return builder.Build();
	}
}