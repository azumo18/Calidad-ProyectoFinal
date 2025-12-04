using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Calidad_ProyectoFinal.Tests
{
    /// <summary>
    /// Pruebas de Integración End-to-End y User Acceptance Testing (UAT)
    /// Estas pruebas simulan flujos completos de usuario desde el inicio hasta el fin
    /// </summary>
    public class IntegrationAndUATTests : UiTestBase
    {
        #region Flujos Completos de Usuario (UAT)

        /// <summary>
        /// UAT: Flujo completo de registro de nuevo usuario
        /// Escenario: Un nuevo usuario se registra en el sistema exitosamente
        /// </summary>
        [Fact]
        public void UAT_NewUserRegistration_CompleteFlow()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    // GIVEN: El usuario está en la ventana de login
                    var fakeMessageService = new FakeMessageService();
                    var loginWindow = new LoginWindow(fakeMessageService, true);
                    
                    // WHEN: El usuario hace clic en "Sign Up"
                    var signupState = loginWindow.DisplaySignupWindow();
                    Assert.Equal(DialogState.Loaded, signupState);
                    
                    // THEN: Se muestra la ventana de registro
                    var signupWindow = new SignupWindow(fakeMessageService, true);
                    
                    // WHEN: El usuario completa el formulario con datos válidos
                    signupWindow.DisplaynameBox.Text = "New User Test";
                    signupWindow.UsernameBox.Text = $"newuser{DateTime.Now.Ticks}@example.com";
                    signupWindow.PasswordBox.Password = "NewUser1!";
                    signupWindow.RepeatPasswordBox.Password = "NewUser1!";
                    
                    // THEN: Todos los campos deben validarse correctamente
                    signupWindow.ValidateEmail();
                    signupWindow.ValidatePassword();
                    signupWindow.ValidateMatchingPasswords();
                    
                    Assert.Empty(signupWindow.UsernameError.Text);
                    Assert.Empty(signupWindow.PasswordError.Text);
                    Assert.Empty(signupWindow.RepeatPasswordError.Text);
                    Assert.True(signupWindow.SignupButton.IsEnabled);
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

        /// <summary>
        /// UAT: Flujo completo de login de usuario existente
        /// Escenario: Un usuario existente inicia sesión exitosamente o muestra diálogo
        /// </summary>
        [Fact]
        public void UAT_ExistingUserLogin_CompleteFlow()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    // GIVEN: Usuario en la ventana principal
                    var mainWindow = new MainWindow(true);
                    
                    // WHEN: Se muestra el diálogo de login
                    var loginState = mainWindow.DisplayLoginWindow();
                    
                    // THEN: El diálogo debe mostrarse (Loaded) o completarse (Succeeded)
                    // Depende de si el usuario cancela o completa el login
                    Assert.True(loginState == DialogState.Loaded || loginState == DialogState.Succeeded,
                        $"Expected Loaded or Succeeded but got {loginState}");
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

        /// <summary>
        /// UAT: Flujo de recuperación de contraseña - Validación de formulario
        /// Escenario: Un usuario olvida su contraseña y valida el formulario
        /// </summary>
        [Fact]
        public void UAT_ForgotPassword_FormValidation()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    // GIVEN: Usuario en la ventana de login
                    var fakeMessageService = new FakeMessageService();
                    var loginWindow = new LoginWindow(fakeMessageService, true);
                    
                    // WHEN: El usuario hace clic en "Forgot Password"
                    var forgotPasswordState = loginWindow.DisplayForgotPasswordWindow();
                    Assert.Equal(DialogState.Loaded, forgotPasswordState);
                    
                    // THEN: Se muestra la ventana de recuperación
                    var forgotPasswordWindow = new ForgotPasswordWindow(fakeMessageService);
                    
                    // WHEN: El usuario ingresa su email
                    forgotPasswordWindow.UsernameBox.Text = "valid@example.com";
                    
                    // THEN: El email debe validarse correctamente
                    forgotPasswordWindow.ValidateEmail();
                    Assert.Empty(forgotPasswordWindow.UsernameError.Text);
                    Assert.True(forgotPasswordWindow.ResetPasswordButton.IsEnabled);
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

        /// <summary>
        /// UAT: Flujo de logout
        /// Escenario: Verificar que logout limpia los datos correctamente
        /// </summary>
        [Fact]
        public void UAT_Logout_ClearsData()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    // GIVEN: Ventana principal
                    var mainWindow = new MainWindow(true);
                    
                    // WHEN: Usuario ejecuta logout sin login previo
                    mainWindow.Logout();
                    
                    // THEN: Los datos deben estar vacíos
                    Assert.Empty(mainWindow.userGreeting.Text);
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

        #endregion

        #region Pruebas de Integración entre Módulos

        /// <summary>
        /// Integración: Validación de email integrada con UI de Login
        /// </summary>
        [Fact]
        public void Integration_EmailValidation_WithLoginUI()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var loginWindow = new LoginWindow(fakeMessageService, true);
                    
                    // Probar integración con email inválido
                    loginWindow.UsernameBox.Text = "invalid-email";
                    
                    // El sistema debe validar y mostrar error
                    loginWindow.LoginAsync().GetAwaiter().GetResult();
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

        /// <summary>
        /// Integración: Validación de contraseña integrada con UI de Signup
        /// </summary>
        [Fact]
        public void Integration_PasswordValidation_WithSignupUI()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var signupWindow = new SignupWindow(fakeMessageService, true);
                    
                    // Configurar contraseña inválida
                    signupWindow.PasswordBox.Password = "weak";
                    signupWindow.RepeatPasswordBox.Password = "weak";
                    
                    // Validar
                    signupWindow.ValidatePassword();
                    
                    // Debe mostrar error y deshabilitar botón
                    Assert.NotEmpty(signupWindow.PasswordError.Text);
                    Assert.False(signupWindow.SignupButton.IsEnabled);
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

        /// <summary>
        /// Integración: UserData limpieza después de Logout
        /// </summary>
        [Fact]
        public void Integration_UserData_ClearsAfterLogout()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var mainWindow = new MainWindow(true);
                    
                    // Verificar estado inicial
                    var initialToken = CurrentUserData.GetIdToken();
                    
                    // Logout debe limpiar UserData
                    mainWindow.Logout();
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

        /// <summary>
        /// Integración: Validación en tiempo real con UI
        /// </summary>
        [Fact]
        public void Integration_RealTimeValidation_WithUI()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var signupWindow = new SignupWindow(fakeMessageService, true);
                    
                    // Empezar con email inválido
                    signupWindow.UsernameBox.Text = "invalid";
                    signupWindow.ValidateEmail();
                    Assert.NotEmpty(signupWindow.UsernameError.Text);
                    Assert.False(signupWindow.SignupButton.IsEnabled);
                    
                    // Corregir a email válido
                    signupWindow.UsernameBox.Text = "valid@example.com";
                    signupWindow.ValidateEmail();
                    Assert.Empty(signupWindow.UsernameError.Text);
                    Assert.True(signupWindow.SignupButton.IsEnabled);
                    
                    // Agregar contraseña inválida
                    signupWindow.PasswordBox.Password = "weak";
                    signupWindow.ValidatePassword();
                    Assert.NotEmpty(signupWindow.PasswordError.Text);
                    Assert.False(signupWindow.SignupButton.IsEnabled);
                    
                    // Corregir contraseña
                    signupWindow.PasswordBox.Password = "Strong1!";
                    signupWindow.RepeatPasswordBox.Password = "Strong1!";
                    signupWindow.ValidatePassword();
                    signupWindow.ValidateMatchingPasswords();
                    
                    Assert.Empty(signupWindow.PasswordError.Text);
                    Assert.Empty(signupWindow.RepeatPasswordError.Text);
                    Assert.True(signupWindow.SignupButton.IsEnabled);
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

        #endregion

        #region Pruebas de Flujos de Error

        /// <summary>
        /// UAT: Flujo de validación de formulario de registro
        /// Escenario: Usuario completa el formulario de registro correctamente
        /// </summary>
        [Fact]
        public void UAT_SignupForm_ValidatesCorrectly()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var signupWindow = new SignupWindow(fakeMessageService, true);
                    
                    // Llenar formulario con datos válidos
                    signupWindow.DisplaynameBox.Text = "Test User";
                    signupWindow.UsernameBox.Text = "test@example.com";
                    signupWindow.PasswordBox.Password = "A1234!";
                    signupWindow.RepeatPasswordBox.Password = "A1234!";
                    
                    // Validar todos los campos
                    signupWindow.ValidateEmail();
                    signupWindow.ValidatePassword();
                    signupWindow.ValidateMatchingPasswords();
                    
                    // Verificar que no hay errores
                    Assert.Empty(signupWindow.UsernameError.Text);
                    Assert.Empty(signupWindow.PasswordError.Text);
                    Assert.Empty(signupWindow.RepeatPasswordError.Text);
                    Assert.True(signupWindow.SignupButton.IsEnabled);
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

        /// <summary>
        /// UAT: Flujo de error - Login con credenciales incorrectas
        /// Escenario: Usuario intenta iniciar sesión con contraseña incorrecta
        /// </summary>
        [Fact]
        public void UAT_LoginWithWrongPassword_ShowsError()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var loginWindow = new LoginWindow(fakeMessageService, true);
                    
                    loginWindow.UsernameBox.Text = "azunigamo@ucenfotec.ac.cr";
                    loginWindow.PasswordBox.Password = "WrongPassword123!";
                    
                    loginWindow.LoginAsync().GetAwaiter().GetResult();
                    
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

        /// <summary>
        /// UAT: Flujo de error - Contraseñas no coinciden en registro
        /// Escenario: Usuario ingresa contraseñas diferentes en el formulario de registro
        /// </summary>
        [Fact]
        public void UAT_SignupWithMismatchedPasswords_ShowsError()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var signupWindow = new SignupWindow(fakeMessageService, true);
                    
                    signupWindow.DisplaynameBox.Text = "Test User";
                    signupWindow.UsernameBox.Text = "test@example.com";
                    signupWindow.PasswordBox.Password = "Password1!";
                    signupWindow.RepeatPasswordBox.Password = "Different2@";
                    
                    signupWindow.ValidatePassword();
                    signupWindow.ValidateMatchingPasswords();
                    
                    Assert.NotEmpty(signupWindow.RepeatPasswordError.Text);
                    Assert.False(signupWindow.SignupButton.IsEnabled);
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

        #endregion

        #region Pruebas de Usabilidad

        /// <summary>
        /// Usabilidad: Los mensajes de error deben ser claros y útiles
        /// </summary>
        [Fact]
        public void Usability_ErrorMessages_AreClearAndHelpful()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var signupWindow = new SignupWindow(fakeMessageService, true);
                    
                    // Email inválido
                    signupWindow.UsernameBox.Text = "invalid-email";
                    signupWindow.ValidateEmail();
                    
                    Assert.Equal("Invalid email format.", signupWindow.UsernameError.Text);
                    Assert.Contains("email", signupWindow.UsernameError.Text.ToLower());
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

        /// <summary>
        /// Usabilidad: El botón debe deshabilitarse cuando hay errores
        /// </summary>
        [Fact]
        public void Usability_ButtonDisabled_WhenErrorsPresent()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var signupWindow = new SignupWindow(fakeMessageService, true);
                    
                    // Con errores, el botón debe estar deshabilitado
                    signupWindow.UsernameBox.Text = "invalid";
                    signupWindow.ValidateEmail();
                    
                    Assert.False(signupWindow.SignupButton.IsEnabled);
                    
                    // Al corregir, el botón debe habilitarse
                    signupWindow.UsernameBox.Text = "valid@example.com";
                    signupWindow.ValidateEmail();
                    
                    Assert.True(signupWindow.SignupButton.IsEnabled);
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

        /// <summary>
        /// Usabilidad: La navegación entre ventanas debe ser intuitiva
        /// </summary>
        [Fact]
        public void Usability_WindowNavigation_IsIntuitive()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var loginWindow = new LoginWindow(fakeMessageService, true);
                    
                    // Desde login se puede ir a signup
                    var signupState = loginWindow.DisplaySignupWindow();
                    Assert.Equal(DialogState.Loaded, signupState);
                    
                    // Desde login se puede ir a forgot password
                    var forgotState = loginWindow.DisplayForgotPasswordWindow();
                    Assert.Equal(DialogState.Loaded, forgotState);
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

        #endregion
    }
}
