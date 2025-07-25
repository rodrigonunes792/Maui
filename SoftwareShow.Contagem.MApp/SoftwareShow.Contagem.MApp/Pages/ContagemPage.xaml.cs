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

    // Construtor sem par�metros para fallback
    public ContagemPage()
    {
        InitializeComponent();
        _viewModel = new ContagemViewModel();
        BindingContext = _viewModel;
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        var result = await DisplayAlert("Aten��o",
            "Deseja cancelar a cria��o da contagem?",
            "Sim", "N�o");

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