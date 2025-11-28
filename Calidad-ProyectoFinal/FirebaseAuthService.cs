using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Calidad_ProyectoFinal
{
    /// <summary>
    /// Calls Firebase Auth API to perform different tasks
    /// </summary>
    public static class FirebaseAuthService
    {
        private static readonly IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
            .Build();

        private static readonly string apiKey = config["Firebase:ApiKey"];

        /// <summary>
        /// Handles user sign-up calls
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="password">User's password</param>
        /// <param name="httpClient">Optional Http Client used for mock testing</param>
        /// <returns></returns>
        /// <exception cref="Exception">Handles sign-up errors</exception>
        public static async Task<string> SignUpAsync(string email, string password, HttpClient? httpClient)
        {
            var payload = new
            {
                email,
                password,
                returnSecureToken = true
            };

            httpClient ??= new();
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={apiKey}", content);
            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Signup failed: {GetErrorMessageFromJson(doc)}");
            }

            CurrentUserData.LoadCurrentUserDataFromJson(doc);
            return "SUCCESS";
        }

        /// <summary>
        /// Handles user profile update calls (in this case for Display Name updates only)
        /// </summary>
        /// <param name="idToken">User's JWT token gotten from either sign-up or login</param>
        /// <param name="displayName">User's chosen Display Name on the app</param>
        /// <param name="httpClient">Optional Http Client used for mock testing</param>
        /// <returns></returns>
        /// <exception cref="Exception">Handles user profile update errors</exception>
        public static async Task<string> UpdateProfileAsync(string idToken, string displayName, HttpClient? httpClient)
        {
            var payload = new
            {
                idToken,
                displayName,
                returnSecureToken = true
            };

            httpClient ??= new();
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"https://identitytoolkit.googleapis.com/v1/accounts:update?key={apiKey}", content);
            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Profile update failed: {GetErrorMessageFromJson(doc)}");

            CurrentUserData.SetDisplayNameFromJson(doc);
            return "SUCCESS";
        }

        /// <summary>
        /// Handles user login calls
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="password">User's password</param>
        /// <param name="httpClient">Optional Http Client used for mock testing</param>
        /// <returns></returns>
        /// <exception cref="Exception">Handles user login errors</exception>
        public static async Task<string> LoginAsync(string email, string password, HttpClient? httpClient)
        {
            var payload = new
            {
                email,
                password,
                returnSecureToken = true
            };

            httpClient ??= new();
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={apiKey}", content);
            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Login failed: {GetErrorMessageFromJson(doc)}");

            CurrentUserData.LoadCurrentUserDataFromJson(doc);
            return "SUCCESS";
        }

        /// <summary>
        /// Handles user password reset calls
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="httpClient">Optional Http Client used for mock testing</param>
        /// <returns></returns>
        /// <exception cref="Exception">Handles user password reset errors</exception>
        public static async Task<string> ResetPasswordAsync(string email, HttpClient? httpClient)
        {
            var payload = new { requestType = "PASSWORD_RESET", email };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            
            httpClient ??= new();
            var response = await httpClient.PostAsync($"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={apiKey}", content);
            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Password reset failed: {GetErrorMessageFromJson(doc)}");

            // Firebase sends reset email automatically
            return "SUCCESS";
        }

        /// <summary>
        /// Gets the Firebase Auth API response's error message from json
        /// </summary>
        /// <param name="doc">Json formated API response</param>
        /// <returns>Error message</returns>
        private static string GetErrorMessageFromJson(JsonDocument doc)
        {
            try
            {
                return doc.RootElement.GetProperty("error").GetProperty("message").GetString();
            }
            catch (Exception) { /* Field not found */ }
            return "ERROR_UNKNOWN";
        }
    }
}
