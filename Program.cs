using System;
using System.Threading;
using System.Windows.Forms;

namespace TESTS
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Application.Run(new start());
            }
            catch (Exception ex)
            {
                HandleFatalException("Критическая ошибка запуска приложения.", ex);
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleFatalException("Ошибка в UI-потоке.", e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception ?? new Exception("Unknown unhandled exception");
            HandleFatalException("Необработанная ошибка приложения.", ex);
        }

        private static void HandleFatalException(string title, Exception ex)
        {
            MessageBox.Show(
                title + Environment.NewLine + ex.Message,
                "Ошибка запуска",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }
}
