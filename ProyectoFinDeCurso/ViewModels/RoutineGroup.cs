using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinDeCurso.ViewModels
{
    public class RoutineGroup : List<Routines>
    {
        public bodyPartEnum MuscleGroupId { get; set; }

        // 👇 Esto usa tu método de extensión automáticamente
        public string DisplayName => MuscleGroupId.ToFriendlyName();

        public RoutineGroup(bodyPartEnum muscleGroupId, IEnumerable<Routines> routine)
            : base(routine)
        {
            MuscleGroupId = muscleGroupId;
        }
    }
}
