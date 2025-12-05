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
        public int routineID { get; set; }// Identificador único de la rutina en la base de datos
        [Column("NameRoutine")]
        public string nameRoutine { get; set; } = string.Empty; // Nombre de la rutina en la base de datos
        [Column("DescriptionRoutine")]
        public string description { get; set; } = string.Empty; // Descripción de la rutina en la base de datos
        [Column("imageRoutine")]
        public string image { get; set; } = string.Empty; // Nombre de la imagen de la rutina en la base de datos
        [Column("bodyPartRoutine")]
        public bodyPartEnum muscleGroup { get; set; } = bodyPartEnum.nothing; // Grupo muscular de la rutina en la base de datos
        [Column("typeUser")]
        public userTypeEnum typeUser { get; set; } = userTypeEnum.nothing; // Tipo de usuario para el que está destinada la rutina en la base de datos
        [Column("difficultyRoutine")]
        public dificultyEnum difficulty { get; set; } = dificultyEnum.nothing; // Dificultad de la rutina en la base de datos
        [Column("userID")]
        public int userID { get; set; } = -1;  // Identificador del usuario que creó la rutina en la base de datos
        [Ignore] 
        public ObservableCollection<Exercise> Exercises { get; set; } = new(); //obtiene los ejercicios de la rutina
        [Ignore]
        public int IsAdmin { get; set; } = -1; // Indica si el ejercicio está siendo visto en modo administrador
        [Ignore]
        public string TitleSetsOrTime { get; set; } = string.Empty; //título de la primera página de rutinas
        public bool IsAdminMode //tipos de acceso para cada tipo de usuario
        {
            get
            {
                return IsAdmin switch
                {
                    0 => false, // usuario normal
                    1 => true, // administrador
                    2 => true,//todos
                    _ => throw new NotImplementedException(),
                };
            }
        }
        [Ignore]
        public Brush AuraColor // propiedad calculada para obtener el pincel de degradado según la dificultad
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
        private Brush CreateBrush(Color start, Color end) // crea los colores del degradado
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
