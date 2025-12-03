using Android.App;
using Android.OS;
using Android.Runtime;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;

namespace ProyectoFinDeCurso
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership) // Constructor de la clase MainApplication
            : base(handle, ownership) // Llama al constructor base de MauiApplication
        {
        }

        public override void OnCreate() // Método que se llama al crear la aplicación
        {
            base.OnCreate(); // Llama al método base OnCreate

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O) // Verifica si la versión de Android es Oreo o superior
            {
                var nativeChannel = new NotificationChannel(
                    "alarm_channel",          // ID del canal
                    "Alarmas",                // Nombre visible
                    NotificationImportance.High
                ); // Crea un nuevo canal de notificación con alta importancia

                nativeChannel.EnableVibration(true); // Habilita la vibración para el canal
                nativeChannel.EnableLights(true); // Habilita las luces para el canal


                var manager = (NotificationManager)GetSystemService(NotificationService); // Obtiene el servicio de notificaciones
                manager.CreateNotificationChannel(nativeChannel); // Crea el canal de notificación en el sistema
            }

            var channels = new List<NotificationChannelRequest> // Crea una lista de solicitudes de canales de notificación
            {
                new NotificationChannelRequest // Define un canal de notificación
                {
                    Id = "alarm_channel",
                    Name = "Alarmas",
                    Importance = AndroidImportance.High,
                    EnableLights = true,
                    EnableVibration = true
                }
            };

            LocalNotificationCenter.CreateNotificationChannels(channels); // Crea los canales de notificación utilizando el centro de notificaciones local
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp(); // Crea la aplicación Maui
    }
}