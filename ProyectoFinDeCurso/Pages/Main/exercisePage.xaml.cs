
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;
using CommunityToolkit.Maui.Media;
namespace ProyectoFinDeCurso.Pages.Main;

public partial class exercisePage : ContentPage
{
    private HashSet<VisualElement> animatedElements = new HashSet<VisualElement>();

    private readonly DbService _dbService;
    private ExerciseFilterViewModel _filter;
    public exercisePage(DbService dbService, userTypeEnum userType)
    {
        InitializeComponent();
        _dbService = dbService;
        verificationUserType(userType);
        // Solo asignamos el BindingContext, no llamamos OnAppearing manualmente
        _filter = new ExerciseFilterViewModel(_dbService);
        BindingContext = _filter;

    }

    private async void OnExerciseTapped(object sender, EventArgs e)
    {
        try
        {
            if ((sender as Frame)?.BindingContext is Exercise selectedExercise)
            {
                // Traemos la instancia actual desde la DB
                Exercise exerciseFromDb = await _dbService.GetExerciseById(selectedExercise.execiseID);

                var modalPage = new ContentPage
                {

                    BackgroundColor = Color.FromHex("#D69C90"),
                    Content = new Frame
                    {
                        BackgroundColor = Color.FromHex("#C44B4B"),
                        CornerRadius = 20,
                        Margin = 1,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        Content = new VerticalStackLayout
                        {
                            Padding = 4,
                            Children =
            {
                new Button
                {
                    Text = "X",
                    BackgroundColor =  Color.FromHex("#BF9F9F"),
                     CornerRadius = 999,
                    WidthRequest = 40,
                    HeightRequest = 40,
                    FontSize =17,
                    Padding = new Thickness(0),
                     Margin = new Thickness(0, 5, 5, 0),
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Start,
                    Command = new Command(async () =>
                        await Navigation.PopModalAsync())
                },
                new Label
                {
                    Text = exerciseFromDb.name,
                    FontSize = 24,
                    HorizontalOptions = LayoutOptions.Center,
                    TextColor = Colors.Black
                },
                new Label
                {
                    Text = exerciseFromDb.description,
                    FontSize = 13,
                    HorizontalOptions = LayoutOptions.Center,
                    TextColor = Colors.Black,
                    Margin = 1.5
                },
                new Frame
                            {
                                CornerRadius = 15,
                                HasShadow = true,
                                BackgroundColor = Colors.Black,
                                Padding = 0,
                                Margin = new Thickness(2,3,2,4),
                                Content = new MediaElement
                                {
                                    Source = MediaSource.FromResource("prueba.mp4"),
                                    Aspect = Aspect.AspectFit,
                                    ShouldShowPlaybackControls = true,
                                    HeightRequest = 325,
                                    WidthRequest = 500
                                }
                            },
               
                }
                        }
                    }
                };

                // Muestra la ventana modal
                await Navigation.PushModalAsync(modalPage);
            }
        }
        catch (Exception ex)
        {
            // Muestra un mensaje de error amigable
            await Application.Current.MainPage.DisplayAlert(
                "Error",
                $"Ocurrió un error al abrir el ejercicio:\n{ex.Message}",
                "OK"
            );
        }
    }
    private void verificationUserType(userTypeEnum userType)
    {
        if (!userType.Equals(userTypeEnum.admin))
        {
            //eliminateE.IsVisible = false;
        }
    }
    private async void eliminateExercise(object sender, EventArgs e)
    {
        var button = sender as Button;

        var exercise = button?.BindingContext as Exercise;

        if (exercise != null)
        { 
            await _dbService.DeleteExerciseById(exercise.execiseID);
        }
    }

