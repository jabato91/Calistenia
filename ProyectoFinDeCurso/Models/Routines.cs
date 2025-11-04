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
        [Column("typeUser")]
        public userTypeEnum typeUser { get; set; } = userTypeEnum.nothing;

        [Ignore] 
        public ObservableCollection<Exercise> Exercises { get; set; } = new(); //obtiene los ejercicios de la rutina
        [Ignore]
        public int IsAdmin { get; set; } = -1; // Indica si el ejercicio está siendo visto en modo administrador

        public bool IsAdminMode
        {
            get
            {
                return IsAdmin switch
                {
                    0 => false,
                    1 => true,
                    2 => true,
                    _ => throw new NotImplementedException(),
                };
            }
        }
    }
}
