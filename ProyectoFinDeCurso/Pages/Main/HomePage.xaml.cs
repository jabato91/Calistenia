using ProyectoFinDeCurso.Pages.Main;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;
namespace ProyectoFinDeCurso.Pages;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
        new CreateExercises(new DbService());
    }

    
    private async void LogoutButton(object sender, EventArgs e)
    {
        SecureStorage.Remove("user_email");
        SecureStorage.Remove("user_id");
        await Navigation.PushModalAsync(new LoginPage(new DbService()));
    }
}
