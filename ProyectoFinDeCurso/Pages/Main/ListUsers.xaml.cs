using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.ViewModels;
namespace ProyectoFinDeCurso.Pages.Main;

public partial class ListUsers : ContentPage
{
	
   
    public ListUsers()
	{
		InitializeComponent();

		BindingContext = new ListUsersViewModel();
    }




}