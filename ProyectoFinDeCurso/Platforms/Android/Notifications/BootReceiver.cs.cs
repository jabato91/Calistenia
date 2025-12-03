using Android.App;
using Android.Content;
using Android.App;
using Android.Content;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Services;
using SQLite;
using Microsoft.Maui.Storage;
using System.IO;

namespace ProyectoFinDeCurso.Platforms.Android
{
    [BroadcastReceiver(Enabled = true, Exported = true)] //permite ejecutar aún que la aplicación esté abierta
    [IntentFilter(new[] { Intent.ActionBootCompleted, Intent.ActionReboot })] //registra eventos que se unsarán en el dispositivo
    public class BootReceiver : BroadcastReceiver //hereda esta clase que permite llamar a la app aún que no esté abierta
    {
        public override void OnReceive(Context context, Intent intent) //ejecuta cuando android lanza un evento
        {
            if (intent.Action == Intent.ActionBootCompleted ||
            intent.Action == Intent.ActionReboot)
            {
                int userId = Preferences.Get("lastUserId", -1);
                if (userId == -1)
                    return;

                string dbPath = Path.Combine(FileSystem.AppDataDirectory, "DbService.db3");

                var connection = new SQLiteConnection(dbPath);
                connection.CreateTable<Alarm>();

                var alarms = connection.Table<Alarm>()
                                       .Where(a => a.userID == userId)
                                       .ToList();

                foreach (var alarm in alarms)
                {
                    AlarmScheduler.ScheduleAlarm(alarm);
                }
            }
        }
    }
}