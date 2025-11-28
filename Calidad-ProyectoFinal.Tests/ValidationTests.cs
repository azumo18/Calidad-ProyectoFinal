using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Calidad_ProyectoFinal.Tests
{
    public class ValidationTests
    {
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

        [Theory]
        [InlineData("A234!", true)]
        [InlineData("a234!", false)]
        [InlineData("A2345", false)]
        [InlineData("A23!", false)]
        [InlineData("A23456789!", true)]
        [InlineData("A234567890!", false)]
        public void IsValidPassword_WorksForVariousCases(string password, bool expected)
        {
            var result = Validation.IsValidPassword(password);
            Assert.Equal(expected, result);
        }
    }
}
