using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Calidad_ProyectoFinal.Tests
{
    public class LoginWindowTests :UiTestBase
    {
        /// <summary> Tests that valid credentials allow the user to continue to the main window </summary>
        [Fact]
        public void Login_Click_ValidCredentials_ContinueToMainWindow()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var window = new LoginWindow(fakeMessageService, true);
                    window.UsernameBox.Text = "azunigamo@ucenfotec.ac.cr";
                    window.PasswordBox.Password = "A1234!";
                    window.LoginAsync().GetAwaiter().GetResult();
                    Assert.Equal(DialogState.Succeeded, window.State);
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

        /// <summary> Tests that invalid credentials display an error message </summary>
        [Fact]
        public void Login_Click_InvalidCredentials_DisplayError()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var window = new LoginWindow(fakeMessageService, true);
                    window.UsernameBox.Text = "test@example.com";
                    window.PasswordBox.Password = "12345";
                    window.LoginAsync().GetAwaiter().GetResult();
                    Assert.Equal("Error", fakeMessageService.LastCaption);
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

        /// <summary> Tests that submitting an empty form displays an error message </summary>
        [Fact]
        public void Login_Click_EmptyForm_DisplayError()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var window = new LoginWindow(fakeMessageService, true);
                    window.LoginAsync().GetAwaiter().GetResult();
                    Assert.Equal("Error", fakeMessageService.LastCaption);
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

        /// <summary> Tests that clicking the signup button opens the signup window </summary>
        [Fact]
        public void Signup_Click_DisplaySignupWindow()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var window = new LoginWindow(fakeMessageService, true);
                    var dialogState = window.DisplaySignupWindow();
                    Assert.Equal(DialogState.Loaded, dialogState);
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

        /// <summary> Tests that clicking the forgot password button opens the forgot password window </summary>
        [Fact]
        public void ForgotPassword_Click_DisplayForgotPasswordWindow()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var window = new LoginWindow(fakeMessageService, true);
                    var dialogState = window.DisplayForgotPasswordWindow();
                    Assert.Equal(DialogState.Loaded, dialogState);
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
