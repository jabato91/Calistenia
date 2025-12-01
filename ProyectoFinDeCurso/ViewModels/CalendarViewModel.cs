using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using ProyectoFinDeCurso.Models;


namespace ProyectoFinDeCurso.ViewModels
{
    public class CalendarViewModel : INotifyPropertyChanged
    {

        public ObservableCollection<CalendarDay> Days { get; } = new();

        private DateTime _displayMonth;
        private CalendarDay _selectedDay;

        public DateTime DisplayMonth
        {
            get => _displayMonth;
            set
            {
                if (_displayMonth != value)
                {
                    _displayMonth = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayMonth)));
                    GenerateDays();
                }
            }
        }

        public CalendarDay SelectedDay
        {
            get => _selectedDay;
            set
            {
                if (_selectedDay != value)
                {
                    if (_selectedDay != null)
                        _selectedDay.IsSelected = false;

                    _selectedDay = value;

                    if (_selectedDay != null)
                        _selectedDay.IsSelected = true;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedDay)));
                }
            }
        }

        public ICommand NextMonthCommand { get; }
        public ICommand PreviousMonthCommand { get; }
        public ICommand DayTappedCommand { get; }
        private readonly Action<CalendarDay> _onDaySelected;
        public CalendarViewModel(Action<CalendarDay> onDaySelected)
        {
            _onDaySelected = onDaySelected;
            DisplayMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            NextMonthCommand = new Command(() =>
            {
                DisplayMonth = DisplayMonth.AddMonths(1);
            });

            PreviousMonthCommand = new Command(() =>
            {
                DisplayMonth = DisplayMonth.AddMonths(-1);
            });

            DayTappedCommand = new Command<CalendarDay>(day =>
            {
                if (day == null) return;

                if (!day.IsCurrentMonth)
                {
                    DisplayMonth = new DateTime(day.Date.Year, day.Date.Month, 1);
                }

                SelectedDay = day;

                _onDaySelected?.Invoke(day); 
            });

            GenerateDays();
        }

        private void GenerateDays()
        {
            Days.Clear();

            var firstOfMonth = new DateTime(DisplayMonth.Year, DisplayMonth.Month, 1);

            int diff = ((int)firstOfMonth.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            var startDate = firstOfMonth.AddDays(-diff);

            var today = DateTime.Today;

            for (int i = 0; i < 42; i++)
            {
                var date = startDate.AddDays(i);

                Days.Add(new CalendarDay
                {
                    Date = date,
                    IsCurrentMonth = date.Month == DisplayMonth.Month,
                    IsToday = date.Date == today.Date,
                    IsSelected = SelectedDay != null && SelectedDay.Date.Date == date.Date
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}