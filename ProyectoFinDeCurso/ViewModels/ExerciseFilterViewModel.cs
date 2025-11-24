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
        private bool _isLoaded;
        public bool IsLoaded => _isLoaded;
        private readonly DbService _dbService;
        private readonly userTypeEnum _userType;
        private List<Exercise> _cachedExercises = new();
        private Brush _auraColor;
        private string _nameRoutineFilter = string.Empty;
        private bodyPartEnum _bodyPartFilter = bodyPartEnum.nothing;
        private dificultyEnum _dificultyFilter = dificultyEnum.nothing;

        public ObservableCollection<ExerciseGroup> _filteredExercises { get; set; }
    = new ObservableCollection<ExerciseGroup>();

        public ObservableCollection<Exercise> Exercises { get; set; }
    = new ObservableCollection<Exercise>();


        public List<Exercise> CachedExercises
        {
            get => _cachedExercises;
            set
            {
                if (_cachedExercises != value)
                {
                    _cachedExercises = value;
                    OnPropertyChanged(nameof(AuraColor));
                }
            }
        }
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

        public ObservableCollection<ExerciseGroup> FilteredExercises
        {
            get => _filteredExercises;
            private set
            {
                _filteredExercises = value;
                OnPropertyChanged(nameof(FilteredExercises));
            }
        }



        public ExerciseFilterViewModel(DbService dbService, userTypeEnum userType)
        {
            _dbService = dbService;
            _userType = userType;

            Exercises = new ObservableCollection<Exercise>();
        }

        public async Task LoadExercisesAsync(bool forceReload = false)
        {

            if (_isLoaded && !forceReload)
                return;

            CachedExercises = await _dbService.GetExercisesCached();

            if (_userType == userTypeEnum.admin)
                CachedExercises.ForEach(e => e.IsAdmin = true);

            _isLoaded = true;

            UpdateFilteredExercises();
        }

        public void UpdateFilteredExercises()
        {
            var filtered = CachedExercises.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(NameRoutineFilter))
            {
                filtered = filtered.Where(r =>
                    r.name?.Contains(NameRoutineFilter, StringComparison.OrdinalIgnoreCase) == true);
            }

            if (BodyPartFilter != bodyPartEnum.nothing)
                filtered = filtered.Where(r => r.muscleGroupId == BodyPartFilter);

            if (DificultyFilter != dificultyEnum.nothing)
                filtered = filtered.Where(r => r.dificulty == DificultyFilter);

            var grouped = filtered
                .GroupBy(r => r.muscleGroupId)
                .Select(g => new ExerciseGroup(
                    g.Key,
                    g.OrderBy(e => e.dificulty)
                ));

            FilteredExercises.Clear();

            foreach (var g in grouped)
                FilteredExercises.Add(g);
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}