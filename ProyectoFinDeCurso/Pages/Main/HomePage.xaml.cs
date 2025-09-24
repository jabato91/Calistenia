using ProyectoFinDeCurso.Pages.Main;
using ProyectoFinDeCurso.Services;
namespace ProyectoFinDeCurso.Pages;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
	}

    private async void UsersBotton(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new ListUsers()); 
    }
}
