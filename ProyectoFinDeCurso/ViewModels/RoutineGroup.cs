using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinDeCurso.ViewModels
{
    public class RoutineGroup 
    {
        public bodyPartEnum MuscleGroupId { get; set; }
        public string DisplayName
        {
            get
            {
                try
                {
                    return MuscleGroupId.ToFriendlyName(); // traducción a español
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERROR DisplayName] " + ex.Message);
                    return string.Empty;
                }
            }
        }

        public ObservableCollection<Routines> Routines { get; set; } // actualiza los datos

        public RoutineGroup(bodyPartEnum muscleGroupId, IEnumerable<Routines> routines) // filtra por tipo de grupo muscular
        {
            try
            {
                MuscleGroupId = muscleGroupId;
                Routines = new ObservableCollection<Routines>(routines);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR RoutineGroup Constructor] " + ex.Message);
                Routines = new ObservableCollection<Routines>(); // evita crash
            }
        }
    }
}
