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

        public ObservableCollection<User> Users { get; } = new();

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
                    UpdateFilteredUsers();
                }
            }
        }

        private IEnumerable<User> _filteredUsers = new List<User>();
        public IEnumerable<User> FilteredUsers
        {
            get => _filteredUsers;
            private set
            {
                _filteredUsers = value;
                OnPropertyChanged(nameof(FilteredUsers));
            }
        }


        // ====================================================
        // CONSTRUCTOR
        // ====================================================
        public ListUsersViewModel(DbService dbService)
        {
            _dbService = dbService;

            Users.CollectionChanged += (s, e) => UpdateFilteredUsers();

            _ = LoadUsersAsync();
        }


        // ====================================================
        // CARGA DE USUARIOS (CON CACHÉ)
        // ====================================================
        private async Task LoadUsersAsync()
        {
            var allUsers = await _dbService.GetUsersCached(); // ⚡ Carga desde memoria

            Users.Clear();
            foreach (var user in allUsers)
                Users.Add(user);

            UpdateFilteredUsers();
        }


        // ====================================================
        // FILTRADO OPTIMIZADO
        // ====================================================
        private void UpdateFilteredUsers()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredUsers = Users.ToList();
                return;
            }

            var filtered = Users
                .Where(u =>
                    u.Email?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true
                )
                .ToList();

            FilteredUsers = filtered;
        }


        // ====================================================
        // PROPERTY CHANGED
        // ====================================================
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}