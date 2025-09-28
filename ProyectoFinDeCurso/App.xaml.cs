using ProyectoFinDeCurso.Pages;
using ProyectoFinDeCurso.Services;
namespace ProyectoFinDeCurso
{
    public partial class App : Application
    {
        public App(LoginPage loginPage)
        {
            InitializeComponent();

            MainPage = new NavigationPage(new LoginPage(new DbService()));
        }

        protected override async void OnStart()
        {
            try { 
                var userId = await SecureStorage.GetAsync("user_id");

                if(!string.IsNullOrEmpty(userId)) 
                {
                    MainPage = new NavigationPage(new HomePage());
                }
                else
                {
                    MainPage = new NavigationPage(new LoginPage(new DbService()));
                }
            }
            catch
            {
                MainPage = new NavigationPage(new LoginPage(new DbService()));
            }
        }
    }
}
