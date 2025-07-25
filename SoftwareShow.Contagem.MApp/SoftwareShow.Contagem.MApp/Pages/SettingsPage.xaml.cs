namespace SoftwareShow.Contagem.MApp.Pages;

public partial class SettingsPage : ContentPage
{
    private bool _isLoading = false;

    public SettingsPage()
    {
        InitializeComponent();
        LoadCurrentSettings();
    }

    private async void LoadCurrentSettings()
    {
        try
        {
            // Carregar configurações salvas
            var savedApiUrl = await SecureStorage.GetAsync("api_url");
            if (!string.IsNullOrEmpty(savedApiUrl))
            {
                ApiEntry.Text = savedApiUrl;
            }
        }
        catch (Exception ex)
        {
            // Log silencioso em produção ou usar ILogger se configurado
            System.Diagnostics.Debug.WriteLine($"Erro ao carregar configurações: {ex.Message}");
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await HandleNavigateBack();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await HandleNavigateBack();
    }

    private async Task HandleNavigateBack()
    {
        // Verificar se há mudanças não salvas
        if (await HasUnsavedChanges())
        {
            var result = await DisplayAlert("Atenção",
                "Há alterações não salvas. Deseja realmente sair sem salvar?",
                "Sair sem salvar", "Continuar editando");

            if (!result) return; // Usuário escolheu continuar editando
        }

        // Navegar de volta para LoginPage
        try
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
        catch (Exception ex)
        {
            // Log do erro e feedback para o usuário
            System.Diagnostics.Debug.WriteLine($"Erro na navegação: {ex.Message}");
            await DisplayAlert("Erro", "Erro ao navegar. Tente novamente.", "OK");
        }
    }

    private async Task<bool> HasUnsavedChanges()
    {
        try
        {
            var currentApiUrl = await SecureStorage.GetAsync("api_url") ?? string.Empty;
            var entryApiUrl = ApiEntry.Text?.Trim() ?? string.Empty;

            return currentApiUrl != entryApiUrl;
        }
        catch
        {
            return false;
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (_isLoading) return;

        var apiUrl = ApiEntry.Text?.Trim();

        // Validação
        if (string.IsNullOrWhiteSpace(apiUrl))
        {
            await DisplayAlert("Erro", "Informe o endereço da API", "OK");
            return;
        }

        // Validar se é uma URL válida
        if (!Uri.TryCreate(apiUrl, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            await DisplayAlert("Erro", "Informe uma URL válida (ex: https://api.exemplo.com)", "OK");
            return;
        }

        SetLoadingState(true);

        try
        {
            // Testar conectividade com a API (opcional)
            // await TestApiConnection(apiUrl);

            // Salvar configurações
            await SecureStorage.SetAsync("api_url", apiUrl);
            await SecureStorage.SetAsync("api_configured", "true");

            await DisplayAlert("Sucesso", "Configurações salvas com sucesso!", "OK");

            // Voltar para tela anterior
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", "Erro ao salvar configurações. Tente novamente.", "OK");
            // Manter apenas para debug durante desenvolvimento
            System.Diagnostics.Debug.WriteLine($"Erro ao salvar: {ex.Message}");
        }
        finally
        {
            SetLoadingState(false);
        }
    }

    private void SetLoadingState(bool isLoading)
    {
        _isLoading = isLoading;
        LoadingOverlay.IsVisible = isLoading;
        SaveButton.IsEnabled = !isLoading;
        ApiEntry.IsEnabled = !isLoading;
    }

    private async Task<bool> TestApiConnection(string apiUrl)
    {
        try
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(10);

            var response = await httpClient.GetAsync($"{apiUrl}/health");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}