using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Pages.Detail;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;
using System.Collections.ObjectModel;
using System.Reflection.PortableExecutable;

namespace ProyectoFinDeCurso.Pages.Main;

public partial class RoutinesPage : ContentPage
{
    private readonly DbService _dbService;
    private static userTypeEnum _userType;
    private RoutinesFilterViewModel routinesViewModel;
    private ExerciseFilterViewModel _exerciseFilterViewModel;

    public RoutinesPage(DbService dbService, userTypeEnum userType)
    {
        _exerciseFilterViewModel = new ExerciseFilterViewModel(dbService, userType);
        _dbService = dbService;
        _userType = userType; // ✅ Asigna primero el tipo de usuario
        routinesViewModel = new RoutinesFilterViewModel(_dbService, _userType);
        InitializeComponent();
        BindingContext = routinesViewModel;
    }

    private async void OnExerciseTapped(object sender, EventArgs e)
    {
        try
        {
            if ((sender as Grid)?.BindingContext is Exercise selectedExercise)
            {
                await Navigation.PushModalAsync(new ExerciseDetailPage(_dbService, _exerciseFilterViewModel, selectedExercise, ExerciseMode.View));
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

    private async void accessRoutine(object sender, TappedEventArgs e)
    {
        try
        {
            if ((sender as Border)?.BindingContext is Routines selectedRoutine)
            {
                await Navigation.PushModalAsync(new RoutineDetailPage(_dbService, routinesViewModel, selectedRoutine, ExerciseMode.View));
            }
        }
        catch (Exception ex)
        {
            // Muestra un mensaje de error amigable
            await DisplayAlert(
                "Error",
                $"Ocurrió un error al abrir la rutina:\n{ex.Message}",
                "OK"
            );
        }

    }



    private async void createRoutine(object sender, TappedEventArgs e)
    {
        await Navigation.PushModalAsync(new RoutineDetailPage(_dbService, routinesViewModel, null, ExerciseMode.create));
    } 
    
    private async void OnExpanded(object sender, ExpandedChangedEventArgs e)
    {
        if (sender is not Expander expander)
            return;

        if (expander.Content is not VisualElement content)
            return;

        if (e.IsExpanded)
        {
            // 🔹 ANIMACIÓN AL ABRIR
            content.Opacity = 0;
            content.TranslationY = -20;
            await Task.WhenAll(
                content.FadeTo(1, 250, Easing.SinInOut),
                content.TranslateTo(0, 0, 250, Easing.SinInOut)
            );
        }
       
    }
    private async void filterExercises(object sender, EventArgs e)
    {

        await Navigation.PushModalAsync(new RoutineDetailPage( filterViewModel: routinesViewModel, mode: ExerciseMode.filter));
    }
}