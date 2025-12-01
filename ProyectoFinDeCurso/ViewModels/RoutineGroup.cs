using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinDeCurso.ViewModels
{
    public class RoutineGroup 
    {
        public bodyPartEnum MuscleGroupId { get; set; }
        public string DisplayName => MuscleGroupId.ToFriendlyName(); // traducción a español

        public ObservableCollection<Routines> Routines { get; set; } // actualiza los datos

        public RoutineGroup(bodyPartEnum muscleGroupId, IEnumerable<Routines> routines) // filtra por tipo de grupo muscular
        {
            MuscleGroupId = muscleGroupId;
            Routines = new ObservableCollection<Routines>(routines);
        }
    }
}
