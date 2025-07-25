using SoftwareShow.Contagem.MApp.Interfaces;
using ZXing.Net.Maui.Controls;

namespace SoftwareShow.Contagem.MApp.Droid
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder.UseSharedMauiApp().UseBarcodeReader();

#if ANDROID
            builder.Services.AddTransient<IOrientationService, OrientationService>();


#endif

            return builder.Build();
        }
    }
}
