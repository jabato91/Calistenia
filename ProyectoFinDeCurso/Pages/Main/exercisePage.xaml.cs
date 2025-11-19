

using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Pages.Detail;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;
using IOPath = System.IO.Path;
namespace ProyectoFinDeCurso.Pages.Main;

public partial class exercisePage : ContentPage
{
    private readonly DbService _dbService;
    private readonly ExerciseFilterViewModel _filter;
    private readonly userTypeEnum _userType;
    public bool IsAdminMode => _userType == userTypeEnum.admin;
    public exercisePage(DbService dbService, userTypeEnum userType)
    {
        InitializeComponent();

        _dbService = dbService;
        _userType = userType;

        // crear solo una vez
        _filter = new ExerciseFilterViewModel(_dbService, _userType);
        BindingContext = _filter;

    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!_filter.Initialized)
        {
            await Task.Delay(50); // Deja renderizar la UI
            await _filter.LoadExercisesAsync();
        }
    }
    private async void OnExerciseTapped(object sender, EventArgs e)
    {
        try
        {
            if ((sender as Border)?.BindingContext is Exercise selectedExercise)
            {
                await Navigation.PushModalAsync(
                    new ExerciseDetailPage(_dbService, _filter, selectedExercise, ExerciseMode.View)
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

        // 🔥 Esto actualiza la colección correctamente
        _filter.Exercises.Remove(exercise);

        // No necesitas llamar a OnPropertyChanged para Exercises,
        // ObservableCollection ya notifica automáticamente.
        _filter.UpdateFilteredExercises();
    }
    private async void modifyExercise(object sender, EventArgs e)
    {
        if ((sender as ImageButton)?.BindingContext is not Exercise selectedExercise)
            return;

        await Navigation.PushModalAsync(
            new ExerciseDetailPage(_dbService, _filter, selectedExercise, ExerciseMode.Edit)
        );
    }

    private async void createExercise(object sender, TappedEventArgs e)
    {
        await Navigation.PushModalAsync(
            new ExerciseDetailPage(_dbService, _filter, null, ExerciseMode.create)
        );
    }

    private async void filterExercises(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(
            new ExerciseDetailPage(filter: _filter, mode: ExerciseMode.filter)
        );
    }
    private void OnSwipeRight(object sender, SwipedEventArgs e)
    {

        if (Application.Current.MainPage is FlyoutPage flyout)
        {
            flyout.IsPresented = true; // Abre el menú lateral
        }
    }
}