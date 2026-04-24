// ============================================================
//  Program.cs  –  Application entry point
// ============================================================

using System;
using System.Windows.Forms;

namespace HospitalMedSystem
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Enable modern Windows visual styles
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Launch the main window
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                // Top-level exception handler – last safety net
                MessageBox.Show(
                    "A fatal error occurred:\n\n" + ex.Message +
                    "\n\nThe application will now close.",
                    "Fatal Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
