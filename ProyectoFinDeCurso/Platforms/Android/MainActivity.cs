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
                            | ConfigChanges.Density)] // Configuraciones para evitar reinicios innecesarios
    public class MainActivity : MauiAppCompatActivity 
    {
        protected override void OnCreate(Bundle savedInstanceState) // Método que se llama al crear la actividad
        {
            base.OnCreate(savedInstanceState); // Llama al método base OnCreate

            RequestedOrientation = ScreenOrientation.Portrait; // Fija la orientación de la pantalla en modo retrato

            if (OperatingSystem.IsAndroidVersionAtLeast(33)) // Verifica si la versión de Android es al menos 33 (Android 13)
            {
                if (CheckSelfPermission(Android.Manifest.Permission.PostNotifications) // Verifica si el permiso de notificaciones está concedido
                    != Permission.Granted) // Si el permiso no está concedido
                {
                    RequestPermissions(
                        new[] { Android.Manifest.Permission.PostNotifications },
                        0
                    ); // Solicita el permiso de notificaciones al usuario
                }
            }
        }
    }
}
