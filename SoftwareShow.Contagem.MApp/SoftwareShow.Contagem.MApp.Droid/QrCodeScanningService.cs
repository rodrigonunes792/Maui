using SoftwareShow.Contagem.MApp.Interfaces;
using SoftwareShow.Contagem.MApp.Pages;
using ZXing.Net.Maui;
using Microsoft.Maui.Controls;
namespace SoftwareShow.Contagem.MApp
{
    public class QrCodeScanningService : IQrCodeScanningService
    {
        public async Task<string> ScanAsync()
        {
            var scanPage = new QrCodeScanPage();

            // Se você está usando Shell
            await Shell.Current.Navigation.PushModalAsync(scanPage);

            // Ou se não estiver usando Shell:
            // await Application.Current.MainPage.Navigation.PushModalAsync(scanPage);

            var result = await scanPage.WaitForScanResult();

            await Shell.Current.Navigation.PopModalAsync();
            // Ou: await Application.Current.MainPage.Navigation.PopModalAsync();

            return result ?? string.Empty;
        }
    }
}
