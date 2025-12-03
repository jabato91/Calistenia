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
            _dbService = dbService;
            _day = day;
            _mode = mode;
            BuildUI();
        }

        private void BuildUI() // Construye la interfaz de ejercicios según el modo
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

            }
        }
        private async Task BuildViewUI() //muestra el contenido del día seleccionado
        {
            var userId = await SecureStorage.GetAsync("user_id"); //obtiene la id del usuario
            if (userId == null) { //verifica si no es nulo
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


                    // ====== FILA DESCRIPCIÓN ======

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

        private async Task BuildCreateAlarmUI() //build para crear los avisos al usuario
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
            var filterAlarm = alarmas.Where(x => x.userID == int.Parse(userId)).ToList();
            // ==== TÍTULO ====
            var titleLabel = new Label
            {
                Text = "Alarmas",
                FontSize = 28,
                FontFamily = "EatMeAlive",
                TextColor = Color.FromArgb("#C77B30"),
                HorizontalOptions = LayoutOptions.Start
            };
            VerticalStackLayout CreateDay(string text)
            {
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
            new CheckBox
            {
                Color = Color.FromArgb("#CFC86D"),
                HorizontalOptions = LayoutOptions.Center
            }
        }
                };
            }
            // ==== BOTÓN "+" ====
            var addButton = new Button
            {
                Text = "+",
                FontSize = 28,
                BackgroundColor = Color.FromArgb("#3B2523"),
                TextColor = Color.FromArgb("#CFC86D"),
                FontFamily = "Forresten",
                CornerRadius = 10,
                WidthRequest = 50,
                HeightRequest = 50,
                 Command = new Command(async () =>
                 {
                     // ---- CONTROLES QUE QUEREMOS RECUPERAR ----

                     Entry entryName = new Entry
                     {
                         Placeholder = "Ejemplo: Despertar",
                         FontFamily = "Comfortaa",
                         TextColor = Color.FromArgb("#E9C68A"),
                         PlaceholderColor = Color.FromArgb("#C49362")
                     };

                     TimePicker timePicker = new TimePicker
                     {
                         TextColor = Color.FromArgb("#E9C68A"),
                         FontFamily = "Comfortaa"
                     };
                     timePicker.Time = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
                     // Lista donde guardaremos TODOS los checkboxes
                     List<CheckBox> dayCheckboxes = new List<CheckBox>();

                     // Crea cada día con su checkbox
                     VerticalStackLayout CreateDay(string text)
                     {
                         var check = new CheckBox
                         {
                             Color = Color.FromArgb("#CFC86D"),
                             HorizontalOptions = LayoutOptions.Center
                         };

                         dayCheckboxes.Add(check);

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
                                check
                            }
                         };
                     }
                     var scrollView = new ScrollView
                     {
                         Content = new VerticalStackLayout
                         {
                             Padding = 20,
                             Spacing = 20,

                             Children =
        {
            new Label
            {
                Text = "Nueva alarma",
                FontSize = 28,
                FontFamily = "EatMeAlive",
                TextColor = Color.FromArgb("#C77B30"),
                HorizontalOptions = LayoutOptions.Start
            },

            new Border
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
                        new Label
                        {
                            Text = "Nombre",
                            FontSize = 18,
                            FontFamily = "ComfortaaBold",
                            TextColor = Color.FromArgb("#CFC86D")
                        },

                        entryName,

                        new Label
                        {
                            Text = "Hora",
                            FontSize = 18,
                            FontFamily = "ComfortaaBold",
                            TextColor = Color.FromArgb("#CFC86D")
                        },

                        timePicker,

                        new Label
                        {
                            Text = "Repetir",
                            FontSize = 18,
                            FontFamily = "ComfortaaBold",
                            TextColor = Color.FromArgb("#CFC86D")
                        },

                        new HorizontalStackLayout
                        {
                            Spacing = 3,
                            Children =
                            {
                                CreateDay("L"),
                                CreateDay("M"),
                                CreateDay("X"),
                                CreateDay("J"),
                                CreateDay("V"),
                                CreateDay("S"),
                                CreateDay("D")
                            }
                        }
                    }
                }
            }
        }
                         }
                     };

                     var buttonBar = new HorizontalStackLayout
                     {
                         Spacing = 10,
                         Padding = 20,
                         BackgroundColor = Color.FromArgb("#1A1A1A"),
                         Children =
                        {
                            new Button
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

                            new Button
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
                                     var user = await _dbService.GetUserById(int.Parse(userId));
                                     string name = entryName.Text;
                                TimeSpan time = timePicker.Time;

                                bool monday = dayCheckboxes[0].IsChecked;
                                bool tuesday = dayCheckboxes[1].IsChecked;
                                bool wednesday = dayCheckboxes[2].IsChecked;
                                bool thursday = dayCheckboxes[3].IsChecked;
                                bool friday = dayCheckboxes[4].IsChecked;
                                bool saturday = dayCheckboxes[5].IsChecked;
                                bool sunday = dayCheckboxes[6].IsChecked;

                                Alarm newAlarm = new Alarm
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


                                    // 👇 Si userID es bool, esto es lo único que puedo poner
                                    userID = user.UserID
                                };
                                    await _dbService.Create(newAlarm);
                                    AlarmScheduler.ScheduleAlarm(newAlarm);
                                    await DisplayAlert("Alarma creada", $"Sonará a las {newAlarm.FormattedTime}", "OK");
                                    await Navigation.PopModalAsync();

                                await Navigation.PopModalAsync();
                                })
                            }
                        }
                     };
                     var modalPage = new ContentPage
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
                                scrollView,
                                buttonBar
                            }
                         }
                     };

                     // ASIGNAR FILAS
                     Grid.SetRow(scrollView, 0);
                     Grid.SetRow(buttonBar, 1);

                     await Navigation.PushModalAsync(modalPage);

                 })
            };


            // ==== HEADER ====
            var headerGrid = new Grid
            {
                ColumnDefinitions =
        {
            new ColumnDefinition { Width = GridLength.Star },
            new ColumnDefinition { Width = 50 }
        }
            };

            headerGrid.Children.Add(titleLabel);
            Grid.SetColumn(titleLabel, 0);

            headerGrid.Children.Add(addButton);
            Grid.SetColumn(addButton, 1);

            // ==== COLLECTIONVIEW ====
            var alarmsCollection = new CollectionView
            {
                ItemsSource = filterAlarm,
                SelectionMode = SelectionMode.None,
                VerticalOptions = LayoutOptions.FillAndExpand,

                ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical)
                {
                    ItemSpacing = 25
                },

                EmptyView = new Label
                {
                    Text = "No hay alarmas aún",
                    TextColor = Color.FromArgb("#CFC86D"),
                    FontSize = 18,
                    FontFamily = "ComfortaaBold",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
            };

            alarmsCollection.ItemTemplate = new DataTemplate(() =>
            {
                // ===== LABEL NOMBRE =====
                var nameLabel = new Label
                {
                    FontSize = 18,
                    FontFamily = "ComfortaaBold",
                    TextColor = Color.FromArgb("#CFC86D")
                };
                nameLabel.SetBinding(Label.TextProperty, "Name");

                // ===== LABEL HORA =====
                var hourLabel = new Label
                {
                    FontSize = 16,
                    FontFamily = "Comfortaa",
                    TextColor = Color.FromArgb("#E9C68A")
                };
                hourLabel.SetBinding(Label.TextProperty, "FormattedTime");

                // ===== DÍAS =====
                var weekLayout = new HorizontalStackLayout
                {
                    Spacing = 12,
                    HorizontalOptions = LayoutOptions.Start
                };

                VerticalStackLayout CreateDay(string text, string binding)
                {
                    var check = new CheckBox
                    {
                        Color = Color.FromArgb("#CFC86D"),
                        IsEnabled = false,
                        Scale = 0.9,
                        HorizontalOptions = LayoutOptions.Center
                    };
                    check.SetBinding(CheckBox.IsCheckedProperty, binding);

                    return new VerticalStackLayout
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

                weekLayout.Children.Add(CreateDay("L", "Monday"));
                weekLayout.Children.Add(CreateDay("M", "Tuesday"));
                weekLayout.Children.Add(CreateDay("X", "Wednesday"));
                weekLayout.Children.Add(CreateDay("J", "Thursday"));
                weekLayout.Children.Add(CreateDay("V", "Friday"));
                weekLayout.Children.Add(CreateDay("S", "Saturday"));
                weekLayout.Children.Add(CreateDay("D", "Sunday"));


                // ===== BOTÓN EDITAR =====
                var editButton = new ImageButton
                {
                    Source = "editar.png",
                    WidthRequest = 30,
                    HeightRequest = 30,
                    BackgroundColor = Colors.Transparent,
                    HorizontalOptions = LayoutOptions.End,
                    Margin = new Thickness(0, 0, 8, 0)
                };
                editButton.SetBinding(ImageButton.CommandParameterProperty, ".");
                

                // ===== BOTÓN ELIMINAR =====
                var deleteButton = new ImageButton
                {
                    Source = "papelera.png",
                    WidthRequest = 30,
                    HeightRequest = 30,
                    BackgroundColor = Colors.Transparent,
                    HorizontalOptions = LayoutOptions.End,
                    
                };
                deleteButton.SetBinding(ImageButton.CommandParameterProperty, ".");
                deleteButton.Command = new Command<Alarm>(async (alarm) =>
                {
                    bool confirm = await Application.Current.MainPage.DisplayAlert(
                        "Eliminar alarma",
                        $"¿Eliminar la alarma \"{alarm.Name}\"?",
                        "Sí",
                        "No"
                    );

                    if (confirm)
                    {
                        LocalNotificationCenter.Current.Cancel(alarm.AlarmID);
                        await _dbService.Delete(alarm);
                    }
                });


                // ===== GRID PARA ALINEAR TODO =====
                var grid = new Grid
                {
                    ColumnDefinitions =
        {
            new ColumnDefinition { Width = GridLength.Star },  // texto y días
            new ColumnDefinition { Width = GridLength.Auto },  // editar
            new ColumnDefinition { Width = GridLength.Auto }   // eliminar
        }
                };

                // SUBSTACK IZQUIERDO (nombre, hora, días)
                var dataStack = new VerticalStackLayout
                {
                    Spacing = 10,
                    Children = { nameLabel, hourLabel, weekLayout }
                };

                // AGREGAR A CELDAS
                Grid.SetColumn(dataStack, 0);
                grid.Children.Add(dataStack);

                Grid.SetColumn(editButton, 1);
                grid.Children.Add(editButton);

                Grid.SetColumn(deleteButton, 2);
                grid.Children.Add(deleteButton);


                // ===== BORDER EXTERNO =====
                return new Border
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

            // ==== CONTENEDOR DE LISTA ====
            var listContainer = new Border
            {
                BackgroundColor = Color.FromArgb("#3B2523"),
                Stroke = Color.FromArgb("#D4A857"),
                StrokeThickness = 2,
                Padding = new Thickness(15),
                StrokeShape = new RoundRectangle { CornerRadius = 20 },
                VerticalOptions = LayoutOptions.FillAndExpand,
                Content = alarmsCollection
            };

            // ==== BOTÓN SALIR (FIJO ABAJO) ====
            var exitButton = new Button
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

            exitButton.Clicked += async (s, e) =>
            {
                await Navigation.PopModalAsync();
            };


            var mainGrid = new Grid
            {
                RowDefinitions =
    {
        new RowDefinition { Height = GridLength.Star },   // contenido scrollable
        new RowDefinition { Height = GridLength.Auto }    // botón salir fijo
    },
                Padding = new Thickness(20),
                BackgroundColor = Color.FromArgb("#1A1A1A")
            };

            // Fila 0 → contenido dentro de ScrollView
            var contentLayout = new VerticalStackLayout
            {
                Spacing = 20,
                Children =
    {
        headerGrid,
        listContainer
    }
            };

            // Scroll externo
            var scrollContent = new ScrollView
            {
                Content = contentLayout
            };

            // Añadir a la fila 0
            mainGrid.Children.Add(scrollContent);
            Grid.SetRow(scrollContent, 0);

            // Fila 1 → botón salir fijo
            mainGrid.Children.Add(exitButton);
            Grid.SetRow(exitButton, 1);

            // Establecer contenido final
            Content = mainGrid;
        }

        public void ScheduleAlarm(Alarm alarm)
        {
            DateTime now = DateTime.Now;
            DateTime nextTrigger = FindNextTriggerDay(alarm, now);

            var request = new NotificationRequest
            {
                NotificationId = alarm.AlarmID,
                Title = "CalisSAPP",
                Description = "Es Hora de Entrenar!!!!",

                Android =
        {
            ChannelId = "alarm_channel",
            AutoCancel = false
        },

                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = nextTrigger,
                    RepeatType = NotificationRepeat.Weekly
                }
            };

            LocalNotificationCenter.Current.Show(request);
        }
        
        DateTime FindNextTriggerDay(Alarm alarm, DateTime now)
        {
            bool[] days = new bool[]
            {
                alarm.Monday,
                alarm.Tuesday,
                alarm.Wednesday,
                alarm.Thursday,
                alarm.Friday,
                alarm.Saturday,
                alarm.Sunday
            };

            int todayIndex = ConvertToLunesIndex(now.DayOfWeek);

            for (int offset = 0; offset < 7; offset++)
            {
                int checkIndex = (todayIndex + offset) % 7; 

                if (days[checkIndex])
                {
                    DateTime candidate = now.Date
                        .AddDays(offset)
                        .AddHours(alarm.Hour)
                        .AddMinutes(alarm.Minute);

                    if (candidate > now)
                        return candidate;
                }
            }

            return now.AddMinutes(1);
        }
        int ConvertToLunesIndex(DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Monday => 0,
                DayOfWeek.Tuesday => 1,
                DayOfWeek.Wednesday => 2,
                DayOfWeek.Thursday => 3,
                DayOfWeek.Friday => 4,
                DayOfWeek.Saturday => 5,
                DayOfWeek.Sunday => 6,
                _ => 0
            };
        }
       
    }
}