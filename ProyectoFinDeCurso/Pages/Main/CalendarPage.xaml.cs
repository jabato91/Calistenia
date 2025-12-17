using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Pages.Detail;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProyectoFinDeCurso.Pages.Main;

public partial class CalendarPage : ContentPage
{
    private readonly DbService _dbService;
    private readonly userTypeEnum _userType;
    public CalendarPage(DbService dbService, userTypeEnum userType) //Inyección de dependencia
    {
        InitializeComponent();
        BindingContext = new CalendarViewModel(DaySelected); //Pasamos el método DaySelected al ViewModel
        _dbService = dbService; //Asignamos el servicio de base de datos a una variable local
        _userType = userType;
        NavigationPage.SetTitleView(this, BuildTitleView()); // Establece la vista personalizada del título
    }
    private async void DaySelected(CalendarDay day) //Método que se ejecuta al seleccionar un día en el calendario
    {
        try
        {
            DateTime fecha = day.Date; //Obtenemos la fecha seleccionada

            string fechaTexto = fecha.ToString("dd/MM/yyyy"); //Convertimos la fecha a texto

            await Navigation.PushModalAsync(
                new CalendarDetailPage(_dbService, ModeEnum.View, fechaTexto)
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en DaySelected: {ex.Message}");
            await DisplayAlert("Error", "No se pudo abrir el detalle del día.", "OK");
        }
    }
    private async void OnAddAlarm(object sender, EventArgs e) //Método al pulsar botón añadir alarma
    {
        try
        {
            await Navigation.PushModalAsync(
                new CalendarDetailPage(_dbService, ModeEnum.create)
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en OnAddAlarm: {ex.Message}");
            await DisplayAlert("Error", "No se pudo abrir la creación de alarma.", "OK");
        }
    }
    private async void profile(object sender, EventArgs e) // Evento perfil de usuario
    {
        try
        {
            await Navigation.PushModalAsync(
                new UserDetailPage(null, _dbService, _userType, ModeEnum.View)
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en profile: {ex.Message}");
            await DisplayAlert("Error", "No se pudo abrir el perfil del usuario.", "OK");
        }
    }
    private View BuildTitleView() // Construye la vista personalizada del título
    {
        var grid = new Grid
        {
            Padding = new Thickness(10, 5),
            VerticalOptions = LayoutOptions.Center,
            ColumnDefinitions =
        {
            new ColumnDefinition { Width = GridLength.Auto },    // (0) Izquierda
            new ColumnDefinition { Width = GridLength.Star },    // (1) Centro
            new ColumnDefinition { Width = GridLength.Auto }     // (2) Derecha
        }
        };

        var titleLabel = new Label // titulo
        {
            Text = "Calendario",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            FontSize = 22,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#C77B30"),
            FontFamily = "Forresten",
            Margin = new Thickness(0, 0, 0, 0)
        };
        Grid.SetColumn(titleLabel, 1);
        grid.Children.Add(titleLabel);

        var profileButton = new ImageButton // boton perfil
        {
            Source = "icono_predeterminado.png",
            WidthRequest = 35,
            HeightRequest = 35,
            BackgroundColor = Colors.Transparent,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.End,
            Margin = new Thickness(0, 0, 5, 0)
        };
        Grid.SetColumn(profileButton, 2); // columna derecha

        // Evento de perfil
        profileButton.Clicked += profile;

        grid.Children.Add(profileButton);

        return grid;
    }
}