using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;

namespace ProyectoFinDeCurso.Pages.Main;

public partial class exercisePage : ContentPage
{

    private readonly DbService _dbService;

    public exercisePage(DbService dbService)
    {
        InitializeComponent();
        _dbService = dbService;

        // Solo asignamos el BindingContext, no llamamos OnAppearing manualmente
        BindingContext = new ProyectoFinDeCurso.ViewModels.ExerciseFilterViewModel(_dbService);
    }
    private void OpenExercise(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Exercise selectedExercise)
        {
            // 👉 Aquí haces lo que quieras al tocar un ejercicio
            // Ejemplo: mostrar un mensaje o navegar a otra página
            DisplayAlert("Ejercicio seleccionado", selectedExercise.name, "OK");

            // Limpia la selección (opcional)
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}