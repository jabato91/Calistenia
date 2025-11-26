using Microsoft.Maui.Controls;
using ProyectoFinDeCurso.Flyout;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Pages;
using ProyectoFinDeCurso.Pages.Main;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;
namespace ProyectoFinDeCurso
{
    public partial class App : Application
    {
        private readonly DbService _dbService = new DbService();
        
        public App()
        {
            InitializeComponent();

            
            MainPage = new NavigationPage(new ContentPage// Página temporal mientras cargamos
            {
                Content = new ActivityIndicator
                {
                    IsRunning = true,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                },
                Title = "Cargando..."
            });

            _ = InitAsync();//ejecuta datos antes de ejecutar la aplicación
        }

        private async Task InitAsync()//ejecuta datos antes de ejecutar la aplicación
        {
            try
            {

                await _dbService.InitTablesAsync(); //inicia la tabla
                var userId = await SecureStorage.GetAsync("user_id"); //obtiene la id del usuario
                
                if (string.IsNullOrEmpty(userId)) //verifica si es null
                {
                    var exercises = await _dbService.GetExercisesAsync(); //obtiene los ejercicios de la base de datos
                    var routines = await _dbService.GetRoutinesAsync(); //obtiene las rutinas de la base de datos
                    if (!exercises.Any()) //verifica si contiene datos
                    {
                        var initializer = new CreateExercises(_dbService); //crea la clase de ejercicios
                        await initializer.InitAsync(); //ingresa los ejercicios en la base de datos
                    }
                    if (!routines.Any())//verifica si contiene datos
                    {
                        var initializer = new createRoutine(_dbService); //crea la clase de rutinas
                        await initializer.InitAsync(); // ingresa las rutinas en la base de datos
                    }
                    await _dbService.CreateUserAdmin(); //crea el usuario administrador si no existe
                    
                    MainPage = new NavigationPage(new LoginPage(_dbService));   // asigna el inicio de la aplicación en la clase LoginPage
                }
                else
                {
                    var user = await _dbService.GetUserById(int.Parse(userId));  //obtiene el usuario de la base de datos
                    var exercises = await _dbService.GetExercisesAsync(); //obtiene los ejercicios de la base de datos
                    var routines = await _dbService.GetRoutinesAsync(); //obtiene las rutinas de la base de datos
                    if (!exercises.Any()) //verifica si contiene datos
                    {
                        var initializer = new CreateExercises(_dbService); //crea la clase de ejercicios
                        await initializer.InitAsync(); //ingresa los ejercicios en la base de datos
                    }
                    if (!routines.Any())//verifica si contiene datos
                    {
                        var initializer = new createRoutine(_dbService); //crea la clase de rutinas
                        await initializer.InitAsync(); // ingresa las rutinas en la base de datos
                    }
                    MainPage = new userFlyoutPage(_dbService, user.userType);// asigna el inicio de la aplicación en la clase userFlyoutPage
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en InitAsync: " + ex.Message);
                MainPage = new NavigationPage(new LoginPage(_dbService));
            }
        }
    }
}
