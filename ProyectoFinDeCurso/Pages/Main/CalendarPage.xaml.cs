using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Pages.Detail;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;

namespace ProyectoFinDeCurso.Pages.Main;

public partial class CalendarPage : ContentPage
{
    private readonly DbService _dbService;
    public CalendarPage(DbService dbService) //Inyección de dependencia
    {
        InitializeComponent();
        BindingContext = new CalendarViewModel(DaySelected); //Pasamos el método DaySelected al ViewModel
        _dbService = dbService; //Asignamos el servicio de base de datos a una variable local
    }
    private async void DaySelected(CalendarDay day) //Método que se ejecuta al seleccionar un día en el calendario
    {
        DateTime fecha = day.Date; //Obtenemos la fecha seleccionada

        string fechaTexto = fecha.ToString("dd/MM/yyyy"); //Convertimos la fecha a texto en formato dd/MM/yyyy
        await Navigation.PushModalAsync(
                new CalendarDetailPage(_dbService,ModeEnum.View, fechaTexto) //Navegamos y creamos la página de detalle del calendario pasando la fecha seleccionada
            );
    }
    private async void OnAddAlarm(object sender, EventArgs e) //Método que se ejecuta al pulsar el botón de añadir alarma
    {
        await Navigation.PushModalAsync(
                new CalendarDetailPage(_dbService, ModeEnum.create)
            ); //Navegamos y creamos la página de detalle del calendario en modo creación
    }
}