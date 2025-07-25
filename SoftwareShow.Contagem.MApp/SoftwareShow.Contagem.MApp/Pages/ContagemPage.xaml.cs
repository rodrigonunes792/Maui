using SoftwareShow.Contagem.MApp.ViewModels;

namespace SoftwareShow.Contagem.MApp.Pages;

public partial class ContagemPage : ContentPage
{
    private ContagemViewModel _viewModel;

    public ContagemPage(ContagemViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    // Construtor sem parâmetros para fallback
    public ContagemPage()
    {
        InitializeComponent();
        _viewModel = new ContagemViewModel();
        BindingContext = _viewModel;
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        var result = await DisplayAlert("Atenção",
            "Deseja cancelar a criação da contagem?",
            "Sim", "Não");

        if (result)
        {
            await Shell.Current.GoToAsync("//ContagemListPage");
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CarregarDadosAsync();
    }
}