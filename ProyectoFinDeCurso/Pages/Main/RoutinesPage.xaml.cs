using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Pages.Detail;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;
using System.Collections.ObjectModel;
using System.Reflection.PortableExecutable;

namespace ProyectoFinDeCurso.Pages.Main;

public partial class RoutinesPage : ContentPage
{
    private readonly DbService _dbService;
    private static userTypeEnum _userType;
    private RoutinesFilterViewModel _filter;
    private ExerciseFilterViewModel _exerciseFilterViewModel;

    public RoutinesPage(DbService dbService, userTypeEnum userType)
    {
        try
        {
            _dbService = dbService;
            _userType = userType;
            _filter = new RoutinesFilterViewModel(_dbService, _userType); // asigno el viewmodel

            InitializeComponent();
            BindingContext = _filter; // asigno el bindingcontext

            NavigationPage.SetTitleView(this, BuildTitleView()); // Establece la vista personalizada del título
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inicializando RoutinesPage: {ex.Message}");
            Application.Current.MainPage.DisplayAlert("Error", "No se pudo cargar la página.", "OK");
        }
    }
    protected override async void OnAppearing() // Carga las rutinas al aparecer la página
    {
        base.OnAppearing(); // Llama al método base OnAppearing
        try
        {
            await _filter.LoadRoutinesAsync();  // Carga las rutinas desde la base de datos
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cargando rutinas: {ex.Message}");
            await DisplayAlert("Error", "No se pudieron cargar las rutinas.", "OK");
        }
    }
    private async void OnExerciseTapped(object sender, EventArgs e) // Maneja el evento de toque en un ejercicio
    {
        try
        {
            if ((sender as Grid)?.BindingContext is Exercise selectedExercise) // Verifica si el contexto de enlace es un ejercicio
            {
                await Navigation.PushModalAsync(new ExerciseDetailPage(_dbService, _exerciseFilterViewModel, selectedExercise, ModeEnum.View)); // Navega a la página de detalles del ejercicio en modo de vista
            }
        }
       
        catch (Exception ex)
        {
            // Muestra un mensaje de error amigable
            await DisplayAlert(
                "Error",
                $"Ocurrió un error al abrir el ejercicio:\n{ex.Message}",
                "OK"
            );
        }
    }

    private async void accessRoutine(object sender, TappedEventArgs e) // Maneja el evento de toque en una rutina
    {
        try
        {
            if ((sender as Border)?.BindingContext is Routines selectedRoutine) // Verifica si el contexto de enlace es una rutina
            {
                await Navigation.PushModalAsync(new RoutineDetailPage(_dbService, _filter,mode: ModeEnum.View, routine: selectedRoutine,userType: _userType)); // Navega a la página de detalles de la rutina en modo de vista
            }
        }
        catch (Exception ex)
        {
            // Muestra un mensaje de error amigable
            await DisplayAlert(
                "Error",
                $"Ocurrió un error al abrir la rutina:\n{ex.Message}",
                "OK"
            );
        }

    }



    private async void createRoutine(object sender, TappedEventArgs e) // Maneja el evento de creación de una nueva rutina
    {
        try
        {
            await Navigation.PushModalAsync(new RoutineDetailPage(_dbService, _filter,userType: _userType,mode: ModeEnum.create)); // Navega a la página de detalles de la rutina en modo de creación
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "No se pudo crear una nueva rutina.", "OK");
        }
    } 
    
    private async void OnExpanded(object sender, ExpandedChangedEventArgs e) // Maneja el evento de expansión de un Expander
    {
        try
        {
            if (sender is not Expander expander) // Verifica si el remitente es un Expander
            return;

        if (expander.Content is not VisualElement content) // Verifica si el contenido es un VisualElement
            return;

        if (e.IsExpanded) // Si el Expander está expandido, realiza la animación
        {
            content.Opacity = 0;
            content.TranslationY = -20;
            await Task.WhenAll(
                content.FadeTo(1, 250, Easing.SinInOut),
                content.TranslateTo(0, 0, 250, Easing.SinInOut)
            );
        }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error animando expander: {ex.Message}");
        }

    }
    private async void filterExercises(object sender, EventArgs e) // Maneja el evento de filtrado de ejercicios
    {
        try
        {
            await Navigation.PushModalAsync(new RoutineDetailPage( filterViewModel: _filter, mode: ModeEnum.filter)); // Navega a la página de detalles de la rutina en modo de filtro
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "No se pudo abrir el filtro de rutinas.", "OK");
        }
    }

    private async void profile(object sender, EventArgs e) // Maneja el evento del botón de perfil
    {
        try
        {
                await Navigation.PushModalAsync(
               new UserDetailPage(null, _dbService, _userType, ModeEnum.View)
           ); // Navega a la página de detalles del usuario en modo de vista
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "No se pudo abrir el perfil.", "OK");
        }
    }
    private View BuildTitleView() // Construye la vista personalizada del título
    {
        // Grid principal del TitleView
        var grid = new Grid
        {
            Padding = new Thickness(10, 5),
            VerticalOptions = LayoutOptions.Center,
            ColumnDefinitions =
        {
            new ColumnDefinition { Width = GridLength.Auto },   // (0) margen izquierda / decoración opcional
            new ColumnDefinition { Width = GridLength.Star },   // (1) título centrado
            new ColumnDefinition { Width = GridLength.Auto }    // (2) botón perfil
        }
        };

        // ***** TÍTULO *****
        var titleLabel = new Label // titulo centrado
        {
            Text = "Inicio",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            FontSize = 22,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#C77B30"),
            FontFamily = "Forresten",
            Margin = new Thickness(0, 0, 0, 0)
        };
        Grid.SetColumn(titleLabel, 1);
        grid.Children.Add(titleLabel); // Añade el título al grid

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
        Grid.SetColumn(profileButton, 2); // Coloca el botón en la columna 2

        profileButton.Clicked += profile; // Conecta al handler existente

        grid.Children.Add(profileButton); // Añade el botón al grid

        return grid; // Devuelve el grid completo como la vista del título
    }
}