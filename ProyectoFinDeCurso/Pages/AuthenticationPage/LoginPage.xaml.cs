using ProyectoFinDeCurso.Flyout;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Pages.Authentication;
using ProyectoFinDeCurso.Pages.Detail;
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
        try
        {
            String emailInput = loginEmail.Text; //obtiene el correo directamente del xaml
            String passwordInput = loginPassword.Text; //obtiene la contraseña directamente del xaml

            if (string.IsNullOrWhiteSpace(emailInput) || string.IsNullOrWhiteSpace(passwordInput)) //Verifica que los campos no estén vacíos
            {
                await DisplayAlert("Error", "Por favor, rellena todos los campos", "OK");
                return;
            }

            List<User> users = new();
            try
            {
                users = await _dbService.GetUsersAsync(); //Obtiene todos los usuarios de la base de datos
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR GetUsersAsync] " + ex.Message);
                await DisplayAlert("Error", "No se pudieron obtener los usuarios.", "OK");
                return;
            }

            User user = null;
            try
            {
                user = users.FirstOrDefault(u =>
                    u.Email == emailInput &&
                    PasswordHasher.VerifyPassword(passwordInput, u.Password)); //Busca el usuario
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR VerifyPassword] " + ex.Message);
            }

            if (user == null) //Si no encuentra el usuario, muestra un mensaje de error
            {
                await DisplayAlert("Error", "Usuario no encontrado", "OK");
                return;
            }

            try
            {
                await SecureStorage.SetAsync("user_email", emailInput); //Guarda el email
                await SecureStorage.SetAsync("user_id", user.UserID.ToString()); //Guarda la ID
                Preferences.Set("lastUserId", user.UserID);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR SecureStorage] " + ex.Message);
                await DisplayAlert("Error", "No se pudo guardar la sesión.", "OK");
                return;
            }

            try
            {
                if (Application.Current != null)
                {
                    Application.Current.MainPage = new userFlyoutPage(_dbService, user.userType);
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo navegar a la página principal. La aplicación no está inicializada.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR Navigation] " + ex.Message);
                await DisplayAlert("Error", "No se pudo cargar la página principal.", "OK");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR LoginButton] " + ex.Message);
            await DisplayAlert("Error", "Ha ocurrido un error inesperado.", "OK");
        }
    }

    private async void RegisterButton(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new RegisterPage(_dbService)); //Navega hacia la página Home, permitiendo volver a la página anterior
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR RegisterButton] " + ex.Message);
            await DisplayAlert("Error", "No se pudo abrir la página de registro.", "OK");
        }
    }
    private async void ForgotPassword(object sender, TappedEventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new UserDetailPage(new User(),_dbService,mode: Enums.ModeEnum.create)); //Navega hacia la página 
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR RegisterButton] " + ex.Message);
            await DisplayAlert("Error", "No se pudo abrir la página para cambiar contraseña.", "OK");
        }
    }
}