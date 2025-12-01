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
        private List<Exercise> _cachedExercises = new();
        private Brush _auraColor;
        private string _nameRoutineFilter = string.Empty;
        private bodyPartEnum _bodyPartFilter = bodyPartEnum.nothing;
        private dificultyEnum _dificultyFilter = dificultyEnum.nothing;

        public ObservableCollection<ExerciseGroup> _filteredExercises { get; set; } = new ObservableCollection<ExerciseGroup>(); //Actualiza la colección observable de grupos de ejercicios filtrados

        public ObservableCollection<Exercise> Exercises { get; set; } //almacena los ejercicios cargados desde la base de datos
    = new ObservableCollection<Exercise>();


        public List<Exercise> CachedExercises //almacena en caché la lista completa de ejercicios obtenidos de la base de datos
        {
            get => _cachedExercises;
            set
            {
                if (_cachedExercises != value)
                {
                    _cachedExercises = value;
                    OnPropertyChanged(nameof(CachedExercises));
                }
            }
        }
        public Brush AuraColor  //propiedad para el color del aura basado en la dificultad del ejercicio
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

        public string NameRoutineFilter //propiedad para el filtro de nombre de rutina
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

        public bodyPartEnum BodyPartFilter //propiedad para el filtro de grupo muscular
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

        public dificultyEnum DificultyFilter //propiedad para el filtro de dificultad
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

        public ObservableCollection<ExerciseGroup> FilteredExercises //propiedad para los ejercicios filtrados agrupados que actualiza la interfaz de usuario
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

            Exercises = new ObservableCollection<Exercise>(); // Inicializa la colección de ejercicios
        }

        public async Task LoadExercisesAsync() //método para cargar los ejercicios desde la base de datos
        {


            CachedExercises = await _dbService.GetExercisesAsync(); // Obtiene los ejercicios desde la base de datos y los almacena en caché

            if (_userType == userTypeEnum.admin) // Si el usuario es admin, marca todos los ejercicios como administradores
                CachedExercises.ForEach(e => e.IsAdmin = true);


            UpdateFilteredExercises(); // Actualiza la lista de ejercicios filtrados
        }

        public void UpdateFilteredExercises() //método para actualizar la lista de ejercicios filtrados según los criterios seleccionados
        {
            var filtered = CachedExercises.AsEnumerable(); // Comienza con todos los ejercicios en caché

            if (!string.IsNullOrWhiteSpace(NameRoutineFilter)) // Aplica el filtro de nombre si se ha especificado
            {
                filtered = filtered.Where(r =>
                    r.name?.Contains(NameRoutineFilter, StringComparison.OrdinalIgnoreCase) == true); // Filtra por nombre de rutina
            }

            if (BodyPartFilter != bodyPartEnum.nothing) // Aplica el filtro de grupo muscular si se ha especificado
                filtered = filtered.Where(r => r.muscleGroupId == BodyPartFilter); // Filtra por grupo muscular
             
            if (DificultyFilter != dificultyEnum.nothing) // Aplica el filtro de dificultad si se ha especificado
                filtered = filtered.Where(r => r.dificulty == DificultyFilter); // Filtra por dificultad

            var grouped = filtered
                .GroupBy(r => r.muscleGroupId)
                .Select(g => new ExerciseGroup(
                    g.Key,
                    g.OrderBy(e => e.dificulty)
                )); // Agrupa los ejercicios filtrados por grupo muscular y los ordena por dificultad dentro de cada grupo

            FilteredExercises.Clear(); // Limpia la colección observable de ejercicios filtrados

            foreach (var g in grouped) // Agrega cada grupo de ejercicios filtrados a la colección observable
                FilteredExercises.Add(g); // Actualiza la interfaz de usuario
        }


        public event PropertyChangedEventHandler? PropertyChanged; // Evento para notificar cambios en las propiedades
        private void OnPropertyChanged(string name) => // Método para invocar el evento PropertyChanged
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}