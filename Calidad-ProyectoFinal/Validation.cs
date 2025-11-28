using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <summary>
        /// Validates email format through regex
        /// </summary>
        /// <param name="email">Email to be tested</param>
        /// <returns>True if valid email</returns>
        public static bool IsValidEmail(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        /// <summary>
        /// Validates user passwords through regex
        /// </summary>
        /// <param name="password">Password to be tested</param>
        /// <returns>True if valid password</returns>
        public static bool IsValidPassword(string password)
        {
            string pattern = @"^(?=.*[A-Z])(?=.*[^a-zA-Z0-9]).{5,10}$";
            return Regex.IsMatch(password, pattern);
        }

        /// <summary>
        /// Validates matching passwords on 'Password' and 'Repeat password' fields
        /// </summary>
        /// <param name="password">Password field value</param>
        /// <param name="repeatPassword">Repeat password field value</param>
        /// <returns>True is matching passwords</returns>
        public static bool DoPasswordsMatch(string password, string repeatPassword)
        {
            return (password == repeatPassword);
        }

        /// <summary>
        /// Validates if there are visible errors on the window
        /// </summary>
        /// <param name="window">Window to be tested</param>
        /// <returns>True if visible errors are found</returns>
        public static bool DoErrorsExist(Window window)
        {
            var formErrorStyle = (Style)Application.Current.FindResource("FormError");
            var errors = FindVisualChildren<TextBlock>(window).Where(e => e.Style == formErrorStyle && e.Visibility == Visibility.Visible).ToList();
            return errors.Count > 0;
        }

        /// <summary>
        /// Helper method for searching elements of a certain type within a container
        /// </summary>
        /// <typeparam name="T">Type of object to be searched for</typeparam>
        /// <param name="parent">Container</param>
        /// <returns></returns>
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                    if (child is T t)
                        yield return t;

                    foreach (T descendant in FindVisualChildren<T>(child))
                        yield return descendant;
                }
            }
        }
    }
}
