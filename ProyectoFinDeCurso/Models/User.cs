using ProyectoFinDeCurso.Enums;
using SQLite;

namespace ProyectoFinDeCurso.Models
{
    [Table("User")]
    public class User
    {
        
        [PrimaryKey, AutoIncrement, Column("Id")]
        public int UserID { get; set; }

        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [Column("FirstSurname")]
        public string FirstSurname { get; set; } = string.Empty;

        [Column("SecondSurname")]
        public string SecondSurname { get; set; } = string.Empty;

        [Column("Email")]
        public string Email { get; set; } = string.Empty;

        [Column("Password")]
        public string Password { get; set; } = string.Empty;

        [MaxLength(9), Column("Phone")]
        public string Phone { get; set; } = string.Empty;

        [Column("UserType")]
        public userTypeEnum userType { get; set; } = userTypeEnum.nothing;

        [Ignore] // Para que SQLite no intente mapearla
        public string FullName => $"{Name} {FirstSurname} {SecondSurname}".Trim();
        
       
    }
}