using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;

namespace ProyectoFinDeCurso.Pages.Main;

public partial class RoutinesPage : ContentPage
{
    private readonly DbService _dbService;
    public RoutinesPage(DbService dbService)
	{
        _dbService = dbService;
		InitializeComponent();

		BindingContext = new RoutinesFilterViewModel(_dbService);
    }
}