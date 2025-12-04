using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Calidad_ProyectoFinal
{
    public static class CurrentUserData
    {
        private static string LocalId = string.Empty;
        private static string Email = string.Empty;
        private static string DisplayName = string.Empty;
        private static string IdToken = string.Empty;
        private static bool Registered = false;
        private static string RefreshToken = string.Empty;
        private static string ExpiresIn = string.Empty;

        /// <summary> Exposes current user's JWT token value </summary>
        /// <returns>Current user's JWT token</returns>
        public static string GetIdToken() {  return IdToken; }

        /// <summary> Exposes current user's display name </summary>
        /// <returns>Current user's display name</returns>
        public static string GetDisplayName() { return DisplayName; }

        /// <summary> Sets a new display name from json document </summary>
        /// <param name="doc">Json document</param>
        public static void SetDisplayNameFromJson(JsonDocument doc)
        {
            
            DisplayName = GetPropertyString(doc, "displayName");
        }

        /// <summary> Loads current user data from json document </summary>
        /// <param name="doc"></param>
        public static void LoadCurrentUserDataFromJson(JsonDocument doc)
        {
            LocalId = GetPropertyString(doc, "localId");
            Email = GetPropertyString(doc, "email");
            DisplayName = GetPropertyString(doc, "displayName");
            IdToken = GetPropertyString(doc, "idToken");
            Registered = GetPropertyBoolean(doc, "registered");
            RefreshToken = GetPropertyString(doc, "refreshToken");
            ExpiresIn = GetPropertyString(doc, "expiresIn");
        }

        /// <summary> Helper method to retrieve a string type property's value from a json document </summary>
        /// <param name="doc">Json document</param>
        /// <param name="property">Property to be searched for</param>
        /// <returns>Property's value</returns>
        private static string GetPropertyString(JsonDocument doc, string property)
        {
            try
            {
                var value = doc.RootElement.GetProperty(property).GetString();
                if (value is not null) return value;
            }
            catch (Exception) { /* Property not found */ }
            return string.Empty;
        }

        /// <summary> Helper method to retrieve a boolean type property's value from a json document </summary>
        /// <param name="doc">Json document</param>
        /// <param name="property">Property to be searched for</param>
        /// <returns>Property's value</returns>
        private static bool GetPropertyBoolean(JsonDocument doc, string property)
        {
            try
            {
                return doc.RootElement.GetProperty(property).GetBoolean();
            }
            catch (Exception)
            {
                //Property not found
                return false;
            }
        }

        /// <summary> Resets the current user to initial values </summary>
        public static void Logout()
        {
            LocalId = string.Empty;
            Email = string.Empty;
            DisplayName = string.Empty;
            IdToken = string.Empty;
            Registered = false;
            RefreshToken = string.Empty;
            ExpiresIn = string.Empty;
        }
    }
}
