
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.Pages.Main;
namespace ProyectoFinDeCurso.Pages.Authentication;
using Microsoft.Maui.Storage;
using ProyectoFinDeCurso.ViewModels;

public partial class RegisterPage : ContentPage
{
    private readonly DbService _dbService; //crea la clase DbService

    public RegisterPage(DbService dbService)
    {
        InitializeComponent();
        _dbService = dbService; //obtiene los parametros de la clase
    }

    async void RegisterUser(object sender, EventArgs e)
    {
        try
        {
            // Validar campos obligatorios
            if (string.IsNullOrWhiteSpace(nameRegister.Text) || //verifica que estén rellenado todos los campos
                string.IsNullOrWhiteSpace(firstSurname.Text) ||
                string.IsNullOrWhiteSpace(SecondSurname.Text) ||
                string.IsNullOrWhiteSpace(email.Text) ||
                string.IsNullOrWhiteSpace(Password.Text) ||
                string.IsNullOrWhiteSpace(checkPassword.Text) ||
                string.IsNullOrWhiteSpace(phone.Text))
            {
                await DisplayAlert("Error", "Todos los campos son obligatorios", "OK");
                return;
            }

            if (Password.Text != checkPassword.Text) //verifica si ha puesto bien las contraseñas, teniendo que ser iguales
            {
                await DisplayAlert("Error", "Las contraseñas son diferentes entre si", "OK");
                return;
            }

            if (phone.Text.Length != 9 || !phone.Text.All(char.IsDigit)) // Valida si el teléfono tiene 9 dígitos y solo contiene números
            {
                await DisplayAlert("Error", "El teléfono debe tener 9 dígitos", "OK");
                return;
            }

            List<User> listUsers = new List<User>();
            try
            {
                listUsers = await _dbService.GetUsersAsync(); //obtiene la lista de usuarios
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR GetUsersAsync] " + ex.Message);
                await DisplayAlert("Error", "No se pudo obtener la lista de usuarios.", "OK");
                return;
            }

            foreach (User getUser in listUsers) //bucle sobre la cantidad de usuarios que tiene listUsers
            {
                if (getUser.Email.Equals(email.Text)) //verifica si existe ya el correo con el que pretende registrarse
                {
                    await DisplayAlert("Error", "El correo ya existe", "OK");
                    return;
                }
            }

            String passwordHash = string.Empty;
            try
            {
                passwordHash = PasswordHasher.HashPassword(Password.Text); //incripta la contraseña
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR HashPassword] " + ex.Message);
                await DisplayAlert("Error", "No se pudo procesar la contraseña.", "OK");
                return;
            }

            var user = new User // Crea modelo del usuario a registrar
            {
                Name = nameRegister.Text,
                FirstSurname = firstSurname.Text,
                SecondSurname = SecondSurname.Text,
                Email = email.Text,
                Phone = phone.Text,
                Password = passwordHash,
                userType = Enums.userTypeEnum.user
            };

            try
            {
                await _dbService.Create(user); //Crea usuario en la base de datos
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR CreateUser] " + ex.Message);
                await DisplayAlert("Error", "No se pudo registrar el usuario.", "OK");
                return;
            }

            await DisplayAlert("Éxito", "Usuario registrado correctamente", "OK");

            nameRegister.Text = string.Empty;
            firstSurname.Text = string.Empty;
            SecondSurname.Text = string.Empty;
            email.Text = string.Empty;
            checkPassword.Text = string.Empty; //limpia todos los editores de texto
            phone.Text = string.Empty;
            Password.Text = string.Empty;

            try
            {
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR Navigation.PopAsync] " + ex.Message);
                await DisplayAlert("Error", "No se pudo volver a la pantalla anterior.", "OK");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR RegisterUser] " + ex.Message);
            await DisplayAlert("Error", "Ha ocurrido un error inesperado.", "OK");
        }
    }
}
