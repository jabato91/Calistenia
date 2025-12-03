using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using ProyectoFinDeCurso.Models;

namespace ProyectoFinDeCurso.Services
{
    public static class AlarmScheduler
    {
        public static void ScheduleAlarm(Alarm alarm) //programa la alarma semanal
        {
            DateTime now = DateTime.Now; //obtiene la hora y fecha actual
            DateTime nextTrigger = FindNextTriggerDay(alarm, now); // calcula la próxima vez que deba sonar la alarma

            var request = new NotificationRequest //crea la notificación programada
            {
                NotificationId = alarm.AlarmID,
                Title = "CalisSAPP",
                Description = $"¡Es hora de la rutina \"{alarm.Name}\"! 🏋️\nEs hora de entrenar!!!",

                Android = new AndroidOptions //crea y mantiene la notificación en el movil
                {
                    ChannelId = "alarm_channel", //usa el canal que notifica
                    AutoCancel = false, //se mantiene en pantalla hasta que el usuario lo elimine
                },

                Schedule = new NotificationRequestSchedule //programa el horario a la que sonará
                {
                    NotifyTime = nextTrigger, //fecha y hora a la que sonará la proxima vez
                    RepeatType = NotificationRepeat.Weekly, //Repite la notificación cada semana
                }
            };

            LocalNotificationCenter.Current.Show(request); //muestra la notificación
        }

        private static DateTime FindNextTriggerDay(Alarm alarm, DateTime now) //encuentra la próxima vez que sonará la alarma
        {
            bool[] days =
            {
                alarm.Monday,
                alarm.Tuesday,
                alarm.Wednesday,
                alarm.Thursday,
                alarm.Friday,
                alarm.Saturday,
                alarm.Sunday
            }; //son condicionales

            int today = (int)DateTime.Now.DayOfWeek - 1;
            if (today < 0) today = 6;

            for (int i = 0; i < 7; i++) //busca el próximo dia disponible
            {
                int d = (today + i) % 7;
                if (days[d])
                {
                    var candidate = now.Date.AddDays(i)
                        .AddHours(alarm.Hour)
                        .AddMinutes(alarm.Minute);

                    if (candidate > now)
                        return candidate;
                }
            }

            return now.AddMinutes(1);
        }
    }
}