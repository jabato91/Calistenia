namespace ProyectoFinDeCurso.Pages;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

	private async void LoginButton(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new HomePage());
	}
}