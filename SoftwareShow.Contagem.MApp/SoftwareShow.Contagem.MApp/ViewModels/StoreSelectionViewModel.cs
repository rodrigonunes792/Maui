using SoftwareShow.Contagem.MApp.Models;
using SoftwareShow.Contagem.MApp.Interfaces;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SoftwareShow.Contagem.MApp.Service;

namespace SoftwareShow.Contagem.MApp.ViewModels;

public class StoreSelectionViewModel : INotifyPropertyChanged
{
    private readonly IDatabaseService _databaseService;
    private readonly IRestService _restService;

    private List<LojaUsuario> _allStores = new();
    private ObservableCollection<LojaUsuario> _filteredStores = new();
    private string _searchText = string.Empty;
    private bool _isSyncEnabled = true;
    private LojaUsuario? _selectedStore;
    private string _syncStatus = string.Empty;
    private bool _isSyncing = false;
    private bool _isLoading = false;
    private bool _canCancelSync = true;
    private CancellationTokenSource? _syncCancellationTokenSource;

    public ObservableCollection<LojaUsuario> FilteredStores
    {
        get => _filteredStores;
        set
        {
            _filteredStores = value;
            OnPropertyChanged();
        }
    }

    public bool IsSyncEnabled
    {
        get => _isSyncEnabled;
        set
        {
            _isSyncEnabled = value;
            OnPropertyChanged();
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
            FilterStores();
        }
    }

    public string SearchDisplayText
    {
        get
        {
            if (_selectedStore != null)
            {
                return $"{_selectedStore.COD_LOJA:D4} - {_selectedStore.NOME_LOJA}";
            }
            return _searchText;
        }
        set
        {
            _searchText = value ?? string.Empty;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ShowSearchIcon));
            OnPropertyChanged(nameof(ShowClearIcon));

            if (string.IsNullOrEmpty(_searchText) && _selectedStore != null)
            {
                SelectedStore = null;
            }

