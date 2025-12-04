using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calidad_ProyectoFinal
{
    /// <summary> Service to show message boxes </summary>
    public interface IMessageService
    {
        void Show(string message, string caption);
        void ShowInfo(string message);
        void ShowError(string message);
    }

    /// <summary> Implementation of IMessageService using MessageBox </summary>
    public class MessageBoxService : IMessageService
    {
        public void Show(string message, string caption)
        {
            System.Windows.MessageBox.Show(message, caption);
        }
        public void ShowInfo(string message)
        {
            System.Windows.MessageBox.Show(message, "Information");
        }
        public void ShowError(string message)
        {
            System.Windows.MessageBox.Show(message, "Error");
        }
    }
}
