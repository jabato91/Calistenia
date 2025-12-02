using Android.App;
using Android.Content.PM;
using Android.OS;

namespace ProyectoFinDeCurso
{
    [Activity(
        Theme = "@style/Maui.SplashTheme",
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges = ConfigChanges.ScreenSize
                            | ConfigChanges.Orientation
                            | ConfigChanges.UiMode
                            | ConfigChanges.ScreenLayout
                            | ConfigChanges.SmallestScreenSize
                            | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // 🔒 BLOQUEAR ORIENTACIÓN A VERTICAL (PORTRAIT)
            RequestedOrientation = ScreenOrientation.Portrait;

            // 🔔 Permiso notificaciones Android 13+
            if (OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                if (CheckSelfPermission(Android.Manifest.Permission.PostNotifications)
                    != Permission.Granted)
                {
                    RequestPermissions(
                        new[] { Android.Manifest.Permission.PostNotifications },
                        0
                    );
                }
            }
        }
    }
}
