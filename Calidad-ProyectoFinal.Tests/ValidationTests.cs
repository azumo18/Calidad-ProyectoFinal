using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Xunit;

namespace Calidad_ProyectoFinal.Tests
{
    public class ValidationTests
    {
        /// <summary> Validates email format through regex </summary>
        [Theory]
        [InlineData("user@example.com", true)]
        [InlineData("john.doe@domain.co.uk", true)]
        [InlineData("invalid-email", false)]
        [InlineData("missing@domain", false)]
        [InlineData("missingatsign.com", false)]
        public void IsValidEmail_WorksForVariousCases(string email, bool expected)
        {
            var result = Validation.IsValidEmail(email);
            Assert.Equal(expected, result);
        }

        /// <summary> Validates user passwords through regex </summary>
        [Theory]
        [InlineData("A234!", true)]         // Capital letter + length = 5 + special char
        [InlineData("a234!", false)]        // Missing capital letter
        [InlineData("A2345", false)]        // Missing special char
        [InlineData("A23!", false)]         // Missing minimum length
        [InlineData("!23456789A", true)]    // Special char + length = 10 + capital letter
        [InlineData("!234567890A", false)]  // Missing maximum length
        public void IsValidPassword_WorksForVariousCases(string password, bool expected)
        {
            var result = Validation.IsValidPassword(password);
            Assert.Equal(expected, result);
        }

        /// <summary> Validates matching passwords on 'Password' and 'Repeat password' fields </summary>
        [Theory]
        [InlineData("A234!", "A234!", true)]
        [InlineData("A234!", "no matching pw", false)]
        public void DoPasswordsMatch_WorksForVariousCases(string password, string repeatPassword, bool shouldMatch)
        {
            var result = Validation.DoPasswordsMatch(password, repeatPassword);
            if(shouldMatch) Assert.True(result);
            else Assert.False(result);
        }

        /// <summary> Validates if there are any visible error messages on collection </summary>
        [Theory]
        [InlineData(new Visibility[]{ Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed }, false)]
        [InlineData(new Visibility[] { Visibility.Collapsed, Visibility.Hidden, Visibility.Visible }, true)]
        public void DoErrorsExist_WorksForVariousCases(Visibility[] visibilities, bool expected)
        {
            var result = Validation.DoErrorsExist([.. visibilities]);
            Assert.Equal(expected, result);
        }
    }
}
