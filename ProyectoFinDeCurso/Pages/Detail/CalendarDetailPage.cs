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
        public CalendarDetailPage(DbService dbService, string day, ModeEnum mode)
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
                new Label
                {
                    Text = "Detalles",
                    FontSize = 26,
                    TextColor = Color.FromArgb("#C77B30"),
                    FontFamily = "EatMeAlive",
                    HorizontalOptions = LayoutOptions.Center
                },

                new Label
                {
                    Text = _day,
                    FontSize = 18,
                    TextColor = Color.FromArgb("#C49362"),
                    FontFamily = "ComfortaaBold",
                    HorizontalOptions = LayoutOptions.Center
                },

                routinesCollection,

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
    }
}