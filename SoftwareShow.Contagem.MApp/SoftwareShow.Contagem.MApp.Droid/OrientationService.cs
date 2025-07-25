using Android.Content.PM;
using Microsoft.Maui.Controls;
using SoftwareShow.Contagem.MApp.Droid;
using SoftwareShow.Contagem.MApp.Interfaces;

[assembly: Dependency(typeof(OrientationService))]

namespace SoftwareShow.Contagem.MApp.Droid
{
    public class OrientationService : IOrientationService
    {
        public void ForcePortrait()
        {
            MainActivity.Current?.RunOnUiThread(() =>
            {
                MainActivity.Current.RequestedOrientation = ScreenOrientation.Portrait;
            });
        }

        public void ForceLandscape()
        {
            MainActivity.Current?.RunOnUiThread(() =>
            {
                MainActivity.Current.RequestedOrientation = ScreenOrientation.Landscape;
            });
        }

        public void AllowAllOrientations()
        {
            MainActivity.Current?.RunOnUiThread(() =>
            {
                MainActivity.Current.RequestedOrientation = ScreenOrientation.Unspecified;
            });
        }
    }
}
