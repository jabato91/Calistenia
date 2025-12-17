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
        private readonly ModeEnum _mode;
        private readonly Routines _routine;
        private static userTypeEnum _userType;
        private RoutinesFilterViewModel? _filterViewModel;
        private readonly DbService _dbService;
        private static TimeSpan timeBetweenReps = TimeSpan.Zero;
        private static TimeSpan timeBetweenExercises = TimeSpan.Zero;
        private Boolean firstExercise = false;
        private static String timeToStart;
        private static String timeToEnd;

        public RoutineDetailPage(DbService? dbService = null,RoutinesFilterViewModel? filterViewModel = null,userTypeEnum userType = userTypeEnum.user ,Routines? routine = null, ModeEnum mode = ModeEnum.nothing) {
            try
            {
                _userType = userType;
            _filterViewModel = filterViewModel;
            _routine = routine;
            _mode = mode;
            _dbService = dbService;

            BuildUI();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inicializando CalendarDetailPage: {ex.Message}");
                Application.Current?.MainPage?.DisplayAlert("Error", "No se pudo cargar la página de rutina.", "OK");
            }
        }
        private void BuildUI()// Construye la interfaz de usuario según el modo
        {
            try { 
            switch (_mode)
            {
                case ModeEnum.create:
                    BuildCreateUI();
                    break;
                case ModeEnum.View:
                    BuildViewUI();
                    break;

                case ModeEnum.filter:
                    BuildFilterRoutineUI();
                    break;
            }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void BuildViewUI() // Construye la interfaz de usuario para ver los detalles de la rutina
        {
            try { 
            ObservableCollection<Exercise> exercisesInRoutine = new ObservableCollection<Exercise>(); // Lista de ejercicios en la rutina
            var nameRoutine = new Label //nombre de la rutina
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

            var menuButton = new ImageButton // boton para mostrar el menu de opciones
            {
                Source = "puntos.png",
                BackgroundColor = Colors.Transparent,
                WidthRequest = 30,
                HeightRequest = 30,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Start
            };
            List<Exercise> exercises = _routine.Exercises.ToList(); //obtiene los ejercicios de la rutina

            foreach (Exercise exercise in exercises) //añade los ejercicios a la lista de ejercicios en la rutina
            {
                exercisesInRoutine.Add(exercise);
            }
            var collectionExercises = new CollectionView //muestra los ejercicios en la rutina
            {
                ItemsSource = exercisesInRoutine, //fuente de datos que recoge de los ejercicios de la rutina
                IsGrouped = true, //indica que los elementos están agrupados

                GroupHeaderTemplate = new DataTemplate(() => //plantilla para el encabezado del grupo
                {
                    var headerLabel = new Label //nombre de la rutina
                    {
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 18,
                        FontFamily = "Forresten",
                        TextColor = Colors.Orange,
                        Margin = new Thickness(10, 5)
                    };
                    headerLabel.SetBinding(Label.TextProperty, "Name"); //recoge el nombre de la rutina
                    return headerLabel; //devuelve el encabezado del grupo
                }),

            };
            var eliminateRoutine = new Label { Text = "Eliminar", TextColor = Colors.Black }; //opción del desplazable para eliminar la rutina
            var editRoutine = new Label { Text = "Modificar", TextColor = Colors.Black };//opción del desplazable para modificar la rutina
           
            var menuFrame = new Border //menu desplegable para eliminar o modificar la rutina
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
                       editRoutine //agrega las opciones al menu desplegable
                    }
                }
            };
            var userId = await SecureStorage.GetAsync("user_id");
            var user = await _dbService.GetUserById(int.Parse(userId));
            if (!_userType.Equals(userTypeEnum.admin) && !_routine.userID.Equals(user.UserID)) //verifica si el usuario es admin o el creador de la rutina
            {
                menuFrame.IsVisible = false; //oculta el menu desplegable si no es admin o creador
                menuButton.IsVisible = false;
            }
            var eleccionExercise = exercisesInRoutine.FirstOrDefault(x => !x.exerciseFinished && !x.expaded); //selecciona el primer ejercicio que no ha sido terminado ni expandido
            if (eleccionExercise != null) //si existe un ejercicio seleccionado
            {
                eleccionExercise.expaded = true; //expande el ejercicio seleccionado
            }
            var finnishButton = new Button //botón para cancelar y cerrar la rutina
            {
                Text = "Acabar rutina",
                BackgroundColor = Colors.Purple,
                TextColor = Colors.White,
                CornerRadius = 8,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.End,
                Margin = new Thickness(10, 0, 0, 10),
                IsVisible = false,
                Command = new Command(async () => //comando para cerrar la rutina
                {
                    string hora = DateTime.Now.ToString("HH:mm:ss");
                    timeToEnd = hora;
                    await RegisterInCalendar();
                    await Navigation.PopModalAsync();
                })
            };
            var cancelButton = new Button //botón para cancelar y cerrar la rutina
            {
                Text = "Cancelar",
                BackgroundColor = Colors.Purple,
                TextColor = Colors.White,
                CornerRadius = 8,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.End,
                Margin = new Thickness(10, 0, 0, 10),
                Command = new Command(async () => //comando para cerrar la rutina
                {
                    await Navigation.PopModalAsync();

                    foreach (Exercise exercise in exercisesInRoutine) //reinicia los ejercicios de la rutina
                    {

                        exercise.expaded = false;
                        exercise.exerciseFinished = false;

                    }
                })
            };
            CollectionView listExercise = new CollectionView //muestra la lista de ejercicios en la rutina
            {
                ItemsSource = exercisesInRoutine, //recoge los ejercicios de la rutina
                HorizontalScrollBarVisibility = ScrollBarVisibility.Default, //barra de desplazamiento horizontal
                VerticalScrollBarVisibility = ScrollBarVisibility.Default, //barra de desplazamiento vertical
                ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepScrollOffset, //mantiene la posición de desplazamiento al actualizar los elementos
                ItemTemplate = new DataTemplate(() => // muestra cada ejercicio en la rutina
                {
                    
                    var headerLabel = new Label //nombre del ejercicio
                    {
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Colors.White,
                        FontSize = 18
                    };
                    headerLabel.SetBinding(Label.TextProperty, "name"); //asigna el nombre del ejercicio
                    var expanderExercise = new Expander //expansor para mostrar los detalles del ejercicio
                    {
                        Header = headerLabel, //agrega el nombre del ejercicio al encabezado del expansor

                    };
                    
                    expanderExercise.SetBinding(Expander.IsExpandedProperty, "expaded", BindingMode.TwoWay); //vincula la propiedad IsExpanded del expansor a la propiedad expaded del ejercicio
                    expanderExercise.SetBinding(Expander.IsEnabledProperty, "expaded", BindingMode.TwoWay); //vincula la propiedad IsEnabled del expansor a la propiedad expaded del ejercicio
                    expanderExercise.BindingContextChanged += (s, e) => //evento para manejar el cambio de contexto de enlace
                    {
                        
                        var exercise = (Exercise)((Expander)s).BindingContext; //obtiene el ejercicio actual del contexto de enlace
                        if (exercise.expaded && !((Expander)s).IsExpanded) //si el ejercicio está expandido y el expansor no está expandido
                        {
                            ((Expander)s).IsExpanded = true; //expande el expansor
                        }
                        if (exercise != null) //si el ejercicio no es nulo
                        {
                            if(eleccionExercise.Equals(exercise)) //si el ejercicio es el ejercicio seleccionado
                            {
                                firstExercise = true;//marca que es el primer ejercicio
                            }

                            if (!exercise.expaded) //si el ejercicio no está expandido
                            {
                                ((Expander)s).IsExpanded = false; //colapsa el expansor
                                ((Expander)s).IsEnabled = false;
                            }
                            else//si no
                            {
                                ((Expander)s).IsExpanded = true;
                                ((Expander)s).IsEnabled = true;
                                
                            }
                        }
                    };
                    var startButton = new Button //botón para empezar el ejercicio
                    {
                        BackgroundColor = Color.FromArgb("#3B2523"),
                        TextColor = Color.FromArgb("#CFC86D"),
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.Fill,
                        
                        FontFamily = "Forresten",
                        Text = "Empezar Ejercicio",
                        CommandParameter = new Binding(".") // El objeto Exercise actual
                    };
                    
                    
                    startButton.Clicked += async (s, e) =>// Evento Clicked para manejar el botón
                    {
                        
                        
                        var btn = (Button)s;
                        var exercise = (Exercise)btn.BindingContext; // recoge el ejercicio actual del contexto de enlace
                        if (exercise == null) //si el ejercicio es nulo
                            return; //sale del evento

                        if (eleccionExercise.Equals(exercise)) //si el ejercicio es el ejercicio seleccionado
                        {
                            var tcs = new TaskCompletionSource<bool>(); //crea una tarea para esperar a que el usuario configure los descansos
                            var pickerMinutosSets = new Picker //picker para seleccionar los minutos de descanso entre repeticiones
                            {
                                Title = "Minutos",
                                TextColor = Colors.White,
                                BackgroundColor = Color.FromArgb("#3B2523"),
                                WidthRequest = 100
                            };
                            for (int i = 0; i <= 59; i++)
                                pickerMinutosSets.Items.Add(i.ToString("00")); //asigna los minutos al picker

                            var pickerSegundosSets = new Picker //picker para seleccionar los segundos de descanso entre repeticiones
                            {
                                Title = "Segundos",
                                TextColor = Colors.White,
                                BackgroundColor = Color.FromArgb("#3B2523"),
                                WidthRequest = 100
                            };
                            for (int i = 0; i <= 59; i++) 
                                pickerSegundosSets.Items.Add(i.ToString("00"));//añade los segundos al picker

                            pickerMinutosSets.SelectedIndex = 0; //selecciona el primer índice del picker
                            pickerSegundosSets.SelectedIndex = 30; //selecciona el segundo índice del picker

                            var pickerMinutosReps = new Picker //picker para seleccionar los minutos de descanso entre ejercicios
                            {
                                Title = "Minutos",
                                TextColor = Colors.White,
                                BackgroundColor = Color.FromArgb("#3B2523"),
                                WidthRequest = 100
                            };
                            for (int i = 0; i <= 59; i++)
                                pickerMinutosReps.Items.Add(i.ToString("00"));

                            var pickerSegundosReps = new Picker //picker para seleccionar los segundos de descanso entre ejercicios
                            {
                                Title = "Segundos",
                                TextColor = Colors.White,
                                BackgroundColor = Color.FromArgb("#3B2523"),
                                WidthRequest = 100
                            };
                            var cancelButton = new Button //botón para salir del modal
                            {
                                Text = "Salir",
                                BackgroundColor = Colors.Orange,
                                TextColor = Colors.White,
                                CornerRadius = 10,
                                FontFamily = "ComfortaaBold",
                                Padding = new Thickness(10, 6),
                                HorizontalOptions = LayoutOptions.Fill,
                                Command = new Command(async () => await Navigation.PopModalAsync()) //comando para cerrar el modal
                            };
                            for (int i = 0; i <= 59; i++)
                                pickerSegundosReps.Items.Add(i.ToString("00")); //añade los segundos al picker

                            pickerMinutosReps.SelectedIndex = 0; //selecciona el primer índice del picker
                            pickerSegundosReps.SelectedIndex = 30; //selecciona el segundo índice del picker

                            var modalDuracion = new ContentPage //modal para configurar los descansos
                            {
                                BackgroundColor = Color.FromRgba(0, 0, 0, 0.45),

                                Content = new Border
                                {
                                    BackgroundColor = Color.FromArgb("#251A18"),
                                    StrokeShape = new RoundRectangle { CornerRadius = 25 },
                                    Stroke = Colors.Orange,
                                    StrokeThickness = 2,
                                    Padding = 20,
                                    Margin = new Thickness(30, 80),

                                    Shadow = new Shadow
                                    {
                                        Offset = new Point(0, 6),
                                        Radius = 12,
                                    },

                                    Content = new VerticalStackLayout
                                    {
                                        Spacing = 25,

                                    Children =
                                    {
                                        new Label //titulo del modal
                                        {
                                            Text = "Configurar descansos",
                                            HorizontalTextAlignment = TextAlignment.Center,
                                            FontSize = 26,
                                            TextColor = Colors.Orange,
                                            FontFamily = "EatMeAlive"
                                        },

                                        new Label //titulo tiempo entre repeticiones
                                        {
                                            Text = "Tiempo entre repeticiones",
                                            HorizontalTextAlignment = TextAlignment.Center,
                                            FontSize = 18,
                                            TextColor = Colors.White,
                                            Margin = new Thickness(0, 5)
                                        },

                                        new HorizontalStackLayout //muestra los pickers de minutos y segundos para el tiempo entre repeticiones
                                        {
                                            HorizontalOptions = LayoutOptions.Center,
                                            Spacing = 15,
                                            Children =
                                            {
                                                pickerMinutosSets,
                                                pickerSegundosSets
                                            }
                                        },

                                        new Label //titulo tiempo entre ejercicios
                                        {
                                            Text = "Tiempo entre ejercicios",
                                            HorizontalTextAlignment = TextAlignment.Center,
                                            FontSize = 18,
                                            TextColor = Colors.White,
                                            Margin = new Thickness(0, 15, 0, 0)
                                        },

                                        new HorizontalStackLayout //muestra los pickers de minutos y segundos para el tiempo entre ejercicios
                                        {
                                            HorizontalOptions = LayoutOptions.Center,
                                            Spacing = 15,
                                            Children =
                                            {
                                                pickerMinutosReps,
                                                pickerSegundosReps
                                            }
                                        },

                                        new Button //botón para aceptar la configuración de descansos
                                        {
                                            Text = "Aceptar",
                                            BackgroundColor = Colors.Orange,
                                            TextColor = Colors.White,
                                            CornerRadius = 15,
                                            FontAttributes = FontAttributes.Bold,
                                            Padding = new Thickness(12, 10),
                                            FontFamily = "ComfortaaBold",
                                            Command = new Command(async () => //comando para aceptar la configuración
                                            {
                                                int minSets = pickerMinutosSets.SelectedIndex; //recoge los minutos seleccionados
                                                int segSets = pickerSegundosSets.SelectedIndex;//recoge los segundos seleccionados

                                                timeBetweenReps = new TimeSpan(0, minSets, segSets); //asigna el tiempo entre repeticiones

                                                int minReps = pickerMinutosReps.SelectedIndex; //recoge los minutos seleccionados
                                                int segReps = pickerSegundosReps.SelectedIndex;//recoge los segundos seleccionados

                                                timeBetweenExercises = new TimeSpan(0, minReps, segReps); //asigna el tiempo entre ejercicios

                                                await Navigation.PopModalAsync(); //sale del modal
                                                tcs.TrySetResult(true); //establece el resultado de la tarea como verdadero
                                                string hora = DateTime.Now.ToString("HH:mm:ss");
                                                timeToStart = hora; //asigna la hora de inicio
                                            })
                                        },

                                        new Button //botón para cancelar y salir del modal
                                        {
                                            Text = "Cancelar",
                                            BackgroundColor = Color.FromArgb("#4A2E2A"),
                                            TextColor = Colors.White,
                                            CornerRadius = 15,
                                            FontFamily = "ComfortaaBold",
                                            Padding = new Thickness(12, 10),
                                            Command = new Command(async () => await Navigation.PopModalAsync())
                                        }
                                    }
                                    }
                                }
                            };

                            await Navigation.PushModalAsync(modalDuracion); //muestra el modal para configurar los descansos
                            await tcs.Task; //espera a que el usuario configure los descansos
                        }
                        int count = exercise.reps; //recoge el número de repeticiones del ejercicio
                        var nameExercise = new Label //nombre del ejercicio
                        {
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 18,
                            FontFamily = "Forresten",
                            TextColor = Color.FromArgb("#C77B30"),
                            Margin = new Thickness(10, 5),
                            Text = exercise.name
                        };
                        
                        var repetitionExercise = new Label //número de repeticiones del ejercicio
                        {
                            Text = $"Repeticion numero: {count}",
                            TextColor = Colors.White,
                            Margin = new Thickness(10, 5)
                        };
                        bool isIsometric = false; //indica si el ejercicio es isométrico
                        var setsOrTimeExercise = new Label(); //muestra las series o el tiempo del ejercicio
                        if (!exercise.muscleGroupId.Equals(bodyPartEnum.isometric)) //verifica si el ejercicio no es isométrico
                        {
                            setsOrTimeExercise = new Label //muestra las series del ejercicio
                            {
                                Text = $"Series: {exercise.sets}",
                                TextColor = Colors.White,
                                Margin = new Thickness(10, 5)
                            };
                            isIsometric = false;
                        }
                        else
                        {
                            setsOrTimeExercise = new Label //muestra el tiempo del ejercicio
                            {
                                Text = $"Tiempo: {exercise.seconds} segundos",
                                TextColor = Colors.White,
                                Margin = new Thickness(10, 5)
                            };
                            isIsometric = true;
                        }
                        var timeButton = new Button //botón para empezar el ejercicio isométrico
                        {
                            Text = "Empezar Ejercicio",
                            BackgroundColor = Colors.Orange,
                            TextColor = Colors.White,
                            HeightRequest = 40,
                            WidthRequest = 150,  
                            CornerRadius = 15,
                            FontFamily = "ComfortaaBold",
                            FontSize = 13,
                            IsVisible = isIsometric,
                            HorizontalOptions = LayoutOptions.Center
                        };
                        bool exerciseCompleted = false; //indica si el ejercicio ha sido completado
                        timeButton.Command = new Command(async () => //comando para manejar el botón de tiempo
                        {
                            if (timeBetweenReps.TotalSeconds > 0) //verifica si hay tiempo entre repeticiones
                                await ShowCountdown(TimeSpan.FromSeconds(exercise.seconds)); //muestra la cuenta regresiva del tiempo del ejercicio isométrico

                            timeButton.IsVisible = false; //oculta el botón de tiempo
                            exerciseCompleted = true; //marca el ejercicio como completado
                        });
                        var imageExercise = new Image //imagen del ejercicio
                        {
                                Source = exercise.image,
                                HeightRequest = 200,
                                WidthRequest = 200,
                                Aspect = Aspect.AspectFill,
                                HorizontalOptions = LayoutOptions.Center
                            };
                        var pauseButton = new Button //botón para pausar o terminar el ejercicio
                        {
                            Text = "Descansar",
                            BackgroundColor = Colors.Orange,
                            TextColor = Colors.White,
                            CornerRadius = 15,
                            FontFamily = "ComfortaaBold",
                            FontSize = 16,
                            Padding = new Thickness(12, 10),
                            Margin = new Thickness(0, 10, 0, 0),
                        };
                        
                        pauseButton.Command = new Command(async () => //boton para descansar o terminar el ejercicio
                        {
                            if (exercise.muscleGroupId.Equals(bodyPartEnum.isometric) && !exerciseCompleted) //verifica si el ejercicio es isométrico y no ha sido completado
                            {
                                await DisplayAlert(
                                     "Ejercicio isométrico",
                                     "Termina el ejercicio para poder descansar.",
                                     "Aceptar"
                                 );
                                return;
                            }
                            
                            if (count == 0) //verifica si ha terminado las rutinas
                            {
                                timeButton.IsVisible = false; //oculta el botón de tiempo
                                exercise.exerciseFinished = true; //marca el ejercicio como terminado
                                exercise.expaded = false; //colapsa el ejercicio

                                if (timeBetweenExercises.TotalSeconds > 0) //verifica si hay tiempo entre ejercicios
                                    await ShowCountdown(timeBetweenExercises); //muestra la cuenta regresiva del tiempo entre ejercicios

                                await Navigation.PopModalAsync(); //cierra el modal del ejercicio

                                var siguiente = exercisesInRoutine.FirstOrDefault(x => !x.exerciseFinished); //selecciona el siguiente ejercicio que no ha sido terminado
                                if (siguiente != null) {  //si existe un siguiente ejercicio
                                    siguiente.expaded = true; //expande el siguiente ejercicio
                                }
                                else
                                {
                                    finnishButton.IsVisible = true;
                                }
                            }
                            else
                            {
                                count--; //decrementa el contador de repeticiones
                                repetitionExercise.Text = $"Repetición número: {count}"; //actualiza el texto del número de repeticiones
                                if (count == 0) //verifica si ha terminado las repeticiones
                                {
                                    pauseButton.Text = "Terminar Ejercicio"; //cambia el texto del botón a terminar ejercicio
                                    if (exercise.muscleGroupId.Equals(bodyPartEnum.isometric))//si el ejercicio es isométrico
                                    { 
                                        timeButton.IsVisible = true; //muestra el botón de tiempo
                                    }
                                    
                                }
                                else
                                {
                                    pauseButton.Text = "Descansar"; //cambia el texto del botón a descansar
                                    if (exercise.muscleGroupId.Equals(bodyPartEnum.isometric))//si el ejercicio es isométrico
                                    { 
                                        timeButton.IsVisible = true; //muestra el botón de tiempo
                                        exerciseCompleted = false;
                                    }
                                }
                                if (timeBetweenReps.TotalSeconds > 0) //verifica si hay tiempo entre repeticiones
                                    await ShowCountdown(timeBetweenReps); //muestra la cuenta regresiva del tiempo entre repeticiones
                            }
                        });
                        var modalPage = new ContentPage //modal para mostrar los detalles del ejercicio
                        {
                            BackgroundColor = Color.FromRgba(0, 0, 0, 0.5),

                            Content = new Border
                            {
                                BackgroundColor = Color.FromArgb("#241A18"),
                                StrokeShape = new RoundRectangle { CornerRadius = 25 },
                                StrokeThickness = 2,
                                Stroke = Colors.Orange,
                                Margin = new Thickness(30, 100, 30, 100),

                                Shadow = new Shadow
                                {
                                    Offset = new Point(0, 6),
                                    Radius = 12,
                                },

                                Content = new ScrollView //contenedor desplazable para el contenido del ejercicio
                                {
                                    Content = new VerticalStackLayout
                                    {
                                        Padding = new Thickness(20),
                                        Spacing = 15,
                                        Children =
                                        {
                                            nameExercise,

                                            new Border
                                            {
                                                StrokeShape = new RoundRectangle { CornerRadius = 15 },
                                                BackgroundColor = Color.FromArgb("#3B2A28"),
                                                Padding = 8,
                                                Content = imageExercise,
                                                HorizontalOptions = LayoutOptions.Center,
                                            },

                                            repetitionExercise,

                                            new HorizontalStackLayout
                                            {
                                                Spacing = 10,
                                                Children =
                                                {
                                                    setsOrTimeExercise,
                                                    timeButton
                                                }
                                            },

                                            pauseButton,

                                            new Button //botón para cancelar el ejercicio y volver al inicio de la rutina
                                            {
                                                Text = "Cancelar",
                                                BackgroundColor = Color.FromArgb("#4A2E2A"),
                                                TextColor = Colors.White,
                                                CornerRadius = 15,
                                                FontFamily = "ComfortaaBold",
                                                FontSize = 15,
                                                Padding = new Thickness(12, 8),
                                                Margin = new Thickness(0, 5, 0, 0),

                                                Command = new Command(async () =>
                                                {
                                                    bool primer = true; //variable para reiniciar los ejercicios
                                                    foreach (var ex in exercisesInRoutine) //reinicia los ejercicios de la rutina
                                                    {
                                                        ex.expaded = primer;
                                                        ex.exerciseFinished = false;
                                                        primer = false;
                                                    }

                                                    await Navigation.PopModalAsync(); //cierra el modal del ejercicio
                                                })
                                            }
                                        }
                                    }
                                }
                            }
                        };
                        await Navigation.PushModalAsync(modalPage); //muestra el modal del ejercicio
                    };
                    expanderExercise.Content = new HorizontalStackLayout //contenido del expansor del ejercicio
                    {
                        Padding = new Thickness(10),
                        HorizontalOptions = LayoutOptions.Center,
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
                    frame.Content = new VerticalStackLayout //contenido del marco del ejercicio
                    {
                        Children = { expanderExercise } //agrega el expansor al marco
                    };
                    return frame; //devuelve el marco del ejercicio
                })
            };



            eliminateRoutine.GestureRecognizers.Add(new TapGestureRecognizer //opción del desplegable para eliminar la rutina
            {
                Command = new Command(async () => //ejecuta la eliminación de la rutina
                {
                    await _dbService.Delete(_routine); //elimina la rutina de la base de datos

                    _filterViewModel.Routines.Clear();//limpia la lista de rutinas del filtro
                    var routines = await _dbService.GetRoutinesAsync(); //recoge las rutinas de la base de datos
                    var routinesExercises = await _dbService.GetRoutinesExercisesAsync(); //recoge los ejercicios de las rutinas de la base de datos
                    foreach (RoutinesExercises routineExercise in routinesExercises) //elimina los ejercicios asociados a la rutina
                    {
                        if (routineExercise.RoutineID.Equals(_routine.routineID)) //verifica si el ejercicio pertenece a la rutina
                        {
                            await _dbService.Delete(routineExercise); //elimina el ejercicio asociado a la rutina
                        }
                    }

                    _filterViewModel.UpdateFilteredRoutines(); //actualiza la lista de rutinas del filtro
                    await Navigation.PopModalAsync(); //cierra la rutina actual
                })
            });
            editRoutine.GestureRecognizers.Add(new TapGestureRecognizer //opción del desplegable para editar la rutina
            {
                Command = new Command(async () =>
                {
                    ModifyOrCreateRoutine(_routine); //llama al método para modificar o crear la rutina
                })
            });
            
            var horizontalButtons = new HorizontalStackLayout
            {
                Padding = new Thickness(10),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Children =
                        {
                            cancelButton,
                            finnishButton
                        }
            };
            var headerGrid = new Grid //grid para el encabezado de la rutina
            {
                ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            }
            };

            Grid.SetColumn(nameRoutine, 0); //asigna la columna 0 al nombre de la rutina
            headerGrid.Children.Add(nameRoutine); // Añade el nombre de la rutina al encabezado

            Grid.SetColumn(menuButton, 1); //asigna la columna 1 al botón de menú
            headerGrid.Children.Add(menuButton); // Añade el botón de menú al encabezado

            var baseGrid = new Grid //grid base de la rutina
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

            Grid.SetRow(headerGrid, 0); // asigna la fila 0 al encabezado
            baseGrid.Children.Add(headerGrid); // Añade el encabezado al grid base

            Grid.SetRow(listExercise, 1); // asigna la fila 1 a la lista de ejercicios
            baseGrid.Children.Add(listExercise); // Añade la lista de ejercicios al grid base

            Grid.SetRow(horizontalButtons, 2); //asigna la fila 2 al botón cancelar
            baseGrid.Children.Add(horizontalButtons); // Añade el botón cancelar al grid base

            var overlay = new AbsoluteLayout //overlay para el menú desplegable
            {
                IsVisible = false,
                InputTransparent = true,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            
            menuFrame.Opacity = 0; //  oculto al inicio
            AbsoluteLayout.SetLayoutBounds(menuFrame, new Rect(1, 0, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize)); // posición superior derecha
            AbsoluteLayout.SetLayoutFlags(menuFrame, AbsoluteLayoutFlags.PositionProportional); // usa proporciones para la posición
            overlay.Children.Add(menuFrame); // Añade el menú al overlay

            var closeTap = new ContentView { BackgroundColor = Colors.Transparent }; // área transparente para cerrar el menú al tocar fuera de él
            closeTap.GestureRecognizers.Add(new TapGestureRecognizer //gesto para cerrar el menú
            {
                Command = new Command(async () => //comando para cerrar el menú
                {
                    await menuFrame.FadeTo(0, 150);
                    overlay.IsVisible = false;
                    overlay.InputTransparent = true;
                })
            });
            AbsoluteLayout.SetLayoutBounds(closeTap, new Rect(0, 0, 1, 1)); // ocupa toda el área
            AbsoluteLayout.SetLayoutFlags(closeTap, AbsoluteLayoutFlags.All); // usa todas las proporciones
            overlay.Children.Insert(0, closeTap); // Añade el área de cierre al overlay (detrás del menú)

            var root = new Grid(); //grid raíz de la rutina
            root.Children.Add(baseGrid); // Añade el grid base al grid raíz
            root.Children.Add(overlay); // Añade el overlay al grid raíz

            menuButton.Clicked += async (s, e) => //evento al hacer clic en el botón de menú
            {
                if (!overlay.IsVisible) //si el overlay no es visible
                {
                    overlay.IsVisible = true; //muestra el overlay
                    overlay.InputTransparent = false; //habilita la interacción con el overlay
                    await menuFrame.FadeTo(1, 200); //muestra el menú con una animación de desvanecimiento
                }
                else
                {
                    await menuFrame.FadeTo(0, 150); //oculta el menú con una animación de desvanecimiento
                    overlay.IsVisible = false; //oculta el overlay
                    overlay.InputTransparent = true; //deshabilita la interacción con el overlay
                }
            };

            // === PÁGINA ===
            var routine = new ContentPage //página de la rutina
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
                    Content = root //asigna el grid raíz como contenido de la página
                }
            };
            Content = routine.Content; //muestra el contenido de la rutina
            BackgroundColor = routine.BackgroundColor; //asigna el color de fondo de la rutina
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inicializando CalendarDetailPage: {ex.Message}");
                Application.Current?.MainPage?.DisplayAlert("Error", "No se puedo mostrar la página.", "OK");
            }

        }


        private void BuildCreateUI() //crea una nueva rutina
        {
            try { 
            ModifyOrCreateRoutine();  //llama al método para modificar o crear la rutina
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inicializando CalendarDetailPage: {ex.Message}");
                Application.Current?.MainPage?.DisplayAlert("Error", "No se puedo mostrar la página.", "OK");
            }
        }
        private void BuildFilterRoutineUI() //filtro de rutinas
        {
            try { 
            var filterBodyPartEntry = new Picker //crea el picker para seleccionar la parte del cuerpo
            {
                Title = "Tipo Cuerpo",
                TextColor = Color.FromArgb("#C49362"),
                TitleColor = Color.FromArgb("#856D54"),
                ItemsSource = enumExtension.BodyTranslations.Values.ToList(),
                SelectedItem = enumExtension.BodyTranslations[bodyPartEnum.nothing],
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill
            };

            var filterDificultyEntry = new Picker //crea el picker para seleccionar la dificultad
            {
                Title = "Tipo de dificultad",
                TextColor = Color.FromArgb("#C49362"),
                TitleColor = Color.FromArgb("#856D54"),
                ItemsSource = enumExtension.DifficultyTranslations.Values.ToList(),
                SelectedItem = enumExtension.DifficultyTranslations[dificultyEnum.nothing],
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill
            };
            
            var filterNameEntry = new Entry //crea la entrada para el nombre de la rutina
            {
                Placeholder = "Nombre de la rutina",
                Keyboard = Keyboard.Text,
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
            };

            var filterButton = new Button //botón para aplicar el filtro
            {
                Text = "Filtrar",
                BackgroundColor = Color.FromArgb("ffd700"),
                TextColor = Colors.White,
                CornerRadius = 3,
                Command = new Command(async () => //comando para aplicar el filtro
                {
                    _filterViewModel ??= new RoutinesFilterViewModel(_dbService, userTypeEnum.nothing); // Asegurarse de que el ViewModel no sea nulo

                    _filterViewModel!.NameRoutineFilter =
                    string.IsNullOrWhiteSpace(filterNameEntry.Text)
                    ? null
                    : filterNameEntry.Text; //actualiza datos del filtro de nombre

                    var selectedDiff = enumExtension.DifficultyTranslations.FirstOrDefault(x => x.Value == (string)filterDificultyEntry.SelectedItem).Key; //recoge la dificultad seleccionada
                    _filterViewModel.DificultyFilter = selectedDiff; //actualiza datos del filtro de dificultad

                    var selectedBody = enumExtension.BodyTranslations.FirstOrDefault(x => x.Value == (string)filterBodyPartEntry.SelectedItem).Key; //regoce la parte del cuerpo seleccionada
                    _filterViewModel.BodyPartFilter = selectedBody; //actualiza datos del filtro de parte del cuerpo

                    await Navigation.PopModalAsync();//cierra el modal de filtro
                })
            };

            var cancelButton = new Button //botón para cancelar el filtro
            {
                Text = "Cancelar",
                BackgroundColor = Color.FromArgb("ffd700"),
                TextColor = Colors.White,
                CornerRadius = 3,
                Command = new Command(async () => await Navigation.PopModalAsync()) //comando para cerrar el modal de filtro
            };

            // UI FINAL
            var modalPage = new ContentPage //creación de la página modal para el filtro
            {
                BackgroundColor = Color.FromRgba(0, 0, 0, 0.6),
                Content = new Border
                {
                    BackgroundColor = Color.FromArgb("#2E1E1B"),
                    StrokeShape = new RoundRectangle { CornerRadius = 10 },
                    Margin = 1,
                    Padding = 3,
                    WidthRequest = 350,
                    HeightRequest = 350,
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
                        FontFamily = "EatMeAlive",
                        Margin = new Thickness(0,0,0,30)
                    },
                    new Label
                    {
                        Text = "Nombre Routina",
                        FontSize = 12,
                        TextColor = Color.FromArgb("#856D54"),
                        FontAttributes = FontAttributes.Bold,
                        FontFamily = "ComfortaaBold"
                    },
                        filterNameEntry,
                        filterBodyPartEntry,

                   new HorizontalStackLayout
                    {
                        Margin = new Thickness(0,20,0,0),
                        Spacing = 10,
                        HorizontalOptions = LayoutOptions.Center,
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

            Content = modalPage.Content; //muestra el contenido del modal
            BackgroundColor = modalPage.BackgroundColor; //muestra el color de fondo del modal
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inicializando CalendarDetailPage: {ex.Message}");
                Application.Current?.MainPage?.DisplayAlert("Error", "No se puedo mostrar la página.", "OK");
            }
        }

        private async Task ShowCountdown(TimeSpan time) //crea una cuenta regresiva
        {
            try { 
            var tcs = new TaskCompletionSource<bool>(); // para esperar hasta que se complete la cuenta regresiva
            int segundosRestantes = (int)time.TotalSeconds; // segundos totales de la cuenta regresiva
            bool cerrado = false; // para evitar cierres múltiples

            var labelTiempo = new Label //etiqueta para mostrar el tiempo restante
            {
                Text = time.ToString(@"mm\:ss"),
                FontSize = 72,
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold
            };

            var btnCancelar = new Button //botón para cancelar la cuenta regresiva
            {
                Text = "Cancelar",
                BackgroundColor = Colors.Gray,
                TextColor = Colors.White,
                CornerRadius = 10,
                Padding = 10,
                HorizontalOptions = LayoutOptions.Center
            };

            var modalCuentaAtras = new ContentPage //página modal para la cuenta regresiva
            {
                BackgroundColor = Color.FromArgb("#1A1A1A"),
                Content = new VerticalStackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    Spacing = 20,
                    Children = { labelTiempo, btnCancelar } //añade la etiqueta y el botón al contenido
                }
            };

            await Navigation.PushModalAsync(modalCuentaAtras); //muestra el modal de la cuenta regresiva

            var timer = Application.Current.Dispatcher.CreateTimer(); // crea un temporizador para actualizar la cuenta regresiva
            timer.Interval = TimeSpan.FromSeconds(1); // intervalo de 1 segundo

            async Task CerrarModalAsync() //método para cerrar el modal de la cuenta regresiva
            {
                if (cerrado) return;
                cerrado = true;

                timer.Stop();
                try { 
                    await Navigation.PopModalAsync(); 
                } catch { 
                    /* ignorar si ya se cerró */ 
                }
                tcs.TrySetResult(true); // señala que la cuenta regresiva ha terminado o se ha cancelado
            }

            btnCancelar.Clicked += async (s, e) => await CerrarModalAsync(); //evento al hacer clic en el botón cancelar

            timer.Tick += async (s, e) => //evento del temporizador para actualizar la cuenta regresiva
            {
                segundosRestantes--; // decrementa los segundos restantes

                if (segundosRestantes <= 0) // si el tiempo se ha agotado
                {
                    labelTiempo.Text = "¡Tiempo!"; // muestra el mensaje de tiempo agotado
                    labelTiempo.TextColor = Colors.OrangeRed; // cambia el color del texto
                    timer.Stop(); // detiene el temporizador

                    await Task.Delay(2000); // espera antes de cerrar
                    await CerrarModalAsync(); // cierra el modal
                }
                else
                {
                    labelTiempo.Text = TimeSpan.FromSeconds(segundosRestantes).ToString(@"mm\:ss"); //muestra el tiempo restante en formato mm:ss
                }
            };

            timer.Start(); // inicia el temporizador
            await tcs.Task; // espera hasta que la cuenta regresiva termine o se cancele
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inicializando CalendarDetailPage: {ex.Message}");
                Application.Current?.MainPage?.DisplayAlert("Error", "No se puedo mostrar la alarma.", "OK");
            }
        }
       
        private void ModifyOrCreateRoutine(Routines routine = null) //método para modificar o crear una rutina
        {
            try { 
            bool newRoutine = false; // indica si es una nueva rutina
            if (routine == null)// si no se proporciona una rutina, se crea una nueva
            {  
                newRoutine = true; // marca como nueva rutina
                routine = new Routines // crea una nueva rutina con valores predeterminados
                {
                    nameRoutine = "",
                    description = "",
                    muscleGroup = bodyPartEnum.nothing,
                    difficulty = dificultyEnum.nothing
                };
            }
            var nameRoutineEntry = new Entry // entrada para el nombre de la rutina
            {
                Placeholder = "Nombre de la rutina",
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                Text = routine.nameRoutine,
                HorizontalOptions = LayoutOptions.Fill
            };
            var DescriptionRoutineEntry = new Entry // entrada para la descripción de la rutina
            {
                Text = routine.description,
                Placeholder = "Descripción de la rutina",
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),

                HorizontalOptions = LayoutOptions.Fill
            };
            var bodyPartEnumPicker = new Picker // picker para seleccionar la parte del cuerpo
            {
                Title = "Tipo Cuerpo",
                ItemsSource = enumExtension.BodyTranslations.Values.ToList(),
                SelectedItem = enumExtension.BodyTranslations[routine.muscleGroup],
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill
            };
            var dificultyPicker = new Picker // picker para seleccionar la dificultad
            {
                Title = "Tipo de dificultad",
                ItemsSource = enumExtension.DifficultyTranslations.Values.ToList(),
                SelectedItem = enumExtension.DifficultyTranslations[routine.difficulty],
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill
            };
            var addExercises = new Button // botón para agregar ejercicios a la rutina
            {
                Text = "Agregar Ejercicios",
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill
            };
            ObservableCollection<Exercise> exercisesInRoutine = new ObservableCollection<Exercise>(); // colección de ejercicios en la rutina
            if (routine.Exercises != null)// si la rutina tiene ejercicios
            { 
                foreach (Exercise exercise in routine.Exercises) // agrega los ejercicios existentes a la colección
                {
                    exercisesInRoutine.Add(exercise);
                }
            }
            bodyPartEnumPicker.SelectedIndexChanged += async (s, e) => //evento al cambiar la selección en el picker de parte del cuerpo
            {
                // Verificamos si hay un elemento seleccionado
                if (bodyPartEnumPicker.SelectedItem is string selectedText) // si hay un elemento seleccionado
                {
                    if (exercisesInRoutine != null && exercisesInRoutine.Any()) // si hay ejercicios en la rutina
                    {
                        // Mostrar advertencia al usuario
                        bool answer = await DisplayAlert(
                            "Advertencia",
                            "Si cambias la parte del cuerpo, se borrarán todos los jercicios seleccionados.\n ¿Deseas cambiar la rutina?",
                            "Sí, vaciar",
                            "No"
                        ); // avisa al usuario sobre la pérdida de ejercicios

                        if (answer) // si el usuario confirma
                        {
                            exercisesInRoutine.Clear(); // vacía la lista de ejercicios
                            Console.WriteLine("Lista de ejercicios vaciada.");
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            };
            addExercises.Clicked += async (s, e) => //evento al hacer clic en el botón de agregar ejercicios
            {

                string selectedText = bodyPartEnumPicker.SelectedItem as string; // obtiene el texto seleccionado en el picker de parte del cuerpo
                bodyPartEnum selectedEnum = enumExtension.BodyTranslations.FirstOrDefault(x => x.Value == selectedText).Key; // obtiene el enum correspondiente al texto seleccionado

                if (selectedEnum.Equals(bodyPartEnum.nothing)) // si no se ha seleccionado una parte del cuerpo
                {
                    await DisplayAlert("Error", "Seleccione un grupo muscular", "OK");
                    return; // muestra un error y sale del método
                }
                else
                {
                    List<Exercise> exercises = await _dbService.GetExercisesAsync(); // obtiene la lista de ejercicios desde la base de datos
                    List<Exercise> ExercisesSelecter; // lista para almacenar los ejercicios filtrados

                    if (selectedEnum.Equals(bodyPartEnum.torsoAndArms)) //filtra ejercicios para torso y brazos
                    {
                        ExercisesSelecter = exercises.Where(ex =>
                            ex.muscleGroupId.Equals(bodyPartEnum.triceps)
                            || ex.muscleGroupId.Equals(bodyPartEnum.biceps)
                            || ex.muscleGroupId.Equals(bodyPartEnum.chest)
                            || ex.muscleGroupId.Equals(bodyPartEnum.back)).ToList();
                    }
                    else if (selectedEnum.Equals(bodyPartEnum.arms)) //filtra ejercicios para brazos
                    {
                        ExercisesSelecter = exercises.Where(ex =>
                            ex.muscleGroupId.Equals(bodyPartEnum.triceps)
                            || ex.muscleGroupId.Equals(bodyPartEnum.biceps)).ToList();
                    }
                    else if (selectedEnum.Equals(bodyPartEnum.torso)) //filtra ejercicios para torso
                    {
                        ExercisesSelecter = exercises.Where(ex =>
                            ex.muscleGroupId.Equals(bodyPartEnum.chest)
                            || ex.muscleGroupId.Equals(bodyPartEnum.back)).ToList();
                    }
                    else
                    {
                        ExercisesSelecter = exercises.Where(ex => ex.muscleGroupId.Equals(selectedEnum)).ToList(); //filtra ejercicios para la parte del cuerpo seleccionada
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
                        .ToList(); // agrupa los ejercicios por parte del cuerpo y dificultad

                    ObservableCollection<Exercise> exerciseSelection = new ObservableCollection<Exercise>(); // colección para almacenar los ejercicios seleccionados

                    var lookForExercise = new SearchBar // barra de búsqueda para filtrar ejercicios
                    {
                        Placeholder = "Buscar ejercicio..."
                    };

                    var collectionExercises = new CollectionView // vista de colección para mostrar los ejercicios
                    {
                        ItemsSource = grouped,
                        IsGrouped = true,
                        GroupHeaderTemplate = new DataTemplate(() => // plantilla para el encabezado del grupo
                        {
                            var difficultyBar = new BoxView // barra de dificultad
                            {
                                WidthRequest = 6,
                                CornerRadius = 3,
                                HorizontalOptions = LayoutOptions.Start,
                                VerticalOptions = LayoutOptions.Fill
                            };
                            difficultyBar.SetBinding(BoxView.ColorProperty, "HeaderColor"); //asigna el color según la dificultad

                            var headerLabel = new Label // etiqueta para el encabezado del grupo
                            {
                                FontAttributes = FontAttributes.Bold,
                                FontSize = 18,
                                FontFamily = "Forresten",
                                TextColor = Colors.Orange,
                                Margin = new Thickness(10, 5)
                            };
                            headerLabel.SetBinding(Label.TextProperty, "HeaderText"); //asigna el texto del encabezado

                            var grid = new Grid // grid para el encabezado del grupo
                            {
                                ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = 10 },
                        new ColumnDefinition { Width = GridLength.Star }
                    },
                                BackgroundColor = Color.FromArgb("#2B1A19")
                            };

                            grid.Add(difficultyBar, 0, 0); //añade la barra de dificultad al grid
                            grid.Add(headerLabel, 1, 0); //añade la etiqueta al grid

                            return grid;
                        }),
                        ItemTemplate = new DataTemplate(() => // plantilla para los elementos de la colección
                        {
                            var nameExercise = new Label // etiqueta para el nombre del ejercicio
                            {
                                FontAttributes = FontAttributes.Bold,
                                TextColor = Colors.White
                            };
                            nameExercise.SetBinding(Label.TextProperty, "name"); //asigna el nombre del ejercicio

                            var frame = new Border // marco para el ejercicio
                            {
                                Margin = 5,
                                Padding = 10,
                                BackgroundColor = Color.FromArgb("#3B2523"),
                                StrokeShape = new RoundRectangle
                                {
                                    CornerRadius = 8
                                },
                            };

                            frame.Content = new VerticalStackLayout //contenido del marco del ejercicio
                            {
                                Children = { nameExercise }
                            };
                            var tapGesture = new TapGestureRecognizer(); // gesto para detectar toques en el ejercicio
                            if (bodyPartEnumPicker.SelectedItem is string selectedText) // verifica si hay un elemento seleccionado en el picker de parte del cuerpo
                            {

                                    tapGesture.Tapped += async (s, e) => //evento al tocar el ejercicio
                                    {
                                        if (frame.BindingContext is Exercise selectedExercise) // verifica y recoge el contexto de enlace es un ejercicio
                                        {
                                            if (!selectedExercise.muscleGroupId.Equals(bodyPartEnum.isometric)) // si el ejercicio no es isométrico
                                            {
                                                var getExerciseLabel = new Label // etiqueta para el nombre del ejercicio
                                                {
                                                    FontAttributes = FontAttributes.Bold,
                                                    FontSize = 18,
                                                    FontFamily = "Forresten",
                                                    TextColor = Color.FromArgb("#C77B30"),
                                                    Margin = new Thickness(10, 5),
                                                    Text = selectedExercise.name
                                                };

                                                var repsLabel = new Entry // entrada para el número de repeticiones
                                                {
                                                    Placeholder = "Número de repeticiones",
                                                    Keyboard = Keyboard.Numeric
                                                };
                                                repsLabel.TextChanged += (s, e) => //evento al cambiar el texto en la entrada de repeticiones
                                                {
                                                    if (!string.IsNullOrEmpty(repsLabel.Text)) // si la entrada no está vacía
                                                    {
                                                        string onlyDigits = new string(repsLabel.Text.Where(char.IsDigit).ToArray()); // filtra solo los dígitos
                                                        if (repsLabel.Text != onlyDigits) // si el texto contiene caracteres no numéricos
                                                        {
                                                            repsLabel.Text = onlyDigits; // limpia si es texto
                                                        }
                                                    }
                                                };
                                                var setsLabel = new Entry // entrada para el número de series
                                                {
                                                    Placeholder = "Número de series",
                                                    Keyboard = Keyboard.Numeric
                                                };
                                                setsLabel.TextChanged += (s, e) => //evento al cambiar el texto en la entrada de series
                                                {
                                                    if (!string.IsNullOrEmpty(setsLabel.Text)) // si la entrada no está vacía
                                                    {
                                                        string onlyDigits = new string(setsLabel.Text.Where(char.IsDigit).ToArray()); // filtra solo los dígitos
                                                        if (setsLabel.Text != onlyDigits) // si el texto contiene caracteres no numéricos
                                                        {
                                                            setsLabel.Text = onlyDigits; // limpia si es texto
                                                        }
                                                    }
                                                };
                                                var modalPage = new ContentPage // página modal para agregar el ejercicio
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
                                                                        new Button // botón para crear el ejercicio
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
                                                                                newExercise.reps = int.TryParse(repsLabel.Text, out int reps) ? reps : 0; // asigna repeticiones
                                                                                newExercise.sets = int.TryParse(setsLabel.Text, out int sets) ? sets : 0; // asigna series
                                                                                newExercise.name = getExerciseLabel.Text; // asigna el nombre del ejercicio

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
                                                                            Command = new Command(async () => await Navigation.PopModalAsync()) //comando para cerrar el modal
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                };

                                                await Navigation.PushModalAsync(modalPage); //muestra el modal para agregar el ejercicio
                                            }
                                            else // si el ejercicio es isométrico
                                            {
                                               
                                                    var getExerciseLabel = new Label // etiqueta para el nombre del ejercicio
                                                    {
                                                        FontAttributes = FontAttributes.Bold,
                                                        FontSize = 18,
                                                        FontFamily = "Forresten",
                                                        TextColor = Color.FromArgb("#C77B30"),
                                                        Margin = new Thickness(10, 5),
                                                        Text = selectedExercise.name
                                                    };

                                                    var repsLabel = new Entry // entrada para el número de repeticiones
                                                    {
                                                        Placeholder = "Número de repeticiones",
                                                        Keyboard = Keyboard.Numeric
                                                    };
                                                    repsLabel.TextChanged += (s, e) => // evento al cambiar el texto en la entrada de repeticiones
                                                    {
                                                        if (!string.IsNullOrEmpty(repsLabel.Text)) // si la entrada no está vacía
                                                        {
                                                            string onlyDigits = new string(repsLabel.Text.Where(char.IsDigit).ToArray()); // filtra solo los dígitos
                                                            if (repsLabel.Text != onlyDigits) // si el texto contiene caracteres no numéricos
                                                            {
                                                                repsLabel.Text = onlyDigits; // limpia si es texto
                                                            }
                                                        }
                                                    };
                                                    var timeLabel = new Entry // entrada para el tiempo de ejecución
                                                    {
                                                        Placeholder = "Tiempo de ejecución",
                                                        Keyboard = Keyboard.Numeric
                                                    };
                                                    timeLabel.TextChanged += (s, e) => // evento al cambiar el texto en la entrada de tiempo
                                                    {
                                                        if (!string.IsNullOrEmpty(timeLabel.Text)) // si la entrada no está vacía
                                                        {
                                                            string onlyDigits = new string(timeLabel.Text.Where(char.IsDigit).ToArray()); // filtra solo los dígitos
                                                            if (timeLabel.Text != onlyDigits) // si el texto contiene caracteres no numéricos
                                                            {
                                                                timeLabel.Text = onlyDigits; // limpia si es texto
                                                            }
                                                        }
                                                    };
                                                    var modalPage = new ContentPage // página modal para agregar el ejercicio isométrico
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
                                                                        new Button // botón para crear el ejercicio
                        {
                                                                            Text = "Crear",
                                                                            Command = new Command(async () =>
                                                                            {
                                                                                if (string.IsNullOrEmpty(repsLabel.Text) || string.IsNullOrEmpty(timeLabel.Text)) // si las entradas están vacías
                                                                                {
                                                                                    await DisplayAlert("Error", "Introduce series y tiempo de ejecución.", "OK");
                                                                                    return; // muestra un error y sale del método
                                                                                }
                                                                                if(repsLabel.Text.Equals("0") || timeLabel.Text.Equals("0")) // si las entradas son 0
                                                                                {
                                                                                    await DisplayAlert("Error", "El tiempo y las repeticiones deben ser mayores que 0.", "OK");
                                                                                    return; // muestra un error y sale del método
                                                                                }
                                                                                // Crear una copia del ejercicio seleccionado
                                                                                var newExercise = selectedExercise.Clone();

                                                                                // Asignar los valores específicos de este nuevo ejercicio
                                                                                newExercise.reps = int.TryParse(repsLabel.Text, out int s) ? s : 0;
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
                                                                            Command = new Command(async () => await Navigation.PopModalAsync()) // sale del modal
                                                                        }
                                                                    }
                                                                }
                                                                }
                                                            }
                                                        }
                                                    };

                                                    await Navigation.PushModalAsync(modalPage); // muestra el modal para agregar el ejercicio
                                            }
                                            }
                                        
                                    };
                                        
                                
                                frame.GestureRecognizers.Add(tapGesture); // añade el gesto al marco del ejercicio
                                return frame; // devuelve el marco del ejercicio
                            }
                            frame.GestureRecognizers.Add(tapGesture); // añade el gesto al marco del ejercicio
                            return frame; // devuelve el marco del ejercicio
                        })
                    };
                    if (exercisesInRoutine != null) // si hay ejercicios en la rutina
                    {
                        foreach (var exercise in exercisesInRoutine) // recorre los ejercicios en la rutina
                        {
                            exerciseSelection.Add(exercise);
                        }
                    }

                    var selectedExercisesView = new CollectionView //lista para ver los ejercicios seleccionados
                    {

                        ItemsSource = exerciseSelection, // fuente de datos es la selección de ejercicios
                        EmptyView = new Label // vista vacía si no hay ejercicios seleccionados
                        {
                            Text = "No hay ejercicios seleccionados.",
                            TextColor = Colors.Gray,
                            HorizontalOptions = LayoutOptions.Center
                        },
                        ItemTemplate = new DataTemplate(() => // plantilla para los elementos de la colección
                        {
                            var nameLabel = new Label // etiqueta para el nombre del ejercicio
                            {
                                FontAttributes = FontAttributes.Bold,
                                TextColor = Colors.White
                            };
                            nameLabel.SetBinding(Label.TextProperty, "name"); // asigna el nombre del ejercicio

                            var setsAndRepsLabel = new Label // etiqueta para series y repeticiones
                            {
                                FontSize = 12,
                                TextColor = Colors.LightGray
                            };
                            MultiBinding multiBinding = new MultiBinding { }; // vinculación múltiple para series y repeticiones
                            if (bodyPartEnumPicker.SelectedItem is string selectedText) // verifica y recoge un elemento seleccionado en el picker de parte del cuerpo
                            {
                                if (!selectedText.Equals(enumExtension.BodyTranslations[bodyPartEnum.isometric])) // si no es isométrico
                                {
                                    multiBinding = new MultiBinding //muestra series y repeticiones
                                    {
                                        StringFormat = "Repeticiones: {0}\nSeries: {1}"
                                    };
                                    multiBinding.Bindings.Add(new Binding("reps"));
                                    multiBinding.Bindings.Add(new Binding("sets"));
                                }
                                else // si es isométrico
                                {
                                    multiBinding = new MultiBinding // muestra repeticiones y tiempo
                                    {
                                        StringFormat = "Repeticiones: {0}\nTiempo: {1} segundos"
                                    };
                                    multiBinding.Bindings.Add(new Binding("reps"));
                                    multiBinding.Bindings.Add(new Binding("seconds"));
                                }
                            }
                            setsAndRepsLabel.SetBinding(Label.TextProperty, multiBinding); // asigna el texto de series y repeticiones
                            var removeButton = new Button // etiqueta para el botón de eliminar ejercicio
                            {
                                Text = "❌",
                                BackgroundColor = Colors.Transparent,
                                TextColor = Colors.Orange,
                                FontSize = 18,
                                Padding = new Thickness(5)
                            };
                            removeButton.Clicked += (s, e) => // evento al hacer clic en el botón de eliminar
                            {
                                if (removeButton.BindingContext is Exercise exToRemove) // verifica y recoge el contexto de enlace como ejercicio
                                    exerciseSelection.Remove(exToRemove); //elimina el ejercicio de la selección
                            };

                            return new Border // crea el marco para cada ejercicio seleccionado
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

                    var mainLayout = new VerticalStackLayout // diseño principal de la página
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
                                         new Button //botón para agregar los ejercicios seleccionados a la rutina
                                {
                                            Text = "Agregar",
                                            Command = new Command(async () =>
                                            {
                                                exercisesInRoutine.Clear(); // limpia la lista de ejercicios en la rutina

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
                                    new Button // botón para cancelar y cerrar el modal
                                    {
                                        Text = "Cancelar",
                                        Command = new Command(async () => await Navigation.PopModalAsync())
                                    }
                                    }
                                }
                        }
                    };

                    var selectedExercisesPage = new ContentPage // página para mostrar los ejercicios seleccionados
                    {
                        BackgroundColor = Color.FromArgb("#2E1E1B"),
                        Content = new ScrollView { Content = mainLayout }
                    };

                    await Navigation.PushModalAsync(selectedExercisesPage); // muestra la página modal de ejercicios seleccionados
                }
            };
            var collectionExercises = new CollectionView // lista para ver los ejercicios añadidos a la rutina
            {
                ItemsSource = exercisesInRoutine,
                WidthRequest = 300,
                EmptyView = new Label // vista vacía si no hay ejercicios añadidos
                {
                    Text = "No hay ejercicios añadidos.",
                    TextColor = Colors.Gray,
                    HorizontalOptions = LayoutOptions.Center
                },
                ItemTemplate = new DataTemplate(() => // plantilla para los elementos de la colección
                {
                    var nameLabel = new Label // nombre del ejercicio
                    {
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Colors.White
                    };
                    nameLabel.SetBinding(Label.TextProperty, "name"); // asigna el nombre del ejercicio

                    var repsLabel = new Label // etiqueta para repeticiones, series o tiempo
                    {
                        TextColor = Colors.LightGray,
                        FontSize = 12
                    };

                    MultiBinding multiBinding = new MultiBinding { }; // vinculación múltiple para repeticiones, series o tiempo
                    if (bodyPartEnumPicker.SelectedItem is string selectedText) // verifica y recoge un elemento seleccionado en el picker de parte del cuerpo
                    {
                        if (!selectedText.Equals(enumExtension.BodyTranslations[bodyPartEnum.isometric])) // si no es isométrico
                        {
                            multiBinding = new MultiBinding { StringFormat = "Repeticiones: {0} | Series: {1}" };
                            multiBinding.Bindings.Add(new Binding("reps")); // recoge repeticiones
                            multiBinding.Bindings.Add(new Binding("sets"));// recoge series
                        }
                        else // si es isométrico
                        {
                            multiBinding = new MultiBinding { StringFormat = "Repeticiones: {0} | Tiempo: {1} segundos" };
                            multiBinding.Bindings.Add(new Binding("reps")); // recoge repeticiones
                            multiBinding.Bindings.Add(new Binding("seconds")); // recoge tiempo
                        }
                    }
                    repsLabel.SetBinding(Label.TextProperty, multiBinding); // asigna el texto de repeticiones, series o tiempo

                    return new HorizontalStackLayout // diseño horizontal para cada ejercicio añadido
                    {
                        Spacing = 10,
                        Children = { nameLabel, repsLabel }
                    };
                })
            };
            var finishButton = new Button // botón para guardar o modificar la rutina
            {
                Command = new Command(async () =>
                {

                    if (nameRoutineEntry.Text == null || string.IsNullOrWhiteSpace(nameRoutineEntry.Text) ||
                    DescriptionRoutineEntry.Text == null || string.IsNullOrWhiteSpace(DescriptionRoutineEntry.Text) ||
                    !exercisesInRoutine.Any()) //verifica que los campos no estén vacíos y que haya al menos un ejercicio
                    {
                        await DisplayAlert("Error", "Rellena todos lo campos, y añade por lo menos un ejercicio", "OK");
                        return; // muestra un error y sale del método
                    }
                    else // si los campos están completos
                    {
                        if (newRoutine) // si es una nueva rutina
                        {
                            //rellena los campos de la rutina
                            var selectedBodyTranslation = bodyPartEnumPicker.SelectedItem.ToString();
                            var selectedDificultyTranslation = dificultyPicker.SelectedItem.ToString();
                            var selectedBodyEnum = enumExtension.BodyTranslations.FirstOrDefault(x => x.Value == selectedBodyTranslation).Key;
                            var selectedDificultyEnum = enumExtension.DifficultyTranslations.FirstOrDefault(x => x.Value == selectedDificultyTranslation).Key;
                            var userId = await SecureStorage.GetAsync("user_id");
                            var user = await _dbService.GetUserById(int.Parse(userId));
                            Routines createRoutine;
                            if (user.userType.Equals(userTypeEnum.admin)) // crea la rutina según el tipo de usuario
                            {
                                createRoutine = new Routines { nameRoutine = nameRoutineEntry.Text, description = DescriptionRoutineEntry.Text, muscleGroup = selectedBodyEnum, typeUser = userTypeEnum.all, userID = 0, difficulty = selectedDificultyEnum };
                            }
                            else
                            {
                                createRoutine = new Routines { nameRoutine = nameRoutineEntry.Text, description = DescriptionRoutineEntry.Text, muscleGroup = selectedBodyEnum, difficulty = selectedDificultyEnum, typeUser = userTypeEnum.all, userID = user.UserID };
                            }

                            await _dbService.Create(createRoutine); // crea la rutina en la base de datos

                            foreach (Exercise exercise in exercisesInRoutine) // crea las relaciones entre la rutina y los ejercicios
                            {
                                RoutinesExercises routineExercises = new RoutinesExercises { RoutineID = createRoutine.routineID, ExerciseID = exercise.execiseID, sets = exercise.sets, reps = exercise.reps, seconds = exercise.seconds };
                                await _dbService.Create(routineExercises);
                            }
                        }
                        else
                        { // si es para modificar una rutina existente
                            var existingRelations = await _dbService.GetRoutinesExercisesAsync(); // obtiene las relaciones existentes

                            //modifica los campos de la rutina
                            var selectedBodyTranslation = bodyPartEnumPicker.SelectedItem.ToString();
                            var selectedDificultyTranslation = dificultyPicker.SelectedItem.ToString();
                            var selectedBodyEnum = enumExtension.BodyTranslations.FirstOrDefault(x => x.Value == selectedBodyTranslation).Key;
                            var selectedDificultyEnum = enumExtension.DifficultyTranslations.FirstOrDefault(x => x.Value == selectedDificultyTranslation).Key;
                            routine.nameRoutine = nameRoutineEntry.Text;
                            routine.description = DescriptionRoutineEntry.Text;
                            routine.muscleGroup = selectedBodyEnum;
                            routine.difficulty = selectedDificultyEnum;
                            routine.Exercises = exercisesInRoutine;


                            var eliminateExercises = existingRelations.Where(r => r.RoutineID.Equals(routine.routineID)).ToList(); // obtiene las relaciones a eliminar

                            foreach (var relation in eliminateExercises) // elimina las relaciones si exisitieran
                            {
                                if (relation.RoutineID.Equals(routine.routineID))
                                {
                                    await _dbService.Delete(relation);
                                }
                            }
                            foreach (Exercise exercise in routine.Exercises) // crea las nuevas relaciones entre la rutina y los ejercicios
                            {
                                await _dbService.Create(new RoutinesExercises { RoutineID = routine.routineID, ExerciseID = exercise.execiseID, sets = exercise.sets, reps = exercise.reps, seconds = exercise.seconds });
                            }
                            await _dbService.Update(routine); //actualiza la rutina en la base de datos

                        }
                    }
                    await _filterViewModel.LoadRoutinesAsync(); // recarga las rutinas
                    _filterViewModel.UpdateFilteredRoutines(); //actualiza las rutinas filtradas

                    await Navigation.PopModalAsync(); // cierra el modal

                })
            };
            var Tittle = new Label //titulo de la página
            {
                FontSize = 24,

                TextColor = Color.FromArgb("#C77B30"),

                HorizontalOptions = LayoutOptions.Fill,
                HorizontalTextAlignment = TextAlignment.Center,
                FontFamily = "EatMeAlive"
            };
            if (newRoutine) //verifica si es nueva la rutina
            {
                Tittle.Text = "Crear Rutina";
                finishButton.Text = "Crear";
            }
            else
            {
                Tittle.Text = "Modificar Rutina";
                finishButton.Text = "Modificar";
            }
                var modalPage = new ContentPage // contenido de la página modal para crear/modificar la rutina
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
                    Tittle,

                    nameRoutineEntry,
                    DescriptionRoutineEntry,
                    dificultyPicker,
                    bodyPartEnumPicker,
                    addExercises,
                    collectionExercises,
                    new HorizontalStackLayout
                    {
                        Spacing = 10,
                        Children =
                        {
                            finishButton,

                        new Button // botón para cancelar y cerrar el modal
                        {
                            Text = "Cancelar",
                            Command = new Command(async () => await Navigation.PopModalAsync()) // sale del modal
                        }
                        }
                    }
                }
                        }
                    }
                };
            Content = modalPage.Content; // muestra el contenido del modal
            BackgroundColor = modalPage.BackgroundColor; // muestra el color de fondo del modal
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inicializando CalendarDetailPage: {ex.Message}");
                Application.Current?.MainPage?.DisplayAlert("Error", "No se puedo crear o modifica la rutina.", "OK");
            }
        }
       
        private async Task RegisterInCalendar() //registra la rutina en el calendario
        {
            try { 
            var userId = await SecureStorage.GetAsync("user_id"); //obtiene la id del usuario
            if(userId == null)
            {
                await DisplayAlert(
                                     "Ejercicio isométrico",
                                     "usuario no identificado.",
                                     "Aceptar"
                                 );
                return;
            }
            var user = await _dbService.GetUserById(int.Parse(userId));  //obtiene el usuario de la base de datos
            string fecha = DateTime.Today.ToString("dd/MM/yyyy");
            if (_routine != null)
            {
                var regiter = new RegisterLogging { routineID = _routine.routineID, userID = user.UserID,  day = fecha, timeToStart = timeToStart, timeToEnd = timeToEnd}; //crea el registro
                await _dbService.Create(regiter); //guarda el registro
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inicializando CalendarDetailPage: {ex.Message}");
                Application.Current?.MainPage?.DisplayAlert("Error", "No se puedo guardar el registro.", "OK");
            }
        }
    }
}
