
using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Pages.Detail;
using ProyectoFinDeCurso.Pages.Main;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;
namespace ProyectoFinDeCurso.Pages;

public partial class HomePage : ContentPage
{
    private readonly DbService _dbService;
    private readonly userTypeEnum _userType;
    public HomePage(DbService dbService, userTypeEnum userType)
	{
        _dbService = dbService;
        _userType = userType;
        InitializeComponent();
        // Inicializa los ejercicios solo si no existen
        var initializer = new CreateExercises(_dbService);
    }

    
    private async void LogoutButton(object sender, EventArgs e)
    {
        SecureStorage.Remove("user_email");
        SecureStorage.Remove("user_id");
        await Navigation.PushModalAsync(new LoginPage(new DbService()));
    }
    private async void profile(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(
           new UserDetailPage(_dbService, _userType, ModeEnum.View)
       );
    }
}
