using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Pages.Detail;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;

namespace ProyectoFinDeCurso.Pages.Main;

public partial class CalendarPage : ContentPage
{
    private readonly DbService _dbService;
    public CalendarPage(DbService dbService)
    {
        InitializeComponent();
        BindingContext = new CalendarViewModel(DaySelected);
        _dbService = dbService;
    }
    private async void DaySelected(CalendarDay day)
    {
        DateTime fecha = day.Date;

        await DisplayAlert(
            "Día seleccionado",
            $"Fecha creada: {fecha:dd/MM/yyyy}",
            "OK"
        );
        string fechaTexto = fecha.ToString("dd/MM/yyyy");
        await Navigation.PushModalAsync(
                new CalendarDetailPage(_dbService,ModeEnum.View, fechaTexto)
            );
    }
    private async void OnAddAlarm(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(
                new CalendarDetailPage(_dbService, ModeEnum.Edit)
            );
    }
}