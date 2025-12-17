using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinDeCurso.Models
{
    public class RegisterLogging
    {
        [PrimaryKey, AutoIncrement, Column("registerID")]
        public int registerID { get; set; }// Identificador único del registro de rutinas en la base de datos
        [Column("routineID")] // identificador de la rutina en la base de datos
        public int routineID { get; set; } //identificador de la rutina
        [Column("userID")] //identificador del usuario en la base de datos
        public int userID { get; set; } //identificador del usuario
        [Column("day")]
        public string day { get; set; } = string.Empty; // Día del registro de la rutina en la base de datos
        [Column("TimeToStart")]
        public string timeToStart { get; set; } = string.Empty; // Hora de inicio del registro de la rutina en la base de datos
        [Column("TimeToEnd")]
        public string timeToEnd { get; set; } = string.Empty; // Hora de finalización del registro de la rutina en la base de datos

    }
}
