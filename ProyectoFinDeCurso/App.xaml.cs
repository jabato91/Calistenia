using ProyectoFinDeCurso.Pages;

namespace ProyectoFinDeCurso
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new LoginPage());
        }
    }
}
