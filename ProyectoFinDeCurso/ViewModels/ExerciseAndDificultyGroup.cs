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
        public string HeaderText => $"{DisplayPartName}   {DisplayDificultyName}"; // Texto del encabezado que combina el nombre del grupo muscular y la dificultad
        public string DisplayPartName => _muscleGroupId.ToFriendlyName(); // Nombre traducido del grupo muscular
        public string DisplayDificultyName => _dificulty.ToDificultyName(); // Nombre traducido de la dificultad
        private string _searchText = string.Empty; // Texto de búsqueda para filtrar ejercicios

        public ExerciseAndDificultyGroup(bodyPartEnum muscleGroupId, dificultyEnum dificulty, IEnumerable<Exercise> exercises) // Constructor que inicializa el grupo con el grupo muscular, dificultad y ejercicios
            : base(exercises)
        {
            _muscleGroupId = muscleGroupId;
            _dificulty = dificulty;
        }
        public Color HeaderColor => _dificulty switch // Propiedad calculada para obtener el color del encabezado según la dificultad
        {
            dificultyEnum.easy => Colors.Green,
            dificultyEnum.medium => Colors.Orange,
            dificultyEnum.hard => Colors.Red,
            dificultyEnum.extreme => Colors.Purple,
            _ => Colors.Gray
        };
    }
}
