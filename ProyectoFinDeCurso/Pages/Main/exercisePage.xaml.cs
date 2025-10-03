using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;

namespace ProyectoFinDeCurso.Pages.Main;

public partial class exercisePage : ContentPage
{
	public DbService _dbService;
	public exercisePage(DbService dbService)
	{
		InitializeComponent();
		_dbService = dbService;
		CreateExercises createExercises = new CreateExercises(_dbService);
    }


	
}