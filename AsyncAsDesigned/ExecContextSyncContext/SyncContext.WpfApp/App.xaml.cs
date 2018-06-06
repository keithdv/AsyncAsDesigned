using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SyncContext.WpfApp
{
    public partial class App
    {

        public App()
        {

            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            // See exercise 4
            // This is where exceptions are raised if you do not await a Task and it has an exception
        }

        private void Application_Startup(object sender, System.Windows.StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is AggregateException)
            {
                MessageBox.Show($"Fatal Error: {((AggregateException)e.Exception).Flatten().Message}");
            }
            else if (e.Exception is Exception)
            {
                MessageBox.Show($"Fatal Error: {((Exception)e.Exception).Message}");
            }
            else
            {
                MessageBox.Show($"Fatal Error!");
            }

            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is AggregateException)
            {
                MessageBox.Show($"Fatal Error: {((AggregateException)e.ExceptionObject).Flatten().Message}");
            }
            else if (e.ExceptionObject is Exception)
            {
                MessageBox.Show($"Fatal Error: {((Exception)e.ExceptionObject).Message}");
            } else
            {
                MessageBox.Show($"Fatal Error!");
            }

        }
    }
}
