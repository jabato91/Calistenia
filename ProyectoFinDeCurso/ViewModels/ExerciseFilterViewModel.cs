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
                    : Exercises.Where(e => e.name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

                return filtered
                    .GroupBy(e => e.muscleGroupId)
                    .Select(g => new ExerciseGroup(g.Key, g.OrderBy(e => e.dificulty)))
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
        private async void LoadExercises()
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