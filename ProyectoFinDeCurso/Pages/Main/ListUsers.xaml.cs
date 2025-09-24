using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.ViewModels;
namespace ProyectoFinDeCurso.Pages.Main;

public partial class ListUsers : ContentPage
{
	public List<User> users = new List<User>();
	
   
    public ListUsers()
	{
		InitializeComponent();

		BindingContext = new ListUsersViewModel();
    }




}