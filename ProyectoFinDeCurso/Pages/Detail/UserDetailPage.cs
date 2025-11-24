using Microsoft.Maui.Controls.Shapes;
using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinDeCurso.Pages.Detail
{
    public class UserDetailPage : ContentPage
    {
        private readonly ModeEnum _mode;
        private readonly userTypeEnum _userType;
        private readonly DbService _dbService;
        public UserDetailPage(DbService dbService, userTypeEnum userType, ModeEnum mode) {
            _dbService = dbService;
            _userType = userType;
            _mode = mode;

            BuildUI();
        }

        private void BuildUI()
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

        private async void BuildViewUI()
        {
            var userId = await SecureStorage.GetAsync("user_id");
            if (string.IsNullOrEmpty(userId))
            {
                await DisplayAlert("Error", "No se ha encontrado el ID de usuario en el almacenamiento seguro.", "OK");
                return;
            }

            var user = await _dbService.GetUserById(int.Parse(userId));

            BackgroundColor = new Color(0, 0, 0, 0.5f); 
            View CreateField(string title, string value)
            {
                return new VerticalStackLayout
                {
                    Spacing = 2,
                    Children =
            {
                new Label
                {
                    Text = title,
                    FontSize = 12,
                    TextColor = Color.FromArgb("#856D54"),
                    FontAttributes = FontAttributes.Bold,
                    FontFamily = "ComfortaaBold"
                },
                new Label
                {
                    Text = value,
                    FontSize = 14,
                    TextColor = Color.FromArgb("#C49362"),
                    BackgroundColor = Color.FromArgb("#3B2523"),
                    FontFamily = "ComfortaaBold",
                }
            }
                };
            }

            var nameField = CreateField("Nombre", user.Name);
            var surnameField = CreateField("Apellidos", user.surNames);
            var emailField = CreateField("Email", user.Email);
            var typeField = CreateField("Tipo de usuario", user.userType.ToString());

            // ---------- Botón cancelar ----------
            var cancelButton = new Button
            {
                Text = "Salir",
                BackgroundColor = Color.FromArgb("ffd700"),
                TextColor = Colors.White,
                CornerRadius = 3,
                FontFamily = "ComfortaaBold",
                FontSize = 16,
                Padding = new Thickness(10, 6),
                HorizontalOptions = LayoutOptions.Fill,
                Command = new Command(async () => await Navigation.PopModalAsync())
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
                        Spacing = 20,
                        Children =
            {
                nameField,
                surnameField,
                emailField,
                typeField,
                cancelButton
            }
                    }
                }
            };


            Content = modalPage.Content;
            BackgroundColor = modalPage.BackgroundColor;
        }
    }
}
