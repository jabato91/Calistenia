
using CommunityToolkit.Maui.Media;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;
using System;
using System.Collections.ObjectModel;
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
        // Solo asignamos el BindingContext, no llamamos OnAppearing manualmente
        _filter = new ExerciseFilterViewModel(_dbService,userType);
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

                    BackgroundColor = Color.FromArgb("#D69C90"),
                    Content = new Frame
                    {
                        BackgroundColor = Color.FromArgb("#C44B4B"),
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
                    BackgroundColor =  Color.FromArgb("#BF9F9F"),
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
            await DisplayAlert(
                "Error",
                $"Ocurrió un error al abrir el ejercicio:\n{ex.Message}",
                "OK"
            );
        }
    }
    
    private async void eliminateExercise(object sender, EventArgs e)
    {
        var eliminate = sender as ImageButton;

        var exercise = eliminate?.BindingContext as Exercise; //recoge el ejercicio al que está asociado

        if (exercise != null)
        {
            await _dbService.DeleteExerciseById(exercise.execiseID);
            // Actualizamos la colección del ViewModel
            _filter.Exercises.Remove(exercise);
            _filter.OnPropertyChanged(nameof(_filter.Exercises));
            _filter.OnPropertyChanged(nameof(_filter.FilteredExercises));

        }
    }

    private async void modifyExercise(object sender, EventArgs e)
    {
        if ((sender as ImageButton)?.BindingContext is not Exercise selectedExercise)//recoge el ejercicio al que está asociado
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
            TextColor = Color.FromArgb("#C49362"),
            BackgroundColor = Color.FromArgb("#3B2523"),

            HorizontalOptions = LayoutOptions.Fill
        };

        var descEntry = new Entry
        {
            Text = exerciseFromDb.description,
            Placeholder = "Descripción",
            TextColor = Color.FromArgb("#C49362"),
            BackgroundColor = Color.FromArgb("#3B2523"),
            HorizontalOptions = LayoutOptions.Fill
        };

        var imageButton = new ImageButton
        {
            Source = exerciseFromDb.image,
            HorizontalOptions = LayoutOptions.Fill,
            WidthRequest = 75,
            HeightRequest = 75
        };
        imageButton.Clicked += async (s, e) =>
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Selecciona una imagen",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                imageButton.Source = ImageSource.FromFile(result.FullPath);
            }
        };
        var bodyPartEnumPicker = new Picker
        {
            Title = "Tipo Cuerpo",
            ItemsSource = traducciones.Values.ToList(),
            SelectedItem = traducciones[exerciseFromDb.muscleGroupId],
            TextColor = Color.FromArgb("#C49362"),
            BackgroundColor = Color.FromArgb("#3B2523"),
            HorizontalOptions = LayoutOptions.Fill
        };

        // Creamos la página modal
        var modalPage = new ContentPage
        {
            BackgroundColor = Color.FromRgba(0, 0, 0, 0.6),
            Content = new Frame

            {
                BackgroundColor = Color.FromArgb("#2E1E1B"),
                CornerRadius = 20,
                Margin = 1,
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

                        TextColor = Color.FromArgb("#C77B30"),

                        HorizontalOptions = LayoutOptions.Fill,
                        HorizontalTextAlignment = TextAlignment.Center,
                        FontFamily="EatMeAlive"
                    },
                    nameEntry,
                    descEntry,
                    imageButton,
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
                                 exerciseFromDb.name = nameEntry.Text ?? "";
                                 exerciseFromDb.description = descEntry.Text ?? "";

                                if (imageButton.Source is FileImageSource fileSource)
                                {
                                    
                                    string rutaOrigen = fileSource.File; //obtiene la ruta de la carpeta
                                    string nombreArchivo = Path.GetFileName(rutaOrigen);

                                    
                                    string carpetaImagenes = Path.Combine(FileSystem.AppDataDirectory, "Images");
                                    if (!Directory.Exists(carpetaImagenes)) //crea la carpeta si no esiste
                                        Directory.CreateDirectory(carpetaImagenes);

                                    string rutaDestino = Path.Combine(carpetaImagenes, nombreArchivo);//obtener la ruta de destino

                                    if (!File.Exists(rutaDestino) || rutaOrigen != rutaDestino) //verifica si la imagen fue cambiada
                                    {
                                    try
                                    {
                                        File.Copy(rutaOrigen, rutaDestino, overwrite: true);
                                    }
                                    catch (Exception ex)
                                    {
                                        await DisplayAlert("Error", $"No se pudo copiar la imagen: {ex.Message}", "OK");
                                    }
                                }

                                
                                exerciseFromDb.image = nombreArchivo; //guarda el nombre del archivo
                            }

                                await _dbService.Update(exerciseFromDb);

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
    private async void editImage(object sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Selecciona una imagen",
                FileTypes = FilePickerFileType.Images // Puedes poner .Pdf, .Videos, etc.
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo abrir el archivo: {ex.Message}", "OK");
        }

    }
}