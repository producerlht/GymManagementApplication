using GymManagementApplication.Models;
using System.Configuration;
using System.Data;
using System.Windows;

namespace GymManagementApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var loginWindow = new LoginWindow();
            bool? result = loginWindow.ShowDialog(); 

            if (result == true && loginWindow.Tag is User user)
            {

                var mainWindow = new MainWindow(user);
                Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                Application.Current.MainWindow = mainWindow;
                mainWindow.Show();
            }
            else
            {
                Application.Current.Shutdown(); 
            }
        }
    } 
}
