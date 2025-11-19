using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;
using System.Collections.ObjectModel;


namespace ProyectoFinDeCurso.Pages.Detail
{
    public class RoutineDetailPage : ContentPage
    {
        private readonly ExerciseMode _mode;
        private readonly Routines _routine;
        private static userTypeEnum _userType;
        private RoutinesFilterViewModel? _filterViewModel;
        private readonly DbService _dbService;
        private static TimeSpan timeBetweenReps = TimeSpan.Zero;
        private static TimeSpan timeBetweenExercises = TimeSpan.Zero;
        private Boolean firstExercise = false;
        public RoutineDetailPage(DbService? dbService = null,RoutinesFilterViewModel? filterViewModel = null,userTypeEnum userType = userTypeEnum.user ,Routines? routine = null,ExerciseMode mode = ExerciseMode.nothing) {
            _userType = userType;
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
                case ExerciseMode.create:
                    BuildCreateUI();
                    break;

                case ExerciseMode.Edit:
                    BuildEditUI();
                    break;

                case ExerciseMode.View:
                    BuildViewUI();
                    break;

                case ExerciseMode.filter:
                    BuildFilterRoutineUI();
                    break;
            }
        }

        private async void BuildViewUI()
        {
            ObservableCollection<Exercise> exercisesInRoutine = new ObservableCollection<Exercise>();
            var nameRoutine = new Label
            {
                Text = _routine.nameRoutine,
                FontSize = 22,
                FontFamily = "Forresten",
                TextColor = Colors.Orange,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0)
            };

            // === Botón tres puntos ===
            var menuButton = new ImageButton
            {
                Source = "puntos.png",
                BackgroundColor = Colors.Transparent,
                WidthRequest = 30,
                HeightRequest = 30,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Start
            };
            List<Exercise> exercises = _routine.Exercises.ToList();

