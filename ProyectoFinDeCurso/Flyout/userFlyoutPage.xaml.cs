using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.Pages;
using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Pages.Main;
using ProyectoFinDeCurso.Models;
using System.Runtime.CompilerServices;
using ProyectoFinDeCurso.ViewModels;

namespace ProyectoFinDeCurso.Flyout;

public partial class userFlyoutPage : FlyoutPage
{
    private readonly DbService _dbService;
    private readonly userTypeEnum _userType;

    private readonly exercisePage _exercisePage;
    private readonly RoutinesPage _routinesPage;

    private readonly HomePage _homePage;
    private readonly ListUsers _usersPage;

    private readonly NavigationPage _navPage;

    public userFlyoutPage(DbService dbService, userTypeEnum userType,exercisePage? exercisePage = null, RoutinesPage? routinesPage = null)
    {
        InitializeComponent();

        _dbService = dbService;
        _userType = userType;

        // Páginas permanentes
        _exercisePage = exercisePage;//asigación de la página de ejercicios
        _routinesPage = routinesPage;//asigación de la página de rutinas
        _homePage = new HomePage(_dbService,_userType); //asigación de la página de inicio
        _usersPage = new ListUsers(dbService); //asigación de la página de usuarios

        verificationUserType(userType);//verificación del tipo de usuario para mostrar u ocultar opciones

        _navPage = new NavigationPage(_homePage); // Página de navegación inicial
        Detail = _navPage; // Establece la página principal de arranque
    }

    private void NavigateTo(Page targetPage) // Método para navegar a una página específica
    {
        if (Detail is NavigationPage nav && nav.RootPage == targetPage) // Evita recargar la misma página
        {
            IsPresented = false;
            return;
        }

        Detail = new NavigationPage(targetPage); // Navega a la página objetivo si no es la misma
        IsPresented = false;
    }

    private void ExercisePage(object sender, EventArgs e) // Manejador de evento para la página de ejercicios
    {
        if(DeviceInfo.Platform == DevicePlatform.Android) // Verifica si es Android
        {
            NavigateTo(_exercisePage); // Navega a la página de ejercicios existente
        }
        else
        {
            var exerciseVm = new ExerciseFilterViewModel(_dbService, _userType); // Crea una nueva instancia del ViewModel de ejercicios
            NavigateTo(new exercisePage(_dbService, _userType, exerciseVm)); // Navega a una nueva página de ejercicios
        }
        
    }
    

    private void RoutinesPage(object sender, EventArgs e) // Manejador de evento para la página de rutinas
    {
        if (DeviceInfo.Platform == DevicePlatform.Android)// Verifica si es Android
        {
            NavigateTo(_routinesPage); // Navega a la página de rutinas existente
        }
        else
        {
            var routineVm = new RoutinesFilterViewModel(_dbService, _userType); // Crea una nueva instancia del ViewModel de rutinas
            NavigateTo(new RoutinesPage(_dbService, _userType)); // Navega a una nueva página de rutinas
        }

    }

    private void HomePage(object sender, EventArgs e) // Manejador de evento para la página de inicio
        => NavigateTo(_homePage); // Navega a la página de inicio existente

    private void UsersBottonAdmin(object sender, EventArgs e) // Manejador de evento para la página de usuarios
        => NavigateTo(_usersPage); // Navega a la página de usuarios existente
    public void verificationUserType(userTypeEnum userType) // Verifica el tipo de usuario para mostrar u ocultar opciones
    {
        if (!userType.Equals(userTypeEnum.admin)) // Si el usuario no es admin
        {
            listUsers.IsVisible = false; // Oculta la opción de lista de usuarios
        }
    }
    private void LogoutButton(object sender, EventArgs e) // Manejador de evento para cerrar sesión
    {
        SecureStorage.RemoveAll(); // Elimina todos los datos almacenados de forma segura
        Application.Current.MainPage = new NavigationPage(new LoginPage(_dbService)); // Navega a la página de inicio de sesión
    }
}