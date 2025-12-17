using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinDeCurso.ViewModels
{
    class ExerciseAndDificultyGroup : List<Exercise>
    {
        public bodyPartEnum _muscleGroupId { get; set; } // Grupo muscular del ejercicio
        public dificultyEnum _dificulty { get; set; } // Dificultad del ejercicio
        public string HeaderText
        {
            get
            {
                try
                {
                    return $"{DisplayPartName}   {DisplayDificultyName}"; // Texto del encabezado que combina el nombre del grupo muscular y la dificultad
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERROR HeaderText] " + ex.Message);
                    return string.Empty;
                }
            }
        }

        public string DisplayPartName
        {
            get
            {
                try
                {
                    return _muscleGroupId.ToFriendlyName(); // Nombre traducido del grupo muscular
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERROR DisplayPartName] " + ex.Message);
                    return string.Empty;
                }
            }
        }

        public string DisplayDificultyName
        {
            get
            {
                try
                {
                    return _dificulty.ToDificultyName(); // Nombre traducido de la dificultad
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERROR DisplayDificultyName] " + ex.Message);
                    return string.Empty;
                }
            }
        }

        private string _searchText = string.Empty; // Texto de búsqueda para filtrar ejercicios

        public ExerciseAndDificultyGroup(bodyPartEnum muscleGroupId, dificultyEnum dificulty, IEnumerable<Exercise> exercises) // Constructor que inicializa el grupo con el grupo muscular, dificultad y ejercicios
            : base(exercises)
        {
            try
            {
                _muscleGroupId = muscleGroupId;
                _dificulty = dificulty;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR constructor ExerciseAndDificultyGroup] " + ex.Message);
            }
        }

        public Color HeaderColor
        {
            get
            {
                try
                {
                    return _dificulty switch // Propiedad calculada para obtener el color del encabezado según la dificultad
                    {
                        dificultyEnum.easy => Colors.Green,
                        dificultyEnum.medium => Colors.Orange,
                        dificultyEnum.hard => Colors.Red,
                        dificultyEnum.extreme => Colors.Purple,
                        _ => Colors.Gray
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERROR HeaderColor] " + ex.Message);
                    return Colors.Gray;
                }
            }
        }
    }
}
