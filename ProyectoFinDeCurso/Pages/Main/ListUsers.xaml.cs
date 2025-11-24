
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
        _viewModel = new ListUsersViewModel(_dbService);
        BindingContext = _viewModel;
    }
    private async void EliminateUser(object sender, EventArgs e)
    {
        var eliminate = sender as ImageButton;

        var user = eliminate?.BindingContext as User; //recoge el ejercicio al que está asociado

        if (user != null)
        {
            await _dbService.DeleteUserById(user.UserID);
            
            var viewModel = BindingContext as ListUsersViewModel;
            viewModel?.Users.Remove(user); //elimina el usuario de la lista visible

        }
    }
    private async void modifyUser(object sender, EventArgs e)
    {
        if ((sender as ImageButton)?.BindingContext is not User selectedExercise)//recoge el ejercicio al que está asociado
        {

            return;
        }

        await Navigation.PushModalAsync(
                new UserDetailPage(selectedExercise, _dbService, userTypeEnum.nothing, ModeEnum.Edit, _viewModel)
            );

    }
}