using ProyectoFinDeCurso.Services;

namespace ProyectoFinDeCurso.Pages.Main;

public partial class exercisePage : ContentPage
{
	public DbService _dbService;
	public exercisePage(DbService dbService)
	{
		InitializeComponent();
		_dbService = dbService;
    }
}