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
        public int Id { get; set; }

        [Indexed]
        public int RoutineID { get; set; }

        [Indexed]
        public int ExerciseID { get; set; }

        [Column("sets")]
        public int sets { get; set; }
        [Column("Repetitions")]
        public int reps { get; set; }
        [Column("Secons")]
        public int seconds { get; set; } = -1;

    }
}
