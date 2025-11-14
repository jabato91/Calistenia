using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;


namespace ProyectoFinDeCurso.ViewModels
{
   
    public class ListUsersViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>(); //Se usa la colección Oservable para notificar automáticamente a la interfaz gráfica
        private readonly DbService _dbService;
        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    OnPropertyChanged(nameof(FilteredExercisesByMail)); 
                }
            }
        }
        public IEnumerable<User> FilteredExercisesByMail
        {
            get
            {
                var filtered = string.IsNullOrWhiteSpace(SearchText)
                    ? Users
                    : Users.Where(e => e.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

                return filtered
                    .ToList();
            }
        }
        
        public ListUsersViewModel()
        {
            _dbService = new DbService(); 
            LoadUsersAsync();
            Users.CollectionChanged += (s, e) => OnPropertyChanged(nameof(FilteredExercisesByMail));
        }
        
        private async void LoadUsersAsync() //lee los usuarios dentro de la base de datos
        {
            var allUsers = await _dbService.GetUsers(); //se espera hasta que se lean todos los usuarios

            Users.Clear(); //elimina los datos anteriores registrados
            foreach (var user in allUsers) //guarda en la colección anterior creada
            {
                Users.Add(user);
            }
            OnPropertyChanged(nameof(FilteredExercisesByMail));
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
