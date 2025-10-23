using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinDeCurso.ViewModels
{
    public class RoutinesFilterViewModel 
    {
        private readonly DbService _dbService;

        public ObservableCollection<Routines> Routines { get; set; } = new();
    

        public RoutinesFilterViewModel(DbService dbService)
        {
            _dbService = dbService;

            // Carga las rutinas automáticamente
            LoadRoutines();
        }
        public async void LoadRoutines()
        {
            List<Routines> routinesFromDb = await _dbService.GetRoutines();
            List<RoutinesExercises> routinesExercisesFromDb = await _dbService.GetRoutinesExercises();
            List<Exercise> exercisesFromDb = await _dbService.GetEercises();
            foreach (var routine in routinesFromDb)
            {
                {
                    // Filtrar ejercicios de esta rutina
                    var exercisesForRoutine = routinesExercisesFromDb
                        .Where(re => re.RoutineID == routine.routineID)
                        .Join(exercisesFromDb,
                              re => re.ExerciseID,
                              ex => ex.execiseID,
                              (re, ex) => ex);

                    // Agregar ejercicios a la rutina
                    foreach (var ex in exercisesForRoutine)
                        routine.Exercises.Add(ex);

                    Routines.Add(routine);
                }
            }
        }
        
    }
}
