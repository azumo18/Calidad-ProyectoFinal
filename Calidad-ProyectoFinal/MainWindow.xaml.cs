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
    /// <summary> Interaction logic for MainWindow.xaml </summary>
    public partial class MainWindow : Window
    {
        /// <summary> Indicates whether the window is in testing mode </summary>
        public bool IsTesting { get; private set; } = false;

        /// <summary> Window constructor </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary> Window constructor with testing flag </summary>
        /// <param name="isTesting"> Indicates whether the window is in testing mode </param>
        public MainWindow(bool isTesting) : this()
        {
            IsTesting = isTesting;
        }

        /// <summary> Navigates to Login Window on load </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayLoginWindow();
        }

        /// <summary> Navigates to Login Window. Welcomes the user on successful login </summary>
        public DialogState DisplayLoginWindow()
        {
            LoginWindow loginWindow = new(new MessageBoxService(), IsTesting, IsTesting ? null : this);
            loginWindow.ShowDialog();
            if (CurrentUserData.GetIdToken() == string.Empty) Close();
            else userGreeting.Text = $"Welcome{(!CurrentUserData.GetDisplayName().Trim().Equals("") ? $", {CurrentUserData.GetDisplayName().Trim()}" : "")}!";
            return loginWindow.State;
        }

        /// <summary> Logs the current user out of the app </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Logout();
        }

        public void Logout()
        {
            userGreeting.Text = string.Empty;
            CurrentUserData.Logout();
            if(!IsTesting) DisplayLoginWindow();
        }
    }
}