namespace SoftwareShow.Contagem.MApp
{
    public partial class App : Application
    {
        public App()
        {
            try
            {
                InitializeComponent();
                MainPage = new AppShell();
            }
            catch (Exception ex)
            {
                // Log do erro para debug
                System.Diagnostics.Debug.WriteLine($"Erro na inicialização da aplicação: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                
                // Em caso de erro, tentar criar uma página simples
                MainPage = new ContentPage
                {
                    Content = new Label
                    {
                        Text = $"Erro na inicialização: {ex.Message}",
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    }
                };
            }
        }
    }
}
