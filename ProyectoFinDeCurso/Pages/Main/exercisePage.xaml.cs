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
}