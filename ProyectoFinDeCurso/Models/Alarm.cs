using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    namespace ProyectoFinDeCurso.Models
    {
        public class Alarm
        {
            [PrimaryKey, AutoIncrement, Column("alarmID")] //id de la alarma en la base de datos
            public int AlarmID { get; set; } //id de la alarma

           
            [Column("name")] //nombre de la alarma en la base de datos
            public string Name { get; set; } //nombre de la alarma

            // Hora y minutos
            [Column("hour")] //nombre de la hora de la alarma en la base de datos
            public int Hour { get; set; }   //hora de la alarma

            [Column("minute")] // minuto de la alarma en la base de datos
            public int Minute { get; set; } //minutos de la alarma en la base de datos

            [Column("monday")] //condición para activar el dia de la alarma en la base de datos
            public bool Monday { get; set; } = false; //condición del día

        [Column("tuesday")]//condición para activar el dia de la alarma en la base de datos
        public bool Tuesday { get; set; } = false;//condición del día

        [Column("wednesday")]//condición para activar el dia de la alarma en la base de datos
        public bool Wednesday { get; set; } = false;//condición del día

        [Column("thursday")]//condición para activar el dia de la alarma en la base de datos
        public bool Thursday { get; set; } = false;//condición del día

        [Column("friday")]//condición para activar el dia de la alarma en la base de datos
        public bool Friday { get; set; } = false;//condición del día

        [Column("saturday")]//condición para activar el dia de la alarma en la base de datos
        public bool Saturday { get; set; } = false;//condición del día

        [Column("sunday")]//condición para activar el dia de la alarma en la base de datos
        public bool Sunday { get; set; } = false;//condición del día

        [Column("userID")] //asignar alarma al usuario
            public int userID { get; set; } = -1;

            [Ignore]
            public string FormattedTime => $"{Hour:D2}:{Minute:D2}"; //muestra la hora en un formato
        }
}
