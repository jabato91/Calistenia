

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
    private HashSet<VisualElement> animatedElements = new HashSet<VisualElement>();
  
    private readonly DbService _dbService;
    private ExerciseFilterViewModel _filter;
    private static userTypeEnum _userType;
   
    public exercisePage(DbService dbService, userTypeEnum userType)
    {
        _userType = userTypeEnum.nothing;
        _userType = userType;
        InitializeComponent();
        _dbService = dbService;
        // Solo asignamos el BindingContext, no llamamos OnAppearing manualmente
        _filter = new ExerciseFilterViewModel(_dbService,_userType);
        BindingContext = _filter;

    }

    private async void OnExerciseTapped(object sender, EventArgs e)
    {
        try
        {
            if ((sender as Border)?.BindingContext is Exercise selectedExercise)
            {
                await Navigation.PushModalAsync(new ExerciseDetailPage(_dbService, _filter,selectedExercise, ExerciseMode.View));
            }
        }
        catch (Exception ex)
        {
            // Muestra un mensaje de error amigable
            await DisplayAlert(
                "Error",
                $"Ocurrió un error al abrir el ejercicio:\n{ex.Message}",
                "OK"
            );
        }
    }
    
    private async void eliminateExercise(object sender, EventArgs e)
    {
        var eliminate = sender as ImageButton;

        var exercise = eliminate?.BindingContext as Exercise; //recoge el ejercicio al que está asociado

        if (exercise != null)
        {
            await _dbService.DeleteExerciseById(exercise.execiseID);
            // Actualizamos la colección del ViewModel
            _filter.Exercises.Remove(exercise);
            _filter.OnPropertyChanged(nameof(_filter.Exercises));
            _filter.OnPropertyChanged(nameof(_filter.FilteredExercises));

        }
    }

    private async void modifyExercise(object sender, EventArgs e)
    {
        if ((sender as ImageButton)?.BindingContext is not Exercise selectedExercise)
            return;

        await Navigation.PushModalAsync(new ExerciseDetailPage(_dbService, _filter, selectedExercise, ExerciseMode.Edit));
    }
   
    private async void createExercise(object sender, TappedEventArgs e)
    {
        await Navigation.PushModalAsync(new ExerciseDetailPage(_dbService, _filter,null, ExerciseMode.Create));
    }
}