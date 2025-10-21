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
            List<Exercise> existing = await _dbService.GetEercises();
            var newExercises = createExerciseList();
            var existingNames = existing.Select(e => e.name).ToList();

            foreach (var exercise in newExercises)
            {
                if (!existingNames.Contains(exercise.name))
                {
                    await _dbService.Create(exercise);
                }
            }
        }
        public List<Exercise> createExerciseList()
        {
            List<Exercise> exerciseNames = new List<Exercise>
            {
                new Exercise { name = "Flexiones", description = "Las flexiones son un ejercicio de fuerza que trabaja pecho, brazos y hombros usando el peso corporal.\r\nSe realizan apoyando manos y pies en el suelo, bajando el pecho y extendiendo los brazos.\r\nMejoran la resistencia y fortalecen el core sin necesidad de equipamiento.", image = "flexiones.png", muscleGroupId = Enums.bodyPartEnum.chest },
                new Exercise { name = "Sentadillas", description = "Las sentadillas fortalecen piernas y glúteos usando el peso corporal.\r\nSe realizan flexionando las rodillas y bajando la cadera como si te sentaras.\r\nMejoran la fuerza, el equilibrio y la estabilidad del core.", image = "sentadillas.png", muscleGroupId = Enums.bodyPartEnum.leg },
                new Exercise { name = "Tabla", description = "La tabla (plank) es un ejercicio isométrico que fortalece el core, espalda y hombros.\r\nSe realiza apoyando antebrazos y pies, manteniendo el cuerpo recto y firme.\r\nMejora la estabilidad, la postura y la resistencia muscular.", image = "plank.png", muscleGroupId = Enums.bodyPartEnum.abdomen },
                new Exercise { name = "Remo", description = "El remo es un ejercicio que trabaja espalda, hombros y brazos.\r\nSe realiza tirando de un peso hacia el torso mientras se mantiene la espalda recta.\r\nMejora la fuerza, la postura y la estabilidad del core.", image = "bicepcurl.png", muscleGroupId = Enums.bodyPartEnum.biceps },
                new Exercise { name = "Flexiones de Triceps", description = "Las flexiones de tríceps fortalecen principalmente los brazos y el pecho.\r\nSe realizan con las manos más juntas, bajando el cuerpo manteniendo los codos pegados al torso.\r\nMejoran la fuerza de los tríceps y la estabilidad del core.", image = "tricepdip.png", muscleGroupId = Enums.bodyPartEnum.triceps },
                new Exercise { name = "Estocadas", description = "Las estocadas trabajan piernas y glúteos de forma unilateral.\r\nSe realizan dando un paso adelante y flexionando ambas rodillas hasta casi tocar el suelo.\r\nMejoran el equilibrio, la fuerza y la estabilidad del core.", image = "lunges.png", muscleGroupId = Enums.bodyPartEnum.leg },
                new Exercise { name = "Sentadilla Unilateral", description = "La sentadilla unilateral fortalece piernas y glúteos usando una sola pierna.\r\nSe realiza apoyando una pierna y bajando la cadera como en una sentadilla normal.\r\nMejora el equilibrio, la coordinación y la estabilidad del core.", image = "deadlift.png", muscleGroupId = Enums.bodyPartEnum.leg },
                new Exercise { name = "Fondos en Paralelas", description = "Los fondos en paralelas trabajan pecho, tríceps y hombros.\r\nSe realizan bajando y subiendo el cuerpo entre dos barras paralelas con los brazos extendidos.\r\nMejoran la fuerza del tren superior y la estabilidad del core.", image = "shoulderpress.png", muscleGroupId = Enums.bodyPartEnum.triceps },
                new Exercise { name = "Dominadas", description = "Las dominadas fortalecen espalda, hombros y brazos.\r\nSe realizan colgándose de una barra y elevando el cuerpo hasta que la barbilla supere la barra.\r\nMejoran la fuerza del tren superior y la resistencia muscular.", image = "dominadas.png", muscleGroupId = Enums.bodyPartEnum.back },
                new Exercise { name = "Escaladores", description = "Los escaladores (mountain climbers) trabajan core, brazos y piernas de forma dinámica.\r\nSe realizan en posición de plancha llevando las rodillas al pecho de manera alterna y rápida.\r\nMejoran la resistencia, la coordinación y la fuerza del core.", image = "mountainclimbers.png", muscleGroupId = Enums.bodyPartEnum.abdomen }
            };
            return exerciseNames;
        }
    }
}