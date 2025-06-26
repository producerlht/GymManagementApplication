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
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            LoginWindow loginWindow = new LoginWindow();
            bool? result = loginWindow.ShowDialog();

            if(result ==true)//login successful
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
            else //login failed or cancelled
            {
                Application.Current.Shutdown(); //close the application
            }
        }
    }

}
