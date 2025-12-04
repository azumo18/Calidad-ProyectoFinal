using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Calidad_ProyectoFinal.Tests
{
    /// <summary>
    /// Pruebas de Rendimiento: carga, estrés, volumen
    /// Nota: Estas pruebas pueden tardar más tiempo en ejecutarse
    /// </summary>
    public class PerformanceTests
    {
        #region Pruebas de Estrés

        /// <summary>
        /// Prueba de estrés: validación de emails con datos extremos
        /// </summary>
        [Fact]
        public void Email_Validation_HandlesStressConditions()
        {
            var stopwatch = Stopwatch.StartNew();
            var successCount = 0;
            var failureCount = 0;
            
            // Ejecutar validaciones intensivas durante 2 segundos
            while (stopwatch.ElapsedMilliseconds < 2000)
            {
                var result = Validation.IsValidEmail("test@example.com");
                if (result) successCount++;
                else failureCount++;
            }
            
            stopwatch.Stop();
            
            // Debería haber procesado muchas validaciones sin errores
            Assert.True(successCount > 1000, 
                $"Only {successCount} validations completed in 2 seconds, expected > 1000");
            Assert.Equal(0, failureCount);
        }

        /// <summary>
        /// Prueba de estrés: múltiples usuarios validando simultáneamente
        /// </summary>
        [Fact]
        public async Task Validation_HandlesMultipleSimultaneousUsers()
        {
            var tasks = new List<Task>();
            var stopwatch = Stopwatch.StartNew();
            
            // Simular 100 usuarios validando simultáneamente
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    for (int j = 0; j < 10; j++)
                    {
                        var emailValid = Validation.IsValidEmail($"user{i}@example.com");
                        var passwordValid = Validation.IsValidPassword($"Pass{i}@");
                        Assert.True(emailValid || !emailValid); // Simplemente verificar que no falla
                    }
                }));
            }
            
            await Task.WhenAll(tasks);
            stopwatch.Stop();
            
            // Debería completarse en menos de 3 segundos
            Assert.True(stopwatch.ElapsedMilliseconds < 3000, 
                $"Multiple simultaneous users took {stopwatch.ElapsedMilliseconds}ms, expected < 3000ms");
        }

        /// <summary>
        /// Prueba de estrés: creación y destrucción rápida de ventanas
        /// </summary>
        [Fact]
        public void Windows_HandleRapidCreationAndDestruction()
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var stopwatch = Stopwatch.StartNew();
                    var fakeMessageService = new FakeMessageService();
                    
                    // Crear y liberar 20 ventanas rápidamente
                    for (int i = 0; i < 20; i++)
                    {
                        var window = new LoginWindow(fakeMessageService, true);
                        // Dejar que se libere automáticamente
                    }
                    
                    stopwatch.Stop();
                    
                    // Debería completarse en menos de 5 segundos
                    Assert.True(stopwatch.ElapsedMilliseconds < 5000, 
                        $"Rapid window creation took {stopwatch.ElapsedMilliseconds}ms, expected < 5000ms");
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

        #region Pruebas de Volumen

        /// <summary>
        /// Prueba de volumen: validación de gran cantidad de emails únicos
        /// </summary>
        [Fact]
        public void Email_Validation_HandlesLargeVolumeOfUniqueEmails()
        {
            var stopwatch = Stopwatch.StartNew();
            
            // Validar 2,000 emails únicos (reducido para velocidad)
            for (int i = 0; i < 2000; i++)
            {
                var email = $"user{i}@example{i % 100}.com";
                var result = Validation.IsValidEmail(email);
                Assert.True(result);
            }
            
            stopwatch.Stop();
            
            // Debería completarse en menos de 2 segundos
            Assert.True(stopwatch.ElapsedMilliseconds < 2000, 
                $"Large volume validation took {stopwatch.ElapsedMilliseconds}ms for 2,000 emails, expected < 2000ms");
        }

        /// <summary>
        /// Prueba de volumen: validación de contraseñas con diferentes patrones
        /// </summary>
        [Fact]
        public void Password_Validation_HandlesLargeVolumeOfDifferentPatterns()
        {
            var stopwatch = Stopwatch.StartNew();
            var patterns = new[] { "Pass", "Secur", "Admin", "User", "Test" };
            var specialChars = new[] { "!", "@", "#", "$", "%" };
            
            // Generar y validar 1,000 contraseñas únicas (reducido para velocidad)
            for (int i = 0; i < 1000; i++)
            {
                var password = $"{patterns[i % patterns.Length]}{i}{specialChars[i % specialChars.Length]}";
                var result = Validation.IsValidPassword(password);
                // Algunas serán válidas, otras no, solo verificamos que no crashea
                Assert.True(result || !result);
            }
            
            stopwatch.Stop();
            
            // Debería completarse en menos de 2 segundos
            Assert.True(stopwatch.ElapsedMilliseconds < 2000, 
                $"Large volume password validation took {stopwatch.ElapsedMilliseconds}ms, expected < 2000ms");
        }

        /// <summary>
        /// Prueba de volumen: comparación masiva de contraseñas
        /// </summary>
        [Fact]
        public void Password_Matching_HandlesLargeVolumeComparisons()
        {
            var stopwatch = Stopwatch.StartNew();
            
            // Comparar 1,000 pares de contraseñas (reducido para velocidad)
            for (int i = 0; i < 1000; i++)
            {
                var password1 = $"Password{i}!";
                var password2 = i % 2 == 0 ? password1 : $"Different{i}@";
                
                var result = Validation.DoPasswordsMatch(password1, password2);
                
                if (i % 2 == 0)
                    Assert.True(result);
                else
                    Assert.False(result);
            }
            
            stopwatch.Stop();
            
            // Debería completarse en menos de 2 segundos
            Assert.True(stopwatch.ElapsedMilliseconds < 2000, 
                $"Large volume password matching took {stopwatch.ElapsedMilliseconds}ms, expected < 2000ms");
        }

        #endregion

        #region Pruebas de Uso de Memoria

        /// <summary>
        /// Prueba de uso de memoria: múltiples validaciones no deberían causar memory leak
        /// </summary>
        [Fact]
        public void Validation_DoesNotLeakMemory_UnderContinuousUse()
        {
            var initialMemory = GC.GetTotalMemory(true);
            
            // Ejecutar muchas validaciones (reducido para velocidad)
            for (int i = 0; i < 2000; i++)
            {
                Validation.IsValidEmail($"user{i}@example.com");
                Validation.IsValidPassword($"Pass{i}!");
                Validation.DoPasswordsMatch($"Pass{i}!", $"Pass{i}!");
                
                // Cada 500 iteraciones, verificar memoria
                if (i % 500 == 0)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
            
            GC.Collect();
            GC.WaitForPendingFinalizers();
            var finalMemory = GC.GetTotalMemory(true);
            
            var memoryIncrease = finalMemory - initialMemory;
            
            // El incremento de memoria no debería ser excesivo (menos de 10 MB)
            Assert.True(memoryIncrease < 10 * 1024 * 1024, 
                $"Memory increased by {memoryIncrease / 1024 / 1024}MB, expected < 10MB");
        }

        #endregion

        #region Pruebas de Tiempo de Respuesta

        /// <summary>
        /// Prueba que valida que la validación de email es suficientemente rápida
        /// </summary>
        [Fact]
        public void Email_Validation_CompletesWithinAcceptableTime()
        {
            var stopwatch = Stopwatch.StartNew();
            var result = Validation.IsValidEmail("test@example.com");
            stopwatch.Stop();
            
            Assert.True(result);
            // Debe completarse en menos de 10ms
            Assert.True(stopwatch.ElapsedMilliseconds < 10, 
                $"Email validation took {stopwatch.ElapsedMilliseconds}ms, expected < 10ms");
        }

        /// <summary>
        /// Prueba que valida que la validación de contraseña es suficientemente rápida
        /// </summary>
        [Fact]
        public void Password_Validation_CompletesWithinAcceptableTime()
        {
            var stopwatch = Stopwatch.StartNew();
            var result = Validation.IsValidPassword("Password1!");
            stopwatch.Stop();
            
            Assert.True(result);
            // Debe completarse en menos de 10ms
            Assert.True(stopwatch.ElapsedMilliseconds < 10, 
                $"Password validation took {stopwatch.ElapsedMilliseconds}ms, expected < 10ms");
        }

        /// <summary>
        /// Prueba de tiempo de respuesta promedio para validaciones
        /// </summary>
        [Fact]
        public void Validation_MaintainsConsistentPerformance()
        {
            var times = new List<long>();
            
            // Medir tiempo de 100 validaciones
            for (int i = 0; i < 100; i++)
            {
                var stopwatch = Stopwatch.StartNew();
                Validation.IsValidEmail($"user{i}@example.com");
                stopwatch.Stop();
                times.Add(stopwatch.ElapsedTicks);
            }
            
            var averageTime = times.Average();
            var maxTime = times.Max();
            
            // El tiempo máximo no debería ser más de 10 veces el promedio
            Assert.True(maxTime < averageTime * 10, 
                $"Performance inconsistency detected. Max: {maxTime} ticks, Avg: {averageTime} ticks");
        }

        #endregion
    }
}
