
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.Pages.Main;
namespace ProyectoFinDeCurso.Pages.Authentication;
using Microsoft.Maui.Storage;

public partial class RegisterPage : ContentPage
{
    private readonly DbService _dbService;

    public RegisterPage(DbService dbService)
    {
        InitializeComponent();
        _dbService = dbService;
    }

    async void RegisterUser(object sender, EventArgs e)
    {
        // Validar que los emails coincidan
        if (email.Text != checkEmail.Text)
        {
            await DisplayAlert("Error", "Los correos no coinciden", "OK");
            return;
        }

        // Validar campos obligatorios
        if (string.IsNullOrWhiteSpace(nameRegister.Text) || //verifica que estén rellenado todos los campos
            string.IsNullOrWhiteSpace(firstSurname.Text) ||
            string.IsNullOrWhiteSpace(SecondSurname.Text) ||
            string.IsNullOrWhiteSpace(email.Text) ||
            string.IsNullOrWhiteSpace(phone.Text))
        {
            await DisplayAlert("Error", "Todos los campos son obligatorios", "OK");
            return;
        }

        // Valida si el teléfono tiene 9 dígitos y solo contiene números
        if (phone.Text.Length != 9 || !phone.Text.All(char.IsDigit))
        {
            await DisplayAlert("Error", "El teléfono debe tener 9 dígitos", "OK");
            return;
        }

        // Crea modelo del usuario a registrar
        var user = new User
        {
            Name = nameRegister.Text,
            FirstSurname = firstSurname.Text,
            SecondSurname = SecondSurname.Text,
            Email = email.Text,
            Phone = phone.Text,
            Password = Password.Text,
            userType = Enums.userTypeEnum.user
        };
        //Crea usuario en la base de datos
        await _dbService.Create(user);

        await SecureStorage.SetAsync("user_email", email.Text);
        await SecureStorage.SetAsync("user_id", user.UserID.ToString());

        await DisplayAlert("Éxito", "Usuario registrado correctamente", "OK");

        // Limpiar campos
        nameRegister.Text = string.Empty;
        firstSurname.Text = string.Empty;
        SecondSurname.Text = string.Empty;
        email.Text = string.Empty;
        checkEmail.Text = string.Empty;
        phone.Text = string.Empty;
        Password.Text = string.Empty;

        await Navigation.PopAsync();

    }
}