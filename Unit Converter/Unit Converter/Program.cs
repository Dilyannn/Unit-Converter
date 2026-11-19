using System;
using System.Windows.Forms;
using UnitConverter;

namespace UnitConverter
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}