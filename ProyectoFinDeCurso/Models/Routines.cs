using ProyectoFinDeCurso.Enums;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinDeCurso.Models
{
    public class Routines
    {
        [PrimaryKey, AutoIncrement, Column("routineID")]
        public int routineID { get; set; }
        [Column("NameRoutine")]
        public string nameRoutine { get; set; } = string.Empty;
        [Column("DescriptionRoutine")]
        public string description { get; set; } = string.Empty;
        [Column("imageRoutine")]
        public string image { get; set; } = string.Empty;
        [Column("bodyPartRoutine")]
        public bodyPartEnum muscleGroup { get; set; } = bodyPartEnum.nothing;
        [Column("typeUser")]
        public userTypeEnum typeUser { get; set; } = userTypeEnum.nothing;
        [Column("difficultyRoutine")]
        public dificultyEnum difficulty { get; set; } = dificultyEnum.nothing;
        [Column("userID")]
        public int userID { get; set; } = -1; 
        [Ignore] 
        public ObservableCollection<Exercise> Exercises { get; set; } = new(); //obtiene los ejercicios de la rutina
        [Ignore]
        public int IsAdmin { get; set; } = -1; // Indica si el ejercicio está siendo visto en modo administrador

        public bool IsAdminMode
        {
            get
            {
                return IsAdmin switch
                {
                    0 => false,
                    1 => true,
                    2 => true,
                    _ => throw new NotImplementedException(),
                };
            }
        }
        [Ignore]
        public Brush AuraColor
        {
            get
            {
                return difficulty switch
                {
                    dificultyEnum.easy => CreateBrush(Colors.LightGreen, Colors.Green),
                    dificultyEnum.medium => CreateBrush(Colors.Orange, Colors.DarkOrange),
                    dificultyEnum.hard => CreateBrush(Colors.Red, Colors.DarkRed),
                    dificultyEnum.extreme => CreateBrush(Colors.Purple, Colors.DarkMagenta),
                    _ => CreateBrush(Colors.Gray, Colors.DarkGray),
                };
            }
        }

        // Método privado para crear gradientes verticales
        private Brush CreateBrush(Color start, Color end)
        {
            return new LinearGradientBrush
            {
                StartPoint = new Point(0.5, 1.5),
                EndPoint = new Point(0.5, 1),
                GradientStops =
        {
            new GradientStop(start.WithAlpha(0.75f), 0f),  // 25% opaco
            new GradientStop(end.WithAlpha(0.5f), 1f)      // 0% opaco
        }
            };
        }
    }
}
