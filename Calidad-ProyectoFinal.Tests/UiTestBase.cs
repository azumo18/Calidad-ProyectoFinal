using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Calidad_ProyectoFinal.Tests
{
    public class UiTestBase
    {
        static UiTestBase()
        {
            if (Application.Current == null)
            {
                _ = new Application();
            }

            Application.Current.Resources.MergedDictionaries.Add(
                new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/Calidad-ProyectoFinal;component/Styles.xaml", UriKind.Absolute)
                });
        }
    }
}
