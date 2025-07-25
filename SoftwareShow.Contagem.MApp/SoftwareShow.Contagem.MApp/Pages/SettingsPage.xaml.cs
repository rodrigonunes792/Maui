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
            // Carregar configura��es salvas
            var savedApiUrl = await SecureStorage.GetAsync("api_url");
            if (!string.IsNullOrEmpty(savedApiUrl))
            {
                ApiEntry.Text = savedApiUrl;
            }
        }
        catch (Exception ex)
        {
            // Log silencioso em produ��o ou usar ILogger se configurado
            System.Diagnostics.Debug.WriteLine($"Erro ao carregar configura��es: {ex.Message}");
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
        // Verificar se h� mudan�as n�o salvas
        if (await HasUnsavedChanges())
        {
            var result = await DisplayAlert("Aten��o",
                "H� altera��es n�o salvas. Deseja realmente sair sem salvar?",
                "Sair sem salvar", "Continuar editando");

            if (!result) return; // Usu�rio escolheu continuar editando
        }

        // Navegar de volta para LoginPage
        try
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
        catch (Exception ex)
        {
            // Log do erro e feedback para o usu�rio
            System.Diagnostics.Debug.WriteLine($"Erro na navega��o: {ex.Message}");
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

        // Valida��o
        if (string.IsNullOrWhiteSpace(apiUrl))
        {
            await DisplayAlert("Erro", "Informe o endere�o da API", "OK");
            return;
        }

        // Validar se � uma URL v�lida
        if (!Uri.TryCreate(apiUrl, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            await DisplayAlert("Erro", "Informe uma URL v�lida (ex: https://api.exemplo.com)", "OK");
            return;
        }

        SetLoadingState(true);

        try
        {
            // Testar conectividade com a API (opcional)
            // await TestApiConnection(apiUrl);

            // Salvar configura��es
            await SecureStorage.SetAsync("api_url", apiUrl);
            await SecureStorage.SetAsync("api_configured", "true");

            await DisplayAlert("Sucesso", "Configura��es salvas com sucesso!", "OK");

            // Voltar para tela anterior
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", "Erro ao salvar configura��es. Tente novamente.", "OK");
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