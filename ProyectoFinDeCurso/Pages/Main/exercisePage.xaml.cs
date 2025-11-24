

using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Pages.Detail;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;
using IOPath = System.IO.Path;

namespace ProyectoFinDeCurso.Pages.Main{

    public partial class exercisePage : ContentPage
    {
        private readonly DbService _dbService;
        private readonly ExerciseFilterViewModel _filter;
        private readonly userTypeEnum _userType;
        public bool IsAdminMode => _userType == userTypeEnum.admin;
        public exercisePage(DbService dbService, userTypeEnum userType, ExerciseFilterViewModel filter)
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
            _filter = filter;

            BindingContext = _filter;
            if (DeviceInfo.Platform != DevicePlatform.Android)
            {
                _filter.LoadExercisesAsync(forceReload: true);
            }
        }
        public bool ShouldReload { get; set; }

        // Método para cargar desde fuera
        public async Task LoadAtStartup()
        {
            await _filter.LoadExercisesAsync(forceReload: true);
        }
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

            _filter.CachedExercises.Remove(exercise);
            _filter.Exercises.Remove(exercise);

            _filter.UpdateFilteredExercises();
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

        public async Task PreloadAsync()
        {
            await _filter.LoadExercisesAsync(forceReload: true);
        }
        private async void profile(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(
               new UserDetailPage(null,_dbService, _userType, ModeEnum.View)
           );
        }
    }
}