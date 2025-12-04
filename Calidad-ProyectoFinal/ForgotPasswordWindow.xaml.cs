using System.Windows;
using System.Windows.Controls;

namespace Calidad_ProyectoFinal
{
    /// <summary> Interaction logic for ForgotPasswordWindow.xaml </summary>
    public partial class ForgotPasswordWindow : Window
    {
        /// <summary> Service used to show messages to the user </summary>
        private readonly IMessageService _messageService;

        /// <summary> Represents the current state of the dialog </summary>
        public DialogState State { get; private set; } = DialogState.Uninitialized;

        /// <summary> Indicates whether the window is in testing mode </summary>
        public bool IsTesting { get; private set; } = false;

        /// <summary> Window constructor </summary>
        /// <param name="messageService">Message Service interface (for mocking purposes)</param>
        public ForgotPasswordWindow(IMessageService messageService)
        {
            InitializeComponent();
            _messageService = messageService;
            State = DialogState.Initialized;
        }

        /// <summary> Window constructor with testing flag </summary>
        /// <param name="messageService"></param>
        /// <param name="isTesting"> Indicates whether the window is in testing mode </param>
        public ForgotPasswordWindow(IMessageService messageService, bool isTesting) : this(messageService)
        {
            IsTesting = isTesting;
        }

        /// <summary> Window constructor with owner </summary>
        /// <param name="messageService"></param>
        /// <param name="isTesting"></param>
        /// <param name="owner"> Used to position this window relative to its parent </param>
        public ForgotPasswordWindow(IMessageService messageService, bool isTesting, Window? owner) : this(messageService, isTesting)
        {
            Owner = owner;
        }

        /// <summary> Calls email validation on text change </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UsernameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsInitialized) return;
            ValidateEmail();
        }

        /// <summary> Validates email format and error existance in window elements </summary>
        public void ValidateEmail()
        {
            UsernameError.Text = !Validation.IsValidEmail(UsernameBox.Text) ? "Invalid email format." : string.Empty;
            ResetPasswordButton.IsEnabled = !Validation.DoErrorsExist(Validation.GetErrorMessagesVisibleStates(((StackPanel)Content).Children));
        }

        /// <summary> Calls password reset function on button click </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PasswordReset_Click(object sender, RoutedEventArgs e)
        {
            await ResetPasswordAsync();

            if (!State.Equals(DialogState.Succeeded)) return;
            _messageService.ShowInfo($"If the provided email — {UsernameBox.Text} — is registered in our system, you will receive a secure link to reset your password within a few minutes.\n\nPlease check your spam folder as well.");
            Close();
        }

        /// <summary> Uses Firebase Auth API to send password reset email to the user </summary>
        /// <returns></returns>
        public async Task ResetPasswordAsync()
        {
            ValidateEmail();
            if (!ResetPasswordButton.IsEnabled) return;

            try
            {
                await FirebaseAuthService.ResetPasswordAsync(UsernameBox.Text, null);
                //_messageService.ShowInfo($"If the provided email — {UsernameBox.Text} — is registered in our system, you will receive a secure link to reset your password within a few minutes.\n\nPlease check your spam folder as well.");
                State = DialogState.Succeeded;
                //Close();
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
            if (IsTesting) Close();
        }

        /// <summary> Sets dialog state on close </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsTesting) return;
            if (State != DialogState.Succeeded) State = DialogState.Canceled;
        }

        /// <summary> Fills form with demo data on click </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Demo_Click(object sender, RoutedEventArgs e)
        {
            UsernameBox.Text = "azunigamo@ucenfotec.ac.cr";
        }
    }
}
