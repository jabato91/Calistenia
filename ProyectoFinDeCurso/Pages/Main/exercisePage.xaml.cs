
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
                InitializeComponent(); // Inicializa los componentes de la página
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ejercicios ERROR: " + ex.Message);
                throw;
            }

            _dbService = dbService;
            _userType = userType;
            _filter = new ExerciseFilterViewModel(_dbService,_userType); // ViewModel para filtrar ejercicios
            verificationUserType(_userType); // Verifica el tipo de usuario para mostrar/ocultar elementos
            BindingContext = _filter; // enlaza el ViewModel al contexto de datos de la página

            NavigationPage.SetTitleView(this, BuildTitleView()); // Configura la vista del título personalizado

        }
        protected override async void OnAppearing() // Método que se llama cuando la página aparece
        {
            base.OnAppearing(); // Llama al método base OnAppearing
            await _filter.LoadExercisesAsync(); // Carga los ejercicios utilizando el ViewModel
        }
        private async void OnExerciseTapped(object sender, EventArgs e) // Maneja el evento de toque en un ejercicio
        {
            try
            {
                if ((sender as Border)?.BindingContext is Exercise selectedExercise) // Obtiene el ejercicio seleccionado
                {
                    await Navigation.PushModalAsync(
                        new ExerciseDetailPage(_dbService, _filter, selectedExercise, ModeEnum.View)
                    ); // Navega a la página de detalles del ejercicio en modo vista
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un error al abrir el ejercicio:\n{ex.Message}", "OK");
            }
        }

        private async void eliminateExercise(object sender, EventArgs e) // Maneja el evento de eliminación de un ejercicio
        {
            if ((sender as ImageButton)?.BindingContext is not Exercise exercise) // Obtiene el ejercicio a eliminar
                return;

            bool confirm = await DisplayAlert(
                "Confirmar",
                $"¿Seguro deseas eliminar '{exercise.name}'?",
                "Sí",
                "No"
            ); // Solicita confirmación al usuario

            if (!confirm) return; // Si no se confirma, sale del método

            await _dbService.DeleteExerciseById(exercise.execiseID); // Elimina el ejercicio de la base de datos

            await _filter.LoadExercisesAsync(); // Recarga la lista de ejercicios
        }
        private async void modifyExercise(object sender, EventArgs e) // Maneja el evento de modificación de un ejercicio
        {
            if ((sender as ImageButton)?.BindingContext is not Exercise selectedExercise) // Obtiene el ejercicio a modificar
                return;

            await Navigation.PushModalAsync(
                new ExerciseDetailPage(_dbService, _filter, selectedExercise, ModeEnum.Edit)
            ); // Navega a la página de detalles del ejercicio en modo edición
        }

        private async void createExercise(object sender, TappedEventArgs e) // Maneja el evento de creación de un nuevo ejercicio
        {
            await Navigation.PushModalAsync(
                new ExerciseDetailPage(_dbService, _filter, null, ModeEnum.create)
            ); // Navega a la página de detalles del ejercicio en modo creación
        }

        private async void filterExercises(object sender, EventArgs e) // Maneja el evento de filtrado de ejercicios
        {
            await Navigation.PushModalAsync(
                new ExerciseDetailPage(filter: _filter, mode: ModeEnum.filter)
            ); // Navega a la página de detalles del ejercicio en modo filtrado
        }

        private async void profile(object sender, EventArgs e) // Maneja el evento de perfil de usuario
        {
            await Navigation.PushModalAsync(
               new UserDetailPage(null,_dbService, _userType, ModeEnum.View)
           ); // Navega a la página de detalles del usuario en modo vista
        }
        private View BuildTitleView() // Construye la vista personalizada del título
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

            var titleLabel = new Label // titulo
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

            var profileButton = new ImageButton // boton perfil
            {
                Source = "icono_predeterminado.png",
                WidthRequest = 35,
                HeightRequest = 35,
                BackgroundColor = Colors.Transparent,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
                Margin = new Thickness(0, 0, 5, 0)
            };
            Grid.SetColumn(profileButton, 2); // columna derecha

            // Evento de perfil
            profileButton.Clicked += profile;

            grid.Children.Add(profileButton);

            return grid;
        }
        public void verificationUserType(userTypeEnum userType) // Verifica el tipo de usuario para mostrar/ocultar elementos
        {
            if (userType != userTypeEnum.admin) // Si no es admin, oculta el botón de crear
                Create.IsVisible = false; // Oculta el botón de crear ejercicio
        }
    }
}