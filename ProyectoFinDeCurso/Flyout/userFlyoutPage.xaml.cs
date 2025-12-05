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

    // Navigation principal que no se destruye
    private readonly NavigationPage _navigation;

    // Cache de páginas para Android (mejora rendimiento brutal)
    private readonly Dictionary<string, Page> _pageCache = new();

    private bool IsAndroid => DeviceInfo.Platform == DevicePlatform.Android;
    private bool IsWindows => DeviceInfo.Platform == DevicePlatform.WinUI;

    public userFlyoutPage(DbService dbService, userTypeEnum userType)
    {
        InitializeComponent();

        _dbService = dbService;
        _userType = userType;

        verificationUserType(userType);

        // Crear página inicial
        var home = BuildHomePage();

        // Navigation central estable
        _navigation = new NavigationPage(home);

        Detail = _navigation;   // nunca se reemplaza

        // Cache inicial (solo Android)
        if (IsAndroid)
            _pageCache["Home"] = home;

        IsPresented = false;
    }

    // -------------------------------
    // NAVEGACIÓN INTELIGENTE
    // -------------------------------

    private async Task NavigateToAsync(string key, Func<Page> createPage)
    {
        Page targetPage;

        if (IsAndroid)
        {
            // ANDROID: usar caché al 100%
            if (!_pageCache.TryGetValue(key, out targetPage))
            {
                targetPage = createPage();
                _pageCache[key] = targetPage; // guardar en caché
            }
        }
        else
        {
            // WINDOWS: siempre crear una nueva página
            targetPage = createPage();
        }

        // Si ya estamos en esa página → no hacer nada
        if (_navigation.CurrentPage?.GetType() == targetPage.GetType())
        {
            IsPresented = false;
            return;
        }

        // Reemplazar RootPage sin destruir NavigationPage
        _navigation.Navigation.InsertPageBefore(targetPage, _navigation.RootPage);
        await _navigation.PopToRootAsync(false);

        IsPresented = false;
    }

    // -------------------------------
    // MANEJADORES DE BOTONES
    // -------------------------------

    private async void HomePage(object sender, EventArgs e)
    {
        await NavigateToAsync(
            "Home",
            () => new HomePage(_dbService, _userType)
        );
    }

    private async void ExercisePage(object sender, EventArgs e)
    {
        await NavigateToAsync(
            "Exercises",
            () => new exercisePage(_dbService, _userType)
        );
    }

    private async void RoutinesPage(object sender, EventArgs e)
    {
        await NavigateToAsync(
            "Routines",
            () => new RoutinesPage(_dbService, _userType)
        );
    }
    private async void CalendarPage(object sender, EventArgs e)
    {
        await NavigateToAsync(
         "Calendar",
         () => new CalendarPage(_dbService, _userType)
     );
    }
    private async void UsersBottonAdmin(object sender, EventArgs e)
    {
        await NavigateToAsync(
            "Users",
            () => new ListUsers(_dbService)
        );
    }

    private HomePage BuildHomePage()
    {
        return new HomePage(_dbService, _userType);
    }

    // -------------------------------
    // UTILIDADES
    // -------------------------------

    public void verificationUserType(userTypeEnum userType)
    {
        if (userType != userTypeEnum.admin)
            listUsers.IsVisible = false;
    }

    private void LogoutButton(object sender, EventArgs e)
    {
        SecureStorage.RemoveAll();
        Application.Current.MainPage = new NavigationPage(new LoginPage(_dbService));
    }
}