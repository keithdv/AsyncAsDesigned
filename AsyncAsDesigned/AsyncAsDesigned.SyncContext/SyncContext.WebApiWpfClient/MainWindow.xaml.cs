using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SyncContext.WebApiWpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            httpClientCore = new HttpClient();
            httpClientCore.BaseAddress = new Uri(@"http://localhost:56674/api/");

            httpClientFramework = new HttpClient();
            httpClientFramework.BaseAddress = new Uri(@"http://localhost:59220/api/");

        }

        private HttpClient httpClientCore;
        private HttpClient httpClientFramework;

        public int Count
        {
            get { return (int)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Count.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CountProperty =
            DependencyProperty.Register("Count", typeof(int), typeof(MainWindow), new PropertyMetadata(0));



        public bool AllowUserInput
        {
            get { return (bool)GetValue(AllowUserInputProperty); }
            set { SetValue(AllowUserInputProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowUserInput.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowUserInputProperty =
            DependencyProperty.Register("AllowUserInput", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));



        private int progress = 0;
        public async void pbRunning_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(250);

            while (true)
            {
                progress = (progress + 1) % 100;
                pbRunning.Value = progress;
                await Task.Delay(250);
            }

        }

        public async void CoreNoConfigureAwaitResult_Click(object sender, RoutedEventArgs e)
        {
            await GetInLine(async () =>
            {
                using (var httpResponse = await httpClientCore.GetAsync($@"NoConfigureAwait/Increment?count={Count}"))
                {
                    httpResponse.EnsureSuccessStatusCode();
                    Count = int.Parse(await httpResponse.Content.ReadAsStringAsync());
                }
            });
        }

        public async void FrameworkNoConfigureAwaitResult_Click(object sender, RoutedEventArgs e)
        {
            await GetInLine(async () =>
            {
                using (var httpResponse = await httpClientFramework.GetAsync($@"NoConfigureAwait/Increment?count={Count}"))
                {
                    httpResponse.EnsureSuccessStatusCode();
                    Count = int.Parse(await httpResponse.Content.ReadAsStringAsync());
                }
            });
        }

        public async void CoreNoConfigureAwaitAsync_Click(object sender, RoutedEventArgs e)
        {
            await GetInLine(async () =>
            {
                using (var httpResponse = await httpClientCore.GetAsync($@"NoConfigureAwait/IncrementAsync?count={Count}"))
                {
                    httpResponse.EnsureSuccessStatusCode();
                    Count = int.Parse(await httpResponse.Content.ReadAsStringAsync());
                }
            });
        }

        public async void FrameworkNoConfigureAwaitAsync_Click(object sender, RoutedEventArgs e)
        {
            await GetInLine(async () =>
            {
                using (var httpResponse = await httpClientFramework.GetAsync($@"NoConfigureAwait/IncrementAsync?count={Count}"))
                {
                    httpResponse.EnsureSuccessStatusCode();
                    Count = int.Parse(await httpResponse.Content.ReadAsStringAsync());
                }
            });
        }


        public async void CoreConfigureAwaitFalseResult_Click(object sender, RoutedEventArgs e)
        {
            await GetInLine(async () =>
            {

                using (var httpResponse = await httpClientCore.GetAsync($@"ConfigureAwaitFalse/Increment?count={Count}"))
                {
                    httpResponse.EnsureSuccessStatusCode();
                    Count = int.Parse(await httpResponse.Content.ReadAsStringAsync());
                }
            });
        }

        public async void FrameworkConfigureAwaitFalseResult_Click(object sender, RoutedEventArgs e)
        {
            await GetInLine(async () =>
            {

                using (var httpResponse = await httpClientFramework.GetAsync($@"ConfigureAwaitFalse/Increment?count={Count}"))
                {
                    httpResponse.EnsureSuccessStatusCode();
                    Count = int.Parse(await httpResponse.Content.ReadAsStringAsync());
                }
            });
        }

        // This is one of the few acceptable places to use Async Void
        public async void CoreConfigureAwaitFalseAsync_Click(object sender, RoutedEventArgs e)
        {
            await GetInLine(async () =>
            {

                using (var httpResponse = await httpClientCore.GetAsync($@"ConfigureAwaitFalse/IncrementAsync?count={Count}"))
                {
                    httpResponse.EnsureSuccessStatusCode();
                    Count = int.Parse(await httpResponse.Content.ReadAsStringAsync());
                }
            });
        }

        public async void FrameworkConfigureAwaitFalseAsync_Click(object sender, RoutedEventArgs e)
        {
            await GetInLine(async () =>
            {
                using (var httpResponse = await httpClientFramework.GetAsync($@"ConfigureAwaitFalse/IncrementAsync?count={Count}"))
                {
                    httpResponse.EnsureSuccessStatusCode();
                    Count = int.Parse(await httpResponse.Content.ReadAsStringAsync());
                }
            });
        }

        private Task lastTask = Task.CompletedTask;

        //private async Task GetInLine(Func<Task> func)
        //{
        //    TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();

        //    var t = lastTask;
        //    // Discuss: Should I have a lock around this? 
        //    lastTask = taskCompletionSource.Task;
        //    await t;

        //    AllowUserInput = false;

        //    await func();

        //    AllowUserInput = true;

        //    taskCompletionSource.SetResult(null);

        //}

        private async Task GetInLine(Func<Task> func)
        {
            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();

            var t = lastTask;
            // Discuss: Should I have a lock around this? 
            lastTask = taskCompletionSource.Task;
            await t;

            AllowUserInput = false;

            var funcTask = func();

            // Create a timeout
            await Task.WhenAny(new Task[] { funcTask, Task.Delay(5000) });

            if (!funcTask.IsCompleted)
            {
                MessageBox.Show("Timeout!", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            try
            {
                await funcTask; // Raise any exception
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failure: {ex.Message}");
            }

            AllowUserInput = true;

            taskCompletionSource.SetResult(null);

        }

        private async void CoreTaskExceptionMethod_Click(object sender, RoutedEventArgs e)
        {
            await GetInLine(async () =>
            {
                using (var httpResponse = await httpClientCore.GetAsync($@"TaskException/Method"))
                {
                    if (httpResponse.StatusCode != System.Net.HttpStatusCode.InternalServerError)
                    {
                        MessageBox.Show($"Failure: Expected InternalServerError. Received {httpResponse.StatusCode.ToString()}");
                    }
                }
            });
        }

        private async void FrameworkTaskExceptionMethod_Click(object sender, RoutedEventArgs e)
        {
            await GetInLine(async () =>
            {
                using (var httpResponse = await httpClientFramework.GetAsync($@"TaskException/Method"))
                {
                    if (httpResponse.StatusCode != System.Net.HttpStatusCode.InternalServerError)
                    {
                        MessageBox.Show($"Failure: Expected InternalServerError. Received {httpResponse.StatusCode.ToString()}");
                    }
                }
            });
        }

        private async void CoreTaskExceptionMethodAsync_Click(object sender, RoutedEventArgs e)
        {
            await GetInLine(async () =>
            {
                using (var httpResponse = await httpClientCore.GetAsync($@"TaskException/MethodAsync"))
                {
                    if (httpResponse.StatusCode != System.Net.HttpStatusCode.InternalServerError)
                    {
                        MessageBox.Show($"Failure: Expected InternalServerError. Received {httpResponse.StatusCode.ToString()}");
                    }
                }
            });
        }

        private async void FrameworkTaskExceptionMethodAsync_Click(object sender, RoutedEventArgs e)
        {
            await GetInLine(async () =>
            {
                using (var httpResponse = await httpClientFramework.GetAsync($@"TaskException/MethodAsync"))
                {
                    if (httpResponse.StatusCode != System.Net.HttpStatusCode.InternalServerError)
                    {
                        MessageBox.Show($"Failure: Expected InternalServerError. Received {httpResponse.StatusCode.ToString()}");
                    }
                }
            });
        }




    }
}
