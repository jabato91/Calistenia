using Android.App;
using Android.Content;
using Android.Provider;
using Microsoft.Maui.ApplicationModel;

namespace ProyectoFinDeCurso.Platforms.Android
{
    public static class AlarmPermissionService
    {
        public static void RequestExactAlarmPermission() // Solicita permiso para alarmas exactas
        {
            var alarmManager = (AlarmManager)Platform.AppContext
                .GetSystemService(Context.AlarmService); // Obtiene el servicio de alarmas

            if (!alarmManager.CanScheduleExactAlarms()) // Verifica si no tiene permiso para alarmas exactas
            {
                var intent = new Intent(Settings.ActionRequestScheduleExactAlarm); // Crea un intent para solicitar el permiso
                intent.AddFlags(ActivityFlags.NewTask); // Añade la bandera para iniciar una nueva tarea
                Platform.AppContext.StartActivity(intent); // Inicia la actividad para solicitar el permiso
            }
        }

        public static bool HasExactAlarmPermission() // Verifica si tiene permiso para alarmas exactas
        {
            var alarmManager = (AlarmManager)Platform.AppContext
                .GetSystemService(Context.AlarmService); // Obtiene el servicio de alarmas

            return alarmManager.CanScheduleExactAlarms(); // Retorna si puede programar alarmas exactas
        }
    }
}