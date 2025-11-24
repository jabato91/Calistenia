using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ProyectoFinDeCurso.ViewModels
{
    public class RoutinesFilterViewModel : INotifyPropertyChanged
    {
        private readonly DbService _dbService;
        private readonly userTypeEnum _userType;

        private string _nameRoutineFilter = string.Empty;
        private bodyPartEnum _bodyPartFilter = bodyPartEnum.nothing;
        private dificultyEnum _dificultyFilter = dificultyEnum.nothing;

        private IEnumerable<RoutineGroup> _filteredRoutines = new List<RoutineGroup>();

        public ObservableCollection<Routines> Routines { get; } = new();

        public bool Initialized { get; private set; } = false;
        public RoutinesFilterViewModel(DbService dbService, userTypeEnum userType)
        {
            _dbService = dbService;
            _userType = userType;

            _ = LoadRoutinesAsync(); // Mejor que async void
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
                    UpdateFilteredRoutines();
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
                    UpdateFilteredRoutines();
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
                    UpdateFilteredRoutines();
                }
            }
        }

        public IEnumerable<RoutineGroup> FilteredRoutines
        {
            get => _filteredRoutines;
            private set
            {
                _filteredRoutines = value;
                OnPropertyChanged(nameof(FilteredRoutines));
            }
        }

        public async Task LoadRoutinesAsync()
        {

            var routines = await _dbService.GetRoutinesCached();
            var routinesExercises = await _dbService.GetRoutinesExercisesCached();
            var exercises = await _dbService.GetExercisesCached();

            var userIdString = await SecureStorage.GetAsync("user_id");
            int userId = int.Parse(userIdString);

            var user = await _dbService.GetUserById(userId);

            var routinesUser = routines
                .Where(r => r.userID == user.UserID || r.userID == 0)
                .ToList();

            Routines.Clear();

            int adminFlag = (_userType == userTypeEnum.admin) ? 1 : 0;

            foreach (var routine in routinesUser)
            {
                routine.IsAdmin = adminFlag;

                var routineJoins = routinesExercises
                    .Where(re => re.RoutineID == routine.routineID)
                    .Join(
                        exercises,
                        re => re.ExerciseID,
                        ex => ex.execiseID,
                        
                        (re, ex) => new
                        {
                            Base = ex,
                            re.sets,
                            re.reps,
                            re.seconds
                        }
                    )
                    .ToList();

                foreach (var item in routineJoins)
                {
                    var copy = item.Base.Clone();
                    copy.sets = item.sets;
                    copy.reps = item.reps;
                    copy.seconds = item.seconds;
                    copy.exerciseFinished = false;
                    copy.expaded = false;

                    routine.Exercises.Add(copy);
                }

                Routines.Add(routine);
            }

            UpdateFilteredRoutines();
        }


        public void UpdateFilteredRoutines()
        {
            IEnumerable<Routines> filtered = Routines;

            if (!string.IsNullOrWhiteSpace(NameRoutineFilter))
            {
                filtered = filtered.Where(r =>
                    r.nameRoutine?.Contains(NameRoutineFilter, StringComparison.OrdinalIgnoreCase) == true);
            }

            if (BodyPartFilter != bodyPartEnum.nothing)
            {
                filtered = filtered.Where(r => r.muscleGroup == BodyPartFilter);
            }

            if (DificultyFilter != dificultyEnum.nothing)
            {
                filtered = filtered.Where(r => r.difficulty == DificultyFilter);
            }

            FilteredRoutines =
                filtered
                    .GroupBy(r => r.muscleGroup)
                    .Select(g => new RoutineGroup(g.Key, g))
                    .ToList();
        }
        
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}

