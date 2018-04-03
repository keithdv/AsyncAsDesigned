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

        }

        private void Application_Startup(object sender, System.Windows.StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
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

            App.Current.Shutdown();
        }
    }
}
