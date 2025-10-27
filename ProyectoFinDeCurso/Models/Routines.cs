using ProyectoFinDeCurso.Enums;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinDeCurso.Models
{
    public class Routines
    {
        [PrimaryKey, AutoIncrement, Column("routineID")]
        public int routineID { get; set; }
        [Column("NameRoutine")]
        public string nameRoutine { get; set; } = string.Empty;
        [Column("DescriptionRoutine")]
        public string description { get; set; } = string.Empty;
        [Column("imageRoutine")]
        public string image { get; set; } = string.Empty;
        [Column("bodyPartRoutine")]
        public bodyPartEnum muscleGroup { get; set; } = bodyPartEnum.nothing;
        [Ignore] // Ignorar al guardar en DB
        public ObservableCollection<Exercise> Exercises { get; set; } = new();

        
    }
}
