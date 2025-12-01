using System;
using System.ComponentModel;
using System.Windows.Input;

namespace ProyectoFinDeCurso.ViewModels
{
    public class CalendarViewModel : INotifyPropertyChanged
    {
        
        private DateTime _fechaSeleccionada;
        private DateTime _startDate;
        
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartDate)));
                }
            }
        }
        public DateTime FechaSeleccionada
        {
            get => _fechaSeleccionada;
            set
            {
                if (_fechaSeleccionada != value)
                {
                    _fechaSeleccionada = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FechaSeleccionada)));
                }
            }
        }

        // Comandos para cambiar entre meses
        public ICommand GoNextMonth { get; }
        public ICommand GoPreviousMonth { get; }

        public CalendarViewModel()
        {
            StartDate = DateTime.Today;
            // Fecha inicial
            if (_fechaSeleccionada == default)
                _fechaSeleccionada = DateTime.Today;

            GoNextMonth = new Command(() =>
            {
                StartDate = StartDate.AddMonths(1);
                FechaSeleccionada = StartDate;
            });

            GoPreviousMonth = new Command(() =>
            {
                StartDate = StartDate.AddMonths(-1);
                FechaSeleccionada = StartDate;
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}