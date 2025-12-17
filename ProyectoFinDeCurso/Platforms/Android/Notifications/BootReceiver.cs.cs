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
            intent.Action == Intent.ActionReboot) //si el evento es de arranque o reinicio
            {
                int userId = Preferences.Get("lastUserId", -1); //recoge el id del último usuario que inició sesión
                if (userId == -1) //si no hay usuario logueado
                    return;

                string dbPath = Path.Combine(FileSystem.AppDataDirectory, "DbService.db3"); //ruta de la base de datos

                var connection = new SQLiteConnection(dbPath); //crea la conexión a la base de datos
                connection.CreateTable<Alarm>(); //asegura que la tabla de alarmas existe

                var alarms = connection.Table<Alarm>()
                                       .Where(a => a.userID == userId) //filtra las alarmas del usuario logueado
                                       .ToList();

                foreach (var alarm in alarms) //por cada alarma del usuario
                {
                    AlarmScheduler.ScheduleAlarm(alarm); //programa la alarma en el sistema
                }
            }
        }
    }
}