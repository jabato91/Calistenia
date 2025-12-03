using Microsoft.Maui.Controls;
using ProyectoFinDeCurso.Flyout;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Pages;
using ProyectoFinDeCurso.Pages.Main;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;
#if ANDROID
using ProyectoFinDeCurso.Platforms.Android;
#endif

namespace ProyectoFinDeCurso
{
    public partial class App : Application
    {
        private readonly DbService _dbService = new DbService(); // Servicio de base de datos

        public App()
        {
            InitializeComponent();

            
            MainPage = new NavigationPage(new ContentPage// Pantalla temporal de carga
            {
                Content = new ActivityIndicator
                {
                    IsRunning = true,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                },
                Title = "Cargando..."
            });

            _ = InitAsync(); // Cargar la app
        }

        private async Task InitAsync() // Inicializa la aplicación
        {
            try
            {
                await _dbService.InitTablesAsync(); // Inicializa las tablas de la base de datos
                var userId = await SecureStorage.GetAsync("user_id"); // Obtiene el ID del usuario almacenado de forma segura

                if (string.IsNullOrEmpty(userId)) // Si no hay usuario almacenado, mostrar la página de inicio de sesión
                {
                    var exercises = await _dbService.GetExercisesAsync(); // Obtiene los ejercicios de la base de datos
                    var routines = await _dbService.GetRoutinesAsync(); // Obtiene las rutinas de la base de datos

                    if (!exercises.Any()) // Si no hay ejercicios, inicializarlos
                    {
                        var initializer = new CreateExercises(_dbService); // Inicializador de ejercicios
                        await initializer.InitAsync(); // Ejecuta la inicialización
                    }
                    if (!routines.Any()) // Si no hay rutinas, inicializarlas
                    {
                        var initializer = new createRoutine(_dbService); // Inicializador de rutinas
                        await initializer.InitAsync(); // Ejecuta la inicialización
                    }

                    await _dbService.CreateUserAdmin(); // Crea un usuario administrador por defecto

                    MainPage = new NavigationPage(new LoginPage(_dbService)); // Muestra la página de inicio de sesión
                }
                else
                {
                    Preferences.Set("lastUserId", userId); // Guarda el ID del último usuario en las preferencias
                    var user = await _dbService.GetUserById(int.Parse(userId)); // Obtiene el usuario de la base de datos
                    var exercises = await _dbService.GetExercisesAsync(); // Obtiene los ejercicios de la base de datos
                    var routines = await _dbService.GetRoutinesAsync(); // Obtiene las rutinas de la base de datos

                    if (!exercises.Any()) // Si no hay ejercicios, inicializarlos
                    {
                        var initializer = new CreateExercises(_dbService); // Inicializador de ejercicios
                        await initializer.InitAsync(); // Ejecuta la inicialización
                    }
                    if (!routines.Any()) // Si no hay rutinas, inicializarlas
                    {
                        var initializer = new createRoutine(_dbService); // Inicializador de rutinas
                        await initializer.InitAsync(); // Ejecuta la inicialización
                    }

                    MainPage = new userFlyoutPage(_dbService, user.userType); // Muestra la página principal de la aplicación
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en InitAsync: " + ex.Message);
                MainPage = new NavigationPage(new LoginPage(_dbService));
            }
#if ANDROID // Solicitar permiso de alarmas exactas en Android
AlarmPermissionService.RequestExactAlarmPermission(); // Solicita el permiso al usuario

bool allowed = AlarmPermissionService.HasExactAlarmPermission(); // Verifica si el permiso fue concedido


#endif

        }


    }
}