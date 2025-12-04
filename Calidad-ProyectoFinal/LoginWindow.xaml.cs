using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
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
    /// <summary> Interaction logic for LoginWindow.xaml </summary>
    public partial class LoginWindow : Window
    {
        /// <summary> Service used to show messages to the user </summary>
        private readonly IMessageService _messageService;

        /// <summary> Represents the current state of the dialog </summary>
        public DialogState State { get; private set; } = DialogState.Uninitialized;

        /// <summary> Indicates whether the window is in testing mode </summary>
        public bool IsTesting { get; private set; } = false;

        /// <summary> Window constructor </summary>
        /// <param name="messageService">Message service interface (for mocking purposes)</param>
        public LoginWindow(IMessageService messageService)
        {
            InitializeComponent();
            _messageService = messageService;
        }

        /// <summary> Window constructor with testing flag </summary>
        /// <param name="messageService"></param>
        /// <param name="isTesting">Changes some screen behaviors</param>
        public LoginWindow(IMessageService messageService, bool isTesting) : this(messageService)
        {
            IsTesting = isTesting;
        }

        /// <summary> Window constructor with owner </summary>
        /// <param name="messageService"></param>
        /// <param name="isTesting"></param>
        /// <param name="owner">Positions this window relative to its parent</param>
        public LoginWindow(IMessageService messageService, bool isTesting, Window? owner) : this(messageService, isTesting)
        {
            Owner = owner;
        }

        /// <summary> Calls login function on button click </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            await LoginAsync();
        }

        /// <summary> Uses Firebase Auth API to log the user in </summary>
        /// <returns></returns>
        public async Task LoginAsync()
        {
            try
            {
                await FirebaseAuthService.LoginAsync(UsernameBox.Text, PasswordBox.Password, null);
                if(State == DialogState.Loaded) Close();
                State = DialogState.Succeeded;
            }
            catch (Exception ex)
            {
                _messageService.ShowError(ex.Message);
            }
        }

        /// <summary> Navigates to Sign-Up Window on button click </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Signup_Click(object sender, RoutedEventArgs e)
        {
            DisplaySignupWindow();
        }

        /// <summary> Navigates to Sign-Up Window </summary>
        /// <returns> Dialog state for child window </returns>
        public DialogState DisplaySignupWindow()
        {
            SignupWindow signupWindow = new(new MessageBoxService(), IsTesting, Owner);
            WindowState = WindowState.Minimized;
            Owner?.Activate();
            signupWindow.ShowDialog();
            WindowState = WindowState.Normal;
            if (CurrentUserData.GetIdToken() != string.Empty)
            {
                State = DialogState.Succeeded;
                Close();
            }
            return signupWindow.State;
        }

        /// <summary> Navigates to Password Reset Window on button click </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            DisplayForgotPasswordWindow();
        }

        /// <summary> Navigates to Password Reset Window </summary>
        /// <returns> Dialog state for child window </returns>
        public DialogState DisplayForgotPasswordWindow()
        {
            ForgotPasswordWindow forgotPasswordWindow = new(new MessageBoxService(), IsTesting, Owner);
            WindowState = WindowState.Minimized;
            Owner?.Activate();
            forgotPasswordWindow.ShowDialog();
            WindowState = WindowState.Normal;
            return forgotPasswordWindow.State;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            State = DialogState.Loaded;
            if (IsTesting)
            {
                Demo_Click(sender, e);
                Login_Click(sender, e);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsTesting) return;
            if (State != DialogState.Succeeded) State = DialogState.Canceled;
        }

        /// <summary> Fills in demo credentials on button click </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Demo_Click(object sender, RoutedEventArgs e)
        {
            UsernameBox.Text = "azunigamo@ucenfotec.ac.cr";
            PasswordBox.Password = "A1234!";
        }
    }
}
