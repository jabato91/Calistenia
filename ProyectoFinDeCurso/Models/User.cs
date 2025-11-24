using ProyectoFinDeCurso.Enums;
using SQLite;

namespace ProyectoFinDeCurso.Models
{
    [Table("User")]
    public class User
    {
        
        [PrimaryKey, AutoIncrement, Column("Id")]
        public int UserID { get; set; } // Identificador único del usuario

        [Column("Name")]
        public string Name { get; set; } = string.Empty; // Nombre del usuario en la base de datos

        [Column("FirstSurname")]
        public string FirstSurname { get; set; } = string.Empty; // Primer apellido del usuario en la base de datos

        [Column("SecondSurname")]
        public string SecondSurname { get; set; } = string.Empty; // Segundo apellido del usuario en la base de datos

        [Column("Email")]
        public string Email { get; set; } = string.Empty; // Correo electrónico del usuario en la base de datos

        [Column("Password")]
        public string Password { get; set; } = string.Empty; // Contraseña del usuario en la base de datos

        [MaxLength(9), Column("Phone")]
        public string Phone { get; set; } = string.Empty; // Teléfono del usuario en la base de datos

        [Column("UserType")]
        public userTypeEnum userType { get; set; } = userTypeEnum.nothing; // Tipo de usuario en la base de datos

        [Ignore] // Para que SQLite no intente mapearla
        public string FullName => $"{Name} {FirstSurname} {SecondSurname}".Trim(); // Nombre completo del usuario

        [Ignore] // Para que SQLite no intente mapearla
        public string surNames => $"{FirstSurname} {SecondSurname}".Trim(); // apellidos del usuario

        public String listUserType //Traduccion del userTypeEnum al español
        {
            get { 
                return userType switch
                {
                    userTypeEnum.admin => "Administrador",
                    userTypeEnum.user => "Usuario",
                    userTypeEnum.nothing => "Nada",
                    _ => "Desconocido",
                };
            }
        }


    }
}