using ProyectoFinDeCurso.Flyout;
using ProyectoFinDeCurso.Pages.Authentication;
using ProyectoFinDeCurso.Pages.Main;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;

namespace ProyectoFinDeCurso.Pages;

public partial class LoginPage : ContentPage
{

	public DbService _dbService; // variable que obtiene la base de datos
    public LoginPage(DbService dbService)
	{
		InitializeComponent();
        _dbService = dbService; //obtiene la base de datos ya iniciada
    }

    [Obsolete]
    private async void LoginButton(object sender, EventArgs e)
	{
		String emailInput = loginEmail.Text; //obtiene el correo directamente del xaml
        String passwordInput = loginPassword.Text; //obtiene la contraseña directamente del xaml

        if (string.IsNullOrWhiteSpace(emailInput) || string.IsNullOrWhiteSpace(passwordInput)) //Verifica que los campos no estén vacíos
        {
            await DisplayAlert("Error", "Por favor, rellena todos los campos", "OK");
            return;
        }

        var users = await _dbService.GetUsersAsync(); //Obtiene todos los usuarios de la base de datos
        var user = users.FirstOrDefault(u => u.Email == emailInput && PasswordHasher.VerifyPassword(passwordInput,u.Password)); //Busca el primer usuario con el email y la contraseña introducido

        if (user == null) //Si no encuentra el usuario, muestra un mensaje de error
        {
            await DisplayAlert("Error", "Usuario no encontrado", "OK");
            return;
        }

        await SecureStorage.SetAsync("user_email", emailInput); //Guarda el email del usuario en el almacenamiento seguro
        await SecureStorage.SetAsync("user_id", user.UserID.ToString()); //Guarda el id del usuario en el almacenamiento seguro
        Preferences.Set("lastUserId", user.UserID);
        if (Application.Current != null)
        {
            Application.Current.MainPage = new userFlyoutPage(new DbService(),user.userType); //Navega hacia la página Home, permitiendo no volver a la página anterior
        }
        else
        {
            await DisplayAlert("Error", "No se pudo navegar a la página principal. La aplicación no está inicializada.", "OK");
        }
    }
    private async void RegisterButton(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage(new DbService())); //Navega hacia la página Home, permitiendo volver a la página anterior
    }
    


}