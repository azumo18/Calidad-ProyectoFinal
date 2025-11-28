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
    /// Interaction logic for ForgotPasswordWindow.xaml
    /// </summary>
    public partial class ForgotPasswordWindow : Window
    {
        /// <summary>
        /// Window constructor
        /// </summary>
        /// <param name="owner">Used to position this window relative to owner</param>
        public ForgotPasswordWindow(Window owner)
        {
            InitializeComponent();
            Owner = owner;
        }

        /// <summary>
        /// On event trigger validates email format and error existance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UsernameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Owner is null) return;
            UsernameError.Text = !Validation.IsValidEmail(UsernameBox.Text) ? "Invalid email format." : string.Empty;
            ResetPasswordButton.IsEnabled = !Validation.DoErrorsExist(this);
        }

        /// <summary>
        /// Uses Firebase Auth API to send password reset email to the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PasswordReset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await FirebaseAuthService.ResetPasswordAsync(UsernameBox.Text, null);
                MessageBox.Show("Password reset email send successfully!");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
