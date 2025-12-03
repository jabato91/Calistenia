using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using ProyectoFinDeCurso.Pages;
using ProyectoFinDeCurso.Pages.Authentication;
using ProyectoFinDeCurso.Services;
using SQLitePCL;
using Plugin.LocalNotification;

namespace ProyectoFinDeCurso
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            Batteries_V2.Init(); //inicia sqlite antes de culquier uso

            var builder = MauiApp.CreateBuilder(); //crea el constructor de la app
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()                    // llama a la librería de herramientas
                .UseMauiCommunityToolkitMediaElement()        // llama a la librería de video //
                .UseLocalNotification()                       // llama a la librería de notificaciones locales
                .ConfigureFonts(fonts => //configura las fuentes de la app
                {
                    
                    //
                    fonts.AddFont("Eat Me Alive.ttf", "EatMeAlive");
                    //
                    fonts.AddFont("Forresten.ttf", "Forresten");
                    //
                    fonts.AddFont("Altone Trial-Bold.ttf", "AltoneTrialBold");
                    fonts.AddFont("Altone Trial-BoldOblique.ttf", "AltoneTrialBoldOblique");
                    fonts.AddFont("Altone Trial-Oblique.ttf", "AltoneTrialOblique");
                    fonts.AddFont("Altone Trial-Regular.ttf", "AltoneTrialRegular");
                    fonts.AddFont("Champagne & Limousines Bold Italic.ttf", "ChampagneYLimousinesBoldItalic");
                    fonts.AddFont("Champagne & Limousines Bold.ttf", "ChampagneYLimousinesBold");
                    fonts.AddFont("Champagne & Limousines Italic.ttf", "ChampagneYLimousinesItalic");
                    fonts.AddFont("Champagne & Limousines.ttf", "ChampagneYLimousines");
                    //
                    fonts.AddFont("Comfortaa-Bold.ttf", "ComfortaaBold");
                    //
                    fonts.AddFont("OpenSansRegular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSansSemibold.ttf", "OpenSansSemibold");

                });

            // 🔹 Inyección de dependencias
            builder.Services.AddSingleton<DbService>(); // Servicio de base de datos como singleton
            

#if DEBUG 
            builder.Logging.AddDebug(); //
#endif

            return builder.Build(); 
        }
    }
}