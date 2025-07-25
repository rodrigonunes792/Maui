using SoftwareShow.Contagem.MApp.ViewModels;

namespace SoftwareShow.Contagem.MApp.Pages;

public partial class MenuPage : ContentPage
{

    // Construtor sem parâmetros para fallback
    public MenuPage()
    {
        InitializeComponent();

    }

    private async void OnIniciarContagemTapped(object sender, EventArgs e)
    {
        //await DisplayAlert("Olá", "Vamos iniciar a contagem em Breve", "Ok");
        // TODO: Navegar para próxima tela
       await Shell.Current.GoToAsync("//ContagemListPage");
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        // Voltar para a tela de seleção de lojas
        await Shell.Current.GoToAsync("//storeselection");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        System.Diagnostics.Debug.WriteLine("MenuPage: OnAppearing chamado");
    }
}