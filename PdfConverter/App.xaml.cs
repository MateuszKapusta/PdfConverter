namespace PdfConverter;

public partial class App : Application
{
	public App()
	{
        //Register Syncfusion license
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("KEY");

        InitializeComponent();

		MainPage = new AppShell();
	}
}
