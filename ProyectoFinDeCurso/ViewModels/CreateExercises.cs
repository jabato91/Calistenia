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
    public class CreateExercises
    {
        public DbService _dbService;
        public CreateExercises(DbService dbService)
        {
            _dbService = dbService;
            _ = InitAsync(); // llamamos a InitAsync y esperamos que corra correctamente
        }

        public async Task InitAsync()
        {
            await CreateExercise();
            await CreateRoutine();
        }
        public List<Exercise> createExerciseList()
        {
            List<Exercise> exerciseNames = new List<Exercise>
            {
                new Exercise { name = "Flexiones con Rodillas", description = "Las flexiones son un ejercicio de fuerza que trabaja pecho, brazos y hombros usando el peso corporal.\r\nSe realizan apoyando manos y pies en el suelo, bajando el pecho y extendiendo los brazos.\r\nMejoran la resistencia y fortalecen el core sin necesidad de equipamiento.", image = "flexiones_con_rodillas.png", muscleGroupId = Enums.bodyPartEnum.chest, dificulty = Enums.dificultyEnum.easy, typeUser = userTypeEnum.admin, video = "flexiones.mp4", materials = "Necesario: Peso corporal" },
                new Exercise { name = "Flexiones", description = "Las flexiones son un ejercicio de fuerza que trabaja pecho, brazos y hombros usando el peso corporal.\r\nSe realizan apoyando manos y pies en el suelo, bajando el pecho y extendiendo los brazos.\r\nMejoran la resistencia y fortalecen el core sin necesidad de equipamiento.", image = "flexiones.png", muscleGroupId = Enums.bodyPartEnum.chest, dificulty = Enums.dificultyEnum.easy, typeUser = userTypeEnum.admin, video = "flexiones.mp4",materials = "Necesario: Peso corporal" },
                new Exercise { name = "Dips en Barra", description = "Los dips en barra son un ejercicio de fuerza que trabaja tríceps, pecho y hombros.\r\nSe hacen bajando y subiendo el cuerpo entre dos barras.\r\nFortalecen el tren superior y mejoran la estabilidad sin necesidad de pesas.", image = "DipsEnBarra.png", muscleGroupId = Enums.bodyPartEnum.chest, dificulty = Enums.dificultyEnum.easy, typeUser = userTypeEnum.admin, video = "flexiones.mp4", materials = "Obligatorio: Una Barra" },
                new Exercise { name = "Sentadillas", description = "Las sentadillas fortalecen piernas y glúteos usando el peso corporal.\r\nSe realizan flexionando las rodillas y bajando la cadera como si te sentaras.\r\nMejoran la fuerza, el equilibrio y la estabilidad del core.", image = "sentadillas.png", muscleGroupId = Enums.bodyPartEnum.leg, dificulty = Enums.dificultyEnum.easy, typeUser = userTypeEnum.admin, video = "flexiones.mp4", materials = "Necesario: Peso corporal" },
                new Exercise { name = "Tabla", description = "La tabla (plank) es un ejercicio isométrico que fortalece el core, espalda y hombros.\r\nSe realiza apoyando antebrazos y pies, manteniendo el cuerpo recto y firme.\r\nMejora la estabilidad, la postura y la resistencia muscular.", image = "tabla.png", muscleGroupId = Enums.bodyPartEnum.abdomen,dificulty =  Enums.dificultyEnum.easy, typeUser = userTypeEnum.admin, video = "flexiones.mp4", materials = "Necesario: Peso corporal" },
                new Exercise { name = "Remo Prono", description = "El remo es un ejercicio que trabaja espalda, hombros y brazos.\r\nSe realiza tirando de un peso hacia el torso mientras se mantiene la espalda recta.\r\nMejora la fuerza, la postura y la estabilidad del core.", image = "bicepcurl.png", muscleGroupId = Enums.bodyPartEnum.biceps,dificulty =  Enums.dificultyEnum.medium, typeUser = userTypeEnum.admin, video = "flexiones.mp4", materials = "Obligatorio: Una barra baja"},
                new Exercise { name = "Remo Supino", description = "El remo es un ejercicio que trabaja espalda, hombros y brazos.\r\nSe realiza tirando de un peso hacia el torso mientras se mantiene la espalda recta.\r\nMejora la fuerza, la postura y la estabilidad del core.", image = "bicepcurl.png", muscleGroupId = Enums.bodyPartEnum.biceps,dificulty =  Enums.dificultyEnum.medium, typeUser = userTypeEnum.admin, video = "flexiones.mp4", materials = "Obligatorio: Una barra baja" },
                new Exercise { name = "Pseudo Planche", description = "El remo es un ejercicio que trabaja espalda, hombros y brazos.\r\nSe realiza tirando de un peso hacia el torso mientras se mantiene la espalda recta.\r\nMejora la fuerza, la postura y la estabilidad del core.", image = "bicepcurl.png", muscleGroupId = Enums.bodyPartEnum.biceps,dificulty =  Enums.dificultyEnum.medium, typeUser = userTypeEnum.admin, video = "flexiones.mp4", materials = "Opcional: Unas mini paralelas" },
                new Exercise { name = "Extensiones en TRX", description = "Las flexiones de tríceps fortalecen principalmente los brazos y el pecho.\r\nSe realizan con las manos más juntas, bajando el cuerpo manteniendo los codos pegados al torso.\r\nMejoran la fuerza de los tríceps y la estabilidad del core.", image = "tricepdip.png", muscleGroupId = Enums.bodyPartEnum.triceps,dificulty =  Enums.dificultyEnum.easy, typeUser = userTypeEnum.admin, video = "flexiones.mp4", materials = "Obligatorio: TRX" },
                new Exercise { name = "Fondos para triceps con TRX", description = "Las flexiones de tríceps fortalecen principalmente los brazos y el pecho.\r\nSe realizan con las manos más juntas, bajando el cuerpo manteniendo los codos pegados al torso.\r\nMejoran la fuerza de los tríceps y la estabilidad del core.", image = "tricepdip.png", muscleGroupId = Enums.bodyPartEnum.triceps,dificulty =  Enums.dificultyEnum.easy, typeUser = userTypeEnum.admin, video = "flexiones.mp4", materials = "Obligatorio: TRX" },
                 new Exercise { name = "Fondos para Triceps en Banco", description = "Las flexiones de tríceps fortalecen principalmente los brazos y el pecho.\r\nSe realizan con las manos más juntas, bajando el cuerpo manteniendo los codos pegados al torso.\r\nMejoran la fuerza de los tríceps y la estabilidad del core.", image = "tricepdip.png", muscleGroupId = Enums.bodyPartEnum.triceps,dificulty =  Enums.dificultyEnum.easy, typeUser = userTypeEnum.admin, video = "flexiones.mp4", materials = "Obligatorio: Un banco bajo" },
                new Exercise { name = "Flexiones de Triceps", description = "Las flexiones de tríceps fortalecen principalmente los brazos y el pecho.\r\nSe realizan con las manos más juntas, bajando el cuerpo manteniendo los codos pegados al torso.\r\nMejoran la fuerza de los tríceps y la estabilidad del core.", image = "tricepdip.png", muscleGroupId = Enums.bodyPartEnum.triceps,dificulty =  Enums.dificultyEnum.easy, typeUser = userTypeEnum.admin, video = "flexiones.mp4", materials = "Necesario: Peso corporal" },
                new Exercise { name = "Flexiones en Diamante", description = "Las flexiones de tríceps fortalecen principalmente los brazos y el pecho.\r\nSe realizan con las manos más juntas, bajando el cuerpo manteniendo los codos pegados al torso.\r\nMejoran la fuerza de los tríceps y la estabilidad del core.", image = "tricepdip.png", muscleGroupId = Enums.bodyPartEnum.chest,dificulty =  Enums.dificultyEnum.medium, typeUser = userTypeEnum.admin, video = "flexiones.mp4", materials = "Necesario: Peso corporal" },
                new Exercise { name = "Fondos en Barra", description = "Las flexiones de tríceps fortalecen principalmente los brazos y el pecho.\r\nSe realizan con las manos más juntas, bajando el cuerpo manteniendo los codos pegados al torso.\r\nMejoran la fuerza de los tríceps y la estabilidad del core.", image = "tricepdip.png", muscleGroupId = Enums.bodyPartEnum.triceps,dificulty =  Enums.dificultyEnum.medium, typeUser = userTypeEnum.admin, video = "flexiones.mp4", materials = "Obligatorio: Una barra" },
                new Exercise { name = "Estocadas", description = "Las estocadas trabajan piernas y glúteos de forma unilateral.\r\nSe realizan dando un paso adelante y flexionando ambas rodillas hasta casi tocar el suelo.\r\nMejoran el equilibrio, la fuerza y la estabilidad del core.", image = "estocadas.png", muscleGroupId = Enums.bodyPartEnum.leg,dificulty =  Enums.dificultyEnum.medium, typeUser = userTypeEnum.admin, video = "flexiones.mp4", materials = "Opcional: Una banca baja" },
                new Exercise { name = "Sentadilla Unilateral", description = "La sentadilla unilateral fortalece piernas y glúteos usando una sola pierna.\r\nSe realiza apoyando una pierna y bajando la cadera como en una sentadilla normal.\r\nMejora el equilibrio, la coordinación y la estabilidad del core.", image = "sentadilla_unilateral.png", muscleGroupId = Enums.bodyPartEnum.leg,dificulty =  Enums.dificultyEnum.easy, typeUser = userTypeEnum.admin, video = "flexiones.mp4", materials = "Necesario: Peso corporal" },
                new Exercise { name = "Fondos en Paralelas", description = "Los fondos en paralelas trabajan pecho, tríceps y hombros.\r\nSe realizan bajando y subiendo el cuerpo entre dos barras paralelas con los brazos extendidos.\r\nMejoran la fuerza del tren superior y la estabilidad del core.", image = "shoulderpress.png", muscleGroupId = Enums.bodyPartEnum.triceps,dificulty =  Enums.dificultyEnum.medium, typeUser = userTypeEnum.admin, video = "flexiones.mp4",materials = "Obligatorio: Paralelas altas" },
                new Exercise { name = "Dominadas con Salto", description = "Las dominadas fortalecen espalda, hombros y brazos.\r\nSe realizan colgándose de una barra y elevando el cuerpo hasta que la barbilla supere la barra.\r\nMejoran la fuerza del tren superior y la resistencia muscular.", image = "dominadas.png", muscleGroupId = Enums.bodyPartEnum.back,dificulty =  Enums.dificultyEnum.easy, typeUser = userTypeEnum.admin, video = "flexiones.mp4", materials = "Obligatorio: Una barra alta" },
                new Exercise { name = "Dominadas Supino", description = "Las dominadas fortalecen espalda, hombros y brazos.\r\nSe realizan colgándose de una barra y elevando el cuerpo hasta que la barbilla supere la barra.\r\nMejoran la fuerza del tren superior y la resistencia muscular.", image = "dominadas.png", muscleGroupId = Enums.bodyPartEnum.back,dificulty =  Enums.dificultyEnum.easy, typeUser = userTypeEnum.admin, video = "flexiones.mp4", materials = "Obligatorio: Una barra alta" },
                new Exercise { name = "Dominadas Neutro", description = "Las dominadas fortalecen espalda, hombros y brazos.\r\nSe realizan colgándose de una barra y elevando el cuerpo hasta que la barbilla supere la barra.\r\nMejoran la fuerza del tren superior y la resistencia muscular.", image = "dominadas.png", muscleGroupId = Enums.bodyPartEnum.back,dificulty =  Enums.dificultyEnum.medium, typeUser = userTypeEnum.admin, video = "flexiones.mp4", materials = "Obligatorio: Una barra alta" },
                new Exercise { name = "Dominadas Prono", description = "Las dominadas fortalecen espalda, hombros y brazos.\r\nSe realizan colgándose de una barra y elevando el cuerpo hasta que la barbilla supere la barra.\r\nMejoran la fuerza del tren superior y la resistencia muscular.", image = "dominadas.png", muscleGroupId = Enums.bodyPartEnum.back,dificulty =  Enums.dificultyEnum.medium, typeUser = userTypeEnum.admin, video = "flexiones.mp4", materials = "Obligatorio: Una barra alta" },
                new Exercise { name = "Tuck Front Lever Pull Up", description = "Las dominadas fortalecen espalda, hombros y brazos.\r\nSe realizan colgándose de una barra y elevando el cuerpo hasta que la barbilla supere la barra.\r\nMejoran la fuerza del tren superior y la resistencia muscular.", image = "dominadas.png", muscleGroupId = Enums.bodyPartEnum.back,dificulty =  Enums.dificultyEnum.hard, typeUser = userTypeEnum.admin, video = "flexiones.mp4", materials = "Opcional: Paralelas bajas" },
                new Exercise { name = "Advanced Tuck Pull Up", description = "Las dominadas fortalecen espalda, hombros y brazos.\r\nSe realizan colgándose de una barra y elevando el cuerpo hasta que la barbilla supere la barra.\r\nMejoran la fuerza del tren superior y la resistencia muscular.", image = "dominadas.png", muscleGroupId = Enums.bodyPartEnum.back,dificulty =  Enums.dificultyEnum.extreme, typeUser = userTypeEnum.admin, video = "flexiones.mp4", materials = "Opcional: Paralelas bajas"  },
                new Exercise { name = "Straddle Front Lever Pull Up", description = "Las dominadas fortalecen espalda, hombros y brazos.\r\nSe realizan colgándose de una barra y elevando el cuerpo hasta que la barbilla supere la barra.\r\nMejoran la fuerza del tren superior y la resistencia muscular.", image = "dominadas.png", muscleGroupId = Enums.bodyPartEnum.back,dificulty =  Enums.dificultyEnum.extreme, typeUser = userTypeEnum.admin, video = "flexiones.mp4", materials = "Opcional: Paralelas bajas"  },
                new Exercise { name = "Full Planche Pull Up", description = "Las dominadas fortalecen espalda, hombros y brazos.\r\nSe realizan colgándose de una barra y elevando el cuerpo hasta que la barbilla supere la barra.\r\nMejoran la fuerza del tren superior y la resistencia muscular.", image = "dominadas.png", muscleGroupId = Enums.bodyPartEnum.back,dificulty =  Enums.dificultyEnum.extreme, typeUser = userTypeEnum.admin, video = "flexiones.mp4", materials = "Opcional: Paralelas bajas"  },
                new Exercise { name = "Dominadas a un Brazo", description = "Las dominadas fortalecen espalda, hombros y brazos.\r\nSe realizan colgándose de una barra y elevando el cuerpo hasta que la barbilla supere la barra.\r\nMejoran la fuerza del tren superior y la resistencia muscular.", image = "dominadas.png", muscleGroupId = Enums.bodyPartEnum.back,dificulty =  Enums.dificultyEnum.hard, typeUser = userTypeEnum.admin, video = "flexiones.mp4", materials = "Obligatorio: Una barra alta" },
                new Exercise { name = "Escaladores", description = "Los escaladores (mountain climbers) trabajan core, brazos y piernas de forma dinámica.\r\nSe realizan en posición de plancha llevando las rodillas al pecho de manera alterna y rápida.\r\nMejoran la resistencia, la coordinación y la fuerza del core.", image = "escaladores.png", muscleGroupId = Enums.bodyPartEnum.abdomen,dificulty =  Enums.dificultyEnum.easy, typeUser = userTypeEnum.admin, video = "flexiones.mp4", materials = "Necesario: Peso corporal" },
                new Exercise { name = "Tuck Planche", description = "Los escaladores (mountain climbers) trabajan core, brazos y piernas de forma dinámica.\r\nSe realizan en posición de plancha llevando las rodillas al pecho de manera alterna y rápida.\r\nMejoran la resistencia, la coordinación y la fuerza del core.", image = "escaladores.png", muscleGroupId = Enums.bodyPartEnum.isometric,dificulty =  Enums.dificultyEnum.medium, typeUser = userTypeEnum.admin, video = "flexiones.mp4", materials = "Opcional: Paralelas bajas" },
                new Exercise { name = "Straddle Planche", description = "Los escaladores (mountain climbers) trabajan core, brazos y piernas de forma dinámica.\r\nSe realizan en posición de plancha llevando las rodillas al pecho de manera alterna y rápida.\r\nMejoran la resistencia, la coordinación y la fuerza del core.", image = "escaladores.png", muscleGroupId = Enums.bodyPartEnum.isometric,dificulty =  Enums.dificultyEnum.hard, typeUser = userTypeEnum.admin, video = "flexiones.mp4", materials = "Opcional: Paralelas bajas" },
                new Exercise { name = "Full Planche", description = "Los escaladores (mountain climbers) trabajan core, brazos y piernas de forma dinámica.\r\nSe realizan en posición de plancha llevando las rodillas al pecho de manera alterna y rápida.\r\nMejoran la resistencia, la coordinación y la fuerza del core.", image = "escaladores.png", muscleGroupId = Enums.bodyPartEnum.isometric,dificulty =  Enums.dificultyEnum.extreme, typeUser = userTypeEnum.admin, video = "flexiones.mp4", materials = "Opcional: Paralelas bajas" },
            };
            return exerciseNames;
        }
        public List<Routines> createRoutineList()
        {
            List<Routines> routineList = new List<Routines>
            {
                new Routines { nameRoutine = "Rutina Pierna", description = "Rutina completa para trabajar pierna.", image = "pie.png", muscleGroup = Enums.bodyPartEnum.leg,typeUser = userTypeEnum.admin,difficulty = dificultyEnum.easy},
                 new Routines { nameRoutine = "Rutina Triceps", description = "Rutina completa para trabajar brazo.", image = "triceps.png", muscleGroup = Enums.bodyPartEnum.triceps,typeUser = userTypeEnum.admin, difficulty = dificultyEnum.medium },
                 new Routines { nameRoutine = "Rutina Pecho", description = "Rutina completa para trabajar pecho.", image = "pecho.png", muscleGroup = Enums.bodyPartEnum.chest,typeUser = userTypeEnum.admin, difficulty = dificultyEnum.hard },
                  new Routines { nameRoutine = "Rutina Espalda", description = "Rutina completa para trabajar espalda.", image = "espalda.png", muscleGroup = Enums.bodyPartEnum.back,typeUser = userTypeEnum.admin,difficulty = dificultyEnum.easy },
                  new Routines { nameRoutine = "Rutina Full Planche", description = "Rutina completa para sacar la full planche.", image = "full_planche.png", muscleGroup = Enums.bodyPartEnum.isometric,typeUser = userTypeEnum.admin, difficulty = dificultyEnum.extreme }
            };
            return routineList;
        }
        public async Task CreateExercise()
        {
            List<Exercise> existing = await _dbService.GetEercises();
            var newExercises = createExerciseList();
            var existingNames = existing.Select(e => e.name).ToList();
            if (existing.Count.Equals(0)) {
            foreach (var exercise in newExercises)
            {
                if (!existingNames.Contains(exercise.name))
                {
                    await _dbService.Create(exercise);
                }
            }
             }
        }
        public async Task CreateRoutine()
        {
            List<Routines> routineNames = createRoutineList(); // Rutinas que quieres crear
            List<Exercise> exercises = await _dbService.GetEercises(); // Ejercicios que se pueden asociar
            List<Routines> existing = await _dbService.GetRoutines(); // Rutinas ya en la DB
            if(existing.Count == 0) {


                var routineMappings = new Dictionary<string, (string[], int[], int[])>
                {
                    ["Rutina Pierna"] = (new[] { "Sentadillas", "Estocadas", "Sentadilla Unilateral" }, new[] { 3, 3, 3 }, new[] { 12, 10, 8 }),
                    ["Rutina Triceps"] = (new[] { "Flexiones de Triceps", "Flexiones en Diamante", "Fondos en Barra" }, new[] { 3, 3, 3 }, new[] { 12, 10, 8 }),
                    ["Rutina Pecho"] = (new[] { "Flexiones", "Flexiones en Diamante" }, new[] { 3, 3, 3 }, new[] { 12, 10, 8 }),
                    ["Rutina Espalda"] = (new[] { "Dominadas" }, new[] { 3, 3, 3 }, new[] { 12, 10, 8 }),
                    ["Rutina Full Planche"] = (new[] { "Tuck Planche", "Straddle Planche", "Full Planche" }, new[] { 3, 3, 3 }, new[] { 12, 10, 8 })
                };
                foreach (var routineToAdd in routineNames)
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
                            var (exerciseNames, series, repeticiones) = routineData; //recoge los datos de la rutina
                            var selectedExercises = exercises
                           .Where(ex => exerciseNames.Contains(ex.name, StringComparer.OrdinalIgnoreCase))
                           .ToList(); //filtra los ejercicios que coinciden con los nombres definidos

                            for (int i = 0; i < selectedExercises.Count; i++) //bucle para crear los ejercicios asociados a la rutina
                            {
                                var ex = selectedExercises[i];
                                RoutinesExercises routineExercise;
                                if (!ex.muscleGroupId.Equals(bodyPartEnum.isometric))
                                {
                                
                                    routineExercise = new RoutinesExercises
                                    {
                                        RoutineID = routineId,
                                        ExerciseID = ex.execiseID,
                                        sets = i < series.Length ? series[i] : 0,
                                        reps = i < repeticiones.Length ? repeticiones[i] : 0
                                    };

                                }
                                else
                                {
                                    routineExercise = new RoutinesExercises
                                    {
                                        RoutineID = routineId,
                                        ExerciseID = ex.execiseID,
                                        reps = i < series.Length ? repeticiones[i] : 0,
                                        seconds = ex.seconds
                                    };
                                }
                                    await _dbService.Create(routineExercise);
                            }
                        }
                    }
            }
            }
        }
    }
}