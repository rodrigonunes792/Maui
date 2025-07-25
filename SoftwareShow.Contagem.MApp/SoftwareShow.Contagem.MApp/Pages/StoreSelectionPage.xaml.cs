using SoftwareShow.Contagem.MApp.ViewModels;
using SoftwareShow.Contagem.MApp.Models;
using SoftwareShow.Contagem.MApp.Interfaces;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace SoftwareShow.Contagem.MApp.Pages;

public partial class StoreSelectionPage : ContentPage
{
    private StoreSelectionViewModel _viewModel;
    private readonly IOrientationService _orientationService;

    public StoreSelectionPage(StoreSelectionViewModel viewModel, IOrientationService orientationService)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _orientationService = orientationService;
    }

    // Construtor sem parâmetros para fallback (se necessário)
    public StoreSelectionPage()
    {
        InitializeComponent();
        _viewModel = new StoreSelectionViewModel();
        BindingContext = _viewModel;
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var newValue = e.NewTextValue ?? string.Empty;

        // Se há loja selecionada e o usuário alterou o texto, desseleciona
        if (_viewModel.SelectedStore != null && newValue != _viewModel.SearchDisplayText)
        {
            _viewModel.SelectedStore = null;
        }

        // Atualiza usando SearchDisplayText para evitar loops
        if (_viewModel.SearchDisplayText != newValue)
        {
            _viewModel.SearchDisplayText = newValue;
        }
    }

    private void OnClearButtonClicked(object sender, EventArgs e)
    {
        // Limpa via ViewModel
        _viewModel.OnClear();

        // Limpa o campo de texto
        SearchEntry.Text = string.Empty;

        // Foca no campo de pesquisa
        SearchEntry.Focus();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        // Se estiver sincronizando, perguntar se quer cancelar
        if (_viewModel.IsSyncing)
        {
            var cancelResult = await DisplayAlert("Atenção",
                "Uma sincronização está em andamento. Deseja cancelar a sincronização e sair?",
                "Sim", "Não");

            if (cancelResult)
            {
                _viewModel.CancelSync();
                await Shell.Current.GoToAsync("//LoginPage");
            }
            return;
        }

        var result = await DisplayAlert("Atenção", "Deseja sair do app?", "Sim", "Não");
        if (result)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }

    private void OnStoreItemTapped(object sender, EventArgs e)
    {
        if (sender is Border border && border.BindingContext is LojaUsuario loja)
        {
            _viewModel.SelectedStore = loja;
            System.Diagnostics.Debug.WriteLine($"Loja selecionada: {loja.COD_LOJA} - {loja.NOME_LOJA}");
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel?.ClearSelection();
        SyncSwitch.IsToggled = true;
         DisplayAlert("Orientação", "Forçando retrato", "OK");

        var orientationService = _orientationService;
        orientationService?.ForcePortrait();

    }

    protected override void OnDisappearing()
    {

        // Cancelar sincronização se a página for fechada
        _viewModel?.CancelSync();
        _orientationService?.AllowAllOrientations();
        base.OnDisappearing();
    }

    // Métodos públicos para acesso externo
    public LojaUsuario? GetSelectedStore() => _viewModel.GetSelectedStore();
    public bool GetSyncStatus() => _viewModel.GetSyncStatus();
}