
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
        
        NavigationPage.SetTitleView(this, BuildTitleView()); // Establece la vista personalizada del título
    }

    
    private async void LogoutButton(object sender, EventArgs e) // Maneja el evento de cierre de sesión
    {
        SecureStorage.Remove("user_email");
        SecureStorage.Remove("user_id");
        await Navigation.PushModalAsync(new LoginPage(new DbService()));
    }
    private async void profile(object sender, EventArgs e) // Maneja el evento del botón de perfil
    {
        await Navigation.PushModalAsync(
           new UserDetailPage(null,_dbService, _userType, ModeEnum.View)
       ); // Navega a la página de detalles del usuario en modo de vista
    }
    private View BuildTitleView() // Construye la vista personalizada del título
    {
        var grid = new Grid // Grid principal del TitleView
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

        var titleLabel = new Label // titulo centrado
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

        var profileButton = new ImageButton // botón de perfil a la derecha
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

        return grid; // Devuelve el grid completo como la vista del título
    }
}
