using Android.App;
using Android.Content;
using Android.Provider;
using Microsoft.Maui.ApplicationModel;

namespace ProyectoFinDeCurso.Platforms.Android
{
    public static class AlarmPermissionService
    {
        public static void RequestExactAlarmPermission()
        {
            var alarmManager = (AlarmManager)Platform.AppContext
                .GetSystemService(Context.AlarmService);

            if (!alarmManager.CanScheduleExactAlarms())
            {
                var intent = new Intent(Settings.ActionRequestScheduleExactAlarm);
                intent.AddFlags(ActivityFlags.NewTask);
                Platform.AppContext.StartActivity(intent);
            }
        }

        public static bool HasExactAlarmPermission()
        {
            var alarmManager = (AlarmManager)Platform.AppContext
                .GetSystemService(Context.AlarmService);

            return alarmManager.CanScheduleExactAlarms();
        }
    }
}