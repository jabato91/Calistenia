
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.Pages.Main;
namespace ProyectoFinDeCurso.Pages.Authentication;

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
        if (string.IsNullOrWhiteSpace(nameRegister.Text) ||
            string.IsNullOrWhiteSpace(firstSurname.Text) ||
            string.IsNullOrWhiteSpace(SecondSurname.Text) ||
            string.IsNullOrWhiteSpace(email.Text) ||
            string.IsNullOrWhiteSpace(phone.Text))
        {
            await DisplayAlert("Error", "Todos los campos son obligatorios", "OK");
            return;
        }

        // Validar teléfono (ejemplo: 9 dígitos)
        if (phone.Text.Length != 9 || !phone.Text.All(char.IsDigit))
        {
            await DisplayAlert("Error", "El teléfono debe tener 9 dígitos", "OK");
            return;
        }

        // Crear usuario
        var user = new User
        {
            Name = nameRegister.Text,
            FirstSurname = firstSurname.Text,
            SecondSurname = SecondSurname.Text,
            Email = email.Text,
            Phone = phone.Text,
        };

        await _dbService.Create(user);

        await DisplayAlert("Éxito", "Usuario registrado correctamente", "OK");

        // Limpiar campos
        nameRegister.Text = string.Empty;
        firstSurname.Text = string.Empty;
        SecondSurname.Text = string.Empty;
        email.Text = string.Empty;
        checkEmail.Text = string.Empty;
        phone.Text = string.Empty;

        // ?? Si usas NavigationPage:
        await Navigation.PopAsync(); // Vuelves a la lista y se recarga
    }
}