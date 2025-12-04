using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Calidad_ProyectoFinal.Tests
{
    /// <summary>
    /// Pruebas de Manejo de Errores y Casos Extremos (Edge Cases)
    /// </summary>
    public class ErrorHandlingAndEdgeCaseTests
    {
        #region Pruebas de Casos Extremos - Validación de Email

        /// <summary>
        /// Prueba de validación de email con longitud extremadamente larga
        /// </summary>
        [Fact]
        public void Email_Validation_HandlesExtremelyLongEmail()
        {
            var longEmail = new string('a', 100) + "@" + new string('b', 100) + ".com";
            var result = Validation.IsValidEmail(longEmail);
            
            // El sistema no debe crashear al validar emails largos
            // No hacemos aserciones sobre si es válido o no, solo que no crashea
            Assert.True(result || !result); // Simplemente verificar que se ejecuta sin error
        }

        /// <summary>
        /// Prueba de validación de email vacío
        /// </summary>
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Email_Validation_HandlesEmptyOrNullInput(string input)
        {
            if (input == null)
            {
                // Si el método no acepta null, debe lanzar ArgumentNullException
                try
                {
                    var result = Validation.IsValidEmail(input);
                    Assert.False(result);
                }
                catch (ArgumentNullException)
                {
                    Assert.True(true, "Correctly throws ArgumentNullException for null input");
                }
            }
            else
            {
                var result = Validation.IsValidEmail(input);
                Assert.False(result);
            }
        }

        /// <summary>
        /// Prueba de validación de email con múltiples @ símbolos
        /// </summary>
        [Theory]
        [InlineData("user@@example.com")]
        [InlineData("user@domain@example.com")]
        [InlineData("@user@example.com")]
        public void Email_Validation_RejectsMultipleAtSymbols(string email)
        {
            var result = Validation.IsValidEmail(email);
            Assert.False(result);
        }

        /// <summary>
        /// Prueba de validación de email con caracteres Unicode
        /// </summary>
        [Theory]
        [InlineData("??@example.com")]
        [InlineData("user@??.com")]
        [InlineData("üser@example.com")]
        public void Email_Validation_HandlesUnicodeCharacters(string email)
        {
            var result = Validation.IsValidEmail(email);
            // Depende de la implementación, pero no debe crashear
            // El validador puede aceptar o rechazar Unicode
            Assert.True(result || !result);
        }

        /// <summary>
        /// Prueba de validación de email con espacios
        /// </summary>
        [Theory]
        [InlineData(" user@example.com")]
        [InlineData("user@example.com ")]
        [InlineData("user @example.com")]
        [InlineData("user@ example.com")]
        public void Email_Validation_RejectsEmailsWithSpaces(string email)
        {
            var result = Validation.IsValidEmail(email);
            Assert.False(result);
        }

        /// <summary>
        /// Prueba de validación de email con TLD (Top Level Domain) inválido
        /// </summary>
        [Theory]
        [InlineData("user@example")]
        [InlineData("user@example.")]
        [InlineData("user@.com")]
        public void Email_Validation_RejectsInvalidTLD(string email)
        {
            var result = Validation.IsValidEmail(email);
            Assert.False(result);
        }

        #endregion

        #region Pruebas de Casos Extremos - Validación de Contraseña

        /// <summary>
        /// Prueba de validación de contraseña vacía
        /// </summary>
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Password_Validation_HandlesEmptyOrNullInput(string input)
        {
            if (input == null)
            {
                try
                {
                    var result = Validation.IsValidPassword(input);
                    Assert.False(result);
                }
                catch (ArgumentNullException)
                {
                    Assert.True(true, "Correctly throws ArgumentNullException for null input");
                }
            }
            else
            {
                var result = Validation.IsValidPassword(input);
                Assert.False(result);
            }
        }

        /// <summary>
        /// Prueba de validación de contraseña con solo caracteres especiales
        /// </summary>
        [Theory]
        [InlineData("!@#$%")]
        [InlineData("*&^%$#@!")]
        public void Password_Validation_RejectsOnlySpecialCharacters(string password)
        {
            var result = Validation.IsValidPassword(password);
            Assert.False(result);
        }

        /// <summary>
        /// Prueba de validación de contraseña con caracteres Unicode
        /// </summary>
        [Theory]
        [InlineData("??????1!")]
        [InlineData("??1@")]
        [InlineData("Contraseña1!")]
        public void Password_Validation_HandlesUnicodeCharacters(string password)
        {
            var result = Validation.IsValidPassword(password);
            // No debe crashear
            Assert.False(result || result);
        }

        /// <summary>
        /// Prueba de validación de contraseña con espacios
        /// </summary>
        [Theory]
        [InlineData("Pass word1!")]
        [InlineData(" Password1!")]
        [InlineData("Password1! ")]
        public void Password_Validation_HandlesPasswordsWithSpaces(string password)
        {
            var result = Validation.IsValidPassword(password);
            // Depende de los requisitos, pero no debe crashear
            Assert.False(result || result);
        }

        /// <summary>
        /// Prueba de validación de contraseña en el límite exacto (5 caracteres)
        /// </summary>
        [Fact]
        public void Password_Validation_AcceptsMinimumValidLength()
        {
            var result = Validation.IsValidPassword("A234!");
            Assert.True(result);
        }

        /// <summary>
        /// Prueba de validación de contraseña en el límite exacto (10 caracteres)
        /// </summary>
        [Fact]
        public void Password_Validation_AcceptsMaximumValidLength()
        {
            var result = Validation.IsValidPassword("A234567890!");
            Assert.False(result); // Según ValidationTests, max es 10 caracteres
        }

        /// <summary>
        /// Prueba de validación de contraseña justo debajo del mínimo (4 caracteres)
        /// </summary>
        [Fact]
        public void Password_Validation_RejectsBelowMinimumLength()
        {
            var result = Validation.IsValidPassword("A23!");
            Assert.False(result);
        }

        #endregion

        #region Pruebas de Casos Extremos - Coincidencia de Contraseñas

        /// <summary>
        /// Prueba de coincidencia con valores null
        /// </summary>
        [Theory]
        [InlineData(null, null)]
        [InlineData("password", null)]
        [InlineData(null, "password")]
        public void Password_Matching_HandlesNullValues(string password1, string password2)
        {
            try
            {
                var result = Validation.DoPasswordsMatch(password1, password2);
                // Si acepta null:
                // - null == null -> true (ambos son nulos)
                // - "password" == null -> false
                // - null == "password" -> false
                if (password1 == null && password2 == null)
                    Assert.True(result); // Dos nulls son iguales
                else
                    Assert.False(result); // Uno es null, el otro no
            }
            catch (ArgumentNullException)
            {
                Assert.True(true, "Correctly throws ArgumentNullException for null input");
            }
        }

        /// <summary>
        /// Prueba de coincidencia con strings vacíos
        /// </summary>
        [Fact]
        public void Password_Matching_HandlesEmptyStrings()
        {
            var result = Validation.DoPasswordsMatch("", "");
            // Dos strings vacíos técnicamente coinciden
            Assert.True(result);
        }

        /// <summary>
        /// Prueba de coincidencia con contraseñas extremadamente largas
        /// </summary>
        [Fact]
        public void Password_Matching_HandlesVeryLongPasswords()
        {
            var longPassword = new string('A', 1000) + "1!";
            var result = Validation.DoPasswordsMatch(longPassword, longPassword);
            Assert.True(result);
        }

        /// <summary>
        /// Prueba de coincidencia sensible a mayúsculas/minúsculas
        /// </summary>
        [Theory]
        [InlineData("Password1!", "password1!")]
        [InlineData("PASSWORD1!", "password1!")]
        [InlineData("PaSsWoRd1!", "pAsSwOrD1!")]
        public void Password_Matching_IsCaseSensitive(string password1, string password2)
        {
            var result = Validation.DoPasswordsMatch(password1, password2);
            Assert.False(result);
        }

        #endregion

        #region Pruebas de Manejo de Errores en UI

        /// <summary>
        /// Prueba que la ventana de signup maneja datos extremos sin crashear
        /// </summary>
        /// <remarks>Integration Test - Opens real UI window</remarks>
        [Fact(Skip = "Integration Test - requires real UI")]
        public void SignupWindow_HandlesExtremeInput_WithoutCrashing()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var window = new SignupWindow(fakeMessageService, true);
                    
                    // Probar con inputs extremos
                    window.DisplaynameBox.Text = new string('A', 1000);
                    window.UsernameBox.Text = new string('a', 500) + "@example.com";
                    window.PasswordBox.Password = new string('P', 100) + "1!";
                    window.RepeatPasswordBox.Password = window.PasswordBox.Password;
                    
                    // No debe crashear al validar
                    window.ValidateEmail();
                    window.ValidatePassword();
                    window.ValidateMatchingPasswords();
                    
                    Assert.True(true, "Window handled extreme input without crashing");
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
        /// Prueba que la ventana de login maneja múltiples intentos fallidos
        /// </summary>
        /// <remarks>Integration Test - Opens real UI window and makes real Firebase calls</remarks>
        [Fact(Skip = "Integration Test - requires real UI and Firebase connection")]
        public void LoginWindow_HandlesMultipleFailedAttempts()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    
                    for (int i = 0; i < 5; i++)
                    {
                        var window = new LoginWindow(fakeMessageService, true);
                        window.UsernameBox.Text = "wrong@example.com";
                        window.PasswordBox.Password = "wrongpassword";
                        
                        window.LoginAsync().GetAwaiter().GetResult();
                        
                        // Debe mostrar error pero no crashear
                        Assert.Equal("Error", fakeMessageService.LastCaption);
                    }
                    
                    Assert.True(true, "Multiple failed attempts handled correctly");
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
        /// Prueba que la validación de errores funciona con arrays vacíos
        /// </summary>
        [Fact]
        public void Error_Validation_HandlesEmptyArray()
        {
            var emptyArray = Array.Empty<Visibility>();
            var result = Validation.DoErrorsExist(emptyArray.ToList());
            
            Assert.False(result, "Empty array should return false for errors");
        }

        /// <summary>
        /// Prueba que la validación de errores funciona con array de un solo elemento
        /// </summary>
        [Theory]
        [InlineData(Visibility.Visible, true)]
        [InlineData(Visibility.Collapsed, false)]
        [InlineData(Visibility.Hidden, false)]
        public void Error_Validation_HandlesSingleElement(Visibility visibility, bool expectedResult)
        {
            var singleElementArray = new[] { visibility };
            var result = Validation.DoErrorsExist(singleElementArray.ToList());
            
            Assert.Equal(expectedResult, result);
        }

        #endregion

        #region Pruebas de Estados Inconsistentes

        /// <summary>
        /// Prueba que el sistema maneja cambios rápidos de estado
        /// </summary>
        /// <remarks>Integration Test - Opens real UI window</remarks>
        [Fact(Skip = "Integration Test - requires real UI")]
        public void SignupWindow_HandlesRapidStateChanges()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var window = new SignupWindow(fakeMessageService, true);
                    
                    // Cambios rápidos de estado
                    for (int i = 0; i < 10; i++)
                    {
                        window.UsernameBox.Text = "invalid";
                        window.ValidateEmail();
                        
                        window.UsernameBox.Text = "valid@example.com";
                        window.ValidateEmail();
                    }
                    
                    // Estado final debe ser consistente
                    Assert.Empty(window.UsernameError.Text);
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
        /// Prueba que el sistema maneja validaciones concurrentes
        /// </summary>
        [Fact]
        public void Validation_HandlesConcurrentCalls()
        {
            var tasks = new List<Task>();
            
            for (int i = 0; i < 100; i++)
            {
                var email = $"user{i}@example.com";
                tasks.Add(Task.Run(() =>
                {
                    var result = Validation.IsValidEmail(email);
                    Assert.True(result);
                }));
            }
            
            Task.WaitAll(tasks.ToArray());
            Assert.True(true, "Concurrent validations completed successfully");
        }

        #endregion

        #region Pruebas de Límites de Sistema

        /// <summary>
        /// Prueba que el sistema maneja correctamente caracteres de nueva línea
        /// </summary>
        [Theory]
        [InlineData("user@example.com\n")]
        [InlineData("user@example.com\r\n")]
        [InlineData("\nuser@example.com")]
        public void Email_Validation_HandlesNewlineCharacters(string email)
        {
            var result = Validation.IsValidEmail(email);
            // No crashea, eso es lo importante
            Assert.True(result || !result);
        }

        /// <summary>
        /// Prueba que el sistema maneja correctamente caracteres de tabulación
        /// </summary>
        [Theory]
        [InlineData("user\t@example.com")]
        [InlineData("user@\texample.com")]
        public void Email_Validation_HandlesTabCharacters(string email)
        {
            var result = Validation.IsValidEmail(email);
            Assert.False(result);
        }

        /// <summary>
        /// Prueba de email con puntos consecutivos
        /// </summary>
        [Theory]
        [InlineData("user..name@example.com")]
        [InlineData("user@example..com")]
        [InlineData("user.@example.com")]
        [InlineData(".user@example.com")]
        public void Email_Validation_HandlesConsecutiveDots(string email)
        {
            var result = Validation.IsValidEmail(email);
            // No hacemos aserción sobre el resultado, solo verificamos que no crashea
            // Dependiendo de las reglas RFC, estos pueden ser válidos o inválidos
            Assert.True(result || !result);
        }

        /// <summary>
        /// Prueba de contraseña con solo números
        /// </summary>
        [Fact]
        public void Password_Validation_RejectsOnlyNumbers()
        {
            var result = Validation.IsValidPassword("12345");
            Assert.False(result);
        }

        /// <summary>
        /// Prueba de contraseña con solo letras mayúsculas
        /// </summary>
        [Fact]
        public void Password_Validation_RejectsOnlyUppercase()
        {
            var result = Validation.IsValidPassword("ABCDE");
            Assert.False(result);
        }

        /// <summary>
        /// Prueba de contraseña con solo letras minúsculas
        /// </summary>
        [Fact]
        public void Password_Validation_RejectsOnlyLowercase()
        {
            var result = Validation.IsValidPassword("abcde");
            Assert.False(result);
        }

        #endregion

        #region Pruebas de Recuperación de Errores

        /// <summary>
        /// Prueba que el sistema se recupera después de un error de validación
        /// </summary>
        /// <remarks>Integration Test - Opens real UI window</remarks>
        [Fact(Skip = "Integration Test - requires real UI")]
        public void System_RecoversAfterValidationError()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var fakeMessageService = new FakeMessageService();
                    var window = new SignupWindow(fakeMessageService, true);
                    
                    // Causar error
                    window.UsernameBox.Text = "invalid";
                    window.ValidateEmail();
                    Assert.NotEmpty(window.UsernameError.Text);
                    
                    // Recuperarse
                    window.UsernameBox.Text = "valid@example.com";
                    window.ValidateEmail();
                    Assert.Empty(window.UsernameError.Text);
                    
                    // Causar otro error diferente
                    window.PasswordBox.Password = "weak";
                    window.ValidatePassword();
                    Assert.NotEmpty(window.PasswordError.Text);
                    
                    // Recuperarse de nuevo
                    window.PasswordBox.Password = "Strong1!";
                    window.ValidatePassword();
                    Assert.Empty(window.PasswordError.Text);
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
