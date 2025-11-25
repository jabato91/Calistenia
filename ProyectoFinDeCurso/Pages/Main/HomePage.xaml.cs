
using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Pages.Detail;
using ProyectoFinDeCurso.Pages.Main;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;
namespace ProyectoFinDeCurso.Pages;

public partial class HomePage : ContentPage
{
    private readonly DbService _dbService;
    private readonly userTypeEnum _userType;
    public HomePage(DbService dbService, userTypeEnum userType)
	{
        _dbService = dbService;
        _userType = userType;
        InitializeComponent();
        // Inicializa los ejercicios solo si no existen
        var initializer = new CreateExercises(_dbService);
        NavigationPage.SetTitleView(this, BuildTitleView());
    }

    
    private async void LogoutButton(object sender, EventArgs e)
    {
        SecureStorage.Remove("user_email");
        SecureStorage.Remove("user_id");
        await Navigation.PushModalAsync(new LoginPage(new DbService()));
    }
    private async void profile(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(
           new UserDetailPage(null,_dbService, _userType, ModeEnum.View)
       );
    }
    private View BuildTitleView()
    {
        var grid = new Grid
        {
            Padding = new Thickness(10, 5),
            VerticalOptions = LayoutOptions.Center,
            ColumnDefinitions =
        {
            new ColumnDefinition { Width = GridLength.Auto },   // Columna izquierda (vacía)
            new ColumnDefinition { Width = GridLength.Star },   // Título centrado
            new ColumnDefinition { Width = GridLength.Auto }    // Botón perfil
        }
        };

        // ---- TÍTULO ----
        var titleLabel = new Label
        {
            Text = "Inicio",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            FontSize = 22,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#C77B30"),
            FontFamily = "Forresten",
            Margin = new Thickness(0)
        };
        Grid.SetColumn(titleLabel, 1);
        grid.Children.Add(titleLabel);

        // ---- BOTÓN PERFIL ----
        var profileButton = new ImageButton
        {
            Source = "icono_predeterminado.png",
            WidthRequest = 35,
            HeightRequest = 35,
            BackgroundColor = Colors.Transparent,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 5, 0)
        };
        Grid.SetColumn(profileButton, 2);

        // Conecta al handler existente
        profileButton.Clicked += profile;

        grid.Children.Add(profileButton);

        return grid;
    }
}
