using Microsoft.Maui.Controls.Shapes;
using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProyectoFinDeCurso.Pages.Detail
{
    public class CalendarDetailPage : ContentPage
    {
        private readonly DbService _dbService;
        private readonly string _day;
        private readonly ModeEnum _mode;
        public CalendarDetailPage(DbService dbService, ModeEnum mode, string? day = null)
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
                    
                    break;

                case ModeEnum.Edit:
                    BuildEditAlarmUI();
                    break;

                case ModeEnum.View:
                    BuildViewUI();
                    break;
                case ModeEnum.filter:
                    
                    break;

            }
        }
        private async Task BuildViewUI()
        {
            var userId = await SecureStorage.GetAsync("user_id");
            var user = await _dbService.GetUserById(int.Parse(userId));

            var registerRoutines = await _dbService.GetRegisterLoggingAsync();
            var listRegisterRoutinesByUser = registerRoutines
                .Where(x => x.userID == user.UserID && x.day == _day)
                .ToList();

            var listRoutines = await _dbService.GetRoutinesAsync();

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
                .ToList();

            // COLLECTIONVIEW
            var routinesCollection = new CollectionView
            {
                ItemsSource = routinesData,
                SelectionMode = SelectionMode.None,
                Margin = new Thickness(10, 0, 10, 0),
                HeightRequest = 220,

                // 👇 ESTA ES LA CLAVE PARA DAR ESPACIO REAL
                ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical)
                {
                    ItemSpacing = 25  // 👈 ahora sí se separan los items
                },

                ItemTemplate = new DataTemplate(() =>
                {
                    // ===== ESTILOS GENERALES =====

                    Color titleColor = Color.FromArgb("#D6A77A");  // Dorado más vivo
                    Color valueColor = Color.FromArgb("#F0E6DA");  // Beige claro más legible

                    double titleSize = 15;
                    double valueSize = 17;

                    string titleFont = "ComfortaaBold";
                    string valueFont = "Comfortaa";


                    // ====== FILA NOMBRE ======

                    var nameTitle = new Label
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
                        Opacity = 0.95 // efecto elegante
                    };
                    nameValue.SetBinding(Label.TextProperty, "name");

                    var nameRow = new HorizontalStackLayout
                    {
                        Spacing = 8,
                        Children = { nameTitle, nameValue }
                    };


                    // ====== FILA DESCRIPCIÓN ======

                    var descTitle = new Label
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
                    descValue.SetBinding(Label.TextProperty, "description");

                    var descRow = new HorizontalStackLayout
                    {
                        Spacing = 8,
                        Children = { descTitle, descValue }
                    };


                    // ====== FILA INICIO ======

                    var startTitle = new Label
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
                        TextColor = Color.FromArgb("#E9C68A") // tono dorado para horas
                    };
                    startValue.SetBinding(Label.TextProperty, "startHour");

                    var startRow = new HorizontalStackLayout
                    {
                        Spacing = 8,
                        Children = { startTitle, startValue }
                    };


                    // ====== FILA FIN ======

                    var endTitle = new Label
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
                    endValue.SetBinding(Label.TextProperty, "endHour");

                    var endRow = new HorizontalStackLayout
                    {
                        Spacing = 8,
                        Children = { endTitle, endValue }
                    };


                    // ====== STACK PRINCIPAL ======

                    var stack = new VerticalStackLayout
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


                    // ====== TARJETA ======

                    return new Border
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

                        Content = stack
                    };
                })
            };


            // UI COMPLETA (modal)
            Content = new Border
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
            // === TÍTULO PRINCIPAL ===
            new Label
            {
                Text = "Detalles",
                FontSize = 26,
                TextColor = Color.FromArgb("#C77B30"),
                FontFamily = "EatMeAlive",
                HorizontalOptions = LayoutOptions.Center
            },

            // === FECHA ===
            new Label
            {
                Text = _day,
                FontSize = 18,
                TextColor = Color.FromArgb("#C49362"),
                FontFamily = "ComfortaaBold",
                HorizontalOptions = LayoutOptions.Center
            },

            // === NUEVO TÍTULO ENCIMA DE LA LISTA ===
            new Label
            {
                Text = "Registros del día",
                FontSize = 20,
                FontFamily = "ComfortaaBold",
                TextColor = Color.FromArgb("#D6A77A"),
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 15, 0, 5)
            },

            // === LISTA DE RUTINAS ===
            routinesCollection,
            new Button //botón para empezar el ejercicio
                    {
                        BackgroundColor = Color.FromArgb("#3B2523"),
                        TextColor = Color.FromArgb("#CFC86D"),
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.Fill,

                        FontFamily = "Forresten",
                        Text = "Alarma",
                        Command = new Command(async () => {
                    var alarmaPage = new ContentPage
        {
            BackgroundColor = Color.FromRgba(0, 0, 0, 0.6), // fondo transparente

            Content = new Border
            {
                BackgroundColor = Color.FromArgb("#2E1E1B"),
                StrokeShape = new RoundRectangle { CornerRadius = 18 },
                Stroke = Color.FromArgb("#5A3A35"),
                StrokeThickness = 1,
                Margin = new Thickness(20, 80),
                Padding = new Thickness(20, 25),

                Content = new VerticalStackLayout
                {
                    Spacing = 20,
                    HorizontalOptions = LayoutOptions.Center,

                    Children =
                    {
                        // --- TÍTULO ---
                        new Label
                        {
                            Text = "Alarma",
                            FontSize = 32,
                            FontFamily = "EatMeAlive",
                            HorizontalOptions = LayoutOptions.Center,
                            TextColor = Color.FromArgb("#C77B30"),
                            Margin = new Thickness(0,0,0,5)
                        },

                        // --- SUBTÍTULO ---
                        new Label
                        {
                            Text = "Configura tu recordatorio",
                            FontSize = 17,
                            FontFamily = "ComfortaaBold",
                            TextColor = Color.FromArgb("#C49362"),
                            HorizontalOptions = LayoutOptions.Center
                        },

                        // --- BOTÓN INTERNO DE ACCIÓN ---
                        new Button
                        {
                            BackgroundColor = Color.FromArgb("#3B2523"),
                            TextColor = Color.FromArgb("#CFC86D"),
                            FontFamily = "Forresten",
                            Text = "Activar Alarma",
                            CornerRadius = 12,
                            HeightRequest = 55,
                            WidthRequest = 220,
                            Margin = new Thickness(0,20,0,10),
                            Command = new Command(async () =>
                            {
                                
                            })
                        },

                        // --- BOTÓN DE SALIR ---
                        new Button
                        {
                            Text = "Salir",
                            BackgroundColor = Color.FromArgb("ffd700"),
                            TextColor = Colors.White,
                            CornerRadius = 10,
                            HeightRequest = 45,
                            WidthRequest = 150,
                            HorizontalOptions = LayoutOptions.Center,

                            Command = new Command(async () =>
                            {
                                await Application.Current.MainPage.Navigation.PopModalAsync();
                            })
                        }
                    }
                }
            }
        };

        // ======= MOSTRAR COMO MODAL =========
        await Navigation.PushModalAsync(alarmaPage);
                })
                    },
            // === BOTÓN SALIR ===
            new Button
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

        private async Task BuildEditAlarmUI()
        {
            var alarmas = await _dbService.GetAlarmsAsync();

            // ==== TÍTULO ====
            var titleLabel = new Label
            {
                Text = "Alarmas",
                FontSize = 28,
                FontFamily = "EatMeAlive",
                TextColor = Color.FromArgb("#C77B30"),
                HorizontalOptions = LayoutOptions.Start
            };

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
                HeightRequest = 50
            };

            addButton.Clicked += async (s, e) =>
            {
                await DisplayAlert("Añadir", "Aquí irá la UI para crear una nueva alarma.", "OK");
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
                ItemsSource = alarmas,
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
                },

                ItemTemplate = new DataTemplate(() =>
                {
                    var nameLabel = new Label
                    {
                        FontSize = 18,
                        FontFamily = "ComfortaaBold",
                        TextColor = Color.FromArgb("#CFC86D")
                    };
                    nameLabel.SetBinding(Label.TextProperty, "Name");

                    var hourLabel = new Label
                    {
                        FontSize = 16,
                        FontFamily = "Comfortaa",
                        TextColor = Color.FromArgb("#E9C68A")
                    };
                    hourLabel.SetBinding(Label.TextProperty, "TimeFormatted");

                    var daysLabel = new Label
                    {
                        FontSize = 14,
                        FontFamily = "Comfortaa",
                        TextColor = Color.FromArgb("#C49362")
                    };
                    daysLabel.SetBinding(Label.TextProperty, "DaysFormatted");

                    var stack = new VerticalStackLayout
                    {
                        Spacing = 6,
                        Children = { nameLabel, hourLabel, daysLabel }
                    };

                    return new Border
                    {
                        BackgroundColor = Color.FromArgb("#2E1E1B"),
                        Stroke = Color.FromArgb("#6A463F"),
                        StrokeThickness = 1.5,
                        Margin = new Thickness(0, 10),
                        Padding = new Thickness(15),
                        StrokeShape = new RoundRectangle { CornerRadius = 15 },
                        Content = stack
                    };
                })
            };

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
                BackgroundColor = Color.FromArgb("ffd700"),
                TextColor = Colors.White,
                CornerRadius = 10,
                WidthRequest = 160,
                HeightRequest = 45,
                FontFamily = "Forresten",
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 10, 0, 20)
            };

            exitButton.Clicked += async (s, e) =>
            {
                await Navigation.PopModalAsync();
            };

            // ==== GRID PRINCIPAL (2 FILAS, SEGUNDA PARA EL BOTÓN) ====
            var mainGrid = new Grid
            {
                RowDefinitions =
        {
            new RowDefinition { Height = GridLength.Star },   // contenido
            new RowDefinition { Height = GridLength.Auto }    // botón salir fijo
        },
                Padding = new Thickness(20),
                BackgroundColor = Color.FromArgb("#1A1A1A")
            };

            // Fila 0 → contenido
            var contentLayout = new VerticalStackLayout
            {
                Spacing = 20,
                Children =
        {
            headerGrid,
            listContainer
        }
            };

            mainGrid.Children.Add(contentLayout);
            Grid.SetRow(contentLayout, 0);

            // Fila 1 → botón salir
            mainGrid.Children.Add(exitButton);
            Grid.SetRow(exitButton, 1);

            Content = mainGrid;
        }
    }
}