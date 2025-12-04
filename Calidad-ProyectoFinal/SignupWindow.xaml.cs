using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Calidad_ProyectoFinal
{
    /// <summary> Custom enum to track dialog windows state </summary>
    public enum DialogState
    {
        Uninitialized,
        Initialized,
        Loaded,
        Canceled,
        Succeeded,
    }

    /// <summary> Interaction logic for SignupWindow.xaml </summary>
    public partial class SignupWindow : Window
    {
        /// <summary> Service used to show messages to the user </summary>
        private readonly IMessageService _messageService;

        /// <summary> Tracks dialog window state </summary>
        public DialogState State { get; private set; } = DialogState.Uninitialized;

        /// <summary> Indicates if the window is being used for testing purposes </summary>
        public bool IsTesting { get; private set; } = false;
        
        /// <summary> Window contructor </summary>
        /// <param name="messageService">Message service interface (for mocking purposes)</param>
        public SignupWindow(IMessageService messageService)
        {
            InitializeComponent();
            _messageService = messageService;
            State = DialogState.Initialized;
        }

        /// <summary> Window contructor with testing flag </summary>
        /// <param name="messageService"></param>
        /// <param name="isTesting"> Indicates if the window is being used for testing purposes </param>
        public SignupWindow(IMessageService messageService, bool isTesting) : this(messageService)
        {
            IsTesting = isTesting;
        }

        /// <summary> Window contructor with owner </summary>
        /// <param name="messageService"></param>
        /// <param name="isTesting"></param>
        /// <param name="owner"> Used to position this window relative to its parent </param>
        public SignupWindow(IMessageService messageService, bool isTesting, Window? owner) : this(messageService, isTesting)
        {
            Owner = owner;
        }

        /// <summary> Validates email format on text change </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UsernameBox_TextChanged(object sender, TextChangedEventArgs? e)
        {
            if (State == DialogState.Initialized) return;
            ValidateEmail();
        }

        /// <summary> Validates email format and error existance in window elements </summary>
        public void ValidateEmail()
        {
            UsernameError.Text = !Validation.IsValidEmail(UsernameBox.Text) ? "Invalid email format." : string.Empty;
            SignupButton.IsEnabled = !Validation.DoErrorsExist(Validation.GetErrorMessagesVisibleStates(((StackPanel)Content).Children));
        }

        /// <summary> Validates password format on password change </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (State == DialogState.Initialized) return;
            ValidatePassword();
        }

        /// <summary> Validates password format and error existance in window elements </summary>
        public void ValidatePassword()
        {
            PasswordError.Text = !Validation.IsValidPassword(PasswordBox.Password) ? "Invalid password:\n • Minimum 5 characters\n • At least one capital\n • At least one special character" : string.Empty;
            ValidateMatchingPasswords();
        }

        /// <summary> Validates matching passwords on password change </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RepeatPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (State == DialogState.Initialized) return;
            ValidateMatchingPasswords();
        }

        /// <summary> Validates matching passwords and error existance </summary>
        public void ValidateMatchingPasswords()
        {
            RepeatPasswordError.Text = !Validation.DoPasswordsMatch(PasswordBox.Password, RepeatPasswordBox.Password) ? "Passwords do not match." : string.Empty;
            SignupButton.IsEnabled = !Validation.DoErrorsExist(Validation.GetErrorMessagesVisibleStates(((StackPanel)Content).Children));
        }

        /// <summary> Calls Firebase Auth API sign-up endpoint </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Signup_Click(object sender, RoutedEventArgs e)
        {
            await SignupAsync();
        }

        public async Task SignupAsync()
        {
            ValidateEmail();
            ValidatePassword();
            if (!SignupButton.IsEnabled) return;

            try
            {
                var email = UsernameBox.Text;
                var password = PasswordBox.Password;
                var displayName = DisplaynameBox.Text;
                await FirebaseAuthService.SignUpAsync(email, password, null);
                await FirebaseAuthService.UpdateProfileAsync(CurrentUserData.GetIdToken(), displayName, null);
                if(State == DialogState.Loaded) Close();
                State = DialogState.Succeeded;
            }
            catch (Exception ex)
            {
                _messageService.ShowError(ex.Message);
            }
        }

        /// <summary> Sets dialog state on load </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            State = DialogState.Loaded;
            if(IsTesting) Close();
        }

        /// <summary> Sets dialog state on close </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if(IsTesting) return;
            if (State != DialogState.Succeeded) State = DialogState.Canceled;
        }

        /// <summary> Fills the fields with demo data </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Demo_Click(object sender, RoutedEventArgs e)
        {
            DisplaynameBox.Text = "Aarón";
            UsernameBox.Text = "azunigamo@ucenfotec.ac.cr";
            PasswordBox.Password = "A1234!";
            RepeatPasswordBox.Password = PasswordBox.Password;
        }
    }
}
