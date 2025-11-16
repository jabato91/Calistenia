
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
        if (Password.Text != checkPassword.Text) { //verifica si ha puesto bien las contraseñas, teniendo que ser iguales
            await DisplayAlert("Error", "Las contraseñas son diferentes entre si", "OK");
            return;
        }
        
        if (phone.Text.Length != 9 || !phone.Text.All(char.IsDigit))// Valida si el teléfono tiene 9 dígitos y solo contiene números
        {
            await DisplayAlert("Error", "El teléfono debe tener 9 dígitos", "OK");
            return;
        }
        List<User> listUsers = await _dbService.GetUsers();//obtiene la lista de usuarios
        foreach (User getUser in listUsers)//bucle sobre la cantidad de usuarios que tiene listUsers
        {
            if (getUser.Email.Equals(email.Text)) { //verifica si existe ya el correo con el que pretende registrarse
                await DisplayAlert("Error", "El correo ya existe", "OK");
                return;
            }
        }
        String passwordHash = PasswordHasher.HashPassword(Password.Text); //incripta la contraseña
        
        var user = new User// Crea modelo del usuario a registrar
        {
            Name = nameRegister.Text,
            FirstSurname = firstSurname.Text,
            SecondSurname = SecondSurname.Text,
            Email = email.Text,
            Phone = phone.Text,
            Password = passwordHash,
            userType = Enums.userTypeEnum.user
        };
        
        await _dbService.Create(user);//Crea usuario en la base de datos


        await DisplayAlert("Éxito", "Usuario registrado correctamente", "OK");

        nameRegister.Text = string.Empty;
        firstSurname.Text = string.Empty;
        SecondSurname.Text = string.Empty;
        email.Text = string.Empty;
        checkPassword.Text = string.Empty;//limpia todos los editores de texto
        phone.Text = string.Empty;
        Password.Text = string.Empty;

        await Navigation.PopAsync();

    }
}