            foreach (Exercise exercise in exercises)
            {
                exercisesInRoutine.Add(exercise);
            }
            var collectionExercises = new CollectionView
            {
                ItemsSource = exercisesInRoutine,
                IsGrouped = true,

                GroupHeaderTemplate = new DataTemplate(() =>
                {
                    var headerLabel = new Label
                    {
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 18,
                        FontFamily = "Forresten",
                        TextColor = Colors.Orange,
                        Margin = new Thickness(10, 5)
                    };
                    headerLabel.SetBinding(Label.TextProperty, "Name");
                    return headerLabel;
                }),

            };
            var eliminateRoutine = new Label { Text = "Eliminar" };
            var editRoutine = new Label { Text = "Modificar" };
            // === Menú flotante ===
            var menuFrame = new Border
            {
                BackgroundColor = Colors.White,
                StrokeShape = new RoundRectangle { CornerRadius = 10 },
                Padding = 8,
                Margin = new Thickness(0, 30, 10, 0),
                Content = new VerticalStackLayout
                {
                    Spacing = 8,
                    Children =
                    {
                       eliminateRoutine,
                       editRoutine
                    }
                }
            };
            var userId = await SecureStorage.GetAsync("user_id");
            var user = await _dbService.GetUserById(int.Parse(userId));
            if (!_userType.Equals(userTypeEnum.admin) && !_routine.userID.Equals(user.UserID))
            {
                menuFrame.IsVisible = false;
                menuButton.IsVisible = false;
            }
            var eleccionExercise = exercisesInRoutine.FirstOrDefault(x => !x.exerciseFinished && !x.expaded);
            if (eleccionExercise != null)
            {
                eleccionExercise.expaded = true;
            }
            CollectionView listExercise = new CollectionView
            {
                ItemsSource = exercisesInRoutine,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Default,
                VerticalScrollBarVisibility = ScrollBarVisibility.Default,
                ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepScrollOffset,
                ItemTemplate = new DataTemplate(() =>
                {
                    
                    var headerLabel = new Label
                    {
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Colors.White,
                        FontSize = 18
                    };
                    headerLabel.SetBinding(Label.TextProperty, "name");
                    var expanderExercise = new Expander
                    {
                        Header = headerLabel,
                         
                    };
                    
                    // Enlazamos las propiedades del modelo
                    expanderExercise.SetBinding(Expander.IsExpandedProperty, "expaded", BindingMode.TwoWay);
                    expanderExercise.SetBinding(Expander.IsEnabledProperty, "expaded", BindingMode.TwoWay);
                    expanderExercise.BindingContextChanged += (s, e) =>
                    {
                        
                        var exercise = (Exercise)((Expander)s).BindingContext;
                        if(exercise.expaded && !((Expander)s).IsExpanded)
                        {
                            ((Expander)s).IsExpanded = true;
                        }
                        if (exercise != null)
                        {
                            if(eleccionExercise.Equals(exercise))
                            {
                                firstExercise = true;
                            }
                            // Ejemplo: si el ejercicio está terminado, no permitir expandir
                            if (!exercise.expaded)
                            {
                                ((Expander)s).IsExpanded = false;
                                ((Expander)s).IsEnabled = false;
                            }
                            else
                            {
                                ((Expander)s).IsExpanded = true;
                                ((Expander)s).IsEnabled = true;
                                
                            }
                        }
                    };
                    var startButton = new Button
                    {
                        BackgroundColor = Color.FromArgb("#3B2523"),
                        TextColor = Color.FromArgb("#CFC86D"),
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.Fill,
                        
                        FontFamily = "Forresten",
                        Text = "Empezar Ejercicio",
                        CommandParameter = new Binding(".") // El objeto Exercise actual
                    };
                    
                    // Evento Clicked para manejar el botón
                    startButton.Clicked += async (s, e) =>
                    {
                        
                        
                        var btn = (Button)s;
                        var exercise = (Exercise)btn.BindingContext; // también puedes usar btn.CommandParameter
                        int count = exercise.sets;
                        if (exercise == null)
                            return;
                        if(eleccionExercise.Equals(exercise))
                        {
                            var tcs = new TaskCompletionSource<bool>();
                            var pickerMinutosSets = new Picker
                            {
                                Title = "Minutos",
                                TextColor = Colors.White,
                                BackgroundColor = Color.FromArgb("#3B2523"),
                                WidthRequest = 100
                            };
                            for (int i = 0; i <= 59; i++)
                                pickerMinutosSets.Items.Add(i.ToString("00"));

                            var pickerSegundosSets = new Picker
                            {
                                Title = "Segundos",
                                TextColor = Colors.White,
                                BackgroundColor = Color.FromArgb("#3B2523"),
                                WidthRequest = 100
                            };
                            for (int i = 0; i <= 59; i++)
                                pickerSegundosSets.Items.Add(i.ToString("00"));

                            pickerMinutosSets.SelectedIndex = 0;
                            pickerSegundosSets.SelectedIndex = 30;

                            var pickerMinutosReps = new Picker
                            {
                                Title = "Minutos",
                                TextColor = Colors.White,
                                BackgroundColor = Color.FromArgb("#3B2523"),
                                WidthRequest = 100
                            };
                            for (int i = 0; i <= 59; i++)
                                pickerMinutosReps.Items.Add(i.ToString("00"));

                            var pickerSegundosReps = new Picker
                            {
                                Title = "Segundos",
                                TextColor = Colors.White,
                                BackgroundColor = Color.FromArgb("#3B2523"),
                                WidthRequest = 100
                            };
                            for (int i = 0; i <= 59; i++)
                                pickerSegundosReps.Items.Add(i.ToString("00"));

                            pickerMinutosReps.SelectedIndex = 0;
                            pickerSegundosReps.SelectedIndex = 30;
                            var modalDuracion = new ContentPage
                            {
                                BackgroundColor = Color.FromRgba(0, 0, 0, 0.6),
                                Content = new Border
                                {
                                    BackgroundColor = Color.FromArgb("#2E1E1B"),
                                    StrokeShape = new RoundRectangle { CornerRadius = 20 },
                                    Margin = 1,
                                    VerticalOptions = LayoutOptions.Center,
                                    HorizontalOptions = LayoutOptions.Center,
                                    Content = new VerticalStackLayout
                                    {
                                        Padding = 15,
                                        Spacing = 15,
                                        Children =
                                    {
                                        new Label
                                        {
                                            Text = "Elige el tiempo de descanso entre repeticiones",
                                            FontSize = 20,
                                            TextColor = Colors.Orange,
                                            HorizontalTextAlignment = TextAlignment.Center,
                                            FontFamily="EatMeAlive"
                                        },

                                        // fila de pickers
                                        new HorizontalStackLayout
                                        {
                                            HorizontalOptions = LayoutOptions.Center,
                                            Spacing = 10,
                                            Children = { pickerMinutosSets, pickerSegundosSets }
                                        },
                                        new Label
                                        {
                                            Text = "Elige el tiempo de descanso entre ejercicios",
                                            FontSize = 20,
                                            TextColor = Colors.Orange,
                                            HorizontalTextAlignment = TextAlignment.Center,
                                            FontFamily="EatMeAlive"
                                        },

                                        // fila de pickers
                                        new HorizontalStackLayout
                                        {
                                            HorizontalOptions = LayoutOptions.Center,
                                            Spacing = 10,
                                            Children = { pickerMinutosReps, pickerSegundosReps }
                                        },
                                        new Button
                                        {
                                            Text = "Aceptar",
                                            BackgroundColor = Colors.Orange,
                                            TextColor = Colors.White,
                                            CornerRadius = 10,
                                            Command = new Command(async () =>
                                            {
                                                int minSets = pickerMinutosSets.SelectedIndex;
                                                int segSets = pickerSegundosSets.SelectedIndex;

                                                timeBetweenReps = new TimeSpan(0, minSets, segSets);

                                                int minReps = pickerMinutosReps.SelectedIndex;
                                                int SegReps = pickerSegundosReps.SelectedIndex;

                                                timeBetweenExercises = new TimeSpan(0, minReps, SegReps);

                                                await Navigation.PopModalAsync();
                                                tcs.TrySetResult(true);

                                                })
                                            }
                                        }
                                    }
                                }
                            };

                            await Navigation.PushModalAsync(modalDuracion);
                            await tcs.Task;
                        }
                        var nameExercise = new Label
                        {
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 18,
                            FontFamily = "Forresten",
                            TextColor = Color.FromArgb("#C77B30"),
                            Margin = new Thickness(10, 5),
                            Text = exercise.name
                        };

                        var descriptionExercise = new Label
                        {
                            Text = "Repeticion numero: ",
                            TextColor = Colors.White,
                            Margin = new Thickness(10, 5)
                        };
                        var RepsExercise = new Label
                        {
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 16,
                            FontFamily = "Forresten",
                            TextColor = Colors.White,
                            Margin = new Thickness(10, 5),
                            Text = $"Repetición número: {exercise.reps}"
                        };
                        var SetsExercise = new Label
                        {
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 16,
                            FontFamily = "Forresten",
                            TextColor = Colors.White,
                            Margin = new Thickness(10, 5),
                            Text = $"Series: {count}"
                        };
                        var imageExercise = new Image
                        {
                            Source = exercise.image,
                            HeightRequest = 200,
                            WidthRequest = 200,
                            Aspect = Aspect.AspectFill,
                            HorizontalOptions = LayoutOptions.Center
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
                                        
                                        nameExercise,
                                        imageExercise,
                                        RepsExercise,
                                        SetsExercise,
                                        descriptionExercise,
                                        new Button
                                            {
                                                Text = "Descansar",
                                                Command = new Command(async () =>
                                                {
                                                    if (count == 0)
                                                    {
                                                        // Cerrar este ejercicio
                                                        exercise.exerciseFinished = true;
                                                        exercise.expaded = false; // 🔹 se cerrará automáticamente
                                                        if (timeBetweenExercises.TotalSeconds > 0)
                                                        {
                                                            await ShowCountdown(timeBetweenExercises);
                                                        }
                                                        await Navigation.PopModalAsync();

                                                        // Abrir el siguiente ejercicio
                                                        var siguiente = exercisesInRoutine.FirstOrDefault(x => !x.exerciseFinished);
                                                        if (siguiente != null)
                                                        {
                                                            siguiente.expaded = true; // 🔹 se abrirá automáticamente
                                                        }
                                                    }
                                                    else
                                                    {
                                                        count--;
                                                        descriptionExercise.Text = $"Repetición número: {count}";

                                                        // 🔹 Aquí añadimos el temporizador antes de continuar
                                                        if (timeBetweenReps.TotalSeconds > 0)
                                                        {
                                                            await ShowCountdown(timeBetweenReps);
                                                        }
                                                    }
                                                })
                                            }
                                                                                }
                                }
                            }
                        };
                        await Navigation.PushModalAsync(modalPage);
                    };
                    expanderExercise.Content = new HorizontalStackLayout
                    {
                        Padding = new Thickness(10),
                        HorizontalOptions = LayoutOptions.Center, // <-- Este es el correcto en MAUI
                        VerticalOptions = LayoutOptions.Center,
                        Children =
                        {
                            startButton
                        }
                    };
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
                        Children = { expanderExercise }
                    };
                    return frame;
                })
            };



            eliminateRoutine.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    await _dbService.Delete(_routine);

                    _filterViewModel.Routines.Clear();
                    var routines = await _dbService.GetRoutines();
                    var rountinesExercises = await _dbService.GetRoutinesExercises();

                    foreach (RoutinesExercises routineExercise in rountinesExercises)
                    {
                        if (routineExercise.RoutineID.Equals(_routine.routineID))
                        {
                            await _dbService.Delete(routineExercise);
                        }
                    }

                    _filterViewModel.OnPropertyChanged(nameof(_filterViewModel.FilteredRoutines));
                    _filterViewModel.LoadRoutines();
                    await Navigation.PopModalAsync();
                })
            });
            editRoutine.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    ModifyOrCreateRoutine(_routine);
                })
            });
            // === Botón cancelar ===
            var cancelButton = new Button
            {
                Text = "Cancelar",
                BackgroundColor = Colors.Purple,
                TextColor = Colors.White,
                CornerRadius = 8,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.End,
                Margin = new Thickness(10, 0, 0, 10),
                Command = new Command(async () =>
                {
                    await Navigation.PopModalAsync();

                    foreach (Exercise exercise in exercisesInRoutine)
                    {

                        exercise.expaded = false;
                        exercise.exerciseFinished = false;

                    }
                })
            };

            // === Fila superior: título + botón tres puntos ===
            var headerGrid = new Grid
            {
                ColumnDefinitions =
    {
        new ColumnDefinition(GridLength.Star),
        new ColumnDefinition(GridLength.Auto)
    }
            };

            // Texto centrado en la izquierda
            Grid.SetColumn(nameRoutine, 0);
            headerGrid.Children.Add(nameRoutine);

            // Botón tres puntos a la derecha
            Grid.SetColumn(menuButton, 1);
            headerGrid.Children.Add(menuButton);

            // === BASE GRID ===
            var baseGrid = new Grid
            {
                RowDefinitions =
    {
        new RowDefinition(GridLength.Auto), // header
        new RowDefinition(GridLength.Star), // cuerpo
        new RowDefinition(GridLength.Auto)  // botón cancelar
    },
                Padding = 10,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            // Fila 0 → header (texto + menú)
            Grid.SetRow(headerGrid, 0);
            baseGrid.Children.Add(headerGrid);

            Grid.SetRow(listExercise, 1);
            baseGrid.Children.Add(listExercise);

            // Fila 2 → botón cancelar
            Grid.SetRow(cancelButton, 2);
            baseGrid.Children.Add(cancelButton);

            // === OVERLAY flotante ===
            var overlay = new AbsoluteLayout
            {
                IsVisible = false,
                InputTransparent = true,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            
            menuFrame.Opacity = 0;
            AbsoluteLayout.SetLayoutBounds(menuFrame, new Rect(1, 0, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
            AbsoluteLayout.SetLayoutFlags(menuFrame, AbsoluteLayoutFlags.PositionProportional);
            overlay.Children.Add(menuFrame);

            // Fondo clickable para cerrar
            var closeTap = new ContentView { BackgroundColor = Colors.Transparent };
            closeTap.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    await menuFrame.FadeTo(0, 150);
                    overlay.IsVisible = false;
                    overlay.InputTransparent = true;
                })
            });
            AbsoluteLayout.SetLayoutBounds(closeTap, new Rect(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(closeTap, AbsoluteLayoutFlags.All);
            overlay.Children.Insert(0, closeTap);

            // === ROOT (base + overlay) ===
            var root = new Grid();
            root.Children.Add(baseGrid);
            root.Children.Add(overlay);

            // === Evento del botón ⋮ ===
            menuButton.Clicked += async (s, e) =>
            {
                if (!overlay.IsVisible)
                {
                    overlay.IsVisible = true;
                    overlay.InputTransparent = false;
                    await menuFrame.FadeTo(1, 200);
                }
                else
                {
                    await menuFrame.FadeTo(0, 150);
                    overlay.IsVisible = false;
                    overlay.InputTransparent = true;
                }
            };

            // === PÁGINA ===
            var routine = new ContentPage
            {
                BackgroundColor = Color.FromRgba(0, 0, 0, 0.6),
                Content = new Border
                {
                    BackgroundColor = Color.FromArgb("#2E1E1B"),
                    StrokeShape = new RoundRectangle { CornerRadius = 20 },
                    WidthRequest = 350,
                    HeightRequest = 450,
                    Padding = 10,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Content = root
                }
            };
            Content = routine.Content;
            BackgroundColor = routine.BackgroundColor;

        }

        private void BuildEditUI()
        {
            throw new NotImplementedException();
        }

        private void BuildCreateUI()
        {
            ModifyOrCreateRoutine();
        }
        private void BuildFilterRoutineUI()
        {
            var translationBodyPart = new Dictionary<bodyPartEnum, string>
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

            var translationDificulty = new Dictionary<dificultyEnum, string>
    {
        { dificultyEnum.nothing, "Ninguno" },
        { dificultyEnum.easy, "Fácil" },
        { dificultyEnum.medium, "Medio" },
        { dificultyEnum.hard, "Difícil" },
        { dificultyEnum.extreme, "Extremo" }
    };

            // PICKER: BODY PART
            var filterBodyPartEntry = new Picker
            {
                Title = "Tipo Cuerpo",
                ItemsSource = translationBodyPart.Values.ToList(),
                SelectedItem = translationBodyPart[bodyPartEnum.nothing],
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill
            };

            // PICKER: DIFFICULTY
            var filterDificultyEntry = new Picker
            {
                Title = "Tipo de dificultad",
                ItemsSource = translationDificulty.Values.ToList(),
                SelectedItem = translationDificulty[dificultyEnum.nothing],
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill
            };

            // ENTRY: NAME
            var filterNameEntry = new Entry
            {
                Placeholder = "Nombre de la rutina",
                Keyboard = Keyboard.Text,
                TextColor = Color.FromArgb("#C49362")
            };

            // BOTÓN FILTRAR — ACTUALIZA EL VIEWMODEL
            var filterButton = new Button
            {
                Text = "Filtrar",
                Command = new Command(async () =>
                {
                    _filterViewModel ??= new RoutinesFilterViewModel(_dbService, userTypeEnum.nothing);
                    // 🔥 1. Actualizar filtro de nombre
                    _filterViewModel!.NameRoutineFilter =
                    string.IsNullOrWhiteSpace(filterNameEntry.Text)
                    ? null
                    : filterNameEntry.Text;

                    // 🔥 2. Actualizar filtro de dificultad
                    var selectedDiff = translationDificulty.FirstOrDefault(x => x.Value == (string)filterDificultyEntry.SelectedItem).Key;
                    _filterViewModel.DificultyFilter = selectedDiff;

                    // 🔥 3. Actualizar filtro de parte del cuerpo
                    var selectedBody = translationBodyPart.FirstOrDefault(x => x.Value == (string)filterBodyPartEntry.SelectedItem).Key;
                    _filterViewModel.BodyPartFilter = selectedBody;

                    // 🔥 5. Cerrar modal
                    await Navigation.PopModalAsync();
                })
            };

            var cancelButton = new Button
            {
                Text = "Cancelar",
                Command = new Command(async () => await Navigation.PopModalAsync())
            };

            // UI FINAL
            var modalPage = new ContentPage
            {
                BackgroundColor = Color.FromRgba(0, 0, 0, 0.6),
                Content = new Border
                {
                    BackgroundColor = Color.FromArgb("#2E1E1B"),
                    StrokeShape = new RoundRectangle { CornerRadius = 20 },
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
                        Text = "Buscar Rutina",
                        FontSize = 24,
                        TextColor = Color.FromArgb("#C77B30"),
                        HorizontalOptions = LayoutOptions.Fill,
                        HorizontalTextAlignment = TextAlignment.Center,
                        FontFamily = "EatMeAlive"
                    },
                    filterNameEntry,
                    filterDificultyEntry,
                    filterBodyPartEntry,

                    new HorizontalStackLayout
                    {
                        Spacing = 10,
                        Children =
                        {
                            filterButton,
                            cancelButton
                        }
                    }
                }
                    }
                }
            };

            Content = modalPage.Content;
            BackgroundColor = modalPage.BackgroundColor;
        }

        private async Task ShowCountdown(TimeSpan time)
        {
            var tcs = new TaskCompletionSource<bool>();
            int segundosRestantes = (int)time.TotalSeconds;
            bool cerrado = false; // 🔒 para evitar cierres dobles

            var labelTiempo = new Label
            {
                Text = time.ToString(@"mm\:ss"),
                FontSize = 72,
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold
            };

            var btnCancelar = new Button
            {
                Text = "Cancelar",
                BackgroundColor = Colors.Gray,
                TextColor = Colors.White,
                CornerRadius = 10,
                Padding = 10,
                HorizontalOptions = LayoutOptions.Center
            };

            var modalCuentaAtras = new ContentPage
            {
                BackgroundColor = Color.FromArgb("#1A1A1A"),
                Content = new VerticalStackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    Spacing = 20,
                    Children = { labelTiempo, btnCancelar }
                }
            };

            await Navigation.PushModalAsync(modalCuentaAtras);

            var timer = Application.Current.Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromSeconds(1);

            // Acción común para cerrar el modal
            async Task CerrarModalAsync()
            {
                if (cerrado) return;
                cerrado = true;

                timer.Stop();
                try { await Navigation.PopModalAsync(); } catch { /* ignorar si ya se cerró */ }
                tcs.TrySetResult(true);
            }

            // Evento de cancelar
            btnCancelar.Clicked += async (s, e) => await CerrarModalAsync();

            // Evento del temporizador
            timer.Tick += async (s, e) =>
            {
                segundosRestantes--;

                if (segundosRestantes <= 0)
                {
                    labelTiempo.Text = "¡Tiempo!";
                    labelTiempo.TextColor = Colors.OrangeRed;
                    timer.Stop();

                    await Task.Delay(2000); // espera antes de cerrar
                    await CerrarModalAsync();
                }
                else
                {
                    labelTiempo.Text = TimeSpan.FromSeconds(segundosRestantes).ToString(@"mm\:ss");
                }
            };

            timer.Start();
            await tcs.Task;
        }
       
        private void ModifyOrCreateRoutine(Routines routine = null)
        {
            bool newRoutine = false;
            if (routine == null) { 
                newRoutine = true;
                routine = new Routines
                {
                    nameRoutine = "",
                    description = "",
                    muscleGroup = bodyPartEnum.nothing,
                    difficulty = dificultyEnum.nothing
                };
            }
            var nameRoutineEntry = new Entry
            {
                Placeholder = "Nombre de la rutina",
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                Text = routine.nameRoutine,
                HorizontalOptions = LayoutOptions.Fill
            };
            var DescriptionRoutineEntry = new Entry
            {
                Text = routine.description,
                Placeholder = "Descripción de la rutina",
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),

                HorizontalOptions = LayoutOptions.Fill
            };
            var bodyPartEnumPicker = new Picker
            {
                Title = "Tipo Cuerpo",
                ItemsSource = enumExtension.BodyTranslations.Values.ToList(),
                SelectedItem = enumExtension.BodyTranslations[routine.muscleGroup],
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill
            };
            var dificultyPicker = new Picker
            {
                Title = "Tipo de dificultad",
                ItemsSource = enumExtension.DifficultyTranslations.Values.ToList(),
                SelectedItem = enumExtension.DifficultyTranslations[routine.difficulty],
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
            if(routine.Exercises != null) { 
                foreach (Exercise exercise in routine.Exercises)
                {
                    exercisesInRoutine.Add(exercise);
                }
            }
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
                bodyPartEnum selectedEnum = enumExtension.BodyTranslations.FirstOrDefault(x => x.Value == selectedText).Key;

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
                                if (!selectedText.Equals(enumExtension.BodyTranslations[bodyPartEnum.isometric]))
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
                                                        // Crear una copia independiente del ejercicio base
                                                        var newExercise = selectedExercise.Clone();

                                                        // Asignar los valores específicos
                                                        newExercise.reps = int.TryParse(repsLabel.Text, out int reps) ? reps : 0;
                                                        newExercise.sets = int.TryParse(setsLabel.Text, out int sets) ? sets : 0;
                                                        newExercise.name = getExerciseLabel.Text;

                                                        // Inicializar propiedades de control
                                                        newExercise.exerciseFinished = false;
                                                        newExercise.expaded = false;

                                                        // Agregar la copia a la selección
                                                        exerciseSelection.Add(newExercise);

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
                                                            await DisplayAlert("Error", "Introduce series y tiempo de ejecución.", "OK");
                                                            return;
                                                        }

                                                        // Crear una copia del ejercicio seleccionado
                                                        var newExercise = selectedExercise.Clone();

                                                        // Asignar los valores específicos de este nuevo ejercicio
                                                        newExercise.sets = int.TryParse(repsLabel.Text, out int s) ? s : 0;
                                                        newExercise.seconds = int.TryParse(timeLabel.Text, out int r) ? r : 0;
                                                        newExercise.name = getExerciseLabel.Text;

                                                        // Inicializar propiedades dinámicas
                                                        newExercise.exerciseFinished = false;
                                                        newExercise.expaded = false;

                                                        // Agregar la copia (no el original)
                                                        exerciseSelection.Add(newExercise);

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
                                if (!selectedText.Equals(enumExtension.BodyTranslations[bodyPartEnum.isometric]))
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
                                        var copy = exercise.Clone();

                                        // Mantener las mismas series y repeticiones
                                        copy.sets = exercise.sets;
                                        copy.reps = exercise.reps;
                                        copy.seconds = exercise.seconds;

                                        // Resetear estados visuales si hace falta
                                        copy.exerciseFinished = false;
                                        copy.expaded = false;

                                        // Agregar la copia, no la referencia original
                                        exercisesInRoutine.Add(copy);
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
                WidthRequest = 300,
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
                        if (!selectedText.Equals(enumExtension.BodyTranslations[bodyPartEnum.isometric]))
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
                    Padding = 20,
                    StrokeShape = new RoundRectangle { CornerRadius = 20 },
                    Margin = 1,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,

                    WidthRequest = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density * 0.9,
                    MaximumWidthRequest = 450,
                    MaximumHeightRequest = 500,
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
                            Text = "Modificar",
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
                                    if(newRoutine)
                                    {
                                        var selectedTranslation = bodyPartEnumPicker.SelectedItem.ToString();
                                        var selectedEnum = enumExtension.BodyTranslations.FirstOrDefault(x => x.Value == selectedTranslation).Key;
                                        var userId = await SecureStorage.GetAsync("user_id");
                                        var user = await _dbService.GetUserById(int.Parse(userId));
                                        Routines createRoutine;
                                        if (user.userType.Equals(userTypeEnum.admin))
                                        {
                                            createRoutine = new Routines{nameRoutine = nameRoutineEntry.Text,description = DescriptionRoutineEntry.Text,muscleGroup = selectedEnum,typeUser = userTypeEnum.all,userID = 0};
                                        }
                                        else
                                        {
                                            createRoutine = new Routines{nameRoutine = nameRoutineEntry.Text,description = DescriptionRoutineEntry.Text,muscleGroup = selectedEnum,typeUser = userTypeEnum.all,userID = user.UserID};
                                        }

                                        await _dbService.Create(createRoutine);

                                        foreach (Exercise exercise in exercisesInRoutine)
                                        {
                                            RoutinesExercises routineExercises = new RoutinesExercises{RoutineID = createRoutine.routineID,ExerciseID = exercise.execiseID, sets = exercise.sets, reps = exercise.reps, seconds = exercise.seconds};
                                            await _dbService.Create(routineExercises);
                                        }
                                    }else{
                                        var existingRelations = await _dbService.GetRoutinesExercises();


                                        var selectedTranslation = bodyPartEnumPicker.SelectedItem.ToString();
                                        var selectedEnum = enumExtension.BodyTranslations.FirstOrDefault(x => x.Value == selectedTranslation).Key;
                                        routine.nameRoutine = nameRoutineEntry.Text;
                                        routine.description = DescriptionRoutineEntry.Text;
                                        routine.muscleGroup = selectedEnum;
                                        routine.Exercises = exercisesInRoutine;


                                        var eliminateExercises = existingRelations.Where(r => r.RoutineID.Equals(routine.routineID)).ToList();

                                        foreach (var relation in eliminateExercises)
                                        {
                                            if (relation.RoutineID.Equals(routine.routineID))
                                            {
                                                await _dbService.Delete(relation);
                                            }
                                        }
                                        foreach (Exercise exercise in routine.Exercises)
                                        {
                                             await _dbService.Create(new RoutinesExercises {RoutineID = routine.routineID,ExerciseID = exercise.execiseID,sets = exercise.sets, reps = exercise.reps,seconds = exercise.seconds});
                                        }
                                        await _dbService.Update(_routine);
                                        
                                    }
                                }

                                _filterViewModel.OnPropertyChanged(nameof(_filterViewModel.FilteredRoutines));
                                _filterViewModel.LoadRoutines();
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
            Content = modalPage.Content;
            BackgroundColor = modalPage.BackgroundColor;
        }
       
    }
}
