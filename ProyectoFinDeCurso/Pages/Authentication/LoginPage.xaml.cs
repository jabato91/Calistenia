using ProyectoFinDeCurso.Pages.Authentication;

namespace ProyectoFinDeCurso.Pages;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

	private async void LoginButton(object sender, EventArgs e)
	{
		await Navigation.PushModalAsync(new HomePage()); //Navega hacia la página Home, permitiendo no volver a la página anterior
	}
    private async void RegisterButton(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage(new Services.DbService())); //Navega hacia la página Home, permitiendo no volver a la página anterior
    }
}