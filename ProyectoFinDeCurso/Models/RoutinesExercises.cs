using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinDeCurso.Models
{
    public class RoutinesExercises
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } // Identificador único de la relación rutina-ejercicio

        [Indexed]
        public int RoutineID { get; set; } // Identificador de la rutina

        [Indexed]
        public int ExerciseID { get; set; } // Identificador del ejercicio

        [Column("sets")]
        public int sets { get; set; } // Número de series en la base de datos
        [Column("Repetitions")]
        public int reps { get; set; } // Número de repeticiones en la base de datos
        [Column("Secons")]
        public int seconds { get; set; } = -1; // Número de segundos en la base de datos

    }
}
