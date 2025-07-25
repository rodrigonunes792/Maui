using SoftwareShow.Contagem.MApp.ViewModels;

namespace SoftwareShow.Contagem.MApp.Pages;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _viewModel;

    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//settings");
    }

    protected override void OnAppearing()
    {
        SenhaEntry.Text = string.Empty;
        UsuarioEntry.Text = string.Empty;
    }

    private void OnTogglePasswordClicked(object sender, EventArgs e)
    {
        _viewModel.TogglePasswordVisibility();
        SenhaEntry.IsPassword = !_viewModel.IsPasswordVisible;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var (success, message) = await _viewModel.LoginAsync();

        if (success)
        {
            //await DisplayAlert("Sucesso", message, "OK");
            await Shell.Current.GoToAsync("//storeselection");
            // Navegar para próxima tela quando existir
            // await Shell.Current.GoToAsync("//main");
        }
        else
        {
            if (message.Contains("Configure o endereço"))
            {
                var result = await DisplayAlert("Configuração necessária",
                    message, "Configurar", "Cancelar");

                if (result)
                {
                    await Shell.Current.GoToAsync("//settings");
                }
            }
            else
            {
                await DisplayAlert("Erro", message, "OK");
            }
        }
    }
}