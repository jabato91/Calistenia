
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
        try
        {
            _dbService = dbService;
            InitializeComponent();
            _viewModel = new ListUsersViewModel(_dbService); //crea el viewmodel asociado a la página
            BindingContext = _viewModel; //asocia el viewmodel al bindingcontext de la página
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error iniciando ListUsers: {ex.Message}");
            DisplayAlert("Error", "La página no pudo iniciarse correctamente.", "OK");
        }
    }
    private async void EliminateUser(object sender, EventArgs e) //elimina un usuario
    {
        try
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error eliminando usuario: {ex.Message}");
            await DisplayAlert("Error", "No se pudo eliminar el usuario.", "OK");
        }
    }
    private async void modifyUser(object sender, EventArgs e) //modifica un usuario
    {
        try
        {
            if ((sender as ImageButton)?.BindingContext is not User selectedExercise)//recoge el ejercicio al que está asociado
            {

                return;
            }

            await Navigation.PushModalAsync(
                    new UserDetailPage(selectedExercise, _dbService, userTypeEnum.nothing, ModeEnum.Edit, _viewModel)
                ); // Navega a la página de detalles del ejercicio en modo de edición
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error modificando usuario: {ex.Message}");
            await DisplayAlert("Error", "No se pudo abrir la edición del usuario.", "OK");
        }
    }
}