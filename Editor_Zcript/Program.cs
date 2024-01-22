using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Editor_Zcript
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles(); // Enable visual styles for the application
            Application.SetCompatibleTextRenderingDefault(false); // Set to true if you want to use GDI+ for text rendering
            Application.Run(new Editor_Main()); // Editor_Main is the main form
        }
    }
}