            FilterStores();
        }
    }

    public bool ShowSearchIcon => string.IsNullOrEmpty(_searchText) && _selectedStore == null;

    public bool ShowClearIcon => _selectedStore != null || !string.IsNullOrEmpty(_searchText);

    public bool ShowEmptyView
    {
        get
        {
            var result = _filteredStores.Count == 0 && _selectedStore == null;
            return result;
        }
    }

    public bool ShowContinueButton
    {
        get
        {
            var result = _selectedStore != null && !_isLoading;
            return result;
        }
    }

    public LojaUsuario? SelectedStore
    {
        get => _selectedStore;
        set
        {
            if (value == null && _selectedStore != null)
                return;

            if (_selectedStore?.COD_LOJA != value?.COD_LOJA)
            {
                _selectedStore = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SearchDisplayText));
                OnPropertyChanged(nameof(ShowSearchIcon));
                OnPropertyChanged(nameof(ShowClearIcon));
                OnPropertyChanged(nameof(ShowEmptyView));
                OnPropertyChanged(nameof(ShowContinueButton));
            }
        }
    }

    public string SyncStatusText
    {
        get => _syncStatus;
        private set
        {
            _syncStatus = value;
            OnPropertyChanged();
        }
    }

    public bool IsSyncing
    {
        get => _isSyncing;
        private set
        {
            _isSyncing = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ShowContinueButton));
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        private set
        {
            _isLoading = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ShowContinueButton));
        }
    }

    public bool CanCancelSync
    {
        get => _canCancelSync;
        private set
        {
            _canCancelSync = value;
            OnPropertyChanged();
        }
    }

    // Commands
    public ICommand ClearCommand { get; }
    public ICommand ContinueCommand { get; }
    public ICommand CancelSyncCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public StoreSelectionViewModel(IDatabaseService databaseService, IRestService restService)
    {
        _databaseService = databaseService;
        _restService = restService;

        ClearCommand = new Command(() => OnClear());
        ContinueCommand = new Command(async () => await OnContinue(), () => CanExecuteContinue(null));
        CancelSyncCommand = new Command(() => CancelSync(), () => CanCancelSync);

        LoadStoresAsync();
    }

    // Construtor sem parâmetros para fallback
    public StoreSelectionViewModel() : this(new DatabaseService(), null) { }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async void LoadStoresAsync()
    {
        try
        {
            var lojasJson = await SecureStorage.GetAsync("user_lojas");

            if (!string.IsNullOrEmpty(lojasJson))
            {
                _allStores = JsonConvert.DeserializeObject<List<LojaUsuario>>(lojasJson) ?? new List<LojaUsuario>();
                FilterStores();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao carregar lojas: {ex.Message}");
        }
    }

    private void FilterStores()
    {
        List<LojaUsuario> filtered;

        if (string.IsNullOrWhiteSpace(_searchText))
        {
            filtered = _allStores;
        }
        else
        {
            filtered = _allStores
                .Where(loja =>
                    loja.NOME_LOJA.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
                    loja.COD_LOJA.ToString().Contains(_searchText))
                .ToList();
        }

        FilteredStores.Clear();
        foreach (var loja in filtered)
        {
            FilteredStores.Add(loja);
        }

        if (_selectedStore != null && !filtered.Contains(_selectedStore))
        {
            SelectedStore = null;
        }

        OnPropertyChanged(nameof(ShowEmptyView));
    }

    public void OnClear()
    {
        _selectedStore = null;
        SelectedStore = null;
        SearchText = string.Empty;
        OnPropertyChanged(nameof(ShowSearchIcon));
        OnPropertyChanged(nameof(ShowClearIcon));
        OnPropertyChanged(nameof(ShowEmptyView));
        OnPropertyChanged(nameof(ShowContinueButton));
    }

    private bool CanExecuteContinue(object parameter)
    {
        return _selectedStore != null && !_isLoading;
    }

    private async Task OnContinue()
    {
        if (_selectedStore == null || _isLoading) return;

        try
        {
            // Salvar loja selecionada
            var lojaJson = JsonConvert.SerializeObject(_selectedStore);
            await SecureStorage.SetAsync("selected_store", lojaJson);
            await SecureStorage.SetAsync("sync_enabled", _isSyncEnabled.ToString());

            // Se sincronização estiver habilitada, executar sincronização
            if (_isSyncEnabled && _restService != null)
            {
                await SincronizarDados(_selectedStore);
            }

            // Navegar para próxima tela
            await Shell.Current.GoToAsync("//MenuPage");
        }
        catch (Exception ex)
        {
            SyncStatusText = $"Erro: {ex.Message}";
        }
    }

    private async Task SincronizarDados(LojaUsuario loja)
    {
        if (_restService == null)
        {
            SyncStatusText = "Serviço de API não disponível";
            return;
        }

        IsLoading = true;
        _syncCancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _syncCancellationTokenSource.Token;

        try
        {
            // Kits
            SyncStatusText = $"Sincronizando (Kits) da loja {loja.NOME_LOJA}";
            await Task.Delay(100, cancellationToken); // Para permitir atualização da UI

            var kits = await _restService.BaixarKitAsync(loja.COD_LOJA, cancellationToken);
            await _databaseService.DeleteAllAsync<Kit>();
            if (kits?.Any() == true)
            {
                await _databaseService.InsertAllAsync(kits);
            }

            // Preços
            SyncStatusText = $"Sincronizando (Preços) {loja.NOME_LOJA}";
            await Task.Delay(100, cancellationToken);

            var precos = await _restService.BaixarPrecoAsync(loja.COD_LOJA, cancellationToken);
            await _databaseService.DeleteAllAsync<Preco>();
            if (precos?.Any() == true)
            {
                await _databaseService.InsertAllAsync(precos);
            }

            // Atividades
            SyncStatusText = $"Sincronizando (Atividades) {loja.NOME_LOJA}";
            await Task.Delay(100, cancellationToken);

            var atividades = await _restService.ConsultarAtividadeAsync();
            await _databaseService.DeleteAllAsync<Atividade>();
            if (atividades?.Any() == true)
            {
                await _databaseService.InsertAllAsync(atividades);
            }

            // Códigos de Barras
            SyncStatusText = $"Sincronizando (Código de Barras) {loja.NOME_LOJA}";
            await Task.Delay(100, cancellationToken);

            var codigoBarras = await _restService.BaixarCodigoBarrasAsync(cancellationToken);
            await _databaseService.DeleteAllAsync<CodigoBarras>();
            if (codigoBarras?.Any() == true)
            {
                await _databaseService.InsertAllAsync(codigoBarras);
            }

            // Produtos
            SyncStatusText = $"Sincronizando (Produtos) {loja.NOME_LOJA}";
            await Task.Delay(100, cancellationToken);

            var produtos = await _restService.BaixarProdutoAsync(cancellationToken);
            await _databaseService.DeleteAllAsync<Produto>();
            if (produtos?.Any() == true)
            {
                await _databaseService.InsertAllAsync(produtos);
            }

            // Sucesso
            SyncStatusText = "Dados sincronizados com sucesso";
            await Task.Delay(2000, cancellationToken);
            SyncStatusText = string.Empty;
        }
        catch (OperationCanceledException)
        {
            SyncStatusText = "Sincronização cancelada";
            await Task.Delay(2000);
            SyncStatusText = string.Empty;
        }
        catch (Exception ex)
        {
            SyncStatusText = $"Erro na sincronização: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"Erro na sincronização: {ex.Message}");
            //await Task.Delay(5000);
            SyncStatusText = string.Empty;
        }
        finally
        {
            IsLoading = false;
            CanCancelSync = true;
            _syncCancellationTokenSource?.Dispose();
            _syncCancellationTokenSource = null;

            // Atualizar comandos
            ((Command)ContinueCommand).ChangeCanExecute();
            ((Command)CancelSyncCommand).ChangeCanExecute();
        }
    }

    public void CancelSync()
    {
        if (_syncCancellationTokenSource != null && !_syncCancellationTokenSource.Token.IsCancellationRequested)
        {
            _syncCancellationTokenSource.Cancel();
            SyncStatusText = "Cancelando sincronização...";
            CanCancelSync = false;
        }
    }

    public void ClearSelection()
    {
        _selectedStore = null;           // Remove loja selecionada
        SelectedStore = null;            // Limpa binding
        _isSyncEnabled = true;
        SearchText = string.Empty;       // Limpa texto de busca
        SearchDisplayText = string.Empty; // Limpa campo visual

        // Atualiza a UI para refletir o estado limpo
        OnPropertyChanged(nameof(ShowContinueButton)); // Botão fica invisível
    }

    // Métodos públicos para acesso externo
    public LojaUsuario? GetSelectedStore() => _selectedStore;
    public bool GetSyncStatus() => _isSyncEnabled;
}