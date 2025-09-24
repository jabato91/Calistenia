using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;


namespace ProyectoFinDeCurso.ViewModels
{
   
    public class ListUsersViewModel
    {
        private readonly DbService _dbService;

        

        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>(); //Se usa la colección Oservable para notificar automáticamente a la interfaz gráfica
        public ListUsersViewModel()
        {
            _dbService = new DbService(); //carga la base de datos
            LoadUsersAsync(); //carga el método

        }

        private async void LoadUsersAsync() //lee los usuarios dentro de la base de datos
        {
            var allUsers = await _dbService.GetUsers(); //se espera hasta que se lean todos los usuarios

            Users.Clear(); //elimina los datos anteriores registrados
            foreach (var user in allUsers) //guarda en la colección anterior creada
            {
                Users.Add(user);
            }
        }
    }
}
