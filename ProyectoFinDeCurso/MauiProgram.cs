using Microsoft.Extensions.Logging;
using ProyectoFinDeCurso.Pages;
using ProyectoFinDeCurso.Pages.Authentication;
using ProyectoFinDeCurso.Services;
using CommunityToolkit.Maui;


namespace ProyectoFinDeCurso
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>().ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Madina.ttf", "Mandina"); 
                fonts.AddFont("Avenue de Madison.ttf", "AvenueMadison");
                fonts.AddFont("Ananda Personal Use.ttf", "AnandaPersonal");
                fonts.AddFont("Ananda Black Personal Use.ttf", "AnandaBlack");
            })
            .UseMauiCommunityToolkit();
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