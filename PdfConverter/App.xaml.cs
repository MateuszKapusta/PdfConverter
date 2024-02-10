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

    protected override Window CreateWindow(IActivationState activationState)
    {
        Window window = base.CreateWindow(activationState);

        window.Height = 1020;
        window.Width = 1400;

        return window;
    }
}
