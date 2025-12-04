using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calidad_ProyectoFinal.Tests
{
    public class MainWindowTests : UiTestBase
    {
        /// <summary> Loads the Main Window and displays the Login Window. Logs in a user and verifies greeting is displayed. </summary>
        [Fact]
        public void Window_Load_DisplayLoginWindowAndLogin()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var window = new MainWindow(true);

                    var dialogState = window.DisplayLoginWindow();

                    Assert.Equal(DialogState.Succeeded, dialogState);
                    Assert.NotEmpty(window.userGreeting.Text);
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

        /// <summary> Logs in a user, logs out and verifies greeting and user data are cleared. </summary>
        [Fact]
        public void Logout_Click_ClearGreetingAndUserData()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var window = new MainWindow(true);

                    var dialogState = window.DisplayLoginWindow();

                    Assert.Equal(DialogState.Succeeded, dialogState);
                    Assert.NotEmpty(window.userGreeting.Text);

                    window.Logout();

                    Assert.Empty(window.userGreeting.Text);
                    Assert.Empty(CurrentUserData.GetIdToken());
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
