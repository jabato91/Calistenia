using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ProyectoFinDeCurso.ViewModels
{
    public class RoutinesFilterViewModel : INotifyPropertyChanged
    {
        private readonly DbService _dbService; // Servicio de base de datos
        private readonly userTypeEnum _userType; // Tipo de usuario (admin, user, etc.)

        private string _nameRoutineFilter = string.Empty; // Filtro por nombre de rutina
        private bodyPartEnum _bodyPartFilter = bodyPartEnum.nothing; // Filtro por grupo muscular
        private dificultyEnum _dificultyFilter = dificultyEnum.nothing; // Filtro por dificultad

        private IEnumerable<RoutineGroup> _filteredRoutines = new List<RoutineGroup>(); // Rutinas filtradas agrupadas por grupo muscular

        public ObservableCollection<Routines> Routines { get; } = new(); // Colección de todas las rutinas

        public bool Initialized { get; private set; } = false; // Indica si el ViewModel ha sido inicializado
        public RoutinesFilterViewModel(DbService dbService, userTypeEnum userType) // Constructor
        {
            _dbService = dbService;
            _userType = userType;
        }

        
        public string NameRoutineFilter // Filtro por nombre de rutina
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

        public bodyPartEnum BodyPartFilter // Filtro por grupo muscular
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

        public dificultyEnum DificultyFilter // Filtro por dificultad
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

        public IEnumerable<RoutineGroup> FilteredRoutines // Agrupado por grupo muscular
        {
            get => _filteredRoutines;
            private set
            {
                _filteredRoutines = value;
                OnPropertyChanged(nameof(FilteredRoutines));
            }
        }


        public async Task LoadRoutinesAsync() // Carga las rutinas desde la base de datos
        {
            var routines = await _dbService.GetRoutinesAsync(); // Obtiene todas las rutinas
            var routinesExercises = await _dbService.GetRoutinesExercisesAsync(); // obtiene las rutinas-ejercicios
            var exercises = await _dbService.GetExercisesAsync(); // obtiene los ejercicios

            var userIdString = await SecureStorage.GetAsync("user_id");//obtiene la id del usuario
            int userId = int.Parse(userIdString); //combierte la id a una variable entera

            var user = await _dbService.GetUserById(userId); //obtiene el usuario de la base de datos

            var routinesUser = routines
                .Where(r => r.userID == user.UserID || r.userID == 0)
                .ToList(); // obtiene la rutiene dependiendo de si la id es igual a la id del usuario o si es 0

            Routines.Clear(); //borra los datos de la routina

            int adminFlag = (_userType == userTypeEnum.admin) ? 1 : 0; // mira si el usuario es admin

            foreach (var routine in routinesUser) //obtiene los ejercicios de las rutinas
            {
                routine.IsAdmin = adminFlag; //recoge si es administrador

                routine.Exercises = new ObservableCollection<Exercise>(); //crea una nueva lista de ejercicios

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
                    .ToList(); // obtiene los ejercicios cuando la id de la rutina es la misma que la del ejercicio

                foreach (var item in routineJoins) //alamacena los ejercicios clonándolos
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

            UpdateFilteredRoutines(); //actualiza los datos
        }


        public void UpdateFilteredRoutines() //filtra las rutinas
        {
            IEnumerable<Routines> filtered = Routines; 

            if (!string.IsNullOrWhiteSpace(NameRoutineFilter)) //verifica si Name no es null
            {
                filtered = filtered.Where(r =>
                    r.nameRoutine?.Contains(NameRoutineFilter, StringComparison.OrdinalIgnoreCase) == true); //filtra por nombre
            }

            if (BodyPartFilter != bodyPartEnum.nothing) //verifica si la parte del cuerpo no es nada
            {
                filtered = filtered.Where(r => r.muscleGroup == BodyPartFilter); //filtra por parte del cuerpo
            }

            if (DificultyFilter != dificultyEnum.nothing)//verifica si la dificultad no es nada
            {
                filtered = filtered.Where(r => r.difficulty == DificultyFilter); //filtra por dificultad
            }

            FilteredRoutines =
                filtered
                    .GroupBy(r => r.muscleGroup)
                    .Select(g => new RoutineGroup(g.Key, g))
                    .ToList(); //filtra las rutinas
        }
        
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}

