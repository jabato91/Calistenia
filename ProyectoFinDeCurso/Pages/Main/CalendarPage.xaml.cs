using ProyectoFinDeCurso.ViewModels;

namespace ProyectoFinDeCurso.Pages.Main;

public partial class CalendarPage : ContentPage
{
	public CalendarPage()
	{
		InitializeComponent();
		BindingContext = new CalendarViewModel();
	}
}