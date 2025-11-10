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
        public bodyPartEnum _muscleGroupId { get; set; }
        public dificultyEnum _dificulty { get; set; }
        public string HeaderText => $"{DisplayPartName}   {DisplayDificultyName}";
        public string DisplayPartName => _muscleGroupId.ToFriendlyName();
        public string DisplayDificultyName => _dificulty.ToDificultyName();
        private string _searchText = string.Empty;

        public ExerciseAndDificultyGroup(bodyPartEnum muscleGroupId, dificultyEnum dificulty, IEnumerable<Exercise> exercises)
            : base(exercises)
        {
            _muscleGroupId = muscleGroupId;
            _dificulty = dificulty;
        }
        public Color HeaderColor => _dificulty switch
        {
            dificultyEnum.easy => Colors.Green,
            dificultyEnum.medium => Colors.Orange,
            dificultyEnum.hard => Colors.Red,
            dificultyEnum.extreme => Colors.Purple,
            _ => Colors.Gray
        };
    }
}
