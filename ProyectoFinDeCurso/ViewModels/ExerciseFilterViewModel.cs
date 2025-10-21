#nullable enable
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

        // Lista completa de ejercicios
        public ObservableCollection<Exercise> Exercises { get; set; } = new();

        // Texto de búsqueda
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    OnPropertyChanged(nameof(FilteredExercises)); // actualizar la vista
                    OnPropertyChanged(nameof(ExercisesCount));
                }
            }
        }

        // Propiedad calculada para filtrar ejercicios sin vaciar la colección
        public IEnumerable<ExerciseGroup> FilteredExercises
        {
            get
            {
                var filtered = string.IsNullOrWhiteSpace(SearchText)
                    ? Exercises
                    : Exercises.Where(e =>
                        e.name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

                return filtered
                    .GroupBy(e => e.muscleGroupId)
                    .Select(g => new ExerciseGroup(g.Key, g))
                    .ToList();
            }
        }
        public int ExercisesCount => Exercises?.Count ?? 0;
        public ExerciseFilterViewModel(DbService dbService)
        {
            _dbService = dbService;

            // Inicializa los ejercicios solo si no existen
            var initializer = new CreateExercises(_dbService);

            Exercises.CollectionChanged += (s, e) => OnPropertyChanged(nameof(ExercisesCount));


            LoadExercises();
        }

        // Carga los ejercicios desde la base de datos
        private async void LoadExercises()
        {
            var exercises = await _dbService.GetEercises();

            foreach (var ex in exercises)
                Exercises.Add(ex);

            // Notificar que FilteredExercises cambió al cargar
            OnPropertyChanged(nameof(FilteredExercises));
            OnPropertyChanged(nameof(ExercisesCount));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}