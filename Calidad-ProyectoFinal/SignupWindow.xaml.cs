using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Calidad_ProyectoFinal
{
    /// <summary>
    /// Interaction logic for SignupWindow.xaml
    /// </summary>
    public partial class SignupWindow : Window
    {
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="owner">Used to position this window relative to its owner</param>
        public SignupWindow(Window owner)
        {
            InitializeComponent();
            Owner = owner;
        }

        /// <summary>
        /// Validates email format and error existance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UsernameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Owner is null) return;
            UsernameError.Text = !Validation.IsValidEmail(UsernameBox.Text) ? "Invalid email format." : string.Empty;
            SignupButton.IsEnabled = !Validation.DoErrorsExist(this);
        }

        /// <summary>
        /// Validates password format and error existance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (Owner is null) return;
            PasswordError.Text = !Validation.IsValidPassword(PasswordBox.Password) ? "Invalid password:\n • Minimum 5 characters\n • At least one capital\n • At least one special character" : string.Empty;
            RepeatPasswordBox_PasswordChanged(sender, e);
        }

        /// <summary>
        /// Validates matching passwords and error existance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RepeatPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (Owner is null) return;
            RepeatPasswordError.Text = !Validation.DoPasswordsMatch(PasswordBox.Password, RepeatPasswordBox.Password) ? "Passwords do not match." : string.Empty;
            SignupButton.IsEnabled = !Validation.DoErrorsExist(this);
        }

        /// <summary>
        /// Calls Firebase Auth API sign-up endpoint
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Signup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await FirebaseAuthService.SignUpAsync(UsernameBox.Text, PasswordBox.Password, null);
                await FirebaseAuthService.UpdateProfileAsync(CurrentUserData.GetIdToken(), DisplaynameBox.Text, null);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
