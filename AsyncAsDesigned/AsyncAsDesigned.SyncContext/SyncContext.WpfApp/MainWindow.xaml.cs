using SyncContext.Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using System.Windows.Threading;

namespace WpfApp1
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
        }

        public int Count
        {
            get { return (int)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Count.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CountProperty =
            DependencyProperty.Register("Count", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

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

        public void NoConfigureAwaitResult_Click(object sender, RoutedEventArgs e)
        {
            // Blocked..
            Count = NoConfigureAwait.Increment(Count).Result;
            
        }

        public async void NoConfigureAwaitAsync_Click(object sender, RoutedEventArgs e)
        {
            // This is one of the few acceptable places to use Async Void
            Count = await NoConfigureAwait.Increment(Count);

        }

        public void NoConfigureAwaitWait_Click(object sender, RoutedEventArgs e)
        {
            var t = NoConfigureAwait.Increment(Count);

            //t.Wait(2000); // Timeout even though less then the Task.Delay in NestedMethod because we're blocked
            //Count = t.Result;

            if (t.Wait(2000)) // Always Timeout
            {
                Count = t.Result;
            }
        }

        public void NoConfigureAwaitTaskRun_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() => { Count = NoConfigureAwait.Increment(Count).Result; }).Wait();
        }

        public void NoConfigureAwaitTaskRun2_Click(object sender, RoutedEventArgs e)
        {

            //Count = Task.Run(() => { return ca.Increment(Count).Result; }).Result;

            // Doesn't block..but is it good practice?
            // 1) This is not asyncronous this is multi-threading (No Async / Await keywords)
            // 2) Not easy to read (.Result -> .Result WTF??) even for a simple situation
            // 3) IMO Brittle

            var count = Count;
            Count = Task.Run(() => { return NoConfigureAwait.Increment(count).Result; }).Result;

        }

        public void ConfigureAwaitFalseResult_Click(object sender, RoutedEventArgs e)
        {
            Count = ConfigureAwaitFalse.Increment(Count).Result;
        }

        // This is one of the few acceptable places to use Async Void
        public async void ConfigureAwaitFalseAsync_Click(object sender, RoutedEventArgs e)
        {
            Count = await ConfigureAwaitFalse.Increment(Count);
        }

        private Task lastTask = Task.CompletedTask;

        public async void TaskQueue_Click(object sender, RoutedEventArgs e)
        {
            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
            var t = lastTask;
            // Discuss: Should I have a lock around this? 
            lastTask = taskCompletionSource.Task;
            await t;

            Count = await ConfigureAwaitFalse.Increment(Count);

            taskCompletionSource.SetResult(null);

        }

        private void TaskExceptionMethod_Click(object sender, RoutedEventArgs e)
        {
            TaskException.Method();
        }


        private async void TaskExceptionMethodAsync_Click(object send, RoutedEventArgs e)
        {
            try
            {
                await TaskException.MethodAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception caught {ex.Message}", "Exception", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public async void NotNullSyncContext_Click(object sender, RoutedEventArgs e)
        {
            var nsc = new NullSyncContextBehavior();

            Task[] tasks = new Task[100];

            for (var i = 0; i < 100; i++)
            {
                tasks[i] = nsc.Increment();
            }

            await Task.WhenAll(tasks);

            MessageBox.Show($"Count: {nsc.Count}");

        }

        public async void NullSyncContext_Click(object sender, RoutedEventArgs e)
        {
            var nsc = new NullSyncContextBehavior();

            var sc = System.Threading.SynchronizationContext.Current;

            System.Threading.SynchronizationContext.SetSynchronizationContext(null);

            Task[] tasks = new Task[100];

            for (var i = 0; i < 100; i++)
            {
                tasks[i] = nsc.Increment();
            }

            await Task.WhenAll(tasks);

            MessageBox.Show($"Count: {nsc.Count}");

            System.Threading.SynchronizationContext.SetSynchronizationContext(sc);

        }


    }
}
