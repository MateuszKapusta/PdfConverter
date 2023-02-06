using PdfConverter.Services;
using Syncfusion.Maui.Core.Hosting;
using PdfConverter.Services.FolderPicker;
using System.Runtime.CompilerServices;

namespace PdfConverter;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.RegisterAppServices()
			.RegisterViewModels()
			.RegisterPages()
			.ConfigureSyncfusionCore()
      .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if WINDOWS
        builder.Services.AddTransient<IFolderPicker, Platforms.Windows.Services.FolderPicker>();
#elif MACCATALYST
		builder.Services.AddTransient<IFolderPicker, Platforms.MacCatalyst.Services.FolderPicker>();
#endif

    return builder.Build();
	}

  public static MauiAppBuilder RegisterAppServices(this MauiAppBuilder mauiAppBuilder)
  {
    mauiAppBuilder.Services.AddSingleton<IPdfService, PdfService>();
		return mauiAppBuilder;
  }

  public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
  {
    mauiAppBuilder.Services.AddSingleton<MainPageViewModel>();
    return mauiAppBuilder;
  }

	public static MauiAppBuilder RegisterPages(this MauiAppBuilder mauiAppBuilder)
	{
		mauiAppBuilder.Services.AddSingleton<MainPage>();
		return mauiAppBuilder;
	}

}
