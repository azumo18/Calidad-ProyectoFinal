using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calidad_ProyectoFinal.Tests
{
    public class SignupWindowTests : UiTestBase
    {
        [Fact]
        public void SignUp_Click_ValidCredentials_ContinueToMainWindow()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var window = new SignupWindow(fakeMessageService, true);
                    window.DisplaynameBox.Text = "Test User";
                    window.UsernameBox.Text = "test@example.com";
                    window.PasswordBox.Password = "A1234!";
                    window.RepeatPasswordBox.Password = window.PasswordBox.Password;
                    
                    window.SignupAsync().GetAwaiter().GetResult();

                    //Conditional assertion based on whether the user already exists on live database
                    if (window.State == DialogState.Initialized) Assert.Equal("Signup failed: EMAIL_EXISTS", fakeMessageService.LastMessage);
                    else Assert.Equal(DialogState.Succeeded, window.State); // True happy path
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (exception != null) throw exception;
        }

        [Fact]
        public void SignUp_Click_OnEmptyForm_ValidatesFieldsAndDisablesButton()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var window = new SignupWindow(fakeMessageService, true);

                    window.SignupAsync().GetAwaiter().GetResult();

                    Assert.NotEmpty(window.UsernameError.Text);
                    Assert.NotEmpty(window.PasswordError.Text);
                    Assert.Empty(window.RepeatPasswordError.Text);
                    Assert.False(window.SignupButton.IsEnabled);
                    Assert.Equal(DialogState.Initialized, window.State); // Sign-up aborted
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (exception != null) throw exception;
        }

        [Fact]
        public void Username_TextChange_DisplaysErrorAndDisablesButton_OnInvalidEmail()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var window = new SignupWindow(fakeMessageService, true);
                    window.UsernameBox.Text = "invalid-email";

                    window.ValidateEmail();

                    Assert.Equal("Invalid email format.", window.UsernameError.Text);
                    Assert.False(window.SignupButton.IsEnabled);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (exception != null) throw exception;
        }

        [Fact]
        public void Username_TextChange_RemoveErrorAndEnableButton_OnValidEmail()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var window = new SignupWindow(fakeMessageService, true);
                    window.UsernameBox.Text = "invalid-email";

                    window.ValidateEmail();

                    Assert.Equal("Invalid email format.", window.UsernameError.Text);
                    Assert.False(window.SignupButton.IsEnabled);

                    window.UsernameBox.Text = "test@example.com";

                    window.ValidateEmail();

                    Assert.Empty(window.UsernameError.Text);
                    Assert.True(window.SignupButton.IsEnabled);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (exception != null) throw exception;
        }

        [Fact]
        public void Username_TextChange_RemoveErrorAndKeepButtonDisabled_OnValidEmailButErrorsFound()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var window = new SignupWindow(fakeMessageService, true);
                    window.UsernameBox.Text = "invalid-email";
                    window.PasswordBox.Password = "invalid";

                    window.ValidateEmail();
                    window.ValidatePassword();

                    Assert.NotEmpty(window.UsernameError.Text);
                    Assert.NotEmpty(window.PasswordError.Text);
                    Assert.False(window.SignupButton.IsEnabled);

                    window.UsernameBox.Text = "test@example.com";

                    window.ValidateEmail();

                    Assert.Empty(window.UsernameError.Text);
                    Assert.NotEmpty(window.PasswordError.Text);
                    Assert.False(window.SignupButton.IsEnabled);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (exception != null) throw exception;
        }

        [Fact]
        public void Password_TextChanged_DisplaysErrorAndDisablesButton_OnInvalidPassword()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var window = new SignupWindow(fakeMessageService, true);
                    window.PasswordBox.Password = "invalid";
                    window.RepeatPasswordBox.Password = window.PasswordBox.Password;

                    window.ValidatePassword();

                    Assert.NotEmpty(window.PasswordError.Text);
                    Assert.False(window.SignupButton.IsEnabled);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (exception != null) throw exception;
        }

        [Fact]
        public void Password_TextChanged_RemoveErrorAndEnableButton_OnValidPassword()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var window = new SignupWindow(fakeMessageService, true);
                    window.PasswordBox.Password = "invalid";
                    window.RepeatPasswordBox.Password = window.PasswordBox.Password;

                    window.ValidatePassword();

                    Assert.NotEmpty(window.PasswordError.Text);
                    Assert.False(window.SignupButton.IsEnabled);

                    window.PasswordBox.Password = "Zxcv@";
                    window.RepeatPasswordBox.Password = window.PasswordBox.Password;

                    window.ValidatePassword();

                    Assert.Empty(window.PasswordError.Text);
                    Assert.True(window.SignupButton.IsEnabled);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (exception != null) throw exception;
        }

        [Fact]
        public void Password_TextChanged_RemoveErrorAndKeepButtonDisabled_OnValidPasswordButErrorsFound()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var window = new SignupWindow(fakeMessageService, true);
                    window.UsernameBox.Text = "invalid-email";
                    window.PasswordBox.Password = "invalid";
                    window.RepeatPasswordBox.Password = window.PasswordBox.Password;

                    window.ValidateEmail();
                    window.ValidatePassword();

                    Assert.NotEmpty(window.UsernameError.Text);
                    Assert.NotEmpty(window.PasswordError.Text);
                    Assert.False(window.SignupButton.IsEnabled);

                    window.PasswordBox.Password = "Zxcv@";
                    window.RepeatPasswordBox.Password = window.PasswordBox.Password;

                    window.ValidatePassword();

                    Assert.NotEmpty(window.UsernameError.Text);
                    Assert.Empty(window.PasswordError.Text);
                    Assert.False(window.SignupButton.IsEnabled);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (exception != null) throw exception;
        }

        [Fact]
        public void RepeatPassword_TextChanged_DisplaysErrorAndDisablesButton_OnDifferentPasswords()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var window = new SignupWindow(fakeMessageService, true);
                    window.PasswordBox.Password = "password";
                    window.RepeatPasswordBox.Password = "other";

                    window.ValidateMatchingPasswords();

                    Assert.NotEmpty(window.RepeatPasswordError.Text);
                    Assert.False(window.SignupButton.IsEnabled);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (exception != null) throw exception;
        }

        [Fact]
        public void RepeatPassword_TextChanged_RemoveErrorAndEnablesButton_OnMatchingPasswords()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var window = new SignupWindow(fakeMessageService, true);
                    window.PasswordBox.Password = "Zxcv@";
                    window.RepeatPasswordBox.Password = "other";

                    window.ValidatePassword();

                    Assert.Empty(window.PasswordError.Text);
                    Assert.NotEmpty(window.RepeatPasswordError.Text);
                    Assert.False(window.SignupButton.IsEnabled);

                    window.RepeatPasswordBox.Password = window.PasswordBox.Password;

                    window.ValidateMatchingPasswords();

                    Assert.Empty(window.RepeatPasswordError.Text);
                    Assert.True(window.SignupButton.IsEnabled);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (exception != null) throw exception;
        }

        [Fact]
        public void RepeatPassword_TextChanged_RemoveErrorAndKeepButtonDisabled_OnMatchingPasswordsButErrorsFound()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var window = new SignupWindow(fakeMessageService, true);
                    window.UsernameBox.Text = "invalid-email";
                    window.PasswordBox.Password = "Zxcv@";
                    window.RepeatPasswordBox.Password = "other";

                    window.ValidateEmail();
                    window.ValidatePassword();

                    Assert.NotEmpty(window.UsernameError.Text);
                    Assert.Empty(window.PasswordError.Text);
                    Assert.NotEmpty(window.RepeatPasswordError.Text);
                    Assert.False(window.SignupButton.IsEnabled);

                    window.RepeatPasswordBox.Password = window.PasswordBox.Password;

                    window.ValidateMatchingPasswords();
                    
                    Assert.NotEmpty(window.UsernameError.Text);
                    Assert.Empty(window.RepeatPasswordError.Text);
                    Assert.False(window.SignupButton.IsEnabled);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (exception != null) throw exception;
        }
    }
}
