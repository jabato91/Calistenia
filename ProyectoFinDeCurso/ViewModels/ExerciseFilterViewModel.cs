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
        private readonly userTypeEnum _userType;

        private Brush _auraColor;
        private string _nameRoutineFilter = string.Empty;
        private bodyPartEnum _bodyPartFilter = bodyPartEnum.nothing;
        private dificultyEnum _dificultyFilter = dificultyEnum.nothing;

        private IEnumerable<ExerciseGroup> _filteredExercises = new List<ExerciseGroup>();

        public ObservableCollection<Exercise> Exercises { get; set; }



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
                    UpdateFilteredExercises();
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
                    OnPropertyChanged(nameof(BodyPartFilter));
                    UpdateFilteredExercises();
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
                    OnPropertyChanged(nameof(DificultyFilter));
                    UpdateFilteredExercises();
                }
            }
        }

        public IEnumerable<ExerciseGroup> FilteredExercises
        {
            get => _filteredExercises;
            private set
            {
                _filteredExercises = value;
                OnPropertyChanged(nameof(FilteredExercises));
            }
        }


        public bool Initialized { get; private set; } = false;


        public ExerciseFilterViewModel(DbService dbService, userTypeEnum userType)
        {
            _dbService = dbService;
            _userType = userType;

            Exercises = new ObservableCollection<Exercise>();
        }


        public async Task LoadExercisesAsync()
        {
            if (Initialized)
                return;

            Initialized = true;

            var exercises = await _dbService.GetExercisesCached();


            if (_userType == userTypeEnum.admin)
                exercises.ForEach(ex => ex.IsAdmin = true);

            Exercises.Clear();

            foreach (var ex in exercises)
                Exercises.Add(ex);

            UpdateFilteredExercises();
        }


        public void UpdateFilteredExercises()
        {
            IEnumerable<Exercise> filtered = Exercises;

            if (!string.IsNullOrWhiteSpace(NameRoutineFilter))
            {
                filtered = filtered.Where(r =>
                    r.name?.Contains(NameRoutineFilter, StringComparison.OrdinalIgnoreCase) == true);
            }

            if (BodyPartFilter != bodyPartEnum.nothing)
            {
                filtered = filtered.Where(r => r.muscleGroupId == BodyPartFilter);
            }

            if (DificultyFilter != dificultyEnum.nothing)
            {
                filtered = filtered.Where(r => r.dificulty == DificultyFilter);
            }

            FilteredExercises =
                filtered
                    .GroupBy(r => r.muscleGroupId)
                    .Select(g =>
                        new ExerciseGroup(
                            g.Key,
                            g.OrderBy(r => r.dificulty)
                        )
                    )
                    .ToList();
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}