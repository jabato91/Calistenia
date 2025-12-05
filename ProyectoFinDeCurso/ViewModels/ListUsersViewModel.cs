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
                try
                {
                    if (_searchText != value)
                    {
                        _searchText = value;
                        OnPropertyChanged(nameof(SearchText));
                        UpdateFilteredUsers();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERROR SearchText] " + ex.Message);
                }
            }
        }

        private IEnumerable<User> _filteredUsers = new List<User>();
        public IEnumerable<User> FilteredUsers // Usuarios filtrados según el texto de búsqueda
        {
            get => _filteredUsers;
            private set
            {
                try
                {
                    _filteredUsers = value;
                    OnPropertyChanged(nameof(FilteredUsers));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERROR FilteredUsers Set] " + ex.Message);
                }
            }
        }

        public ListUsersViewModel(DbService dbService)
        {
            try
            {
                _dbService = dbService;

                Users.CollectionChanged += (s, e) =>
                {
                    try
                    {
                        UpdateFilteredUsers(); // Actualiza el filtrado cuando la colección de usuarios cambia
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[ERROR CollectionChanged] " + ex.Message);
                    }
                };

                _ = LoadUsersAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR Constructor ListUsersViewModel] " + ex.Message);
            }
        }

        private async Task LoadUsersAsync() // Carga los usuarios desde la base de datos
        {
            try
            {
                var allUsers = await _dbService.GetUsersAsync();

                Users.Clear();

                foreach (var user in allUsers)
                {
                    try
                    {
                        Users.Add(user); // Agrega cada usuario a la colección observable
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[ERROR Add User] " + ex.Message);
                    }
                }

                UpdateFilteredUsers(); // Actualiza el filtrado después de cargar los usuarios
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR LoadUsersAsync] " + ex.Message);
            }
        }

        public void UpdateFilteredUsers() // Filtra los usuarios según el texto de búsqueda
        {
            try
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
                    .ToList(); // Filtra los usuarios cuyo correo contiene el texto de búsqueda

                FilteredUsers = filtered; // Actualiza la propiedad de usuarios filtrados
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR UpdateFilteredUsers] " + ex.Message);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged; // Evento para notificar cambios en las propiedades

        private void OnPropertyChanged(string propertyName)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR OnPropertyChanged] " + ex.Message);
            }
        }
    }
}