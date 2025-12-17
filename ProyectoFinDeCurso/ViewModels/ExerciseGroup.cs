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

        public string DisplayName
        {
            get
            {
                try
                {
                    return MuscleGroupId.ToFriendlyName(); // Nombre amigable del grupo muscular
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERROR DisplayName] " + ex.Message);
                    return string.Empty;
                }
            }
        }

        public ExerciseGroup(bodyPartEnum muscleGroupId, IEnumerable<Exercise> exercises)
            : base(SafeEnumerable(exercises)) // ordena los ejercicios por grupo muscular
        {
            try
            {
                MuscleGroupId = muscleGroupId;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR constructor ExerciseGroup] " + ex.Message);
            }
        }

        private static IEnumerable<Exercise> SafeEnumerable(IEnumerable<Exercise> exercises)
        {
            try
            {
                return exercises ?? Enumerable.Empty<Exercise>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR SafeEnumerable] " + ex.Message);
                return Enumerable.Empty<Exercise>();
            }
        }
    }
}
