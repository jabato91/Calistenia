
using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Pages.Detail;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;

namespace ProyectoFinDeCurso.Pages.Main{

    public partial class exercisePage : ContentPage
    {
        private readonly DbService _dbService;
        private readonly ExerciseFilterViewModel _filter;
        private readonly userTypeEnum _userType;
        public bool IsAdminMode => _userType == userTypeEnum.admin;
        public exercisePage(DbService dbService, userTypeEnum userType)
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ejercicios ERROR: " + ex.Message);
                throw;
            }

            _dbService = dbService;
            _userType = userType;
            _filter = new ExerciseFilterViewModel(_dbService,_userType);

            BindingContext = _filter;

            NavigationPage.SetTitleView(this, BuildTitleView());
            
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _filter.LoadExercisesAsync();
        }
        public bool ShouldReload { get; set; }
        private async void OnExerciseTapped(object sender, EventArgs e)
        {
            try
            {
                if ((sender as Border)?.BindingContext is Exercise selectedExercise)
                {
                    await Navigation.PushModalAsync(
                        new ExerciseDetailPage(_dbService, _filter, selectedExercise, ModeEnum.View)
                    );
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un error al abrir el ejercicio:\n{ex.Message}", "OK");
            }
        }

        private async void eliminateExercise(object sender, EventArgs e)
        {
            if ((sender as ImageButton)?.BindingContext is not Exercise exercise)
                return;

            bool confirm = await DisplayAlert(
                "Confirmar",
                $"¿Seguro deseas eliminar '{exercise.name}'?",
                "Sí",
                "No"
            );

            if (!confirm) return;

            await _dbService.DeleteExerciseById(exercise.execiseID);

            await _filter.LoadExercisesAsync();
        }
        private async void modifyExercise(object sender, EventArgs e)
        {
            if ((sender as ImageButton)?.BindingContext is not Exercise selectedExercise)
                return;

            await Navigation.PushModalAsync(
                new ExerciseDetailPage(_dbService, _filter, selectedExercise, ModeEnum.Edit)
            );
        }

        private async void createExercise(object sender, TappedEventArgs e)
        {
            await Navigation.PushModalAsync(
                new ExerciseDetailPage(_dbService, _filter, null, ModeEnum.create)
            );
        }

        private async void filterExercises(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(
                new ExerciseDetailPage(filter: _filter, mode: ModeEnum.filter)
            );
        }

        private async void profile(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(
               new UserDetailPage(null,_dbService, _userType, ModeEnum.View)
           );
        }
        private View BuildTitleView()
        {
            var grid = new Grid
            {
                Padding = new Thickness(10, 5),
                VerticalOptions = LayoutOptions.Center,
                ColumnDefinitions =
        {
            new ColumnDefinition { Width = GridLength.Auto },    // (0) Izquierda
            new ColumnDefinition { Width = GridLength.Star },    // (1) Centro
            new ColumnDefinition { Width = GridLength.Auto }     // (2) Derecha
        }
            };

            // ----- TÍTULO -----
            var titleLabel = new Label
            {
                Text = "Ejercicios",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 22,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#C77B30"),
                FontFamily = "Forresten",
                Margin = new Thickness(0, 0, 0, 0)
            };
            Grid.SetColumn(titleLabel, 1);
            grid.Children.Add(titleLabel);

            // ----- BOTÓN DE PERFIL -----
            var profileButton = new ImageButton
            {
                Source = "icono_predeterminado.png",
                WidthRequest = 35,
                HeightRequest = 35,
                BackgroundColor = Colors.Transparent,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
                Margin = new Thickness(0, 0, 5, 0)
            };
            Grid.SetColumn(profileButton, 2);

            // Evento de perfil
            profileButton.Clicked += profile;

            grid.Children.Add(profileButton);

            return grid;
        }
    }
}