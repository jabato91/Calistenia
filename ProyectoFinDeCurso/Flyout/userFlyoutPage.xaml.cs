using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.Pages;
using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Pages.Main;

namespace ProyectoFinDeCurso.Flyout;

public partial class userFlyoutPage : FlyoutPage
{
    public DbService _dbService;
    private static userTypeEnum _userType;
    public userFlyoutPage(DbService dbService,userTypeEnum userType)
	{
        
        InitializeComponent();
        _dbService = dbService;
        _userType = userType;
        verificationUserType(_userType);

    }
    public userFlyoutPage(DbService dbService, string userId)
    {
       
        InitializeComponent();
        _dbService = dbService;
        _ = InitializeAsync(userId);
    }
    private async Task InitializeAsync(string userId)
    {
        var user = await _dbService.GetUserById(int.Parse(userId));

        if (user != null)
        {
            _userType = user.userType;
        }

        verificationUserType(_userType);
    }
    private async void LogoutButton(object sender, EventArgs e)
    {
        SecureStorage.Remove("user_email");
        SecureStorage.Remove("user_id");
        await Navigation.PushModalAsync(new LoginPage(_dbService));
    }

    public void verificationUserType(userTypeEnum userType)
    {
        if (!userType.Equals(userTypeEnum.admin))
        {
            listUsers.IsVisible = false;
        }
    }
    private void UsersBottonAdmin(object sender, EventArgs e)
    {
        this.Detail = new NavigationPage(new ListUsers());
        this.IsPresented = false;
    }
    
    private void HomePage(object sender, EventArgs e)
    {
        
        this.Detail = new NavigationPage(new HomePage());
        IsPresented = false;
    }
    private void ExercisePage(object sender, EventArgs e)
    {
        listUsers.CancelAnimations();
        listUsers.Scale = 1.0;
        this.Detail = new NavigationPage(new exercisePage(_dbService, _userType));
        IsPresented = false;
    }

    private void RoutinesPage(object sender, EventArgs e)
    {
        listUsers.CancelAnimations();
        listUsers.Scale = 1.0;
        this.Detail = new NavigationPage(new RoutinesPage(_dbService));
        IsPresented = false;
    }
    private async void Button_Pressed(object sender, EventArgs e)
    {
        var button = (Button)sender;
        await button.ScaleTo(1.1, 150, Easing.CubicInOut); // Escala suavemente al 110%
        
    }
    private async void Button_Released(object sender, EventArgs e)
    {
        var button = (Button)sender;
        await button.ScaleTo(1.0, 150, Easing.CubicInOut); // Vuelve al tamaño original
    }

}