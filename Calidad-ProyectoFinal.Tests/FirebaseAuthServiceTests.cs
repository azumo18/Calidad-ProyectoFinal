using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Calidad_ProyectoFinal.Tests
{
    public class FirebaseAuthServiceTests
    {
        private class FakeHttpMessageHandler(HttpResponseMessage fakeResponse) : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(fakeResponse);
            }
        }

        private static HttpClient CreateMockHttpClient(string fakeJson, HttpStatusCode statusCode)
        {
            var fakeResponse = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(fakeJson)
            };

            var handler = new FakeHttpMessageHandler(fakeResponse);
            return new HttpClient(handler);
        }

        [Fact]
        public async Task LoginAsync_ReturnsSuccess_WhenValidCredentials()
        {
            var fakeJson = $@"{{
                ""idToken"": ""FAKE_TOKEN"",
                ""email"": ""test@example.com"",
                ""refreshToken"": ""FAKE_REFRESH"",
                ""expiresIn"": ""3600"",
                ""localId"": ""FAKE_USER_ID""
            }}";
            var fakeHttpStatusCode = HttpStatusCode.OK;

            var result = await FirebaseAuthService.LoginAsync("test@example.com", "correctpassword", CreateMockHttpClient(fakeJson, fakeHttpStatusCode));
            Assert.Equal("SUCCESS", result);
        }

        [Fact]
        public async Task LoginAsync_ThrowsException_WhenBadRequest()
        {
            var message = "EMAIL_NOT_FOUND";
            var fakeJson = $@"{{
                ""error"": {{
                    ""code"": 400,
                    ""message"": ""{message}""
                }}
            }}";
            var fakeHttpStatusCode = HttpStatusCode.BadRequest;

            var ex = await Assert.ThrowsAsync<Exception>(async () =>
                await FirebaseAuthService.LoginAsync("test@example.com", "wrongpassword", CreateMockHttpClient(fakeJson, fakeHttpStatusCode))
            );

            Assert.Equal($"Login failed: {message}", ex.Message);
        }

        [Fact]
        public async Task SignUpAsync_ReturnsSuccess_WhenValidCredentials()
        {
            var fakeJson = $@"{{
                ""idToken"": ""FAKE_TOKEN"",
                ""email"": ""test@example.com"",
                ""refreshToken"": ""FAKE_REFRESH"",
                ""expiresIn"": ""3600"",
                ""localId"": ""FAKE_USER_ID""
            }}";
            var fakeHttpStatusCode = HttpStatusCode.OK;

            var result = await FirebaseAuthService.SignUpAsync("test@example.com", "correctpassword", CreateMockHttpClient(fakeJson, fakeHttpStatusCode));
            Assert.Equal("SUCCESS", result);
        }

        [Fact]
        public async Task SignUpAsync_ThrowsException_WhenBadRequest()
        {
            var message = "EMAIL_EXISTS";
            var fakeJson = $@"{{
                ""error"": {{
                    ""code"": 400,
                    ""message"": ""{message}""
                }}
            }}";
            var fakeHttpStatusCode = HttpStatusCode.BadRequest;

            var ex = await Assert.ThrowsAsync<Exception>(async () =>
                await FirebaseAuthService.SignUpAsync("test@example.com", "wrongpassword", CreateMockHttpClient(fakeJson, fakeHttpStatusCode))
            );

            Assert.Equal($"Signup failed: {message}", ex.Message);
        }

        [Fact]
        public async Task ResetPasswordAsync_ReturnsSuccess_WhenValidCredentials()
        {
            var fakeJson = $@"{{
                ""idToken"": ""FAKE_TOKEN"",
                ""email"": ""test@example.com"",
                ""refreshToken"": ""FAKE_REFRESH"",
                ""expiresIn"": ""3600"",
                ""localId"": ""FAKE_USER_ID""
            }}";
            var fakeHttpStatusCode = HttpStatusCode.OK;

            var result = await FirebaseAuthService.ResetPasswordAsync("test@example.com", CreateMockHttpClient(fakeJson, fakeHttpStatusCode));
            Assert.Equal("SUCCESS", result);
        }

        [Fact]
        public async Task ResetPasswordAsync_ThrowsException_WhenBadRequest()
        {
            var message = "EMAIL_NOT_FOUND";
            var fakeJson = $@"{{
                ""error"": {{
                    ""code"": 400,
                    ""message"": ""{message}""
                }}
            }}";
            var fakeHttpStatusCode = HttpStatusCode.BadRequest;

            var ex = await Assert.ThrowsAsync<Exception>(async () =>
                await FirebaseAuthService.ResetPasswordAsync("test@example.com", CreateMockHttpClient(fakeJson, fakeHttpStatusCode))
            );

            Assert.Equal($"Password reset failed: {message}", ex.Message);
        }

        [Fact]
        public async Task UpdateProfileAsync_ReturnsSuccess_WhenValidCredentials()
        {
            var fakeJson = $@"{{
                ""idToken"": ""FAKE_TOKEN"",
                ""email"": ""test@example.com"",
                ""refreshToken"": ""FAKE_REFRESH"",
                ""expiresIn"": ""3600"",
                ""localId"": ""FAKE_USER_ID""
            }}";
            var fakeHttpStatusCode = HttpStatusCode.OK;

            var result = await FirebaseAuthService.UpdateProfileAsync("FAKE_TOKEN", "Fake Display Name", CreateMockHttpClient(fakeJson, fakeHttpStatusCode));
            Assert.Equal("SUCCESS", result);
        }

        [Fact]
        public async Task UpdateProfileAsync_ThrowsException_WhenBadRequest()
        {
            var message = "INVALID_ID_TOKEN";
            var fakeJson = $@"{{
                ""error"": {{
                    ""code"": 400,
                    ""message"": ""{message}""
                }}
            }}";
            var fakeHttpStatusCode = HttpStatusCode.BadRequest;

            var ex = await Assert.ThrowsAsync<Exception>(async () =>
                await FirebaseAuthService.UpdateProfileAsync("FAKE_TOKEN", "Fake Display Name", CreateMockHttpClient(fakeJson, fakeHttpStatusCode))
            );

            Assert.Equal($"Profile update failed: {message}", ex.Message);
        }
    }
}
