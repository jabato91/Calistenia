using ProyectoFinDeCurso.Pages.Main;
using ProyectoFinDeCurso.Services;
namespace ProyectoFinDeCurso.Pages;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
	}

    private async void UsersBotton(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new ListUsers()); 
    }
    private async void LogoutButton(object sender, EventArgs e)
    {
        SecureStorage.Remove("user_email");
        SecureStorage.Remove("user_id");
        await Navigation.PushModalAsync(new LoginPage(new DbService()));
    }
}
