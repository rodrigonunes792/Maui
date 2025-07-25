using SoftwareShow.Contagem.MApp.ViewModels;

namespace SoftwareShow.Contagem.MApp.Pages;

public partial class MenuPage : ContentPage
{

    // Construtor sem par�metros para fallback
    public MenuPage()
    {
        InitializeComponent();

    }

    private async void OnIniciarContagemTapped(object sender, EventArgs e)
    {
        //await DisplayAlert("Ol�", "Vamos iniciar a contagem em Breve", "Ok");
        // TODO: Navegar para pr�xima tela
       await Shell.Current.GoToAsync("//ContagemListPage");
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        // Voltar para a tela de sele��o de lojas
        await Shell.Current.GoToAsync("//storeselection");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        System.Diagnostics.Debug.WriteLine("MenuPage: OnAppearing chamado");
    }
}