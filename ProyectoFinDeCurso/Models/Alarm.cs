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
        [PrimaryKey, AutoIncrement, Column("alarmID")]
        public int AlarmID { get; set; }

        // Nombre que ponga el usuario
        [Column("name")]
        public string Name { get; set; }

        // Hora y minutos
        [Column("hour")]
        public int Hour { get; set; }   // 0–23

        [Column("minute")]
        public int Minute { get; set; } // 0–59

        // Días de la semana (true/false)
        [Column("monday")]
        public bool Monday { get; set; }

        [Column("tuesday")]
        public bool Tuesday { get; set; }

        [Column("wednesday")]
        public bool Wednesday { get; set; }

        [Column("thursday")]
        public bool Thursday { get; set; }

        [Column("friday")]
        public bool Friday { get; set; }

        [Column("saturday")]
        public bool Saturday { get; set; }

        [Column("sunday")]
        public bool Sunday { get; set; }

        // Activada o no
        [Column("isActive")]
        public bool IsActive { get; set; } = true;
        [Column("userID")]
        public bool userID { get; set; } = true;
    }
}
