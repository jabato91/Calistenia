using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;
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
        private readonly User? _user;
        private readonly ListUsersViewModel? _viewModel;
        public UserDetailPage(User? user, DbService dbService, userTypeEnum userType = userTypeEnum.nothing, ModeEnum mode = ModeEnum.nothing, ListUsersViewModel? viewModel = null)
        {
            _dbService = dbService;
            _userType = userType;
            _mode = mode;
            _user = user;
            _viewModel = viewModel;
            BuildUI();
        }

        private void BuildUI()//elección de la UI según el modo
        {
            switch (_mode)
            {
                case ModeEnum.create:

                    break;

                case ModeEnum.Edit:
                    BuildEditUI();
                    break;

                case ModeEnum.View:
                    BuildViewUI();
                    break;
                case ModeEnum.filter:

                    break;
            }
        }
        private async void BuildViewUI()//Muestra los detalles del usuario
        {
            var userId = await SecureStorage.GetAsync("user_id"); //obtiene el id del usuario almacenado de forma segura
            if (string.IsNullOrEmpty(userId)) //si no existe, muestra un error
            {
                await DisplayAlert("Error", "No se ha encontrado el ID de usuario en el almacenamiento seguro.", "OK");
                return;
            }

            var user = await _dbService.GetUserById(int.Parse(userId));//obtiene los detalles del usuario desde la base de datos

            BackgroundColor = new Color(0, 0, 0, 0.5f);
            View CreateField(string title, string value) //método para crear campos de visualización
            {
                return new VerticalStackLayout
                {
                    Spacing = 2,
                    Children =
            {
                new Label//título del campo
                {
                    Text = title,
                    FontSize = 12,
                    TextColor = Color.FromArgb("#856D54"),
                    FontAttributes = FontAttributes.Bold,
                    FontFamily = "ComfortaaBold"
                },
                new Label//valor del campo
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

            var nameField = CreateField("Nombre", user.Name);//crea el campo de nombre
            var surnameField = CreateField("Apellidos", user.surNames);//crea el campo de apellidos
            var emailField = CreateField("Email", user.Email);//crea el campo de email
            var typeField = CreateField("Tipo de usuario", user.userType.ToString());//crea el campo de tipo de usuario

            // ---------- Botón cancelar ----------
            var cancelButton = new Button//botón para salir de la vista
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



            var modalPage = new ContentPage//crea la página modal para mostrar los detalles del usuario
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
        private void BuildEditUI()//Permite editar los detalles del usuario
        {
            
            Entry CreateEntry(string text, string placeholder)//método para crear campos de entrada
            {
                return new Entry//devuelve una entrada con el texto y el marcador de posición especificados
                {
                    Text = text,
                    Placeholder = placeholder,
                    TextColor = Color.FromArgb("#C49362"),
                    BackgroundColor = Color.FromArgb("#3B2523"),
                    PlaceholderColor = Color.FromArgb("#8F6A50"),
                    HorizontalOptions = LayoutOptions.Fill,
                    FontFamily = "ComfortaaBold"
                };
            }

            var nameEntry = CreateEntry(_user?.Name, "Nombre");//crea el campo de entrada para el nombre
            var firstSurnameEntry = CreateEntry(_user?.FirstSurname, "Primer apellido");//crea el campo de entrada para el primer apellido
            var secondSurnameEntry = CreateEntry(_user?.SecondSurname, "Segundo apellido");//crea el campo de entrada para el segundo apellido
            var phoneEntry = CreateEntry(_user?.Phone, "Teléfono");//crea el campo de entrada para el teléfono
            phoneEntry.Keyboard = Keyboard.Telephone;//configura el teclado para entrada telefónica

            var typeUserPicker = new Picker //crea el selector para el tipo de usuario
            {
                Title = "Tipo de Usuario",
                TitleColor = Color.FromArgb("#C49362"),
                ItemsSource = enumExtension.userTypeTranslations.Values.ToList(),
                SelectedItem = enumExtension.userTypeTranslations[_user.userType],
                TextColor = Color.FromArgb("#C49362"),
                BackgroundColor = Color.FromArgb("#3B2523"),
                HorizontalOptions = LayoutOptions.Fill,
                FontFamily = "ComfortaaBold"
            };

            var saveButton = new Button//botón para guardar los cambios
            {
                Text = "Guardar",
                BackgroundColor = Color.FromArgb("ffd700"),
                TextColor = Colors.White,
                CornerRadius = 3,
                FontFamily = "ComfortaaBold",
                WidthRequest = 130,
                Padding = new Thickness(10, 6),
                HorizontalOptions = LayoutOptions.Start
            };

            saveButton.Command = new Command(async () => //comando para guardar los cambios y actualizar la base de datos
            {
                try
                {
                    _user.Name = nameEntry.Text;
                    _user.FirstSurname = firstSurnameEntry.Text;
                    _user.SecondSurname = secondSurnameEntry.Text;
                    _user.Phone = phoneEntry.Text;

                    foreach (var kvp in enumExtension.userTypeTranslations)//actualiza el tipo de usuario basado en la selección del picker
                    {
                        if (kvp.Value == (string)typeUserPicker.SelectedItem)//compara el valor seleccionado con los valores del diccionario
                        {
                            _user.userType = kvp.Key;//asigna el tipo de usuario correspondiente
                            break;
                        }
                    }

                    await _dbService.Update(_user); //actualiza el usuario en la base de datos
                    _viewModel?.UpdateFilteredUsers();//actualiza la lista de usuarios en la vista principal si es necesario
                    await DisplayAlert("Éxito", "Usuario actualizado correctamente", "OK");//muestra un mensaje de éxito
                    await Navigation.PopModalAsync();//cierra la página modal
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.Message, "OK");
                }
            });

           
            var cancelButton = new Button//botón para cancelar la edición
            {
                Text = "Cancelar",
                BackgroundColor = Color.FromArgb("ffd700"),
                TextColor = Colors.White,
                CornerRadius = 3,
                FontFamily = "ComfortaaBold",
                WidthRequest = 100,
                Padding = new Thickness(10, 6),
                HorizontalOptions = LayoutOptions.Start,
                Command = new Command(async () => await Navigation.PopModalAsync())
            };
            var modalPage = new ContentPage//crea la página modal para editar los detalles del usuario
            {
                BackgroundColor = Color.FromRgba(0, 0, 0, 0.6),
                Content = new Frame
                {
                    BackgroundColor = Color.FromArgb("#2E1E1B"),
                    CornerRadius = 20,
                    Margin = 1,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    MaximumWidthRequest = 500,

                    Content = new VerticalStackLayout
                    {
                        Padding = 10,
                        Spacing = 10,

                        Children =
                {
                    new Label
                    {
                        Text = "Editar usuario",
                        FontSize = 24,
                        TextColor = Color.FromArgb("#C49362"),
                        HorizontalTextAlignment = TextAlignment.Center,
                        FontFamily = "EatMeAlive"
                    },

                    nameEntry,
                    firstSurnameEntry,
                    secondSurnameEntry,
                    phoneEntry,
                    typeUserPicker,

                    new HorizontalStackLayout
                    {
                        Spacing = 12,
                        Children =
                        {
                            saveButton,
                            cancelButton
                        }
                    }
                }
                    }
                }
            };

            Content = modalPage.Content;//establece el contenido de la página
            BackgroundColor = modalPage.BackgroundColor;//establece el color de fondo de la página
        }
    }
}
