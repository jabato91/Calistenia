using CommunityToolkit.Maui.Views;
using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;

namespace ProyectoFinDeCurso.Pages.Main;

public partial class exercisePage : ContentPage
{
    private HashSet<VisualElement> animatedElements = new HashSet<VisualElement>();

    private readonly DbService _dbService;
    Exercise selectedExercise;
    public exercisePage(DbService dbService, userTypeEnum userType)
    {
        InitializeComponent();
        _dbService = dbService;
        verificationUserType(userType);
        // Solo asignamos el BindingContext, no llamamos OnAppearing manualmente
        BindingContext = new ProyectoFinDeCurso.ViewModels.ExerciseFilterViewModel(_dbService);
    }
    private void OpenExercise(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Exercise selectedExercise)
        {

            
            
                DisplayAlert("Ejercicio seleccionado", selectedExercise.name, "OK"); // Limpia la selección (opcional) ((CollectionView)sender).SelectedItem = null; }
            
        }
    }
    private async void OnExerciseTapped(object sender, EventArgs e)
    {
        if ((sender as Frame)?.BindingContext is Exercise selectedExercise)
        {
            // Crea la página modal
            var modalPage = new ContentPage
            {
                BackgroundColor = Color.FromRgba(0, 0, 0, 0.6),
                Content = new Frame
                {
                    BackgroundColor = Colors.White,
                    CornerRadius = 20,
                    Margin = 30,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    Content = new VerticalStackLayout
                    {
                        Padding = 20,
                        Children =
                    {
                        new Label
                        {
                            Text = selectedExercise.name,
                            FontSize = 24,
                            HorizontalOptions = LayoutOptions.Center
                        },
                        new Label { Text = selectedExercise.description },
                        new Button
                        {
                            Text = "Cerrar",
                            Command = new Command(async () =>
                                await Navigation.PopModalAsync())
                        }
                    }
                    }
                }
            };

            // Muestra la ventana modal
            await Navigation.PushModalAsync(modalPage);
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
        if ((sender as Button)?.BindingContext is not Exercise selectedExercise)
            return;

        var selectExerciseID = selectedExercise.execiseID;

        Exercise exerciseFromDb = await _dbService.GetExerciseById(selectExerciseID);
        // Crear la página para modificar el ejercicio
        var modalPage = new ContentPage
        {
            BackgroundColor = Color.FromRgba(0, 0, 0, 0.6),
            Content = new Frame
            {
                BackgroundColor = Colors.White,
                CornerRadius = 20,
                Margin = 30,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Content = new VerticalStackLayout
                {
                    Padding = 20,
                    Spacing = 15,
                    Children =
                {
                    new Label
                    {
                        Text = "Modificar ejercicio",
                        FontSize = 24,
                        HorizontalOptions = LayoutOptions.Center
                    },
                    new Entry
                    {
                        Text = exerciseFromDb.name,
                        Placeholder = "Nombre",
                        HorizontalOptions = LayoutOptions.Fill
                    },
                    new Entry
                    {
                        Text = exerciseFromDb.description,
                        Placeholder = "Descripción",
                        HorizontalOptions = LayoutOptions.Fill
                    },
                    new Entry
                    {
                        Text = exerciseFromDb.muscleGroupId.ToString(),
                        Placeholder = "Tipo Cuerpo",
                        HorizontalOptions = LayoutOptions.Fill
                    },
                    new HorizontalStackLayout
                    {
                        Spacing = 10,
                        Children =
                        {
                            new Button
                            {
                                Text = "Guardar",
                                Command = new Command(async (btn) =>
                                {
                                    var stack = (btn as Button)?.Parent as HorizontalStackLayout;
                                    if (stack?.Parent is VerticalStackLayout vstack)
                                    {
                                        var nameEntry = vstack.Children[1] as Entry;
                                        var descEntry = vstack.Children[2] as Entry;

                                        if (nameEntry != null && descEntry != null)
                                        {
                                            selectedExercise.name = nameEntry.Text ?? "";
                                            selectedExercise.description = descEntry.Text ?? "";
                                        }
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

        // Mostrar el modal
        await Navigation.PushModalAsync(modalPage);
    }

    
}