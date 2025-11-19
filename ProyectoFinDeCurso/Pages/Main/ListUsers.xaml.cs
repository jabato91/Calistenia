using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;
namespace ProyectoFinDeCurso.Pages.Main;

public partial class ListUsers : ContentPage
{
    private DbService _dbService;

    public ListUsers(DbService dbService)
	{
        _dbService = dbService;
        InitializeComponent();

		BindingContext = new ListUsersViewModel(_dbService);
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

        User userFromDb = await _dbService.GetUserById(selectedExercise.UserID);

        var nameEntry = new Entry
        {
            Text = userFromDb.Name,
            Placeholder = "Nombre",
            TextColor = Color.FromArgb("#C49362"),
            BackgroundColor = Color.FromArgb("#3B2523"),

            HorizontalOptions = LayoutOptions.Fill
        };
        var firstNameEntry = new Entry
        {
            Text = userFromDb.FirstSurname,
            Placeholder = "Nombre",
            TextColor = Color.FromArgb("#C49362"),
            BackgroundColor = Color.FromArgb("#3B2523"),

            HorizontalOptions = LayoutOptions.Fill
        };
        var secondSurnameEntry = new Entry
        {
            Text = userFromDb.SecondSurname,
            Placeholder = "Nombre",
            TextColor = Color.FromArgb("#C49362"),
            BackgroundColor = Color.FromArgb("#3B2523"),

            HorizontalOptions = LayoutOptions.Fill
        };

        

        var modalPage = new ContentPage
        {
            BackgroundColor = Color.FromRgba(0, 0, 0, 0.6),
            Content = new Frame

            {
            }
        };
        
    }
}