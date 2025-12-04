using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Xunit;

namespace Calidad_ProyectoFinal.Tests
{
    public class ForgotPasswordWindowTests : UiTestBase
    {
        [Fact]
        public void UsernameBox_TextChanged_InvalidEmail_ShowsErrorAndDisablesButton()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var window = new ForgotPasswordWindow(fakeMessageService);
                    window.UsernameBox.Text = "invalid-email";

                    window.ValidateEmail();

                    Assert.Equal("Invalid email format.", window.UsernameError.Text);
                    Assert.False(window.ResetPasswordButton.IsEnabled);
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
        public void UsernameBox_TextChanged_ValidEmail_HidesErrorAndEnablesButton()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var window = new ForgotPasswordWindow(fakeMessageService);
                    window.UsernameBox.Text = "invalid-email";

                    window.ValidateEmail();

                    Assert.Equal("Invalid email format.", window.UsernameError.Text);
                    Assert.False(window.ResetPasswordButton.IsEnabled);

                    window.UsernameBox.Text = "test@example.com";

                    window.ValidateEmail();

                    Assert.Equal(string.Empty, window.UsernameError.Text);
                    Assert.True(window.ResetPasswordButton.IsEnabled);
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
        public void PasswordReset_Click_EmptyForm_DoesNotCallReset()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var window = new ForgotPasswordWindow(fakeMessageService);

                    window.ResetPasswordAsync().GetAwaiter().GetResult();

                    Assert.Equal("Invalid email format.", window.UsernameError.Text);
                    Assert.False(window.ResetPasswordButton.IsEnabled);
                    Assert.Null(fakeMessageService.LastMessage);
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
        public void PasswordReset_Click_ValidEmail_CallReset()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var window = new ForgotPasswordWindow(fakeMessageService);
                    window.UsernameBox.Text = "azunigamo@ucenfotec.ac.cr";

                    window.ResetPasswordAsync().GetAwaiter().GetResult();

                    Assert.Equal(DialogState.Succeeded, window?.State);
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