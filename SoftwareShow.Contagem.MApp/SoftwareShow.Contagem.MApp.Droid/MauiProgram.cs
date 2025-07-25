using SoftwareShow.Contagem.MApp.Interfaces;
using ZXing.Net.Maui.Controls;

namespace SoftwareShow.Contagem.MApp.Droid
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            try
            {
                var builder = MauiApp.CreateBuilder();

                builder.UseSharedMauiApp().UseBarcodeReader();

#if ANDROID
                builder.Services.AddTransient<IOrientationService, OrientationService>();
#endif

                return builder.Build();
            }
            catch (Exception ex)
            {
                // Log do erro para debug
                System.Diagnostics.Debug.WriteLine($"Erro na criação da aplicação MAUI: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                
                // Re-throw para que o sistema possa lidar com o erro
                throw;
            }
        }
    }
}
