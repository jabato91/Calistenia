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

    public userFlyoutPage(DbService dbService, userTypeEnum userType,
                          exercisePage? exercisePage = null, RoutinesPage? routinesPage = null)
    {
        InitializeComponent();

        _dbService = dbService;
        _userType = userType;

        // Páginas permanentes
        _exercisePage = exercisePage;
        _routinesPage = routinesPage;
        _homePage = new HomePage(_dbService,_userType);
        _usersPage = new ListUsers(dbService);

        verificationUserType(userType);

        _navPage = new NavigationPage(_homePage);
        Detail = _navPage;
    }

    private void NavigateTo(Page targetPage)
    {
        if (Detail is NavigationPage nav && nav.RootPage == targetPage)
        {
            IsPresented = false;
            return;
        }

        Detail = new NavigationPage(targetPage);
        IsPresented = false;
    }

    private void ExercisePage(object sender, EventArgs e)
    {
        if(DeviceInfo.Platform == DevicePlatform.Android)
        {
            NavigateTo(_exercisePage);
        }
        else
        {
            var exerciseVm = new ExerciseFilterViewModel(_dbService, _userType);
            NavigateTo(new exercisePage(_dbService, _userType, exerciseVm));
        }
        
    }
    

    private void RoutinesPage(object sender, EventArgs e)
    {
        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            NavigateTo(_routinesPage);
        }
        else
        {
            var routineVm = new RoutinesFilterViewModel(_dbService, _userType);
            NavigateTo(new RoutinesPage(_dbService, _userType));
        }

    }

    private void HomePage(object sender, EventArgs e)
        => NavigateTo(_homePage);

    private void UsersBottonAdmin(object sender, EventArgs e)
        => NavigateTo(_usersPage);
    public void verificationUserType(userTypeEnum userType)
    {
        if (!userType.Equals(userTypeEnum.admin))
        {
            listUsers.IsVisible = false;
        }
    }
    private void LogoutButton(object sender, EventArgs e)
    {
        SecureStorage.RemoveAll();
        Application.Current.MainPage = new NavigationPage(new LoginPage(_dbService));
    }
}