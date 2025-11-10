using ProyectoFinDeCurso.Flyout;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Pages;
using ProyectoFinDeCurso.Services;
namespace ProyectoFinDeCurso
{
    public partial class App : Application
    {
        [Obsolete]
        public App(LoginPage loginPage)
        {
            InitializeComponent();

            MainPage = new NavigationPage(new LoginPage(new DbService()));
        }

        protected override async void OnStart()
        {
            try { 
                var userId = await SecureStorage.GetAsync("user_id");
                DbService _dbService = new DbService();
                User user = await _dbService.GetUserById(int.Parse(userId));
                if (!string.IsNullOrEmpty(userId))
                {
                    
                    MainPage = new userFlyoutPage(new DbService(), userId, user.userType);
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
