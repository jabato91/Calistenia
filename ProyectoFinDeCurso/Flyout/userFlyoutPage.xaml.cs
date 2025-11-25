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

    private NavigationPage _navPage;

    public userFlyoutPage(DbService dbService, userTypeEnum userType)
    {
        InitializeComponent();

        _dbService = dbService;
        _userType = userType;

        verificationUserType(userType);

        // NavigationPage única y estable
        _navPage = new NavigationPage(BuildHomePage());
        Detail = _navPage;
    }

    private void NavigateTo(Page page)
    {
        // Evitar recargar la misma página
        if (Detail is NavigationPage nav &&
            nav.RootPage.GetType() == page.GetType())
        {
            IsPresented = false;
            return;
        }

        // Reemplazar completamente la página de navegación
        Detail = new NavigationPage(page);

        IsPresented = false;
    }
    private void HomePage(object sender, EventArgs e)
    {
        Detail = new NavigationPage(new HomePage(_dbService, _userType));
        IsPresented = false;
    }

    private void ExercisePage(object sender, EventArgs e)
    {
        Detail = new NavigationPage(new exercisePage(_dbService, _userType));
        IsPresented = false;
    }

    private void RoutinesPage(object sender, EventArgs e)
    {
        Detail = new NavigationPage(new RoutinesPage(_dbService, _userType));
        IsPresented = false;
    }

    private void UsersBottonAdmin(object sender, EventArgs e)
    {
        Detail = new NavigationPage(new ListUsers(_dbService));
        IsPresented = false;
    }
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
    private HomePage BuildHomePage()
    {
        return new HomePage(_dbService, _userType);
    }
}