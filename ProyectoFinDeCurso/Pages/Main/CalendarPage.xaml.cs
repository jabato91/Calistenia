namespace ProyectoFinDeCurso.Pages.Main;
using CommunityToolkit.Maui;
using ProyectoFinDeCurso.ViewModels;
using Syncfusion.Maui.Calendar;

public partial class CalendarPage : ContentPage
{
	public CalendarPage()
	{
		InitializeComponent();
        BindingContext = new CalendarViewModel();
    }
    
}