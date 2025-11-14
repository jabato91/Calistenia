#nullable enable
using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace ProyectoFinDeCurso.ViewModels
{
    public class ExerciseFilterViewModel : INotifyPropertyChanged
    {
        private readonly DbService _dbService;
        private string _searchText = string.Empty;
        private static dificultyEnum _dificultyFilter = dificultyEnum.nothing;
        private static bodyPartEnum _bodyPartFilter = bodyPartEnum.nothing;
        private static String _nameRoutineFilter = string.Empty;
        private Brush _auraColor;
        private static userTypeEnum _userType;
        // Lista completa de ejercicios
        public ObservableCollection<Exercise> Exercises { get; set; } = new();

        public Brush AuraColor
        {
            get => _auraColor;
            set
            {
                if (_auraColor != value)
                {
                    _auraColor = value;
                    OnPropertyChanged(nameof(AuraColor));
                }
            }
        }

       

        
        public string NameRoutineFilter
        {
            get => _nameRoutineFilter;
            set
            {
                if (_nameRoutineFilter != value)
                {
                    _nameRoutineFilter = value;
                    OnPropertyChanged(nameof(NameRoutineFilter));
                    OnPropertyChanged(nameof(FilteredExercises));
                }
            }
        }
        public bodyPartEnum BodyPartFilter
        {
            get => _bodyPartFilter;
            set
            {
                if (_bodyPartFilter != value)
                {
                    _bodyPartFilter = value;
                    OnPropertyChanged(nameof(_bodyPartFilter));
                    OnPropertyChanged(nameof(FilteredExercises));
                }
            }
        }
        public dificultyEnum DificultyFilter
        {
            get => _dificultyFilter;
            set
            {
                if (_dificultyFilter != value)
                {
                    _dificultyFilter = value;
                    OnPropertyChanged(nameof(_dificultyFilter));
                    OnPropertyChanged(nameof(FilteredExercises));
                }
            }
        }
        public IEnumerable<ExerciseGroup> FilteredExercises
        {
            get
            {
                IEnumerable<Exercise> filtered = Exercises;

                if (!string.IsNullOrWhiteSpace(NameRoutineFilter))
                {
                    filtered = filtered.Where(r =>
                        r.name?.Contains(NameRoutineFilter, StringComparison.OrdinalIgnoreCase) ?? false);
                }

                if (BodyPartFilter != bodyPartEnum.nothing)
                {
                    filtered = filtered.Where(r => r.muscleGroupId == BodyPartFilter);
                }

                if (DificultyFilter != dificultyEnum.nothing)
                {
                    filtered = filtered.Where(r => r.dificulty == DificultyFilter);
                }

                return filtered
                    .GroupBy(r => r.muscleGroupId)
                    .Select(g => new ExerciseGroup(
                        g.Key,
                        g.OrderBy(r => r.dificulty)   
                    ))
                    .ToList();
            }
        }
        public int ExercisesCount => Exercises?.Count ?? 0;
        public ExerciseFilterViewModel(DbService dbService, userTypeEnum userType)
        {
            _userType = userTypeEnum.nothing;
            _dbService = dbService;

            _userType = userType;

            Exercises.CollectionChanged += (s, e) => OnPropertyChanged(nameof(ExercisesCount));


            LoadExercises();
        }

        // Carga los ejercicios desde la base de datos
        public async void LoadExercises()
        {
            var exercises = await _dbService.GetEercises();
            
            foreach (var ex in exercises) { 
                if(ex.typeUser.Equals(_userType) && _userType.Equals(userTypeEnum.admin))
                {
                    ex.IsAdmin = 1;
                }
                else if(!ex.typeUser.Equals(_userType) && _userType.Equals(userTypeEnum.user))
                {
                    ex.IsAdmin = 0;
                }
                else
                {
                    ex.IsAdmin = 2;
                }

                    Exercises.Add(ex);

                
            }
            // Notificar que FilteredExercises cambió al cargar
            OnPropertyChanged(nameof(FilteredExercises));
            OnPropertyChanged(nameof(ExercisesCount));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        
    }
}