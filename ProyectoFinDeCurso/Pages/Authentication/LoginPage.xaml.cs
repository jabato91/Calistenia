using ProyectoFinDeCurso.Pages.Authentication;
using ProyectoFinDeCurso.Services;

namespace ProyectoFinDeCurso.Pages;

public partial class LoginPage : ContentPage
{

	public DbService _dbService;
    public LoginPage(DbService dbService)
	{
		InitializeComponent();
        _dbService = dbService;
    }

	private async void LoginButton(object sender, EventArgs e)
	{
		String emailInput = loginEmail.Text;
        String passwordInput = loginPassword.Text;

        if (string.IsNullOrWhiteSpace(emailInput) || string.IsNullOrWhiteSpace(passwordInput)) //Verifica que los campos no estén vacíos
        {
            await DisplayAlert("Error", "Por favor, rellena todos los campos", "OK");
            return;
        }

        var users = await _dbService.GetUsers(); //Obtiene todos los usuarios de la base de datos
        var user = users.FirstOrDefault(u => u.Email == emailInput && u.Password == passwordInput); //Busca el usuario con el email introducido

        if (user == null) //Si no encuentra el usuario, muestra un mensaje de error
        {
            await DisplayAlert("Error", "Usuario no encontrado", "OK");
            return;
        }

        await SecureStorage.SetAsync("user_email", emailInput); //Guarda el email del usuario en el almacenamiento seguro
        await SecureStorage.SetAsync("user_id", user.UserID.ToString()); //Guarda el id del usuario en el almacenamiento seguro

        if (Application.Current != null)
        {
            Application.Current.MainPage = new ProyectoFinDeCurso.Flyout.userFlyoutPage(new DbService(),user.userType); //Navega hacia la página Home, permitiendo no volver a la página anterior
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