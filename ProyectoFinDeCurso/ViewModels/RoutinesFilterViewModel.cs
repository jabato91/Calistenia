using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinDeCurso.ViewModels
{
    public class RoutinesFilterViewModel : INotifyPropertyChanged
    {
        private readonly DbService _dbService;
        private string _searchText = string.Empty;
        private static userTypeEnum _userType;
        public ObservableCollection<Routines> Routines { get; set; } = new();

        public RoutinesFilterViewModel(DbService dbService, userTypeEnum userType)
        {
            _userType = userTypeEnum.nothing;
            _dbService = dbService;
            _userType = userType;
            LoadRoutines();
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    OnPropertyChanged(nameof(FilteredRoutines));
                }
            }
        }

        // 🔹 Agrupación por parte del cuerpo
        public IEnumerable<RoutineGroup> FilteredRoutines
        {
            get
            {
                var filtered = string.IsNullOrWhiteSpace(SearchText)
                    ? Routines
                    : Routines.Where(r => r.nameRoutine.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

                return filtered
                    .GroupBy(r => r.muscleGroup)
                    .Select(g => new RoutineGroup(g.Key, g))
                    .ToList();
            }
        }

        private async void LoadRoutines()
        {
            var routinesFromDb = await _dbService.GetRoutines();
            var routinesExercisesFromDb = await _dbService.GetRoutinesExercises();
            var exercisesFromDb = await _dbService.GetEercises();

            Routines.Clear();

            foreach (var routine in routinesFromDb)
            {
                if (routine.typeUser.Equals(_userType) && _userType.Equals(userTypeEnum.admin))
                {
                    routine.IsAdmin = 1;
                }
                else if (!routine.typeUser.Equals(_userType) && _userType.Equals(userTypeEnum.user))
                {
                    routine.IsAdmin = 0;
                }
                else
                {
                    routine.IsAdmin = 2;
                }
                var exercisesForRoutine = routinesExercisesFromDb
                    .Where(re => re.RoutineID == routine.routineID)
                    .Join(exercisesFromDb,
                          re => re.ExerciseID,
                          ex => ex.execiseID,
                          (re, ex) => new
                          {
                              Exercise = ex,
                              Sets = re.sets,
                              Reps = re.reps
                          })
                          .ToList();

                foreach (var ex in exercisesForRoutine)
                {
                    
                    ex.Exercise.sets = ex.Sets;
                    ex.Exercise.reps = ex.Reps;
                    routine.Exercises.Add(ex.Exercise);
                }
                foreach(var ex in routine.Exercises)
                {
                    if(ex == routine.Exercises.Last())
                    {
                        ex.LinePattern = new DoubleCollection() {  };
                    }
                }

                Routines.Add(routine);
            }

            OnPropertyChanged(nameof(FilteredRoutines));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