    private async void modifyExercise(object sender, EventArgs e)
    {
        if ((sender as ImageButton)?.BindingContext is not Exercise selectedExercise)
        {
            
            return;
        }

        // Traemos la instancia actual desde la DB
        Exercise exerciseFromDb = await _dbService.GetExerciseById(selectedExercise.execiseID);

        var traducciones = new Dictionary<bodyPartEnum, string>
    {
        { bodyPartEnum.nothing, "Ninguno" },
        { bodyPartEnum.chest, "Pecho" },
        { bodyPartEnum.leg, "Piernas" },
        { bodyPartEnum.triceps, "Tríceps" },
        { bodyPartEnum.biceps, "Bíceps" },
        { bodyPartEnum.abdomen, "Abdomen" },
        { bodyPartEnum.back, "Espalda" },
        { bodyPartEnum.shoulder, "Hombros" }
    };

        // Creamos controles y los guardamos en variables locales
        var nameEntry = new Entry
        {
            Text = exerciseFromDb.name,
            Placeholder = "Nombre",
            TextColor = Colors.Black,
            BackgroundColor = Colors.LightGray,
            HorizontalOptions = LayoutOptions.Fill
        };

        var descEntry = new Entry
        {
            Text = exerciseFromDb.description,
            Placeholder = "Descripción",
            TextColor = Colors.Black,
            BackgroundColor = Colors.LightGray,
            HorizontalOptions = LayoutOptions.Fill
        };

        var imageEntry = new Entry
        {
            Text = exerciseFromDb.image,
            Placeholder = "Imagen",
            TextColor = Colors.Black,
            BackgroundColor = Colors.LightGray,
            HorizontalOptions = LayoutOptions.Fill
        };

        var bodyPartEnumPicker = new Picker
        {
            Title = "Tipo Cuerpo",
            ItemsSource = traducciones.Values.ToList(),
            SelectedItem = traducciones[exerciseFromDb.muscleGroupId],
            TextColor = Colors.Black,
            BackgroundColor = Colors.LightGray,
            HorizontalOptions = LayoutOptions.Fill
        };

        // Creamos la página modal
        var modalPage = new ContentPage
        {
            BackgroundColor = Color.FromRgba(0, 0, 0, 0.6),
            Content = new Frame
            {
                BackgroundColor = Colors.White,
                CornerRadius = 50,
                Margin = 1, // margen pequeño respecto a la pantalla
                Padding = 1, // padding pequeño para que los controles estén cerca de los bordes
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Content = new VerticalStackLayout
                {
                    Padding = 1, // padding interno mínimo
                    Spacing = 5, // espacio entre elementos
                    Children =
                {
                    new Label
                    {
                        Text = "Modificar ejercicio",
                        FontSize = 24,
                        TextColor = Colors.Black,
                        BackgroundColor = Colors.LightGray,
                        HorizontalOptions = LayoutOptions.Fill,
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    nameEntry,
                    descEntry,
                    imageEntry,
                    bodyPartEnumPicker,
                    new HorizontalStackLayout
                    {
                        Spacing = 10,
                        Children =
                        {
                            new Button
                            {
                                Text = "Guardar",
                                Command = new Command(async () =>
                                {
                                    // Tomamos los valores directamente de las variables
                                    exerciseFromDb.name = nameEntry.Text ?? "";
                                    exerciseFromDb.description = descEntry.Text ?? "";
                                    exerciseFromDb.image = imageEntry.Text ?? "";

                                    if (bodyPartEnumPicker.SelectedIndex >= 0)
                                    {
                                        var selectedEnum = traducciones.Keys.ToList()[bodyPartEnumPicker.SelectedIndex];
                                        exerciseFromDb.muscleGroupId = selectedEnum;
                                    }

                                    // Guardamos en la DB
                                    await _dbService.Update(exerciseFromDb);

                                    // Actualizamos la colección del ViewModel
                                    var index = _filter.Exercises.IndexOf(selectedExercise);
                                    if (index >= 0)
                                    {
                                        _filter.Exercises[index] = exerciseFromDb;
                                        _filter.OnPropertyChanged(nameof(_filter.FilteredExercises));
                                    }

                                    await Navigation.PopModalAsync();
                                })
                            },
                            new Button
                            {
                                Text = "Cancelar",
                                Command = new Command(async () => await Navigation.PopModalAsync())
                            }
                        }
                    }
                }
                }
            }
        };

        // Mostramos el modal
        await Navigation.PushModalAsync(modalPage);
    }
    
}