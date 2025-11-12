    using ProyectoFinDeCurso.Enums;
    using SQLite;
    using System;
    using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace ProyectoFinDeCurso.Models 
    {
        public class Exercise : INotifyPropertyChanged
    {
            [PrimaryKey, AutoIncrement, Column("exerciseID")]
            public int execiseID { get; set; } 
            [Column("Name")]
            public string name { get; set; } = string.Empty;
            [Column("Description")]
            public string description { get; set; } = string.Empty;
            [Column("image")]
            public string image { get; set; } = string.Empty;
        [Column("video")]
        public string video { get; set; } = string.Empty;
        [Column("dificulty")]
            public dificultyEnum dificulty { get; set; } = dificultyEnum.nothing;
            [Column("BodyPart")]
            public bodyPartEnum muscleGroupId { get; set; } = bodyPartEnum.nothing;
            [Column("typeUser")]
            public userTypeEnum typeUser { get; set; } = userTypeEnum.nothing;
            [Column("Materials")]
            public string materials { get; set; } = string.Empty;
        [Ignore]
        public int sets { get; set; } = -1;
        [Ignore]
        public int reps { get; set; } = -1;
        [Ignore]
        public int seconds { get; set; } = -1;
        private bool _expaded;
        private bool _exerciseFinished;
        [Ignore]
        public bool expaded
        {
            get => _expaded;
            set
            {
                if (_expaded != value)
                {
                    _expaded = value;
                    OnPropertyChanged(nameof(expaded));
                }
            }
        }
        [Ignore]
        public bool exerciseFinished
        {
            get => _exerciseFinished;
            set
            {
                if (_exerciseFinished != value)
                {
                    _exerciseFinished = value;
                    OnPropertyChanged(nameof(exerciseFinished));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        [Ignore]
        public DoubleCollection LinePattern { get; set; } = new() { 6, 4 }; // Patrón de línea discontinua si no es el ultimo ejercicio
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
                    _ => false,
                };
            }
        }

        public Brush AuraColor // propiedad calculada para obtener el pincel de degradado según la dificultad
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
        public Color DifficultyColor => dificulty switch
        {
            dificultyEnum.easy => Colors.Green,
            dificultyEnum.medium => Colors.Orange,
            dificultyEnum.hard => Colors.Red,
            dificultyEnum.extreme => Colors.Purple,
            _ => Colors.Gray
        };
        // crea un pincel de degradado radial basado en dos colores
        private static RadialGradientBrush CreateBrush(Color color1, Color color2)
        {
            return new RadialGradientBrush //crea el pincel
            {
                Center = new Point(0.5, 0.5), // centro del degradado
                Radius = 0.9, //radio del degradado
                GradientStops = new GradientStopCollection //colección de paradas de degradado
                {
                    new GradientStop(Colors.Transparent, 0.3f),
                    new GradientStop(color1, 0.8f),
                    new GradientStop(color2, 1f)
                }
            };
        }
        public Exercise Clone()
        {
            return new Exercise
            {
                execiseID = this.execiseID,
                name = this.name,
                description = this.description,
                image = this.image,
                video = this.video,
                dificulty = this.dificulty,
                muscleGroupId = this.muscleGroupId,
                typeUser = this.typeUser,
                reps = this.reps,
                sets = this.sets,
                seconds = this.seconds,
                exerciseFinished = false,
                expaded = false,
                IsAdmin = this.IsAdmin
            };
        }
    }
    }
