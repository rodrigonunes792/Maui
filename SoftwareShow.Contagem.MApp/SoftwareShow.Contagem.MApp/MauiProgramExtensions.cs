using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SoftwareShow.Contagem.MApp.Interfaces;
using SoftwareShow.Contagem.MApp.Pages;
using SoftwareShow.Contagem.MApp.Service;
using SoftwareShow.Contagem.MApp.ViewModels;

namespace SoftwareShow.Contagem.MApp
{
    public static class MauiProgramExtensions
    {
        public static MauiAppBuilder UseSharedMauiApp(this MauiAppBuilder builder)
        {
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Inter-Italic-VariableFont.ttf", "InterItalic");
                    fonts.AddFont("Inter-VariableFont.ttf", "InterRegular");
                });

            // Adicionar logging apenas em DEBUG
#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Registrar HttpClient (necessário em todas as configurações)
            builder.Services.AddSingleton<HttpClient>(provider =>
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("User-Agent", "SoftwareShow.Contagem.MApp/1.0");
                httpClient.Timeout = TimeSpan.FromSeconds(30);
                return httpClient;
            });

            // Registrar Services (necessários em todas as configurações)
            builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
            builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
            builder.Services.AddScoped<IRestService, RestService>();
            builder.Services.AddScoped<IExportService, ExportService>();

            // Registrar ViewModels e Pages (necessários em todas as configurações)
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<LoginPage>();

            builder.Services.AddTransient<StoreSelectionViewModel>();
            builder.Services.AddTransient<StoreSelectionPage>();

            builder.Services.AddTransient<ContagemListViewModel>();
            builder.Services.AddTransient<ContagemListPage>();

            builder.Services.AddTransient<ContagemViewModel>();
            builder.Services.AddTransient<ContagemPage>();

            builder.Services.AddTransient<MenuPage>();
            builder.Services.AddTransient<MainPage>();

            return builder;
        }
    }
}
