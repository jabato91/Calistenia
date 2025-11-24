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
        public static string ToFriendlyName(this bodyPartEnum part) // Extension method for bodyPartEnum
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
        public static string ToDificultyName(this dificultyEnum part) // Extension method for dificultyEnum
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
        public static readonly Dictionary<bodyPartEnum, string> BodyTranslations = new()// Traduccion de bodyPartEnum al español
    {
        { bodyPartEnum.nothing, "Ninguno" },
        { bodyPartEnum.chest, "Pecho" },
        { bodyPartEnum.leg, "Piernas" },
        { bodyPartEnum.triceps, "Tríceps" },
        { bodyPartEnum.biceps, "Bíceps" },
        { bodyPartEnum.abdomen, "Abdomen" },
        { bodyPartEnum.back, "Espalda" },
        { bodyPartEnum.shoulder, "Hombros" },
        { bodyPartEnum.isometric, "Isométrico" },
        { bodyPartEnum.arms, "Brazos" },
        { bodyPartEnum.torso, "Torso" },
        { bodyPartEnum.torsoAndArms, "Torso y Brazos" }
    };

        public  static readonly Dictionary<dificultyEnum, string> DifficultyTranslations = new()// Traduccion de dificultyEnum al español
        {
            { dificultyEnum.nothing, "Ninguno" },
            { dificultyEnum.easy, "Fácil" },
            { dificultyEnum.medium, "Medio" },
            { dificultyEnum.hard, "Difícil" },
            { dificultyEnum.extreme, "Muy Difícil" }
        };
        public static readonly Dictionary<userTypeEnum, string> userTypeTranslations = new() //traduccion de userTypeEnum al español
        {
            { userTypeEnum.nothing, "Ninguno" },
            { userTypeEnum.user, "Usuario" },
            { userTypeEnum.admin, "Administrador" },
        };

    }
}
