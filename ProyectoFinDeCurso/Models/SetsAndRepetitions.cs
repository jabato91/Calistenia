using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinDeCurso.Models
{
    public class SetsAndRepetitions
    {
        [PrimaryKey, AutoIncrement]
        public int setsAndRepsID { get; set; }

        [Indexed]
        public int RoutinesExercisesID { get; set; }

        
    }
}
