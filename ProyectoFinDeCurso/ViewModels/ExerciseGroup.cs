using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinDeCurso.ViewModels
{
    public class ExerciseGroup : List<Exercise>
    {
        public bodyPartEnum MuscleGroupId { get; set; } // Identificador del grupo muscular
        public string DisplayName => MuscleGroupId.ToFriendlyName(); // Nombre amigable del grupo muscular

        public ExerciseGroup(bodyPartEnum muscleGroupId, IEnumerable<Exercise> exercises)
            : base(exercises) // ordena los ejercicios por grupo muscular
        {
            MuscleGroupId = muscleGroupId;
        }
    }
}
