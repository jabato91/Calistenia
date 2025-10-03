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
        [Column("BodyPart")]
        public bodyPartEnum muscleGroupId { get; set; } = bodyPartEnum.nothing;
       
    }
}
