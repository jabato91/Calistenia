using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinDeCurso.ViewModels
{
    public class createRoutine
    {
        public DbService _dbService;

        public createRoutine(DbService dbService)
        {
            try
            {
                _dbService = dbService;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR constructor createRoutine] " + ex.Message);
            }
        }

        public async Task InitAsync() //método para inicializar la creación de rutinas
        {
            try
            {
                await CreateRoutine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR InitAsync] " + ex.Message);
            }
        }

        public List<Routines> createRoutineList() //lista de rutinas a crear
        {
            try
            {
                List<Routines> routineList = new List<Routines>
            {
                new Routines { nameRoutine = "Rutina Pierna", description = "Rutina completa para trabajar pierna.", image = "pie.png", muscleGroup = Enums.bodyPartEnum.leg,typeUser = userTypeEnum.admin,difficulty = dificultyEnum.easy, userID = 0},
                new Routines { nameRoutine = "Rutina Triceps", description = "Rutina completa para trabajar brazo.", image = "triceps.png", muscleGroup = Enums.bodyPartEnum.triceps,typeUser = userTypeEnum.admin, difficulty = dificultyEnum.medium, userID = 0 },
                new Routines { nameRoutine = "Rutina Pecho", description = "Rutina completa para trabajar pecho.", image = "pecho.png", muscleGroup = Enums.bodyPartEnum.chest,typeUser = userTypeEnum.admin, difficulty = dificultyEnum.hard, userID = 0 },
                new Routines { nameRoutine = "Rutina Espalda", description = "Rutina completa para trabajar espalda.", image = "espalda.png", muscleGroup = Enums.bodyPartEnum.back,typeUser = userTypeEnum.admin,difficulty = dificultyEnum.easy, userID = 0 },
                new Routines { nameRoutine = "Rutina Full Planche", description = "Rutina completa para sacar la full planche.", image = "full_planche.png", muscleGroup = Enums.bodyPartEnum.isometric,typeUser = userTypeEnum.admin, difficulty = dificultyEnum.extreme, userID = 0 }
            };
                return routineList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR createRoutineList] " + ex.Message);
                return new List<Routines>();
            }
        }

        public async Task CreateRoutine() //método para crear las rutinas y asociar los ejercicios
        {
            try
            {
                List<Routines> routineNames = createRoutineList(); // Rutinas que quieres crear
                List<Exercise> exercises = await _dbService.GetExercisesAsync(); // Ejercicios que se pueden asociar
                List<Routines> existing = await _dbService.GetRoutinesAsync(); // Rutinas ya en la DB

                if (existing.Count == 0)
                {

                    var routineMappings = new Dictionary<string, (string[], int[], int[])> // Diccionario que mapea los nombres de las rutinas a los ejercicios asociados,repeticiones, y series o segundos
                    {
                        ["Rutina Pierna"] = (new[] { "Sentadillas", "Estocadas", "Sentadilla Unilateral" }, new[] { 12, 10, 8 }, new[] { 3, 3, 3 }),
                        ["Rutina Triceps"] = (new[] { "Flexiones de Triceps", "Flexiones en Diamante", "Fondos en Barra" }, new[] { 12, 10, 8 }, new[] { 3, 3, 3 }),
                        ["Rutina Pecho"] = (new[] { "Flexiones", "Flexiones en Diamante" }, new[] { 7, 10, 12 }, new[] { 4, 3, 3 }),
                        ["Rutina Espalda"] = (new[] { "Dominadas", "Dominadas a un Brazo", "Dominadas con Salto", "Tabla" }, new[] { 15, 10, 5, 10 }, new[] { 3, 3, 3, 3 }),
                        ["Rutina Full Planche"] = (new[] { "Tuck Planche", "Straddle Planche", "Full Planche" }, new[] { 10, 16, 5 }, new[] { 3, 4, 3 })
                    };

                    foreach (var routineToAdd in routineNames) //bucle para añadir las rutinas
                    {
                        try
                        {
                            // Verifica si ya existe una rutina con el mismo nombre
                            bool exists = existing.Any(r => r.nameRoutine.Equals(routineToAdd.nameRoutine, StringComparison.OrdinalIgnoreCase));

                            if (!exists)
                            {
                                // Crea la nueva rutina
                                await _dbService.Create(routineToAdd);

                                // Asegurar que tenga un ID válido
                                int routineId = routineToAdd.routineID;
                                if (routineId <= 0)
                                    continue;

                                // Si existe una asociación definida, buscar los ejercicios y crear el vínculo
                                if (routineMappings.TryGetValue(routineToAdd.nameRoutine, out var routineData))
                                {
                                    var (exerciseNames, seriesORepeticiones, repeticiones) = routineData; //recoge los datos de la rutina

                                    var selectedExercises = exercises
                                        .Where(ex => exerciseNames.Contains(ex.name, StringComparer.OrdinalIgnoreCase))
                                        .ToList(); //filtra los ejercicios que coinciden con los nombres definidos

                                    for (int i = 0; i < selectedExercises.Count; i++) //bucle para crear los ejercicios asociados a la rutina
                                    {
                                        try
                                        {
                                            var ex = selectedExercises[i];
                                            RoutinesExercises routineExercise;

                                            if (!ex.muscleGroupId.Equals(bodyPartEnum.isometric)) //verifica si el ejercicio es isométrico o no
                                            {
                                                routineExercise = new RoutinesExercises //crea la asociación rutina-ejercicio
                                                {
                                                    RoutineID = routineId,
                                                    ExerciseID = ex.execiseID,
                                                    sets = i < seriesORepeticiones.Length ? seriesORepeticiones[i] : 0,
                                                    reps = i < repeticiones.Length ? repeticiones[i] : 0
                                                };
                                            }
                                            else
                                            {
                                                routineExercise = new RoutinesExercises
                                                {
                                                    RoutineID = routineId,
                                                    ExerciseID = ex.execiseID,
                                                    reps = i < repeticiones.Length ? repeticiones[i] : 0,
                                                    seconds = i < seriesORepeticiones.Length ? seriesORepeticiones[i] : 0,
                                                };
                                            }

                                            await _dbService.Create(routineExercise);
                                        }
                                        catch (Exception exInsideLoop)
                                        {
                                            Console.WriteLine("[ERROR creando rutina-ejercicio] " + exInsideLoop.Message);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception exRoutineLoop)
                        {
                            Console.WriteLine("[ERROR en bucle de rutinas] " + exRoutineLoop.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR CreateRoutine] " + ex.Message);
            }
        }
    }
}

