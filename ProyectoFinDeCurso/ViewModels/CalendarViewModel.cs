using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using ProyectoFinDeCurso.Models;


namespace ProyectoFinDeCurso.ViewModels
{
    public class CalendarViewModel : INotifyPropertyChanged
    {

        public ObservableCollection<CalendarDay> Days { get; } = new(); //obtiene los días del mes en curso

        private DateTime _displayMonth; //almacena el mes que se está mostrando
        private CalendarDay _selectedDay; //almacena el día seleccionado

        public DateTime DisplayMonth //propiedad que obtiene o establece el mes que se está mostrando
        {
            get => _displayMonth;
            set
            {
                try
                {
                    if (_displayMonth != value)
                    {
                        _displayMonth = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayMonth))); //notifica el cambio de propiedad
                        GenerateDays(); //genera los días del mes
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR DisplayMonth] {ex.Message}");
                }
            }
        }

        public CalendarDay SelectedDay //propiedad que obtiene o establece el día seleccionado
        {
            get => _selectedDay;
            set
            {
                try
                {
                    if (_selectedDay != value)
                    {
                        if (_selectedDay != null)
                            _selectedDay.IsSelected = false;

                        _selectedDay = value;

                        if (_selectedDay != null)
                            _selectedDay.IsSelected = true;

                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedDay))); //notifica el cambio de propiedad
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR SelectedDay] {ex.Message}");
                }
            }
        }

        public ICommand NextMonthCommand { get; } //comando para ir al siguiente mes
        public ICommand PreviousMonthCommand { get; } //comando para ir al mes anterior
        public ICommand DayTappedCommand { get; } //comando para seleccionar un día

        private readonly Action<CalendarDay> _onDaySelected; //acción que se ejecuta al seleccionar un día

        public CalendarViewModel(Action<CalendarDay> onDaySelected) //constructor que inicializa el ViewModel
        {
            try
            {
                _onDaySelected = onDaySelected; //asigna la acción al campo privado
                DisplayMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1); //inicializa el mes que se está mostrando al mes actual

                NextMonthCommand = new Command(() => //inicializa el comando para ir al siguiente mes
                {
                    try
                    {
                        DisplayMonth = DisplayMonth.AddMonths(1); //incrementa el mes en uno
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR NextMonth] {ex.Message}");
                    }
                });

                PreviousMonthCommand = new Command(() => //inicializa el comando para ir al mes anterior
                {
                    try
                    {
                        DisplayMonth = DisplayMonth.AddMonths(-1); //decrementa el mes en uno
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR PreviousMonth] {ex.Message}");
                    }
                });

                DayTappedCommand = new Command<CalendarDay>(day => //inicializa el comando para seleccionar un día
                {
                    try
                    {
                        if (day == null) return; //verifica que el día no sea nulo

                        if (!day.IsCurrentMonth) //si el día no pertenece al mes actual
                        {
                            DisplayMonth = new DateTime(day.Date.Year, day.Date.Month, 1); //cambia el mes que se está mostrando al mes del día seleccionado
                        }

                        SelectedDay = day; //establece el día seleccionado

                        _onDaySelected?.Invoke(day);  //ejecuta la acción al seleccionar un día
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR DayTapped] {ex.Message}");
                    }
                });

                GenerateDays(); //genera los días del mes
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR CalendarViewModel Constructor] {ex.Message}");
            }
        }

        private void GenerateDays() //método que genera los días del mes
        {
            try
            {
                Days.Clear(); //limpia la colección de días

                var firstOfMonth = new DateTime(DisplayMonth.Year, DisplayMonth.Month, 1); //obtiene el primer día del mes

                int diff = ((int)firstOfMonth.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7; //calcula el desfase para que la semana comience en lunes
                var startDate = firstOfMonth.AddDays(-diff); //obtiene la fecha de inicio para mostrar en el calendario

                var today = DateTime.Today; //obtiene la fecha actual

                for (int i = 0; i < 42; i++) //itera 42 veces para cubrir 6 semanas
                {
                    var date = startDate.AddDays(i); //obtiene la fecha correspondiente al día actual en la iteración

                    Days.Add(new CalendarDay //añade un nuevo día a la colección
                    {
                        Date = date,
                        IsCurrentMonth = date.Month == DisplayMonth.Month,
                        IsToday = date.Date == today.Date,
                        IsSelected = SelectedDay != null && SelectedDay.Date.Date == date.Date
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR GenerateDays] {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged; //evento que notifica cambios en las propiedades
    }
}