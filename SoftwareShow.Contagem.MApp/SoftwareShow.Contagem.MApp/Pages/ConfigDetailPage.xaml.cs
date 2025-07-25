namespace SoftwareShow.Contagem.MApp.Pages;

public partial class ConfigDetailPage : ContentPage
{
	public ConfigDetailPage()
	{
		InitializeComponent();
	}

    private void ConfigDetailPage_Loaded(object sender, EventArgs e)
    {
        EntryBaseUrl.Focus();
    }

    private void ImageButtonFechar_Clicked(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }
}