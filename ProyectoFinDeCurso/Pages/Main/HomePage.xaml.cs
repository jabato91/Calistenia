
using ProyectoFinDeCurso.Pages.Main;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;
namespace ProyectoFinDeCurso.Pages;

public partial class HomePage : ContentPage
{
    DbService _dbService = new DbService();
    public HomePage()
	{
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
}
