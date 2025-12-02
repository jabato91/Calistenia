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
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            // === CANAL NATIVO ANDROID ===
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var nativeChannel = new NotificationChannel(
                    "alarm_channel",          // ID del canal
                    "Alarmas",                // Nombre visible
                    NotificationImportance.High
                );

                nativeChannel.EnableVibration(true);
                nativeChannel.EnableLights(true);


                var manager = (NotificationManager)GetSystemService(NotificationService);
                manager.CreateNotificationChannel(nativeChannel);
            }

            // === CANAL PARA EL PLUGIN LOCAL NOTIFICATION ===
            var channels = new List<NotificationChannelRequest>
            {
                new NotificationChannelRequest
                {
                    Id = "alarm_channel",
                    Name = "Alarmas",
                    Importance = AndroidImportance.High,
                    EnableLights = true,
                    EnableVibration = true
                }
            };

            LocalNotificationCenter.CreateNotificationChannels(channels);
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}