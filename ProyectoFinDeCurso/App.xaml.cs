using ProyectoFinDeCurso.Pages;

namespace ProyectoFinDeCurso
{
    public partial class App : Application
    {
        public App(LoginPage loginPage)
        {
            InitializeComponent();

            MainPage = new NavigationPage(loginPage);
        }
    }
}
