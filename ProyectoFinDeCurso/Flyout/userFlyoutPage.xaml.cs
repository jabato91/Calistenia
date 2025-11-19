using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.Pages;
using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Pages.Main;
using ProyectoFinDeCurso.Models;
using System.Runtime.CompilerServices;

namespace ProyectoFinDeCurso.Flyout;

public partial class userFlyoutPage : FlyoutPage
{
    private readonly DbService _dbService;
    private readonly userTypeEnum _userType;

    private readonly exercisePage _exercisePage;
    private readonly RoutinesPage _routinesPage;

    private readonly NavigationPage _navPage;

    public userFlyoutPage(DbService dbService, userTypeEnum userType,
                          exercisePage exercisePage, RoutinesPage routinesPage)
    {
        InitializeComponent();

        _dbService = dbService;
        _userType = userType;

        _exercisePage = exercisePage;
        _routinesPage = routinesPage;

        verificationUserType(userType);

        _navPage = new NavigationPage(_exercisePage);
        Detail = _navPage;
    }

    private void NavigateTo(Page targetPage)
    {
        if (_navPage.CurrentPage == targetPage)
        {
            IsPresented = false;
            return;
        }

        _navPage.Navigation.InsertPageBefore(targetPage, _navPage.CurrentPage);
        _navPage.PopAsync(false);

        IsPresented = false;
    }

    private void ExercisePage(object sender, EventArgs e)
        => NavigateTo(_exercisePage);

    private void RoutinesPage(object sender, EventArgs e)
        => NavigateTo(_routinesPage);

    private void HomePage(object sender, EventArgs e)
        => NavigateTo(new HomePage());

    private void UsersBottonAdmin(object sender, EventArgs e)
        => NavigateTo(new ListUsers(_dbService));
    public void verificationUserType(userTypeEnum userType)
    {
        if (!userType.Equals(userTypeEnum.admin))
        {
            listUsers.IsVisible = false;
        }
    }
}