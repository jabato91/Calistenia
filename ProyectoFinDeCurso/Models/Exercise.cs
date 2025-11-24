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
            [PrimaryKey, AutoIncrement, Column("exerciseID")] // Identificador único del ejercicio
            public int execiseID { get; set; } // Identificador único del ejercicio
            [Column("Name")]
            public string name { get; set; } = string.Empty; // Nombre del ejercicio en la base de datos
        [Column("Description")]
            public string description { get; set; } = string.Empty; // Descripción del ejercicio en la base de datos
        [Column("image")]
            public string image { get; set; } = string.Empty; //Nombre de la imagen del ejercicio en la base de datos
        [Column("video")]
            public string video { get; set; } = string.Empty; //Nombre del video del ejercicio en la base de datos
        [Column("dificulty")]
            public dificultyEnum dificulty { get; set; } = dificultyEnum.nothing; // Dificultad del ejercicio en la base de datos
        [Column("BodyPart")]
            public bodyPartEnum muscleGroupId { get; set; } = bodyPartEnum.nothing; // Grupo muscular del ejercicio en la base de datos
        [Column("typeUser")]
            public userTypeEnum typeUser { get; set; } = userTypeEnum.nothing; // Tipo de usuario para el que está destinado el ejercicio en la base de datos
        [Column("Materials")]
            public string materials { get; set; } = string.Empty; // Materiales necesarios para el ejercicio en la base de datos
        [Ignore]
        public int sets { get; set; } = -1; // Número de series para el ejercicio
        [Ignore]
        public int reps { get; set; } = -1; // Número de repeticiones para el ejercicio
        [Ignore]
        public int seconds { get; set; } = -1; // Número de segundos para el ejercicio
        private bool _expaded; // Indica si el ejercicio está expandido en al entrar en la rutina
        private bool _exerciseFinished; // Indica si el ejercicio ha sido completado en la rutina
        [Ignore]
        public bool expaded // Indica si el ejercicio está expandido en la interfaz de usuario
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
        public bool exerciseFinished // Indica si el ejercicio ha sido completado en la rutina
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

        public event PropertyChangedEventHandler PropertyChanged; // Evento para notificar cambios en las propiedades
        protected void OnPropertyChanged(string propertyName) // Método para invocar el evento PropertyChanged
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        [Ignore]
        public DoubleCollection LinePattern { get; set; } = new() { 6, 4 }; // Patrón de línea discontinua si no es el ultimo ejercicio
        [Ignore]
        public bool IsAdmin { get; set; } = false; // Indica si el ejercicio está siendo visto en modo administrador

        

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
        public Color DifficultyColor => dificulty switch // propiedad calculada para obtener el color según la dificultad
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
        public Exercise Clone() // Método para clonar el ejercicio
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
