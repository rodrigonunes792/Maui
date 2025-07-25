using SoftwareShow.Contagem.MApp.Models;
using Newtonsoft.Json;

namespace SoftwareShow.Contagem.MApp.Pages;

public partial class StoreSelectionPage : ContentPage
{
    private List<LojaUsuario> _allStores = new();
    private List<LojaUsuario> _filteredStores = new();
    private string _searchText = string.Empty;
    private bool _isSyncEnabled = true;
    private LojaUsuario? _selectedStore;

    public bool IsSyncEnabled
    {
        get => _isSyncEnabled;
        set => _isSyncEnabled = value;
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            FilterStores();
        }
    }

    public StoreSelectionPage()
    {
        InitializeComponent();
        BindingContext = this;
        LoadStoresAsync();
    }

    private async void LoadStoresAsync()
    {
        try
        {
            // Carregar lojas do SecureStorage
            var lojasJson = await SecureStorage.GetAsync("user_lojas");
            if (!string.IsNullOrEmpty(lojasJson))
            {
                _allStores = JsonConvert.DeserializeObject<List<LojaUsuario>>(lojasJson) ?? new List<LojaUsuario>();
                _filteredStores = new List<LojaUsuario>(_allStores);
                CreateStoreItems();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", "Erro ao carregar lojas", "OK");
            System.Diagnostics.Debug.WriteLine($"Erro ao carregar lojas: {ex.Message}");
        }
    }

    private void CreateStoreItems()
    {
        StoresContainer.Children.Clear();

        foreach (var loja in _filteredStores)
        {
            var storeItem = CreateStoreItem(loja);
            StoresContainer.Children.Add(storeItem);
        }
    }

    private View CreateStoreItem(LojaUsuario loja)
    {
        // Border principal
        var border = new Border
        {
            Style = (Style)Resources["StoreItemStyle"]
        };

        // Grid para organizar conteúdo
        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = GridLength.Auto }
            }
        };

        // Label com informações da loja
        var storeLabel = new Label
        {
            Text = $"{loja.COD_LOJA:D4} - {loja.NOME_LOJA}",
            FontSize = 16,
            TextColor = (Color)Resources["TextColor"],
            VerticalOptions = LayoutOptions.Center
        };

        // Indicador de seleção (inicialmente invisível)
        var selectionIndicator = new Label
        {
            Text = "V",
            FontSize = 20,
            TextColor = (Color)Resources["PrimaryColor"],
            VerticalOptions = LayoutOptions.Center,
            IsVisible = false
        };

        grid.Children.Add(storeLabel);
        grid.Children.Add(selectionIndicator);
        Grid.SetColumn(storeLabel, 0);
        Grid.SetColumn(selectionIndicator, 1);

        border.Content = grid;

        // Adicionar gesture recognizer para clique
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (sender, e) => OnStoreItemTapped(loja, selectionIndicator);
        border.GestureRecognizers.Add(tapGesture);

        return border;
    }

    private void OnStoreItemTapped(LojaUsuario loja, Label selectionIndicator)
    {
        // Limpar seleção anterior
        ClearAllSelections();

        // Selecionar nova loja
        _selectedStore = loja;
        selectionIndicator.IsVisible = true;

        // Opcional: navegar automaticamente ou aguardar confirmação
        // await NavigateToNextPage();
    }

    private void ClearAllSelections()
    {
        foreach (Border item in StoresContainer.Children.OfType<Border>())
        {
            if (item.Content is Grid grid)
            {
                var indicator = grid.Children.OfType<Label>().LastOrDefault();
                if (indicator != null)
                {
                    indicator.IsVisible = false;
                }
            }
        }
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        SearchText = e.NewTextValue ?? string.Empty;
    }

    private void FilterStores()
    {
        if (string.IsNullOrWhiteSpace(_searchText))
        {
            _filteredStores = new List<LojaUsuario>(_allStores);
        }
        else
        {
            _filteredStores = _allStores
                .Where(loja =>
                    loja.NOME_LOJA.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
                    loja.COD_LOJA.ToString().Contains(_searchText))
                .ToList();
        }

        CreateStoreItems();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        var result = await DisplayAlert("Atenção", "Deseja sair do app ?", "Sim","Não");

        if (result)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
        await Navigation.PopAsync();
    }

    // Método para obter loja selecionada (para usar em outras telas)
    public LojaUsuario? GetSelectedStore()
    {
        return _selectedStore;
    }

    // Método para obter status da sincronização
    public bool GetSyncStatus()
    {
        return _isSyncEnabled;
    }
}