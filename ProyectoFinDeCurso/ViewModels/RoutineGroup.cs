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
        public string DisplayName => MuscleGroupId.ToFriendlyName();

        public ObservableCollection<Routines> Routines { get; set; }

        public RoutineGroup(bodyPartEnum muscleGroupId, IEnumerable<Routines> routines)
        {
            MuscleGroupId = muscleGroupId;
            Routines = new ObservableCollection<Routines>(routines);
        }
    }
}
