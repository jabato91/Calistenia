using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.Pages;
using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Pages.Main;

namespace ProyectoFinDeCurso.Flyout;

public partial class userFlyoutPage : FlyoutPage
{
    public DbService _dbService;
    private userTypeEnum _userType;
    public userFlyoutPage(DbService dbService,userTypeEnum userType)
	{
		InitializeComponent();
        _dbService = dbService;
        _userType = userType;
        verificationUserType(userType);
    }
    public userFlyoutPage(DbService dbService)
    {
        InitializeComponent();
        _dbService = dbService;
        

    }
    private async void LogoutButton(object sender, EventArgs e)
    {
        SecureStorage.Remove("user_email");
        SecureStorage.Remove("user_id");
        await Navigation.PushModalAsync(new LoginPage(_dbService));
    }

    private void verificationUserType(userTypeEnum userType)
    {
        if (!userType.Equals(userTypeEnum.admin))
        {
            listUsers.IsVisible = false;
        }
    }
    private async void UsersBottonAdmin(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new ListUsers());
    }

    private void ExercisePage(object sender, EventArgs e)
    {
        this.Detail = new NavigationPage(new exercisePage(_dbService));
        IsPresented = false;
    }

    private void 
}