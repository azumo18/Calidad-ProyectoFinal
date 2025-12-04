using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calidad_ProyectoFinal.Tests
{
    public class FakeMessageService : IMessageService
    {
        public string? LastMessage { get; private set; }
        public string? LastCaption { get; private set; }

        public void Show(string message, string caption)
        {
            LastMessage = message;
            LastCaption = caption;
        }

        public void ShowInfo(string message)
        {
            LastMessage = message;
            LastCaption = "Information";
        }

        public void ShowError(string message)
        {
            LastMessage = message;
            LastCaption = "Error";
        }
    }
}
