using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calidad_ProyectoFinal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Basic constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Navigates to Login Window on load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayLoginWindow();
        }

        /// <summary>
        /// Logs the current user out of the app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            userGreeting.Text = string.Empty;
            CurrentUserData.Logout();
            DisplayLoginWindow();
        }

        /// <summary>
        /// Navigates to Login Window
        /// </summary>
        private void DisplayLoginWindow()
        {
            LoginWindow loginWindow = new(this);
            loginWindow.ShowDialog();
            if (CurrentUserData.GetIdToken() == string.Empty) Close();
            else userGreeting.Text = $"Welcome, {CurrentUserData.GetDisplayName()}!";
        }
    }
}