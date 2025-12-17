
using Microsoft.Maui.Controls.Shapes;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProyectoFinDeCurso.Pages.Detail
{
    public class CalendarDetailPage : ContentPage
    {
        private readonly DbService _dbService; //acceso a la base de datos
        private readonly string _day; //identificador del día
        private readonly ModeEnum _mode; //modo de la página (ver, editar, crear, filtrar)
        public CalendarDetailPage(DbService dbService, ModeEnum mode, string? day = null) //constructor obtieniendo los datos
        {
            try
            {
                _dbService = dbService;
                _day = day;
                _mode = mode;
            BuildUI();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inicializando CalendarDetailPage: {ex.Message}");
                Application.Current?.MainPage?.DisplayAlert("Error", "No se pudo cargar la página del calendario.", "OK");
            }
        }

        private void BuildUI() // Construye la interfaz de ejercicios según el modo
        {
            try
            {
                switch (_mode)
                {
                    case ModeEnum.create:
                        BuildCreateAlarmUI();
                        break;

                    case ModeEnum.Edit:
                        break;

                    case ModeEnum.View:
                        BuildViewUI();
                        break;

                    case ModeEnum.filter:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(_mode), $"Modo no soportado: {_mode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error construyendo UI en CalendarDetailPage: {ex.Message}");
                DisplayAlert("Error", "No se pudo cargar la interfaz correctamente.", "OK");
            }
        }
        private async Task BuildViewUI() //muestra el contenido del día seleccionado
        {
            try
            {

                var userId = await SecureStorage.GetAsync("user_id"); //obtiene la id del usuario
                if (userId == null)
                { //verifica si no es nulo
                    await DisplayAlert(
                                                                     "Usuario No registrado",
                                                                     "usuario no identificado.",
                                                                     "Aceptar"
                                                                 );
                    return;
                }

                var user = await _dbService.GetUserById(int.Parse(userId)); //obtiene al usuario

                var registerRoutines = await _dbService.GetRegisterLoggingAsync(); //obtiene los registros de las rutinas acabadas
                var listRegisterRoutinesByUser = registerRoutines
                    .Where(x => x.userID == user.UserID && x.day == _day)
                    .ToList(); //filtra por la ip del usuario

                var listRoutines = await _dbService.GetRoutinesAsync(); //obtiene las rutinas

                // UNIR REGISTRO + RUTINA
                var routinesData = listRegisterRoutinesByUser
                    .Join(listRoutines,
                          reg => reg.routineID,
                          rt => rt.routineID,
                          (reg, rt) => new
                          {
                              name = rt.nameRoutine,
                              description = rt.description,
                              startHour = reg.timeToStart,
                              endHour = reg.timeToEnd
                          })
                    .ToList(); //filtra y obtiene los datos necesarios para mostrarlo

                var routinesCollection = new CollectionView //lista del registro de ese día
                {
                    ItemsSource = routinesData, //obtiene la lista de rutinas con su hora de comienzo y final
                    SelectionMode = SelectionMode.None,
                    Margin = new Thickness(10, 0, 10, 0),
                    HeightRequest = 220,

                    ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical)
                    {
                        ItemSpacing = 25  //separación entre líneas
                    },
                    EmptyView = new Grid
                    {
                        VerticalOptions = LayoutOptions.Fill,
                        HorizontalOptions = LayoutOptions.Fill,
                        Children =
                        {
                            new Label
                            {
                                Text = "No hay datos registrados aún",
                                TextColor = Color.FromArgb("#CFC86D"),
                                FontSize = 18,
                                FontFamily = "ComfortaaBold",
                                HorizontalOptions = LayoutOptions.Center,
                                VerticalOptions = LayoutOptions.Center
                            }
                        }
                    },
                    ItemTemplate = new DataTemplate(() => //muestra las rutinas
                {


                    Color titleColor = Color.FromArgb("#D6A77A");
                    Color valueColor = Color.FromArgb("#F0E6DA");

                    double titleSize = 15;
                    double valueSize = 17;

                    string titleFont = "ComfortaaBold";
                    string valueFont = "Comfortaa";


                    var nameTitle = new Label //título de la rutina
                    {
                        Text = "Nombre:",
                        FontSize = titleSize,
                        FontFamily = titleFont,
                        TextColor = titleColor,
                        Margin = new Thickness(0, 0, 4, 0)
                    };

                    var nameValue = new Label
                    {
                        FontSize = valueSize,
                        FontFamily = valueFont,
                        TextColor = valueColor,
                        Opacity = 0.95
                    };
                    nameValue.SetBinding(Label.TextProperty, "name"); //nombre de la rutina

                    var nameRow = new HorizontalStackLayout
                    {
                        Spacing = 8,
                        Children = { nameTitle, nameValue }//muestra del nombre en fila
                    };



                    var descTitle = new Label //título de la rutina
                    {
                        Text = "Descripción:",
                        FontSize = titleSize,
                        FontFamily = titleFont,
                        TextColor = titleColor
                    };

                    var descValue = new Label
                    {
                        FontSize = valueSize - 1,
                        FontFamily = valueFont,
                        TextColor = valueColor,
                        MaxLines = 2,
                        Opacity = 0.9
                    };
                    descValue.SetBinding(Label.TextProperty, "description"); //descripción de la rutina

                    var descRow = new HorizontalStackLayout
                    {
                        Spacing = 8,
                        Children = { descTitle, descValue } //muestra de la descripción en fila
                    };


                    var startTitle = new Label //título de la hora de inicio
                    {
                        Text = "Inicio:",
                        FontSize = titleSize,
                        FontFamily = titleFont,
                        TextColor = titleColor
                    };

                    var startValue = new Label
                    {
                        FontSize = valueSize,
                        FontFamily = valueFont,
                        TextColor = Color.FromArgb("#E9C68A")
                    };
                    startValue.SetBinding(Label.TextProperty, "startHour"); //hora de comienzo

                    var startRow = new HorizontalStackLayout
                    {
                        Spacing = 8,
                        Children = { startTitle, startValue }//muestra la hora en fila
                    };


                    var endTitle = new Label // título del fin de la rutina
                    {
                        Text = "Fin:",
                        FontSize = titleSize,
                        FontFamily = titleFont,
                        TextColor = titleColor
                    };

                    var endValue = new Label
                    {
                        FontSize = valueSize,
                        FontFamily = valueFont,
                        TextColor = Color.FromArgb("#E9C68A")
                    };
                    endValue.SetBinding(Label.TextProperty, "endHour"); // hora del final

                    var endRow = new HorizontalStackLayout
                    {
                        Spacing = 8,
                        Children = { endTitle, endValue } //muestra la hora en fila
                    };


                    var stack = new VerticalStackLayout //muestra todos los horizontalStackLayout en vertical
                    {
                        Spacing = 12,
                        Children =
                        {
                            nameRow,
                            descRow,
                            startRow,
                            endRow
                        }
                    };


                    return new Border //muestra el contenido
                    {
                        Padding = new Thickness(18, 15),
                        BackgroundColor = Color.FromArgb("#3B2523"),
                        StrokeShape = new RoundRectangle { CornerRadius = 14 },
                        Stroke = Color.FromArgb("#6A463F"),
                        StrokeThickness = 1.5,

                        // Sombras para efecto más pro
                        Shadow = new Shadow
                        {
                            Brush = Colors.Black,
                            Offset = new Point(2, 4),
                            Radius = 8
                        },

                        Content = stack //lo muestra en pantalla
                    };
                })
                };


            Content = new Border // muestra todo el contenido de la página
            {
                BackgroundColor = Color.FromArgb("#2E1E1B"),
                StrokeShape = new RoundRectangle { CornerRadius = 18 },
                Margin = new Thickness(20, 80),
                Padding = new Thickness(15, 20),

                Content = new VerticalStackLayout
                {
                    Padding = new Thickness(10, 0),
                    Spacing = 12,

                    Children =
        {
            new Label //titulo de la página
            {
                Text = "Detalles",
                FontSize = 26,
                TextColor = Color.FromArgb("#C77B30"),
                FontFamily = "EatMeAlive",
                HorizontalOptions = LayoutOptions.Center
            },

            new Label // día seleccíonado por el usuario
            {
                Text = _day,
                FontSize = 18,
                TextColor = Color.FromArgb("#C49362"),
                FontFamily = "ComfortaaBold",
                HorizontalOptions = LayoutOptions.Center
            },

            new Label //Titulo segundario
            {
                Text = "Registros del día",
                FontSize = 20,
                FontFamily = "ComfortaaBold",
                TextColor = Color.FromArgb("#D6A77A"),
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 15, 0, 5)
            },

            routinesCollection, //muestra el registro
            new Button //botón para salir
            {
                Text = "Salir",
                BackgroundColor = Color.FromArgb("ffd700"),
                TextColor = Colors.White,
                CornerRadius = 8,
                WidthRequest = 150,
                HeightRequest = 45,
                HorizontalOptions = LayoutOptions.Center,
                Command = new Command(async () =>
                {
                    await Navigation.PopModalAsync();
                })
            }
        }
                }
            };

            BackgroundColor = Color.FromRgba(0, 0, 0, 0.6);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Join Error: " + ex.Message);
                await DisplayAlert("Error", "Error al crear la página.", "OK");
                return;
            }
        }

        private async Task BuildCreateAlarmUI() //build para crear los avisos al usuario
        {
            try
            {
                var userId = await SecureStorage.GetAsync("user_id"); //obtiene la id del usuario
                if (userId == null) //verifica si el usuario inició sesion
                {
                    await DisplayAlert(
                                                                     "Usuario No registrado",
                                                                     "usuario no identificado.",
                                                                     "Aceptar"
                                                                 );
                    return;
                }

                var alarmas = await _dbService.GetAlarmsAsync(); //obtiene las alarmas
                var filterAlarm = alarmas.Where(x => x.userID == int.Parse(userId)).ToList(); //filtra las alarmas por la id del usuario

                var titleLabel = new Label //título de la página
                {
                    Text = "Alarmas",
                    FontSize = 28,
                    FontFamily = "EatMeAlive",
                    TextColor = Color.FromArgb("#C77B30"),
                    HorizontalOptions = LayoutOptions.Start
                };

                var addButton = new Button //botón para añadir nuevas alarmas
                {
                    Text = "+",
                    FontSize = 28,
                    BackgroundColor = Color.FromArgb("#3B2523"),
                    TextColor = Color.FromArgb("#CFC86D"),
                    FontFamily = "Forresten",
                    CornerRadius = 10,
                    WidthRequest = 50,
                    HeightRequest = 50,
                    Command = new Command(async () => //acción al pulsar el botón
                    {
                        ModifyOrCreateExercise();

                    })
                };

                var headerGrid = new Grid //grid del encabezado
                {
                    ColumnDefinitions =
        {
            new ColumnDefinition { Width = GridLength.Star },
            new ColumnDefinition { Width = 50 }
        }
                };

                headerGrid.Children.Add(titleLabel); //añade el título
                Grid.SetColumn(titleLabel, 0); //posición en la cuadrícula

                headerGrid.Children.Add(addButton); //añade el botón de añadir
                Grid.SetColumn(addButton, 1); //posición en la cuadrícula

                var alarmsCollection = new CollectionView //lista de las alarmas
                {
                    ItemsSource = filterAlarm,
                    SelectionMode = SelectionMode.None,
                    VerticalOptions = LayoutOptions.FillAndExpand,

                    ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical) //disposición vertical
                    {
                        ItemSpacing = 25 //separación entre líneas
                    },

                    EmptyView = new Label //mensaje si no hay alarmas
                    {
                        Text = "No hay alarmas aún",
                        TextColor = Color.FromArgb("#CFC86D"),
                        FontSize = 18,
                        FontFamily = "ComfortaaBold",
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    }
                };

                alarmsCollection.ItemTemplate = new DataTemplate(() => //plantilla para cada alarma
                {
                    var nameLabel = new Label//nombre de la alarma
                    {
                        FontSize = 18,
                        FontFamily = "ComfortaaBold",
                        TextColor = Color.FromArgb("#CFC86D")
                    };
                    nameLabel.SetBinding(Label.TextProperty, "Name");

                    var hourLabel = new Label //hora de la alarma
                    {
                        FontSize = 16,
                        FontFamily = "Comfortaa",
                        TextColor = Color.FromArgb("#E9C68A")
                    };
                    hourLabel.SetBinding(Label.TextProperty, "FormattedTime"); //propiedad formateada de la hora

                    var weekLayout = new HorizontalStackLayout //días de la semana
                    {
                        Spacing = 12,
                        HorizontalOptions = LayoutOptions.Start
                    };

                    VerticalStackLayout CreateDay(string text, string binding) //crea cada día con su checkbox enlazado
                    {
                        var check = new CheckBox
                        {
                            Color = Color.FromArgb("#CFC86D"),
                            IsEnabled = false,
                            Scale = 0.9,
                            HorizontalOptions = LayoutOptions.Center
                        };
                        check.SetBinding(CheckBox.IsCheckedProperty, binding); //enlaza con la propiedad del modelo

                        return new VerticalStackLayout //muestra del día
                        {
                            Spacing = 1,
                            Children =
                {
                new Label
                {
                    Text = text,
                    FontFamily = "ComfortaaBold",
                    TextColor = Color.FromArgb("#CFC86D")
                },
                check
                }
                        };
                    }

                    weekLayout.Children.Add(CreateDay("L", "Monday")); //añade los días a la vista
                    weekLayout.Children.Add(CreateDay("M", "Tuesday"));
                    weekLayout.Children.Add(CreateDay("X", "Wednesday"));
                    weekLayout.Children.Add(CreateDay("J", "Thursday"));
                    weekLayout.Children.Add(CreateDay("V", "Friday"));
                    weekLayout.Children.Add(CreateDay("S", "Saturday"));
                    weekLayout.Children.Add(CreateDay("D", "Sunday"));


                    var editButton = new ImageButton //botón para editar la alarma
                    {
                        Source = "editar.png",
                        WidthRequest = 30,
                        HeightRequest = 30,
                        BackgroundColor = Colors.Transparent,
                        HorizontalOptions = LayoutOptions.End,
                        Margin = new Thickness(0, 0, 8, 0)
                    };
                    editButton.SetBinding(ImageButton.CommandParameterProperty, "."); //enlaza con la alarma actual
                    editButton.Command = new Command<Alarm>(async (alarm) => //acción al pulsar el botón
                    {
                        ModifyOrCreateExercise(alarm);
                    });

                    var deleteButton = new ImageButton //botón para eliminar la alarma
                    {
                        Source = "papelera.png",
                        WidthRequest = 30,
                        HeightRequest = 30,
                        BackgroundColor = Colors.Transparent,
                        HorizontalOptions = LayoutOptions.End,

                    };
                    deleteButton.SetBinding(ImageButton.CommandParameterProperty, "."); //enlaza con la alarma actual
                    deleteButton.Command = new Command<Alarm>(async (alarm) => //acción al pulsar el botón
                    {
                        bool confirm = await Application.Current.MainPage.DisplayAlert(
                            "Eliminar alarma",
                            $"¿Eliminar la alarma \"{alarm.Name}\"?",
                            "Sí",
                            "No"
                        ); //confirma la eliminación

                        if (confirm)
                        {
                            LocalNotificationCenter.Current.Cancel(alarm.AlarmID); //cancela la notificación programada
                            await _dbService.Delete(alarm); //eliminación de la alarma en la base de datos
                        }
                    });


                    var grid = new Grid //grid para organizar los elementos
                    {
                        ColumnDefinitions =
                        {
                        new ColumnDefinition { Width = GridLength.Star },  // texto y días
                        new ColumnDefinition { Width = GridLength.Auto },  // editar
                        new ColumnDefinition { Width = GridLength.Auto }   // eliminar
                        }
                    };

                    var dataStack = new VerticalStackLayout //stack para los datos de la alarma
                    {
                        Spacing = 10,
                        Children = { nameLabel, hourLabel, weekLayout }
                    };

                    Grid.SetColumn(dataStack, 0); //posición en la cuadrícula
                    grid.Children.Add(dataStack);// añade los datos

                    Grid.SetColumn(editButton, 1);//posición en la cuadrícula
                    grid.Children.Add(editButton);// añade los datos

                    Grid.SetColumn(deleteButton, 2);//posición en la cuadrícula
                    grid.Children.Add(deleteButton);// añade los datos


                    return new Border //muestra el contenido de la alarma
                    {
                        BackgroundColor = Color.FromArgb("#2E1E1B"),
                        Stroke = Color.FromArgb("#6A463F"),
                        StrokeThickness = 1.5,
                        Margin = new Thickness(0, 10),
                        Padding = new Thickness(15),
                        StrokeShape = new RoundRectangle { CornerRadius = 15 },
                        Content = grid
                    };
                });
                var listContainer = new Border //contenedor de la lista de alarmas
                {
                    BackgroundColor = Color.FromArgb("#3B2523"),
                    Stroke = Color.FromArgb("#D4A857"),
                    StrokeThickness = 2,
                    Padding = new Thickness(15),
                    StrokeShape = new RoundRectangle { CornerRadius = 20 },
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Content = alarmsCollection
                };

                var exitButton = new Button //botón para salir de la página
                {
                    Text = "Salir",
                    BackgroundColor = Color.FromArgb("#CFC86D"),
                    TextColor = Color.FromArgb("#3B2523"),
                    CornerRadius = 10,
                    HeightRequest = 45,
                    FontFamily = "Forresten",
                    HorizontalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 10, 0, 20)
                };

                exitButton.Clicked += async (s, e) => //acción al pulsar el botón
                {
                    await Navigation.PopModalAsync(); //cierra la página modal
                };


                var mainGrid = new Grid //grid principal de la página
                {
                    RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Star },   // contenido scrollable
                    new RowDefinition { Height = GridLength.Auto }    // botón salir fijo
                },
                    Padding = new Thickness(20),
                    BackgroundColor = Color.FromArgb("#1A1A1A")
                };

                var contentLayout = new VerticalStackLayout //contenido principal
                {
                    Spacing = 20,
                    Children =
    {
        headerGrid, //encabezado
        listContainer //lista de alarmas
    }
                };


                var scrollContent = new ScrollView// Scroll externo
                {
                    Content = contentLayout //contenido principal
                };


                mainGrid.Children.Add(scrollContent);// Añadir a la fila 0
                Grid.SetRow(scrollContent, 0); //posición en la cuadrícula

                mainGrid.Children.Add(exitButton); // Añadir a la fila 1
                Grid.SetRow(exitButton, 1); //posición en la cuadrícula

                Content = mainGrid; //asigna el contenido a la página
            }
            catch (Exception ex)
            {
                Console.WriteLine("Join Error: " + ex.Message);
                await DisplayAlert("Error", "Error al mostrar las alarmas.", "OK");
                return;
            }
        }

        private async void ModifyOrCreateExercise(Alarm alarm = null) //modifica o crea una alarma
        {
            try
            {
                bool boolNewAlarm = false;
            if (alarm == null) //verivica si es nueva la rutina
            {
                alarm = new Alarm();

                boolNewAlarm = true;
            }
            {
                VerticalStackLayout CreateDay(string text, out CheckBox checkBox, bool isPresed) //crea los checkBox
                {
                    checkBox = new CheckBox
                    {
                        Color = Color.FromArgb("#CFC86D"),
                        HorizontalOptions = LayoutOptions.Center,
                        IsChecked = isPresed
                    };

                    return new VerticalStackLayout
                    {
                        Spacing = 3,
                        HorizontalOptions = LayoutOptions.Center,
                        Children =
                            {
                                new Label
                                {
                                    Text = text,
                                    TextColor = Color.FromArgb("#CFC86D"),
                                    FontFamily = "ComfortaaBold",
                                    HorizontalOptions = LayoutOptions.Center
                                },
                                checkBox
                            }
                    };
                }
                List<CheckBox> dayCheckboxes = new List<CheckBox>(); //lista para almacenar los checkboxes de los días
                CheckBox cbL, cbM, cbX, cbJ, cbV, cbS, cbD;

                var daysLayout = new HorizontalStackLayout
                {
                    Children =
                        {
                            CreateDay("L", out cbL,alarm.Monday),
                            CreateDay("M", out cbM, alarm.Tuesday),
                            CreateDay("X", out cbX, alarm.Wednesday),
                            CreateDay("J", out cbJ, alarm.Thursday),
                            CreateDay("V", out cbV, alarm.Friday),
                            CreateDay("S", out cbS, alarm.Saturday),
                            CreateDay("D", out cbD, alarm.Sunday)
                        }
                };

                dayCheckboxes.AddRange(new[] { cbL, cbM, cbX, cbJ, cbV, cbS, cbD });

                Entry entryName = new Entry //entrada del nombre de la alarma
                {
                    Text = alarm.Name,
                    Placeholder = "Ejemplo: Despertar",
                    FontFamily = "Comfortaa",
                    TextColor = Color.FromArgb("#E9C68A"),
                    PlaceholderColor = Color.FromArgb("#C49362")
                };

                TimePicker timePicker = new TimePicker //selector de la hora
                {
                    TextColor = Color.FromArgb("#E9C68A"),
                    FontFamily = "Comfortaa"
                };
                if (!boolNewAlarm)
                {
                    timePicker.Time = new TimeSpan(alarm.Hour, alarm.Minute, 0); //muestra la hora
                }
                {
                    timePicker.Time = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0); //inicializa con la hora actual
                }

                    var scrollView = new ScrollView //contenido scrollable
                    {
                        Content = new VerticalStackLayout //contenido vertical
                        {
                            Padding = 20,
                            Spacing = 20,

                            Children =
        {
            new Label //título del modal
            {
                Text = "Nueva alarma",
                FontSize = 28,
                FontFamily = "EatMeAlive",
                TextColor = Color.FromArgb("#C77B30"),
                HorizontalOptions = LayoutOptions.Start
            },

            new Border //border que contiene el formulario
            {
                BackgroundColor = Color.FromArgb("#3B2523"),
                Stroke = Color.FromArgb("#D4A857"),
                StrokeThickness = 2,
                Padding = 15,
                StrokeShape = new RoundRectangle { CornerRadius = 20 },

                Content = new VerticalStackLayout
                {
                    Spacing = 15,
                    Children =
                    {
                        new Label //etiqueta del nombre
                        {
                            Text = "Nombre",
                            FontSize = 18,
                            FontFamily = "ComfortaaBold",
                            TextColor = Color.FromArgb("#CFC86D")
                        },

                        entryName,

                        new Label //etiqueta de la hora
                        {
                            Text = "Hora",
                            FontSize = 18,
                            FontFamily = "ComfortaaBold",
                            TextColor = Color.FromArgb("#CFC86D")
                        },

                        timePicker, //selector de la hora
                        daysLayout
                    }
                }
            }
        }
                        }
                    };

                    var buttonBar = new HorizontalStackLayout //barra de botones inferior
                    {
                        Spacing = 10,
                        Padding = 20,
                        BackgroundColor = Color.FromArgb("#1A1A1A"),
                        Children =
                        {
                            new Button //botón para cerrar el modal
                            {
                                Text = "Cerrar",
                                BackgroundColor = Color.FromArgb("#CFC86D"),
                                TextColor = Color.FromArgb("#3B2523"),
                                CornerRadius = 10,
                                HeightRequest = 45,
                                FontFamily = "Forresten",
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                Command = new Command(async () =>
                                {
                                    await Navigation.PopModalAsync();
                                })
                            },

                            new Button //botón para crear la alarma
                            {
                                Text = "Crear",
                                BackgroundColor = Color.FromArgb("#CFC86D"),
                                TextColor = Color.FromArgb("#3B2523"),
                                CornerRadius = 10,
                                HeightRequest = 45,
                                FontFamily = "Forresten",
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                Command = new Command(async () =>
                                {
                                     var userId = await SecureStorage.GetAsync("user_id");
                                    if(userId == null)
                                    {
                                        await DisplayAlert(
                                                             "Ejercicio isométrico",
                                                             "usuario no identificado.",
                                                             "Aceptar"
                                                         );
                                        return;
                                    }
                                     var user = await _dbService.GetUserById(int.Parse(userId)); //obtiene al usuario
                                     string name = entryName.Text; //obtiene el nombre de la alarma
                                TimeSpan time = timePicker.Time; //obtiene la hora seleccionada

                                bool monday = dayCheckboxes[0].IsChecked; //obtiene los días seleccionados
                                bool tuesday = dayCheckboxes[1].IsChecked;
                                bool wednesday = dayCheckboxes[2].IsChecked;
                                bool thursday = dayCheckboxes[3].IsChecked;
                                bool friday = dayCheckboxes[4].IsChecked;
                                bool saturday = dayCheckboxes[5].IsChecked;
                                bool sunday = dayCheckboxes[6].IsChecked;
                                    if (boolNewAlarm) // si es nuevo
                                    {
                                        Alarm newAlarm = new Alarm //crea la nueva alarma
                                        {
                                            Name = name,
                                            Hour = time.Hours,
                                            Minute = time.Minutes,

                                            Monday = monday,
                                            Tuesday = tuesday,
                                            Wednesday = wednesday,
                                            Thursday = thursday,
                                            Friday = friday,
                                            Saturday = saturday,
                                            Sunday = sunday,
                                            userID = user.UserID
                                        };
                                        await _dbService.Create(newAlarm); //la guarda en la base de datos
                                        AlarmScheduler.ScheduleAlarm(newAlarm); //la programa
                                        await DisplayAlert("Alarma creada", $"Sonará a las {newAlarm.FormattedTime}", "OK"); //confirma la creación
                                        await Navigation.PopModalAsync(); //cierra el modal
                                    }
                                    else
                                    {
                                        alarm.Name = name;
                                        alarm.Hour = time.Hours;
                                        alarm.Minute = time.Minutes;
                                        alarm.Monday = monday;
                                        alarm.Tuesday = tuesday;
                                        alarm.Wednesday = wednesday;
                                        alarm.Thursday = thursday;
                                        alarm.Friday = friday;
                                        alarm.Saturday = saturday;
                                        alarm.Sunday = sunday;
                                        alarm.userID = user.UserID;

                                        LocalNotificationCenter.Current.Cancel(alarm.AlarmID); //cancela la notificación programada
                                        await _dbService.Update(alarm);
                                        await DisplayAlert("Alarma creada", $"Sonará a las {alarm.FormattedTime}", "OK"); //confirma la creación
                                        await Navigation.PopModalAsync(); //cierra el modal
                                    }
                                 })
                            }
                        }
                    };
                    var modalPage = new ContentPage //página modal
                    {
                        BackgroundColor = Color.FromArgb("#1A1A1A"),
                        Content = new Grid
                        {
                            RowDefinitions =
                            {
                                new RowDefinition { Height = GridLength.Star },  // scroll
                                new RowDefinition { Height = GridLength.Auto }   // botones
                            },
                            Children =
                            {
                                scrollView, //contenido scrollable
                                buttonBar //barra de botones
                            }
                        }
                    };

                    Grid.SetRow(scrollView, 0); //posición en la cuadrícula
                    Grid.SetRow(buttonBar, 1);

                    await Navigation.PushModalAsync(modalPage);

                
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Join Error: " + ex.Message);
                await DisplayAlert("Error", "Error al mostrar las alarmas.", "OK");
                return;
            }

        }
    }
     
}