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

        public async void LoadRoutines()
        {
            // 🔹 Cargar datos desde la base de datos
            var routinesFromDb = await _dbService.GetRoutines();
            var routinesExercisesFromDb = await _dbService.GetRoutinesExercises();
            var exercisesFromDb = await _dbService.GetEercises();

            // Limpiar la colección actual
            Routines.Clear();

            // 🔹 Recorrer todas las rutinas obtenidas
            foreach (var routine in routinesFromDb)
            {
                // Establecer modo admin/usuario según el tipo
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

                // 🔹 Obtener los ejercicios asociados a la rutina desde la tabla intermedia
                // 🔹 Obtener los ejercicios que pertenecen a la rutina actual
                var exercisesForRoutine = routinesExercisesFromDb
                    .Where(re => re.RoutineID == routine.routineID)
                    .Join(exercisesFromDb,
                          re => re.ExerciseID,
                          ex => ex.execiseID,
                          (re, ex) => new
                          {
                              Exercise = ex,
                              Sets = re.sets,
                              Reps = re.reps,
                              Seconds = re.seconds
                          })
                    .ToList();

                // 🔹 Agregar ejercicios clonados a la rutina
                foreach (var ex in exercisesForRoutine)
                {
                    // Crear copia independiente del ejercicio base
                    var exerciseCopy = ex.Exercise.Clone();

                    // Asignar los valores específicos de la rutina
                    exerciseCopy.sets = ex.Sets;
                    exerciseCopy.reps = ex.Reps;
                    exerciseCopy.seconds = ex.Seconds;

                    // Inicializar propiedades dinámicas de control
                    exerciseCopy.exerciseFinished = false;
                    exerciseCopy.expaded = false;

                    // Agregar a la lista de ejercicios de la rutina
                    routine.Exercises.Add(exerciseCopy);
                }

                // Agregar la rutina final a la colección principal
                Routines.Add(routine);
            }

            // Notificar que la propiedad cambió (por si hay filtros)
            OnPropertyChanged(nameof(FilteredRoutines));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


    }
    
}

