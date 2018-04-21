using SyncContext.Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
            Messages = new ObservableCollection<string>();
        }

        public int Count
        {
            get { return (int)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Count.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CountProperty =
            DependencyProperty.Register("Count", typeof(int), typeof(MainWindow), new PropertyMetadata(0));





        public ObservableCollection<string> Messages
        {
            get { return (ObservableCollection<string>)GetValue(MessagesProperty); }
            set { SetValue(MessagesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for vs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessagesProperty =
            DependencyProperty.Register("Messages", typeof(ObservableCollection<string>), typeof(MainWindow), new PropertyMetadata(null));




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




        public async void AsyncAwaitExercise1_Click(object sender, RoutedEventArgs e)
        {
            Output("Exercise 1");

            await SyncContext.Lib.ExploreAsyncAwait.AsyncAwait_A(output: Output);

            Count = Count + 1;

            Output("Exercise 1 Done");
        }

        public async void AsyncAwaitExercise2_Click(object sender, RoutedEventArgs e)
        {
            Output("Exercise 2");

            // During Time.Delay break the application
            // View the Task Debug Window
            // View the thread window - Where is the Main Thread? Answer: Waiting for more messages
            await SyncContext.Lib.ExploreAsyncAwait.AsyncAwait_A(output: Output, pause: 10000);

            Count = Count + 1;
        }

        public void AsyncAwaitExercise3_Click(object sender, RoutedEventArgs e)
        {
            Output("Exercise 3");

            // Exercise 3 - Don't Await
            // Appears to "Work"
            // Don't ignore warning!
            SyncContext.Lib.ExploreAsyncAwait.AsyncAwait_A(Output);

            Output("Exercise 3 Done.");
            Count = Count + 1;
        }

        public void AsyncAwaitExercise4_Click(object sender, RoutedEventArgs e)
        {
            Output("Exercise 4");

            // Exercise 4 - Don't Await - Throw Exception
            SyncContext.Lib.ExploreAsyncAwait.ThrowException();

            Count = Count + 1;
        }




        public async void AsyncAwaitExercise5_Click(object sender, RoutedEventArgs e)
        {
            Output("Exercise 5");

            // Exercise 4 - Don't Await - Throw Exception
            var t = SyncContext.Lib.ExploreAsyncAwait.ThrowException();

            // Step thru in Debugger
            // Reach catch in all three cases
            try
            {
                await t;
            }
            catch (Exception e1) { }

            try
            {
                t.Wait();
            }
            catch (Exception e1) { }

            try
            {
                var i = t.Result;
            }
            catch (Exception e1) { }

            Count = Count + 1;

        }

        public void AsyncAwaitExercise6_Click(object sender, RoutedEventArgs e)
        {
            Output("Exercise 6");

            // Exercise 6 - .Wait() w/ Default of ConfigureAwait(true)
            // Deadlock!!
            SyncContext.Lib.ExploreAsyncAwait.AsyncAwait_A(Output).Wait();


            Count = Count + 1;
        }

        public void AsyncAwaitExercise7_Click(object sender, RoutedEventArgs e)
        {
            Output("Exercise 7");

            var messages = new List<string>();

            // Exercise 7 - .Wait() with ConfigureAwait(false)
            // Deadlock resolved the correct way.
            // UI Thread Blocked 
            // Note output window - Different thread completes the task (Logical Execution 7+) that the main thread is waiting for to complete alleviating the deadlock
            SyncContext.Lib.ExploreAsyncAwait.AsyncAwait_A(output: (s) => messages.Add(s), continueOnCapturedSynchronizationContext: false, pause: 5000).Wait();

            messages.ForEach(s => Messages.Add(s));
            Count = Count + 1;
        }

        public async void AsyncAwaitExercise8_Click(object sender, RoutedEventArgs e)
        {
            Output("Exercise 8");

            // Exercise 8 - SynchronizationContext.Current = Null
            // Though it can cause DeadLocks this shows that DispaterSynchronizationContext is important

            SynchronizationContext.SetSynchronizationContext(null);
            var messages = new List<string>();

            // Exercise 7 - .Wait() with ConfigureAwait(false)
            // Deadlock resolved the correct way.
            // UI Thread Blocked 
            // Note output window - Different thread completes the task (Logical Execution 7+) that the main thread is waiting for to complete alleviating the deadlock
            await SyncContext.Lib.ExploreAsyncAwait.AsyncAwait_A(output: (s) => messages.Add(s), continueOnCapturedSynchronizationContext: false, pause: 5000);

            messages.ForEach(s => Messages.Add(s));
            Count = Count + 1;

        }

        public void AsyncAwaitExercise9_Click(object sender, RoutedEventArgs e)
        {
            Output("Exercise 9");

            // Exercise 9 - .Wait() fix -> ContinueWith

            // Task Continuation
            // UI Thread isn't blocked
            // But if it's this easy just add async void to the button click handler
            SyncContext.Lib.ExploreAsyncAwait.AsyncAwait_A(output: Output, pause: 10).ContinueWith(x =>
             {
                 if (x.Exception != null) { throw x.Exception; } // Be careful not to loose the exception. It's your responsibility!
                 Count = Count + 1;
             });

            Output("Exercise 9 Done");

        }

        public void AsyncAwaitExercise10_Click(object sender, RoutedEventArgs e)
        {

            Output("Exercise 10");

            // Exercise 10 - .Wait() fix -> Task.Run
            // In a nested method adding async void isn't the right

            NestedBusinessLogicMethod(); // I want this to finish before moving on
            Count = Count + 1;

            Output("Exercise 10 Done");

        }

        // Pretend this method is in the Business dll
        // And you can't change it's signature or some other concern
        private void NestedBusinessLogicMethod() // Note: async void is not a solution!! Exceptions are lost
        {
            // Sidepoint - async lambda - signature is async task - perfectly valid and safe
            Func<Task> asyncLamda = async () =>
            {
                await Task.Delay(3000); // Main thread is already blocked
                await SyncContext.Lib.ExploreAsyncAwait.AsyncAwait_A(output: Output, pause: 3000);

                // This causes an error because we are not running on the UI thread
                // SynchronizationContext.Current = null
                // Main Thread is blocked
                // Count = Count + 1;
            };

            // What is this accomplishing??
            // We are executing the Task and it's continuation on a different thread than the main thread
            // and that thread is never blocked, only the main thread, so we don't have a deadlock

            // Also the DispatcherSynchronizationContext within SynchronizationContext.Current is not 
            // set within the Task.Run so SynchronizationContext.Current is NULL

            // Note this in the Output Window - All points are not ThreadID 1 but are the same thread

            // Also note that when the newly created task is executed the Main Thread is already blocked
            // To be clear this is wrapping an asyncronous operation with multithreading - this is adding back in thread contention issues

            Task.Run(asyncLamda).Wait();

            // Sidepoint: This DOES pass the DispatcherSynchronizationContext to the Task - And blocks for the same reasons
            //Task.Factory.StartNew(asyncLamda, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Unwrap().Wait();

        }

        private Task lastTask = Task.CompletedTask;

        public async void AsyncAwaitExercise11_Click(object sender, RoutedEventArgs e)
        {
            Output("Exercise 11");

            // Exercise 11 - Task Queue

            // Each click creates a new TaskCompletionSource object
            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
            var t = lastTask;

            // Discuss: Should I have a lock around this? How many threads can read this point??
            lastTask = taskCompletionSource.Task;
            await t;

            await Task.Delay(1000);

            Count = Count + 1;

            // Allow the next task to Execute
            taskCompletionSource.SetResult(null);

        }

        public async void AsyncAwaitExercise12_Click(object sender, RoutedEventArgs e)
        {
            Output("Exercise 12");

            // Seperate out the TaskQueue logic from the business logic (Count = Count + 1)

            await TaskQueue(async (c) =>
            {
                await Task.Delay(1000, c);
                Count = Count + 1;
            });
        }

        private async Task TaskQueue(Func<CancellationToken, Task> func)
        {

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();

            var t = lastTask;
            // Discuss: Should I have a lock around this? 
            lastTask = taskCompletionSource.Task;
            await t;

            var funcTask = func(cancellationTokenSource.Token);

            // Create a timeout
            // Very useful in Unit Tests
            await Task.WhenAny(new Task[] { funcTask, Task.Delay(2000) });

            if (!funcTask.IsCompleted)
            {
                taskCompletionSource.SetCanceled();
                MessageBox.Show("Timeout!", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {

                try
                {
                    await funcTask; // Raise any exception
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failure: {ex.Message}");
                }
            }

            taskCompletionSource.SetResult(null);

        }

        public async void AsyncAwaitExercise13_Click(object sender, RoutedEventArgs e)
        {
            Output("Exercise 13");

            // Exercise 13 - Constructor Task
            // There is no async constructor (and there shouldn't be)
            // One option is to create an initialize method
            // This shows how you can "hide" this initialize

            var initAsync = new InitializeAsync();

            await initAsync.DoWork();

            Count = Count + 1;

            // Notice the second time we call we don't 
            // execute the Task.Delay
            await initAsync.DoWork();

            Count = Count + 1;
        }


        public async void AsyncAwaitExercise14_Click(object sender, RoutedEventArgs e)
        {
            
            Task[] someTasks = new Task[3];

            someTasks[0] = Task.Delay(200);
            someTasks[1] = Task.Delay(2000);
            someTasks[2] = Task.Delay(2000);

            // Takes 2 seconds not 6
            await Task.WhenAll(someTasks);

        }

        public async void AsyncAwaitExercise15_Click(object sender, RoutedEventArgs e)
        {
            /*
             * Exercise 15 - Task.WhenAll under control
             * Likely it's a bad idea to create too many tasks - both for client and server
             * Easy to use Task.WhenAll but chunk out the Task Batches
             * Yes, this method takes 1 second instead of 100 ms
             * but in a real-world situation this is many times wise
             */

            List<Task> tasks = new List<Task>();

            for (var i = 0; i < 101; i++)
            {
                tasks.Add(Task.Delay(100));

                // Wait for the Tasks every 10 tasks
                if (i % 10 == 0)
                {
                    await Task.WhenAll(tasks);
                    tasks.Clear();
                }
            }

            // Important but easy to forget
            await Task.WhenAll(tasks);

        }
        /*
         * 
         * Exercise 16
         * The default ASP.NET Core SynchronizationContext is null
         * See Stephen Clearly's page https://blog.stephencleary.com/2017/03/aspnetcore-synchronization-context.html
         * He provides a warning under "Beware Implicit Parallelism
         * This highlights that warning
         * NullSyncContext is sometimes off (<100) because multiple threads are running Count++
         * Because there's No SynchronozationContext to continue all Count++
         * executions on the same thread
         * 
         * Both NotNullSyncContext and NullSyncContext should produce 100
         * However NullSyncContext can return less
         * 
         * Solution: Change the approach. Asynchronous code should be written functionally.
         *  That's a broad field but worth researching.
         */
        public async void NotNullSyncContext_Click(object sender, RoutedEventArgs e)
        {
            var nsc = new NullSyncContextBehavior();

            Task[] tasks = new Task[100];

            for (var i = 0; i < 100; i++)
            {
                tasks[i] = nsc.Increment();
            }

            await Task.WhenAll(tasks);

            // Always correct - 100
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

            // Sometimes off because multiple threads are running Count++
            // Because there's No SynchronozationContext to continue all Count++
            // Executions on the same thread
            MessageBox.Show($"Count: {nsc.Count}");

            System.Threading.SynchronizationContext.SetSynchronizationContext(sc);

        }

        public async void AsyncAwaitExercise17_Click(object sender, RoutedEventArgs e)
        {

            // Exercise 17
            // Note the values of AsyncLocal in the output window
            // This is because AsyncLocal is within a container within ExecutionContext
            // Execution.Run is part of back - end mechanics of asynchronous forks(await, Task.Run)

            // Not any real world value - Just bringing together the concepts
            // And driving home the fact that these are instances of object like any other
            // This is the type of work Async / Await keywords are instructing the compiler to do

            var sc = SynchronizationContext.Current;

            await Task.Run(async () =>
            {
                // We are NOT on the UI thread. SynchronizationContext.Current is null
                Debug.WriteLine($"ThreadID: {Thread.CurrentThread.ManagedThreadId} SynchoronizationContext == null: {(SynchronizationContext.Current == null)}");
                await ExploreAsyncAwait.AsyncAwait_A(Output);

                TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();

                // Yet, using SynchronizationContext of the UI Thread and ExecutionContext we can run
                // in the same context as AsyncAwait_C

                sc.Post((o) =>
                {
                    // Spot A

                    // We are now on the UI thread
                    ExecutionContext.Run(ExploreAsyncAwait.executionContextC_Capture, (eo) =>
                    {
                        // AsyncLocal now has the same values as AsyncAwait_C
                        ExploreAsyncAwait.LogicalExecution(11, Output);
                        // And safely interact with UI controls
                        Count = Count + 1;
                    }, null);

                    taskCompletionSource.SetResult(null);

                }, null);

                // Without this we to Spot B before we get to Spot A
                await taskCompletionSource.Task;

                // Spot B
            });



        }

        private void Output(string message)
        {
            Messages.Add(message);
        }

        public void btnClear_Click(object sender, RoutedEventArgs e)
        {
            Messages.Clear();
        }
    }
}
