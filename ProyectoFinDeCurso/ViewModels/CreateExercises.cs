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
           
            InitAsync();
        }

        private async void InitAsync()
        {
            var exercises = createExerciseList();
            foreach (var exercise in exercises)
            {
                await _dbService.Create(exercise);
            }
        }
        public List<Exercise> createExerciseList()
        {
            List<Exercise> exerciseNames = new List<Exercise>
            {
                new Exercise { name = "Push-Up", description = "A bodyweight exercise that primarily targets the chest, shoulders, and triceps.", image = "pushup.png", muscleGroupId = Enums.bodyPartEnum.chest },
                new Exercise { name = "Squat", description = "A lower body exercise that targets the quadriceps, hamstrings, glutes, and calves.", image = "squat.png", muscleGroupId = Enums.bodyPartEnum.leg },
                new Exercise { name = "Plank", description = "A core exercise that strengthens the abdominal muscles, lower back, and shoulders.", image = "plank.png", muscleGroupId = Enums.bodyPartEnum.abdomen },
                new Exercise { name = "Bicep Curl", description = "An isolation exercise that targets the biceps muscles in the upper arms.", image = "bicepcurl.png", muscleGroupId = Enums.bodyPartEnum.biceps },
                new Exercise { name = "Tricep Dip", description = "An exercise that targets the triceps muscles in the back of the upper arms.", image = "tricepdip.png", muscleGroupId = Enums.bodyPartEnum.triceps },
                new Exercise { name = "Lunges", description = "A lower body exercise that targets the quadriceps, hamstrings, glutes, and calves.", image = "lunges.png", muscleGroupId = Enums.bodyPartEnum.leg },
                new Exercise { name = "Deadlift", description = "A compound exercise that targets multiple muscle groups including the back, glutes, hamstrings, and core.", image = "deadlift.png", muscleGroupId = Enums.bodyPartEnum.back },
                new Exercise { name = "Shoulder Press", description = "An exercise that targets the shoulder muscles (deltoids) and triceps.", image = "shoulderpress.png", muscleGroupId = Enums.bodyPartEnum.triceps },
                new Exercise { name = "Bent-Over Row", description = "An exercise that targets the upper back muscles (latissimus dorsi) and biceps.", image = "bentoverrow.png", muscleGroupId = Enums.bodyPartEnum.back },
                new Exercise { name = "Mountain Climbers", description = "A full-body exercise that combines cardio and core strengthening.", image = "mountainclimbers.png", muscleGroupId = Enums.bodyPartEnum.abdomen }
            };
            return exerciseNames;
        }
    }
}