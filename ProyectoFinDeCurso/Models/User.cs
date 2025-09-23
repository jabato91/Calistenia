using SQLite;

namespace ProyectoFinDeCurso.Models
{
    [Table("User")]
    public class User
    {
        [PrimaryKey, AutoIncrement, Column("id")]
        public int Id { get; set; }

        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [Column("FirstSurname")]
        public string FirstSurname { get; set; } = string.Empty;

        [Column("SecondSurname")]
        public string SecondSurname { get; set; } = string.Empty;

        [Column("Email")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(9), Column("Phone")]
        public string Phone { get; set; } = string.Empty;
    }
}