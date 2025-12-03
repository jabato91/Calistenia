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
        private readonly DbService _dbService = new DbService();

        public App()
        {
            InitializeComponent();

            // Pantalla temporal de carga
            MainPage = new NavigationPage(new ContentPage
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

        private async Task InitAsync()
        {
            try
            {
                await _dbService.InitTablesAsync();
                var userId = await SecureStorage.GetAsync("user_id");

                if (string.IsNullOrEmpty(userId))
                {
                    var exercises = await _dbService.GetExercisesAsync();
                    var routines = await _dbService.GetRoutinesAsync();

                    if (!exercises.Any())
                    {
                        var initializer = new CreateExercises(_dbService);
                        await initializer.InitAsync();
                    }
                    if (!routines.Any())
                    {
                        var initializer = new createRoutine(_dbService);
                        await initializer.InitAsync();
                    }

                    await _dbService.CreateUserAdmin();

                    MainPage = new NavigationPage(new LoginPage(_dbService));
                }
                else
                {
                    Preferences.Set("lastUserId", userId);
                    var user = await _dbService.GetUserById(int.Parse(userId));
                    var exercises = await _dbService.GetExercisesAsync();
                    var routines = await _dbService.GetRoutinesAsync();

                    if (!exercises.Any())
                    {
                        var initializer = new CreateExercises(_dbService);
                        await initializer.InitAsync();
                    }
                    if (!routines.Any())
                    {
                        var initializer = new createRoutine(_dbService);
                        await initializer.InitAsync();
                    }

                    MainPage = new userFlyoutPage(_dbService, user.userType);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en InitAsync: " + ex.Message);
                MainPage = new NavigationPage(new LoginPage(_dbService));
            }
#if ANDROID
AlarmPermissionService.RequestExactAlarmPermission();

bool allowed = AlarmPermissionService.HasExactAlarmPermission();


#endif

        }


    }
}