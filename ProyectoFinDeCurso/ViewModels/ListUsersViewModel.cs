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

        public ListUsersViewModel(DbService dbService)
        {
            _dbService = dbService;

            Users.CollectionChanged += (s, e) => UpdateFilteredUsers();

            _ = LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            var allUsers = await _dbService.GetUsersAsync(); 

            Users.Clear();
            foreach (var user in allUsers)
                Users.Add(user);

            UpdateFilteredUsers();
        }

        public void UpdateFilteredUsers()
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
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}