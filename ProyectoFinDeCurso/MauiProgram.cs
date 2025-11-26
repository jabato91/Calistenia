using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using ProyectoFinDeCurso.Pages;
using ProyectoFinDeCurso.Pages.Authentication;
using ProyectoFinDeCurso.Services;
using SQLitePCL;


namespace ProyectoFinDeCurso
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            Batteries_V2.Init(); //inicia sqlite antes de culquier uso

            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()                    // llama a la librería de herramientas
                .UseMauiCommunityToolkitMediaElement()        // llama a la librería de video
                .ConfigureFonts(fonts =>
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
                    fonts.AddFont("Comfortaa-Bold.ttf", "ComfortaaBold");
                    fonts.AddFont("Comfortaa-Light.ttf", "ComfortaaLight");
                    fonts.AddFont("Comfortaa-Regular.ttf", "ComfortaaRegular");
                    fonts.AddFont("cream-DEMO.ttf", "CreamDEMO");
                    fonts.AddFont("FNCocoSans-Thin.ttf", "FNCocoSansThin");
                    fonts.AddFont("Garet-Book.ttf", "GaretBook");
                    fonts.AddFont("Garet-Heavy.ttf", "GaretHeavy");
                    fonts.AddFont("LiberationSans-Bold.ttf", "LiberationSansBold");
                    fonts.AddFont("LiberationSans-BoldItalic.ttf", "LiberationSansBoldItalic");
                    fonts.AddFont("LiberationSans-Italic.ttf", "LiberationSansItalic");
                    fonts.AddFont("LiberationSans-Regular.ttf", "LiberationSansRegular");
                    fonts.AddFont("Louis George Cafe Bold Italic.ttf", "LouisGeorgeCafeBoldItalic");
                    fonts.AddFont("Louis George Cafe Light Italic.ttf", "LouisGeorgeCafeLightItalic");
                    fonts.AddFont("Louis George Cafe Light.ttf", "LouisGeorgeCafeLight");
                    fonts.AddFont("Louis George Cafe.ttf", "LouisGeorgeCafe");
                    fonts.AddFont("Nearo-Bold.ttf", "NearoBold");
                    fonts.AddFont("Nearo-Light.ttf", "NearoLight");
                    fonts.AddFont("Nearo-Medium.ttf", "NearoMedium");
                    fonts.AddFont("Nearo-Regular.ttf", "NearoRegular");
                    fonts.AddFont("Nearo-SemiBold.ttf", "NearoSemiBold");
                    fonts.AddFont("Walkway Black RevOblique.ttf", "WalkwayBlackRevOblique");
                    fonts.AddFont("Walkway Black.ttf", "WalkwayBlack");
                    fonts.AddFont("Walkway Bold RevOblique.ttf", "WalkwayBoldRevOblique");
                });

            // 🔹 Inyección de dependencias
            builder.Services.AddSingleton<DbService>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}