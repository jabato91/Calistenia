    using ProyectoFinDeCurso.Enums;
    using SQLite;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace ProyectoFinDeCurso.Models 
    {
        public class Exercise 
        {
            [PrimaryKey, AutoIncrement, Column("exerciseID")]
            public int execiseID { get; set; } 
            [Column("Name")]
            public string name { get; set; } = string.Empty;
            [Column("Description")]
            public string description { get; set; } = string.Empty;
            [Column("image")]
            public string image { get; set; } = string.Empty;
            [Column("dificulty")]
            public dificultyEnum dificulty { get; set; } = dificultyEnum.nothing;
        [Column("BodyPart")]
            public bodyPartEnum muscleGroupId { get; set; } = bodyPartEnum.nothing;
        [Ignore]
        public int sets { get; set; } = -1;
        [Ignore]
        public int reps { get; set; } = -1;
        [Ignore]
        public DoubleCollection LinePattern { get; set; } = new() { 6, 4 };
        public Brush AuraColor
        {
            get
            {
                return dificulty switch
                {
                    dificultyEnum.easy => CreateBrush(Colors.LightGreen, Colors.Green),
                    dificultyEnum.medium => CreateBrush(Colors.Orange, Colors.DarkOrange),
                    dificultyEnum.hard => CreateBrush(Colors.Red, Colors.DarkRed),
                    dificultyEnum.extreme => CreateBrush(Colors.Purple, Colors.DarkMagenta),
                    _ => CreateBrush(Colors.Gray, Colors.DarkGray),
                };
            }
        }

        // 🔸 Método auxiliar para crear el gradiente radial
        private static RadialGradientBrush CreateBrush(Color color1, Color color2)
        {
            return new RadialGradientBrush
            {
                Center = new Point(0.5, 0.5),
                Radius = 0.9,
                GradientStops = new GradientStopCollection
                {
                    new GradientStop(Colors.Transparent, 0.3f),
                    new GradientStop(color1, 0.8f),
                    new GradientStop(color2, 1f)
                }
            };
        }
    }
    }
