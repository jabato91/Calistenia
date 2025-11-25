
using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Pages.Detail;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;
namespace ProyectoFinDeCurso.Pages.Main;

public partial class ListUsers : ContentPage
{
    private DbService _dbService;
    private ListUsersViewModel _viewModel;
    public ListUsers(DbService dbService)
	{
        _dbService = dbService;
        InitializeComponent();
        _viewModel = new ListUsersViewModel(_dbService); //crea el viewmodel asociado a la página
        BindingContext = _viewModel; //asocia el viewmodel al bindingcontext de la página
    }
    private async void EliminateUser(object sender, EventArgs e) //elimina un usuario
    {
        var eliminate = sender as ImageButton;

        var user = eliminate?.BindingContext as User; //recoge el ejercicio al que está asociado

        if (user != null) // verifica que el usuario no sea nulo
        {
            await _dbService.DeleteUserById(user.UserID); //elimina el usuario de la base de datos

            var viewModel = BindingContext as ListUsersViewModel; //obtiene el viewmodel asociado a la página
            viewModel?.Users.Remove(user); //elimina el usuario de la lista visible

        }
    }
    private async void modifyUser(object sender, EventArgs e) //modifica un usuario
    {
        if ((sender as ImageButton)?.BindingContext is not User selectedExercise)//recoge el ejercicio al que está asociado
        {

            return;
        }

        await Navigation.PushModalAsync(
                new UserDetailPage(selectedExercise, _dbService, userTypeEnum.nothing, ModeEnum.Edit, _viewModel)
            ); // Navega a la página de detalles del ejercicio en modo de edición

    }
}