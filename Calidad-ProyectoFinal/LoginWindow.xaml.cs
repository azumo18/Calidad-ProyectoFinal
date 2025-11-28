using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Calidad_ProyectoFinal
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        /// <summary>
        /// Window constructor
        /// </summary>
        /// <param name="owner">User to position this window relative to its owner</param>
        public LoginWindow(Window owner)
        {
            InitializeComponent();
            Owner = owner;
        }

        /// <summary>
        /// Calls Firebase Auth API login endpoint
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await FirebaseAuthService.LoginAsync(UsernameBox.Text, PasswordBox.Password, null);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        /// <summary>
        /// Navigates to Sign-Up Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Signup_Click(object sender, RoutedEventArgs e)
        {
            SignupWindow signupWindow = new(this);
            WindowState = WindowState.Minimized;
            Owner.Activate();
            signupWindow.ShowDialog();
            WindowState = WindowState.Normal;
            if(CurrentUserData.GetIdToken() != string.Empty) Close();
        }

        /// <summary>
        /// Navigates to Password Reset Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            ForgotPasswordWindow forgotPasswordWindow = new(this);
            WindowState = WindowState.Minimized;
            Owner.Activate();
            forgotPasswordWindow.ShowDialog();
            WindowState = WindowState.Normal;
        }
    }
}
