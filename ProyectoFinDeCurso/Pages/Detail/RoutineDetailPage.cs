using Microsoft.Maui.Controls.Shapes;
using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinDeCurso.Pages.Detail
{
    public class RoutineDetailPage : ContentPage
    {
        private readonly ExerciseMode _mode;
        private readonly Routines _routine;
        private readonly RoutinesFilterViewModel _filterViewModel;
        private readonly DbService _dbService;
        public RoutineDetailPage(DbService dbService,RoutinesFilterViewModel filterViewModel, Routines routine,ExerciseMode mode = ExerciseMode.Create) { 
            _filterViewModel = filterViewModel;
            _routine = routine;
            _mode = mode;
            _dbService = dbService;

            BuildUI();
        }

        private void BuildUI()
        {
            switch (_mode)
            {
                case ExerciseMode.Create:
                    BuildCreateUI();
                    break;

                case ExerciseMode.Edit:
                    BuildEditUI();
                    break;

                case ExerciseMode.View:
                    BuildViewUI();
                    break;
            }
        }

        private void BuildViewUI()
        {
            throw new NotImplementedException();
        }

        private void BuildEditUI()
        {
            throw new NotImplementedException();
        }

        private void BuildCreateUI()
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
                    Placeholder = "Descripción de la rutina",
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
                ObservableCollection<Exercise> exercisesInRoutine = new ObservableCollection<Exercise>();

                bodyPartEnumPicker.SelectedIndexChanged += async (s, e) =>
                {
                    // Verificamos si hay un elemento seleccionado
                    if (bodyPartEnumPicker.SelectedItem is string selectedText)
                    {
                        if (exercisesInRoutine != null && exercisesInRoutine.Any())
                        {
                            // Mostrar advertencia al usuario
                            bool answer = await DisplayAlert(
                                "Advertencia",
                                "Si cambias la parte del cuerpo, se borrarán todos los jercicios seleccionados.\n ¿Deseas cambiar la rutina?",
                                "Sí, vaciar",
                                "No"
                            );

                            if (answer)
                            {
                                exercisesInRoutine.Clear();
                                Console.WriteLine("Lista de ejercicios vaciada.");
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
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

                                var frame = new Border
                                {
                                    Margin = 5,
                                    Padding = 10,
                                    BackgroundColor = Color.FromArgb("#3B2523"),
                                    StrokeShape = new RoundRectangle
                                    {
                                        CornerRadius = 8
                                    },
                                };

                                frame.Content = new VerticalStackLayout
                                {
                                    Children = { nameExercise }
                                };
                                var tapGesture = new TapGestureRecognizer();
                                if (bodyPartEnumPicker.SelectedItem is string selectedText)
                                {
                                    if (!selectedText.Equals(translation[bodyPartEnum.isometric]))
                                    {
                                        // 🔹 Tap: abrir modal para añadir

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
                                                repsLabel.TextChanged += (s, e) =>
                                                {
                                                    if (!string.IsNullOrEmpty(repsLabel.Text))
                                                    {
                                                        string onlyDigits = new string(repsLabel.Text.Where(char.IsDigit).ToArray());
                                                        if (repsLabel.Text != onlyDigits)
                                                        {
                                                            repsLabel.Text = onlyDigits; // limpia si es texto
                                                        }
                                                    }
                                                };
                                                var setsLabel = new Entry
                                                {
                                                    Placeholder = "Número de series",
                                                    Keyboard = Keyboard.Numeric
                                                };
                                                setsLabel.TextChanged += (s, e) =>
                                                {
                                                    if (!string.IsNullOrEmpty(setsLabel.Text))
                                                    {
                                                        string onlyDigits = new string(setsLabel.Text.Where(char.IsDigit).ToArray());
                                                        if (setsLabel.Text != onlyDigits)
                                                        {
                                                            setsLabel.Text = onlyDigits; // limpia si es texto
                                                        }
                                                    }
                                                };
                                                var modalPage = new ContentPage
                                                {
                                                    BackgroundColor = Color.FromRgba(0, 0, 0, 0.6),
                                                    Content = new Border
                                                    {
                                                        BackgroundColor = Color.FromArgb("#2E1E1B"),
                                                        StrokeShape = new RoundRectangle
                                                        {
                                                            CornerRadius = 20
                                                        },
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
                                                            await DisplayAlert("Error", "Introduce series y repeticiones.", "OK");
                                                            return;
                                                        }
                                                        selectedExercise.sets = int.TryParse(repsLabel.Text, out int s) ? s : 0;
                                                        selectedExercise.seconds = int.TryParse(setsLabel.Text, out int r) ? r : 0;
                                                        selectedExercise.name = getExerciseLabel.Text;
                                                        exerciseSelection.Add(selectedExercise);

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
                                    }
                                    else
                                    {
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
                                                repsLabel.TextChanged += (s, e) =>
                                                {
                                                    if (!string.IsNullOrEmpty(repsLabel.Text))
                                                    {
                                                        string onlyDigits = new string(repsLabel.Text.Where(char.IsDigit).ToArray());
                                                        if (repsLabel.Text != onlyDigits)
                                                        {
                                                            repsLabel.Text = onlyDigits; // limpia si es texto
                                                        }
                                                    }
                                                };
                                                var timeLabel = new Entry
                                                {
                                                    Placeholder = "Tiempo de ejecución",
                                                    Keyboard = Keyboard.Numeric
                                                };
                                                timeLabel.TextChanged += (s, e) =>
                                                {
                                                    if (!string.IsNullOrEmpty(timeLabel.Text))
                                                    {
                                                        string onlyDigits = new string(timeLabel.Text.Where(char.IsDigit).ToArray());
                                                        if (timeLabel.Text != onlyDigits)
                                                        {
                                                            timeLabel.Text = onlyDigits; // limpia si es texto
                                                        }
                                                    }
                                                };
                                                var modalPage = new ContentPage
                                                {
                                                    BackgroundColor = Color.FromRgba(0, 0, 0, 0.6),
                                                    Content = new Border
                                                    {
                                                        BackgroundColor = Color.FromArgb("#2E1E1B"),
                                                        StrokeShape = new RoundRectangle
                                                        {
                                                            CornerRadius = 20
                                                        },
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
                                        timeLabel,
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
                                                        if (string.IsNullOrEmpty(repsLabel.Text) || string.IsNullOrEmpty(timeLabel.Text))
                                                        {
                                                            await DisplayAlert("Error", "Introduce series y repeticiones.", "OK");
                                                            return;
                                                        }
                                                        selectedExercise.sets = int.TryParse(repsLabel.Text, out int s) ? s : 0;
                                                        selectedExercise.seconds = int.TryParse(timeLabel.Text, out int r) ? r : 0;
                                                        selectedExercise.name = getExerciseLabel.Text;
                                                        exerciseSelection.Add(selectedExercise);

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
                                    }
                                    frame.GestureRecognizers.Add(tapGesture);
                                    return frame;
                                }
                                frame.GestureRecognizers.Add(tapGesture);
                                return frame;
                            })
                        };
                        if (exercisesInRoutine != null)
                        {
                            foreach (var exercise in exercisesInRoutine)
                            {
                                exerciseSelection.Add(exercise);
                            }
                        }

                        var selectedExercisesView = new CollectionView //lista para ver los ejercicios seleccionados
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

                                var setsAndRepsLabel = new Label
                                {
                                    FontSize = 12,
                                    TextColor = Colors.LightGray
                                };
                                MultiBinding multiBinding = new MultiBinding { };
                                if (bodyPartEnumPicker.SelectedItem is string selectedText)
                                {
                                    if (!selectedText.Equals(translation[bodyPartEnum.isometric]))
                                    {
                                        multiBinding = new MultiBinding
                                        {
                                            StringFormat = "Repeticiones: {0}\nSeries: {1}"
                                        };
                                        multiBinding.Bindings.Add(new Binding("reps"));
                                        multiBinding.Bindings.Add(new Binding("sets"));
                                    }
                                    else
                                    {
                                        multiBinding = new MultiBinding
                                        {
                                            StringFormat = "Repeticiones: {0}\nTiempo: {1} segundos"
                                        };
                                        multiBinding.Bindings.Add(new Binding("reps"));
                                        multiBinding.Bindings.Add(new Binding("seconds"));
                                    }
                                }
                                setsAndRepsLabel.SetBinding(Label.TextProperty, multiBinding);
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

                                return new Border
                                {
                                    Margin = 5,
                                    Padding = 8,
                                    BackgroundColor = Color.FromArgb("#3B2523"),
                                    Content = new HorizontalStackLayout
                                    {
                                        Spacing = 10,
                                        Children = { nameLabel, setsAndRepsLabel, removeButton }
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
                new HorizontalStackLayout
                    {
                        Spacing = 10,
                        Children =
                        {
                             new Button
                        {
                            Text = "Agregar",
                            Command = new Command(async () =>
                            {
                                exercisesInRoutine.Clear();
                                foreach (var exercise in exerciseSelection)
                                {
                                    exercisesInRoutine.Add(exercise);
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
                        };

                        var selectedExercisesPage = new ContentPage
                        {
                            BackgroundColor = Color.FromArgb("#2E1E1B"),
                            Content = new ScrollView { Content = mainLayout }
                        };

                        await Navigation.PushModalAsync(selectedExercisesPage);
                    }
                };
                var collectionExercises = new CollectionView
                {
                    ItemsSource = exercisesInRoutine,
                    EmptyView = new Label
                    {
                        Text = "No hay ejercicios añadidos.",
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

                        var repsLabel = new Label
                        {
                            TextColor = Colors.LightGray,
                            FontSize = 12
                        };

                        MultiBinding multiBinding = new MultiBinding { };
                        if (bodyPartEnumPicker.SelectedItem is string selectedText)
                        {
                            if (!selectedText.Equals(translation[bodyPartEnum.isometric]))
                            {
                                multiBinding = new MultiBinding { StringFormat = "Repeticiones: {0} | Series: {1}" };
                                multiBinding.Bindings.Add(new Binding("reps"));
                                multiBinding.Bindings.Add(new Binding("sets"));
                            }
                            else
                            {
                                multiBinding = new MultiBinding { StringFormat = "Repeticiones: {0} | Tiempo: {1} segundos" };
                                multiBinding.Bindings.Add(new Binding("reps"));
                                multiBinding.Bindings.Add(new Binding("seconds"));
                            }
                        }
                        repsLabel.SetBinding(Label.TextProperty, multiBinding);

                        return new HorizontalStackLayout
                        {
                            Spacing = 10,
                            Children = { nameLabel, repsLabel }
                        };
                    })
                };
                var modalPage = new ContentPage
                {
                    BackgroundColor = Color.FromRgba(0, 0, 0, 0.6),
                    Content = new Border

                    {
                        BackgroundColor = Color.FromArgb("#2E1E1B"),
                        StrokeShape = new RoundRectangle
                        {
                            CornerRadius = 20
                        },
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
                    collectionExercises,
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

                                if(nameRoutineEntry.Text == null || string.IsNullOrWhiteSpace(nameRoutineEntry.Text) ||
                                DescriptionRoutineEntry.Text == null || string.IsNullOrWhiteSpace(DescriptionRoutineEntry.Text) ||
                                !exercisesInRoutine.Any())
                                {
                                    await DisplayAlert("Error", "Rellena todos lo campos, y añade por lo menos un ejercicio", "OK");
                                    return;
                                }
                                else
                                {
                                    var selectedTranslation = bodyPartEnumPicker.SelectedItem.ToString();
                                    var selectedEnum = translation.FirstOrDefault(x => x.Value == selectedTranslation).Key;
                                    Routines createRoutine = new Routines{nameRoutine = nameRoutineEntry.Text,description = DescriptionRoutineEntry.Text,muscleGroup = selectedEnum,typeUser = userTypeEnum.all};
                                    await _dbService.Create(createRoutine);

                                    foreach (Exercise exercise in exercisesInRoutine)
                                    {
                                        RoutinesExercises routineExercises = new RoutinesExercises{RoutineID = createRoutine.routineID,ExerciseID = exercise.execiseID, sets = exercise.sets, reps = exercise.reps, seconds = exercise.seconds};
                                        await _dbService.Create(routineExercises);
                                    }
                                }



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
                Content = modalPage.Content;
                BackgroundColor = modalPage.BackgroundColor;
            }
            ;
        }
    }
}
