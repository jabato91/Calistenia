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
                }
            }
        }

        // Propiedad calculada para filtrar ejercicios sin vaciar la colección
        public IEnumerable<Exercise> FilteredExercises =>
            string.IsNullOrWhiteSpace(SearchText)
                ? Exercises
                : Exercises.Where(e => e.name.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase));

        public ExerciseFilterViewModel(DbService dbService)
        {
            _dbService = dbService;

            // Inicializa los ejercicios solo si no existen
            var initializer = new CreateExercises(_dbService);

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
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}