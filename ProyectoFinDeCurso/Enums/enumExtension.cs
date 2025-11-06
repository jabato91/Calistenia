using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace ProyectoFinDeCurso.Enums
{
    public static class enumExtension
    {
        public static string ToFriendlyName(this bodyPartEnum part)
        {
            return part switch
            {
                bodyPartEnum.nothing => "Nada",
                bodyPartEnum.chest => "Pecho",
                bodyPartEnum.leg => "Piernas",
                bodyPartEnum.triceps => "Tríceps",
                bodyPartEnum.biceps => "Bíceps",
                bodyPartEnum.abdomen => "Abdomen",
                bodyPartEnum.back => "Espalda",
                bodyPartEnum.shoulder => "Hombro",
                bodyPartEnum.isometric => "Isométrico",
                bodyPartEnum.arms => "Brazos",
                bodyPartEnum.torso => "Torso",
                bodyPartEnum.torsoAndArms => "Torso y Brazos",
                _ => throw new NotImplementedException(),
            };
            
        }
        public static string ToDificultyName(this dificultyEnum part)
        {
            return part switch
            {
                dificultyEnum.nothing => "Nada",
                dificultyEnum.easy => "Facil",
                dificultyEnum.medium => "Medio",
                dificultyEnum.hard => "Dificil",
                dificultyEnum.extreme => "Muy Dificil",
                _ => throw new NotImplementedException(),
            };
        }
    }
}
