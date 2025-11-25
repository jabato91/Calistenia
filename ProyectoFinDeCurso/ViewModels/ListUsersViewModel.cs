using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;


namespace ProyectoFinDeCurso.ViewModels
{
    public class ListUsersViewModel : INotifyPropertyChanged
    {
        private readonly DbService _dbService;

        public ObservableCollection<User> Users { get; } = new(); // Colección observable de usuarios

        private string _searchText = string.Empty;
        public string SearchText // Texto de búsqueda por el correo electrónico
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    UpdateFilteredUsers();
                }
            }
        }

        private IEnumerable<User> _filteredUsers = new List<User>(); 
        public IEnumerable<User> FilteredUsers // Usuarios filtrados según el texto de búsqueda
        {
            get => _filteredUsers;
            private set
            {
                _filteredUsers = value;
                OnPropertyChanged(nameof(FilteredUsers));
            }
        }

        public ListUsersViewModel(DbService dbService) 
        {
            _dbService = dbService;

            Users.CollectionChanged += (s, e) => UpdateFilteredUsers(); // Actualiza el filtrado cuando la colección de usuarios cambia

            _ = LoadUsersAsync();
        }

        private async Task LoadUsersAsync() // Carga los usuarios desde la base de datos
        {
            var allUsers = await _dbService.GetUsersAsync(); 

            Users.Clear();
            foreach (var user in allUsers)
                Users.Add(user); // Agrega cada usuario a la colección observable

            UpdateFilteredUsers(); // Actualiza el filtrado después de cargar los usuarios
        }

        public void UpdateFilteredUsers() // Filtra los usuarios según el texto de búsqueda
        {
            if (string.IsNullOrWhiteSpace(SearchText)) // Si no hay texto de búsqueda, muestra todos los usuarios
            {
                FilteredUsers = Users.ToList();
                return;
            }

            var filtered = Users
                .Where(u =>
                    u.Email?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true
                )
                .ToList(); // Filtra los usuarios cuyo correo contiene el texto de búsqueda (sin distinguir mayúsculas/minúsculas)

            FilteredUsers = filtered; // Actualiza la propiedad de usuarios filtrados
        }
        public event PropertyChangedEventHandler? PropertyChanged; // Evento para notificar cambios en las propiedades
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}