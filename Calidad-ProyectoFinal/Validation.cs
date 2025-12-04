using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Calidad_ProyectoFinal
{
    public static class Validation
    {
        /// <summary> Validates email format through regex </summary>
        /// <param name="email">Email to be tested</param>
        /// <returns>True if valid email</returns>
        public static bool IsValidEmail(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        /// <summary> Validates user passwords through regex </summary>
        /// <param name="password">Password to be tested</param>
        /// <returns>True if valid password</returns>
        public static bool IsValidPassword(string password)
        {
            string pattern = @"^(?=.*[A-Z])(?=.*[^a-zA-Z0-9]).{5,10}$";
            return Regex.IsMatch(password, pattern);
        }

        /// <summary> Validates matching passwords on 'Password' and 'Repeat password' fields </summary>
        /// <param name="password">Password field value</param>
        /// <param name="repeatPassword">Repeat password field value</param>
        /// <returns>True is matching passwords</returns>
        public static bool DoPasswordsMatch(string password, string repeatPassword)
        {
            return (password == repeatPassword);
        }

        /// <summary> Validates if there are any visible error messages </summary>
        /// <param name="errorMessagesVisibleSates">Error messages visible states</param>
        /// <returns>True if visibles are found</returns>
        public static bool DoErrorsExist(List<Visibility> errorMessagesVisibleStates)
        {
            return errorMessagesVisibleStates.Contains(Visibility.Visible);
        }

        /// <summary> Helper method to standardize a window's elements filtering for validation </summary>
        /// <param name="elements">Window elements to be filteres</param>
        /// <returns>Filtered elements' visible states</returns>
        public static List<Visibility> GetErrorMessagesVisibleStates(UIElementCollection elements)
        {
            var formErrorStyle = (Style)Application.Current.FindResource("FormError");
            return [.. elements.OfType<TextBlock>().Where(e => e.Style == formErrorStyle).Select(e => e.Visibility)];
        }
    }
}
