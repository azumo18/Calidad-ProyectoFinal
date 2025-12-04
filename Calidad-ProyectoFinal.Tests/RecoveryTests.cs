using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Calidad_ProyectoFinal.Tests
{
    /// <summary>
    /// Pruebas de Recuperación: resiliencia ante fallas, manejo de errores, recuperación ante caídas
    /// </summary>
    public class RecoveryTests
    {
        #region Pruebas de Manejo de Excepciones

        /// <summary>
        /// Prueba que valida que las excepciones no dejan el sistema en estado inconsistente
        /// </summary>
        [Fact]
        public void SignupWindow_ExceptionHandling_DoesNotCorruptState()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var window = new SignupWindow(fakeMessageService, true);
                    
                    // Estado inicial
                    Assert.Equal(DialogState.Initialized, window.State);
                    
                    // Intentar signup con datos vacíos (debería fallar)
                    window.SignupAsync().GetAwaiter().GetResult();
                    
                    // El estado debería seguir siendo Initialized
                    Assert.Equal(DialogState.Initialized, window.State);
                    
                    // El sistema debería seguir permitiendo nuevos intentos
                    window.UsernameBox.Text = "valid@example.com";
                    window.PasswordBox.Password = "Valid1!";
                    window.RepeatPasswordBox.Password = "Valid1!";
                    
                    // Verificar que los campos se pueden modificar
                    Assert.Equal("valid@example.com", window.UsernameBox.Text);
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
        /// Prueba que el sistema se recupera de errores de validación
        /// </summary>
        [Fact]
        public void SignupWindow_RecoverFromValidationErrors()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var window = new SignupWindow(fakeMessageService, true);
                    
                    // Causar error de validación
                    window.UsernameBox.Text = "invalid-email";
                    window.ValidateEmail();
                    Assert.NotEmpty(window.UsernameError.Text);
                    Assert.False(window.SignupButton.IsEnabled);
                    
                    // Recuperarse
                    window.UsernameBox.Text = "valid@example.com";
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

        /// <summary>
        /// Prueba de múltiples errores y recuperación
        /// </summary>
        [Fact]
        public void SignupWindow_HandleMultipleErrorsAndRecover()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var window = new SignupWindow(fakeMessageService, true);
                    
                    // Error en email
                    window.UsernameBox.Text = "invalid";
                    window.ValidateEmail();
                    Assert.NotEmpty(window.UsernameError.Text);
                    
                    // Error en password
                    window.PasswordBox.Password = "weak";
                    window.ValidatePassword();
                    Assert.NotEmpty(window.PasswordError.Text);
                    
                    // Error en password match
                    window.PasswordBox.Password = "Strong1!";
                    window.RepeatPasswordBox.Password = "Different2@";
                    window.ValidatePassword();
                    window.ValidateMatchingPasswords();
                    Assert.NotEmpty(window.RepeatPasswordError.Text);
                    
                    // Recuperarse de todos los errores
                    window.UsernameBox.Text = "valid@example.com";
                    window.PasswordBox.Password = "Strong1!";
                    window.RepeatPasswordBox.Password = "Strong1!";
                    
                    window.ValidateEmail();
                    window.ValidatePassword();
                    window.ValidateMatchingPasswords();
                    
                    Assert.Empty(window.UsernameError.Text);
                    Assert.Empty(window.PasswordError.Text);
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

        #endregion
    }
}
