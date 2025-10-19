using ProyectoFinDeCurso.Pages;
using ProyectoFinDeCurso.Pages.Main;

namespace ProyectoFinDeCurso.Flyout;

public partial class NavegationPages : FlyoutPage
{
	public NavegationPages()
	{
		InitializeComponent();
	}

    private async void OnPagina1Clicked(object sender, EventArgs e)
    {
        Detail = new NavigationPage(new HomePage());
        IsPresented = false; // Cierra el menú
    }

    private async void OnPagina2Clicked(object sender, EventArgs e)
    {
        Detail = new NavigationPage(new ListUsers());
        IsPresented = false;
    }
}