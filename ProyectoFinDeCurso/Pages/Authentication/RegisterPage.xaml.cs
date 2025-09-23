
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Services;

namespace ProyectoFinDeCurso.Pages.Authentication;

public partial class RegisterPage : ContentPage
{
	private bool itIsCorrectEmail = true;
	private readonly DbService _dbService;
	public RegisterPage(DbService dbService)
	{
		
		InitializeComponent();
        _dbService = dbService;
		
    }

    async void RegisterUser(object sender, EventArgs e)
	{
		if (!email.Text.Equals(checkEmail.Text)) 
		{
			await DisplayAlert("Error", "Los correos no coinciden", "OK");
			itIsCorrectEmail = false;
		}

		if (itIsCorrectEmail && !string.IsNullOrWhiteSpace(nameRegister.Text)
							&& !string.IsNullOrWhiteSpace(firstSurname.Text)
							&& !string.IsNullOrWhiteSpace(SecondSurname.Text)
							&& !string.IsNullOrWhiteSpace(email.Text)
							&& !string.IsNullOrWhiteSpace(phone.Text))
		{
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

            
            nameRegister.Text = string.Empty;
            firstSurname.Text = string.Empty;
            SecondSurname.Text = string.Empty;
            email.Text = string.Empty;
            checkEmail.Text = string.Empty;
            phone.Text = string.Empty;
        }
	}
}