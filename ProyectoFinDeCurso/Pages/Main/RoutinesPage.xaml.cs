using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ProyectoFinDeCurso.Pages.Main;

public partial class RoutinesPage : ContentPage
{
    private readonly DbService _dbService;
    private static userTypeEnum _userType;
    public RoutinesPage(DbService dbService, userTypeEnum userType)
	{
        _dbService = dbService;
		InitializeComponent();
        _userType = userType;
		BindingContext = new RoutinesFilterViewModel(_dbService, _userType);
    }

    private async void OnExerciseTapped(object sender, EventArgs e)
    {
        try
        {
            if ((sender as Grid)?.BindingContext is Exercise selectedExercise)
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
                    FontFamily = "calculator",
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
    private async void createRoutine(object sender, TappedEventArgs e)
    {
        {
            var translation = new Dictionary<bodyPartEnum, string>
    {
        { bodyPartEnum.nothing, "Ninguno" },
        { bodyPartEnum.chest, "Pecho" },
        { bodyPartEnum.leg, "Piernas" },
        { bodyPartEnum.triceps, "Tríceps" },
        { bodyPartEnum.biceps, "Bíceps" },
        { bodyPartEnum.abdomen, "Abdomen" },
        { bodyPartEnum.back, "Espalda" },
        { bodyPartEnum.shoulder, "Hombros" },
        { bodyPartEnum.isometric, "Isométrico" },
        { bodyPartEnum.arms, "Brazos" },
        { bodyPartEnum.torso, "Torso" },
        { bodyPartEnum.torsoAndArms, "Torso y Brazos" }
    };
            var nameRoutineEntry = new Entry
            {
                Placeholder = "Nombre de la rutina",
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),

                HorizontalOptions = LayoutOptions.Fill
            };
            var DescriptionRoutineEntry = new Entry
            {
                Placeholder = "Nombre de la rutina",
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),

                HorizontalOptions = LayoutOptions.Fill
            };
            var bodyPartEnumPicker = new Picker
            {
                Title = "Tipo Cuerpo",
                ItemsSource = translation.Values.ToList(),
                SelectedItem = translation[bodyPartEnum.nothing],
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill
            };
            var addExercises = new Button
            {
                Text = "Agregar Ejercicios",
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill
            };
            addExercises.Clicked += async (s, e) =>
            {
                string selectedText = bodyPartEnumPicker.SelectedItem as string;
                bodyPartEnum selectedEnum = translation.FirstOrDefault(x => x.Value == selectedText).Key;

                if (selectedEnum.Equals(bodyPartEnum.nothing))
                {
                    await DisplayAlert("Error", "Seleccione un grupo muscular", "OK");
                    return;
                }
                else
                {
                    List<Exercise> exercises = await _dbService.GetEercises();
                    List<Exercise> ExercisesSelecter;

                    if (selectedEnum.Equals(bodyPartEnum.torsoAndArms))
                    {
                        ExercisesSelecter = exercises.Where(ex =>
                            ex.muscleGroupId.Equals(bodyPartEnum.triceps)
                            || ex.muscleGroupId.Equals(bodyPartEnum.biceps)
                            || ex.muscleGroupId.Equals(bodyPartEnum.chest)
                            || ex.muscleGroupId.Equals(bodyPartEnum.back)).ToList();
                    }
                    else if (selectedEnum.Equals(bodyPartEnum.arms))
                    {
                        ExercisesSelecter = exercises.Where(ex =>
                            ex.muscleGroupId.Equals(bodyPartEnum.triceps)
                            || ex.muscleGroupId.Equals(bodyPartEnum.biceps)).ToList();
                    }
                    else if (selectedEnum.Equals(bodyPartEnum.torso))
                    {
                        ExercisesSelecter = exercises.Where(ex =>
                            ex.muscleGroupId.Equals(bodyPartEnum.chest)
                            || ex.muscleGroupId.Equals(bodyPartEnum.back)).ToList();
                    }
                    else
                    {
                        ExercisesSelecter = exercises.Where(ex => ex.muscleGroupId.Equals(selectedEnum)).ToList();
                    }

                    var grouped = ExercisesSelecter
                        .GroupBy(ex => ex.muscleGroupId)
                        .OrderBy(g => g.Key)
                        .SelectMany(muscleGroup =>
                            muscleGroup
                                .GroupBy(ex => ex.dificulty)
                                .OrderBy(g => g.Key)
                                .Select(diffGroup => new ExerciseAndDificultyGroup(
                                    muscleGroup.Key,
                                    diffGroup.Key,
                                    diffGroup.ToList()
                                ))
                        )
                        .ToList();

                    // 🟩 Cambiar List por ObservableCollection
                    ObservableCollection<Exercise> exerciseSelection = new ObservableCollection<Exercise>();

                    var lookForExercise = new SearchBar
                    {
                        Placeholder = "Buscar ejercicio..."
                    };

                    // 🔹 CollectionView principal (selección de ejercicios)
                    var collectionExercises = new CollectionView
                    {
                        ItemsSource = grouped,
                        IsGrouped = true,
                        GroupHeaderTemplate = new DataTemplate(() =>
                        {
                            var difficultyBar = new BoxView
                            {
                                WidthRequest = 6,
                                CornerRadius = 3,
                                HorizontalOptions = LayoutOptions.Start,
                                VerticalOptions = LayoutOptions.Fill
                            };
                            difficultyBar.SetBinding(BoxView.ColorProperty, "HeaderColor");

                            var headerLabel = new Label
                            {
                                FontAttributes = FontAttributes.Bold,
                                FontSize = 18,
                                FontFamily = "Forresten",
                                TextColor = Colors.Orange,
                                Margin = new Thickness(10, 5)
                            };
                            headerLabel.SetBinding(Label.TextProperty, "HeaderText");

                            var grid = new Grid
                            {
                                ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = 10 },
                        new ColumnDefinition { Width = GridLength.Star }
                    },
                                BackgroundColor = Color.FromArgb("#2B1A19")
                            };

                            grid.Add(difficultyBar, 0, 0);
                            grid.Add(headerLabel, 1, 0);

                            return grid;
                        }),
                        ItemTemplate = new DataTemplate(() =>
                        {
                            var nameExercise = new Label
                            {
                                FontAttributes = FontAttributes.Bold,
                                TextColor = Colors.White
                            };
                            nameExercise.SetBinding(Label.TextProperty, "name");

                            var frame = new Frame
                            {
                                Margin = 5,
                                Padding = 10,
                                BackgroundColor = Color.FromArgb("#3B2523"),
                                CornerRadius = 8,
                                HasShadow = false
                            };

                            frame.Content = new VerticalStackLayout
                            {
                                Children = { nameExercise }
                            };

                            // 🔹 Tap: abrir modal para añadir
                            var tapGesture = new TapGestureRecognizer();
                            tapGesture.Tapped += async (s, e) =>
                            {
                                if (frame.BindingContext is Exercise selectedExercise)
                                {
                                    var getExerciseLabel = new Label
                                    {
                                        FontAttributes = FontAttributes.Bold,
                                        FontSize = 18,
                                        FontFamily = "Forresten",
                                        TextColor = Color.FromArgb("#C77B30"),
                                        Margin = new Thickness(10, 5),
                                        Text = selectedExercise.name
                                    };

                                    var repsLabel = new Entry
                                    {
                                        Placeholder = "Número de repeticiones",
                                        Keyboard = Keyboard.Numeric
                                    };
                                    var setsLabel = new Entry
                                    {
                                        Placeholder = "Número de series",
                                        Keyboard = Keyboard.Numeric
                                    };

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
                                                Padding = 1,
                                                Spacing = 5,
                                                Children =
                                    {
                                        new Label
                                        {
                                            Text = "Añadir ejercicio",
                                            FontSize = 24,
                                            TextColor = Colors.Orange,
                                            HorizontalOptions = LayoutOptions.Fill,
                                            HorizontalTextAlignment = TextAlignment.Center,
                                            FontFamily="EatMeAlive"
                                        },
                                        getExerciseLabel,
                                        repsLabel,
                                        setsLabel,
                                        new HorizontalStackLayout
                                        {
                                            Spacing = 10,
                                            Children =
                                            {
                                                new Button
                                                {
                                                    Text = "Crear",
                                                    Command = new Command(async () =>
                                                    {
                                                        if (string.IsNullOrEmpty(repsLabel.Text) || string.IsNullOrEmpty(setsLabel.Text))
                                                        {
                                                            await Application.Current.MainPage.DisplayAlert("Error", "Introduce series y repeticiones.", "OK");
                                                            return;
                                                        }

                                                        // 🟩 Agregar nuevo ejercicio a la lista observable
                                                        exerciseSelection.Add(new Exercise
                                                        {
                                                            sets = int.TryParse(setsLabel.Text, out int r) ? r : 0,
                                                            reps = int.TryParse(repsLabel.Text, out int s) ? s : 0,
                                                            name = getExerciseLabel.Text
                                                        });

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

                                    await Navigation.PushModalAsync(modalPage);
                                }
                            };

                            frame.GestureRecognizers.Add(tapGesture);
                            return frame;
                        })
                    };

                    // 🟩 NUEVO: lista para mostrar ejercicios seleccionados
                    var selectedExercisesView = new CollectionView
                    {
                        ItemsSource = exerciseSelection,
                        EmptyView = new Label
                        {
                            Text = "No hay ejercicios seleccionados.",
                            TextColor = Colors.Gray,
                            HorizontalOptions = LayoutOptions.Center
                        },
                        ItemTemplate = new DataTemplate(() =>
                        {
                            var nameLabel = new Label
                            {
                                FontAttributes = FontAttributes.Bold,
                                TextColor = Colors.White
                            };
                            nameLabel.SetBinding(Label.TextProperty, "name");

                            var infoLabel = new Label
                            {
                                FontSize = 12,
                                TextColor = Colors.LightGray
                            };
                            infoLabel.SetBinding(Label.TextProperty, new Binding("reps", stringFormat: "Reps: {0}"));

                            var removeButton = new Button
                            {
                                Text = "❌",
                                BackgroundColor = Colors.Transparent,
                                TextColor = Colors.Orange,
                                FontSize = 18,
                                Padding = new Thickness(5)
                            };
                            removeButton.Clicked += (s, e) =>
                            {
                                if (removeButton.BindingContext is Exercise exToRemove)
                                    exerciseSelection.Remove(exToRemove);
                            };

                            return new Frame
                            {
                                Margin = 5,
                                Padding = 8,
                                BackgroundColor = Color.FromArgb("#3B2523"),
                                Content = new HorizontalStackLayout
                                {
                                    Spacing = 10,
                                    Children = { nameLabel, infoLabel, removeButton }
                                }
                            };
                        })
                    };

                    // 🟩 Agregamos todo en un layout vertical
                    var mainLayout = new VerticalStackLayout
                    {
                        Spacing = 10,
                        Padding = 10,
                        Children =
            {
                lookForExercise,
                new Label
                {
                    Text = "Ejercicios seleccionados:",
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 20,
                    TextColor = Colors.Orange
                },
                selectedExercisesView,
                collectionExercises,
                
            }
                    };

                    var selectedExercisesPage = new ContentPage
                    {
                        BackgroundColor = Color.FromArgb("#2E1E1B"),
                        Content = new ScrollView { Content = mainLayout }
                    };

                    await Navigation.PushModalAsync(selectedExercisesPage);
                }
            };
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
                        Text = "Crear Rutina",
                        FontSize = 24,

                        TextColor = Color.FromArgb("#C77B30"),

                        HorizontalOptions = LayoutOptions.Fill,
                        HorizontalTextAlignment = TextAlignment.Center,
                        FontFamily="EatMeAlive"
                    },
                    nameRoutineEntry,
                    DescriptionRoutineEntry,
                    bodyPartEnumPicker,
                    addExercises,
                    new HorizontalStackLayout
                    {
                        Spacing = 10,
                        Children =
                        {
                             new Button
                        {
                            Text = "Guardar",
                            Command = new Command(() =>
                            {





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
                await Navigation.PushModalAsync(modalPage);
            };
        } 
    
    private async void OnExpanded(object sender, ExpandedChangedEventArgs e)
    {
        if (sender is not Expander expander)
            return;

        if (expander.Content is not VisualElement content)
            return;

        if (e.IsExpanded)
        {
            // 🔹 ANIMACIÓN AL ABRIR
            content.Opacity = 0;
            content.TranslationY = -20;
            await Task.WhenAll(
                content.FadeTo(1, 250, Easing.SinInOut),
                content.TranslateTo(0, 0, 250, Easing.SinInOut)
            );
        }
       
    }

}