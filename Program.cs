using System;
using System.IO;
using System.Windows.Forms;

namespace vlc_works015
{
	internal static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
            // UI exceptions
            Application.ThreadException += (sender, e) => {
                LogException(e.Exception);
            };
            // not UI exceptions
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => {
                LogException(e.ExceptionObject as Exception);
            };

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new ClientForm());
		}

        private static void LogException(Exception e) {
            try {
                File.AppendAllText(
                    "ALL_ERRORS.txt", 
                    $"[{DateTime.Now}] {e?.ToString()}\n\t{e?.Message}\n\t{e?.Source}\n\t{e?.StackTrace}", 
                    encoding: System.Text.Encoding.UTF8
                );
            } catch {
            }
        }

    }
}
