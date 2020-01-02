using SyncContext.Lib;
using System;
using System.Collections.Concurrent;
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

namespace SyncContext.WpfCore
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

            context = SynchronizationContext.Current;
            ExploreAsyncAwait.output = s => Output(s);

            this.SizeToContent = SizeToContent.WidthAndHeight;
        }

        readonly SynchronizationContext context;

        public ObservableCollection<string> Messages
        {
            get { return (ObservableCollection<string>)GetValue(MessagesProperty); }
            set { SetValue(MessagesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for vs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessagesProperty =
            DependencyProperty.Register("Messages", typeof(ObservableCollection<string>), typeof(MainWindow), new PropertyMetadata(null));

        // Add messages while not on the UI Thread
        readonly ConcurrentQueue<string> _backLogMessages = new ConcurrentQueue<string>();


        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsExpanded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(MainWindow), new PropertyMetadata(true, OnPropertyChanged));

        public static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var mw = (MainWindow)sender;
            if ((bool)e.NewValue)
            {
                mw.ListFontSize = 14;
            }
            else
            {
                mw.ListFontSize = 24;
            }
        }




        public int ListFontSize
        {
            get { return (int)GetValue(ListFontSizeProperty); }
            set { SetValue(ListFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ListFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListFontSizeProperty =
            DependencyProperty.Register("ListFontSize", typeof(int), typeof(MainWindow), new PropertyMetadata(14));


        private int progress = 0;
        public async void pbRunning_Loaded(object sender, RoutedEventArgs e)
        {
            var method = Type.GetType("System.Threading.Tasks.ThreadPoolTaskScheduler").GetMethod("GetScheduledTasks", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            await Task.Delay(250);

            while (true)
            {
                progress = (progress + 1) % 100;
                pbRunning.Value = progress;

                while (_backLogMessages.TryDequeue(out string result))
                {
                    Messages.Add(result);
                }

                await Task.Delay(250);
            }

        }




        public async void AsyncAwaitExercise1_Click(object sender, RoutedEventArgs e)
        {
            Output("Exercise 1 Start");

            await SyncContext.Lib.ExploreAsyncAwait.AsyncAwait_A();

            // DispatcherSynchronizationContext is the reason this doesn't error
            // Ensures this runs on the UI thread
            Messages.Add("Exercise 1 Continuation");

            Output("Exercise 1 Done");
        }

        public async void AsyncAwaitExercise1b_Click(object sender, RoutedEventArgs e)
        {
            Output("Exercise 1b Start");

            ExecutionContext.SuppressFlow();
            await SyncContext.Lib.ExploreAsyncAwait.AsyncAwait_A();

            Output("Exercise 1b Done");
        }

        public async void AsyncAwaitExercise1c_Click(object sender, RoutedEventArgs e)
        {
            Output("Exercise 1c Start");

            // This how MsTest, Console Application, ASP.NET Core and more run
            // Libraries shared between those platforms and WPF/FORMS/UWP will behave differently

            SynchronizationContext.SetSynchronizationContext(null);

            await SyncContext.Lib.ExploreAsyncAwait.AsyncAwait_A();

            SynchronizationContext.SetSynchronizationContext(context);

            // Note: Not on the UI thread
            // Any interaction with the UI thread like this will cause an error
            // This is why .ConfigureAwait(false) is the answer not setting the static SynchronizationContext.Current to null
            Messages.Add("Error!");

            Output("Exercise 1c Done");
        }


        public async void AsyncAwaitExercise2_Click(object sender, RoutedEventArgs e)
        {
            Output("Exercise 2 Start");

            // During Time.Delay break the application
            // View the Task Debug Window
            // View the thread window - Where is the Main Thread? Answer: Waiting for more messages
            await SyncContext.Lib.ExploreAsyncAwait.AsyncAwait_A(pause: 10000);

            Output("Exercise 2 End");

        }

        public void AsyncAwaitExercise3_Click(object sender, RoutedEventArgs e)
        {
            Output("Exercise 3");

            // Exercise 3 - Don't Await
            // Appears to "Work"
            // Don't ignore warning!
            SyncContext.Lib.ExploreAsyncAwait.AsyncAwait_A();

            Output("Exercise 3 Done.");

        }

        public void AsyncAwaitExercise4_Click(object sender, RoutedEventArgs e)
        {
            Output("Exercise 4");

            // Exercise 4 - Don't Await - Throw Exception
            SyncContext.Lib.ExploreAsyncAwait.ThrowException();

            Output("Exercise 4 Done");
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

            Output("Exercise 5 Done");

        }

        public void AsyncAwaitExercise6_Click(object sender, RoutedEventArgs e)
        {
            Output("Exercise 6");

            // Exercise 6 - .Wait() w/ Default of ConfigureAwait(true)
            // Deadlock!!
            if (!SyncContext.Lib.ExploreAsyncAwait.AsyncAwait_A(pause: 500).Wait(5000))
            {
                throw new Exception("Deadlock!!");
            }

            Output("Exercise 6 Done");

        }

        public void AsyncAwaitExercise7_Click(object sender, RoutedEventArgs e)
        {
            Output("Exercise 7");

            // Exercise 7 - .Wait() with ConfigureAwait(false)
            // Deadlock resolved the correct way using ConfigureAwait(false)
            // UI Thread Blocked 
            // Note the log - Different thread completes the task (Logical Execution 7+) that the main thread is waiting for to complete alleviating the deadlock
            ExploreAsyncAwait.AsyncAwait_A(continueOnCapturedSynchronizationContext: false, pause: 5000)
                .Wait();

            Messages.Add("Exercise 7 Done");

        }

        public void AsyncAwaitExercise8_Click(object sender, RoutedEventArgs e)
        {
            Output("Exercise 8");

            // Exercise 8 - SynchronizationContext.Current = Null
            // Same as ConfigureAwait(false) in Exercise 7
            // But brittle because SynchronizationContext.Current is static so you'll have unexpected behavior in other threads

            SynchronizationContext.SetSynchronizationContext(null);

            SyncContext.Lib.ExploreAsyncAwait.AsyncAwait_A(pause: 3000).Wait();

            // UI Thread
            // No Await so SynchronizationContext doesn't come into play
            Output("Exercise 8 Continuation");

            SynchronizationContext.SetSynchronizationContext(context);

            Output("Exercise 8 Done");

        }

        public void AsyncAwaitExercise9_Click(object sender, RoutedEventArgs e)
        {
            Output("Exercise 9");

            // Exercise 9 - .Wait() fix -> ContinueWith

            // Task Continuation
            // UI Thread isn't blocked
            // But if it's this easy just add async void to the button click handler
            SyncContext.Lib.ExploreAsyncAwait.AsyncAwait_A(pause: 10).ContinueWith(x =>
            {
                if (x.Exception != null) { throw x.Exception; } // Be careful not to loose the exception. It's your responsibility!
                Output("Exercise 9 Continuation");
            });

            Output("Exercise 9 Done");

        }

        public void AsyncAwaitExercise10_Click(object sender, RoutedEventArgs e)
        {

            Output("Exercise 10");

            // Exercise 10 - .Wait() fix -> Task.Run
            // In a nested method adding async void isn't the right

            NestedBusinessLogicMethod(); // I want this to finish before moving on

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
                await SyncContext.Lib.ExploreAsyncAwait.AsyncAwait_A(pause: 3000);

                // This causes an error because we are not running on the UI thread
                // But the main thread is blocked
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

        private int count = 0;
        public async void AsyncAwaitExercise11_Click(object sender, RoutedEventArgs e)
        {

            var run = count; count++;

            Output($"Exercise 11 Run # {run}");

            // Exercise 11 - Task Queue

            // Each click creates a new TaskCompletionSource object
            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
            var t = lastTask;

            // Discuss: Should I have a lock around this? How many threads can read this point??
            lastTask = taskCompletionSource.Task;
            await t;

            await Task.Delay(1000);

            // Allow the next task to Execute
            taskCompletionSource.SetResult(null);

            Output($"Exercise 11 Done #{run}");

        }

        public async void AsyncAwaitExercise12_Click(object sender, RoutedEventArgs e)
        {
            var myCount = count; count++;

            Output($"Exercise 12 #{myCount}");

            // Seperate out the TaskQueue logic from the business logic (Count = Count + 1)

            await TaskQueue(async (c) =>
            {
                await Task.Delay(1000, c);
                Output($"Exercise 12 #{myCount} Done");
            });
        }

        // Shows how TAP was meant for you to write generic Task logic to work for whatever delegate within the task
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

            Output("Exercise 13...");

            // Notice the second time we call we don't 
            // execute the Task.Delay
            await initAsync.DoWork();

            Output("Exercise 13 Done");
        }


        public async void AsyncAwaitExercise14_Click(object sender, RoutedEventArgs e)
        {
            Output("Exercise 14");

            Stopwatch sw = new Stopwatch();

            Task[] someTasks = new Task[3];

            someTasks[0] = Task.Delay(2000);
            someTasks[1] = Task.Delay(2000);
            someTasks[2] = Task.Delay(2000);

            sw.Restart();
            await Task.WhenAll(someTasks);
            sw.Stop();

            Output($"Exercise 14 Done {sw.ElapsedMilliseconds} ");

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
                await ExploreAsyncAwait.AsyncAwait_A();

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
                        ExploreAsyncAwait.Log(11, Output);
                        // And safely interact with UI controls
                        Output("Exercise 17...");
                    }, null);

                    taskCompletionSource.SetResult(null);

                }, null);

                // Without this we to Spot B before we get to Spot A
                await taskCompletionSource.Task;

                // Spot B
            });



        }

        public void AsyncAwaitExercise18_Click(object sender, RoutedEventArgs e)
        {
            Output($"Exercise 18 Start");

            var count = 0;
            AutoResetEvent canContinue = new AutoResetEvent(false);
            var sc = SynchronizationContext.Current;

            Timer timer = new Timer((o) =>
            {
                count++;

                sc.Post(p =>
                {
                    Output($"Exercise 18 {p}");
                }, count);

                if (count == 4)
                {
                    canContinue.Set();
                }
            }, null, 500, 500);

            canContinue.WaitOne();
            timer.Dispose();

            Output($"Exercise 18 Done");

        }

        public async void AsyncAwaitExercise19_Click(object sender, RoutedEventArgs e)
        {
            Output($"Exercise 19 Start");

            var count = 0;
            TaskCompletionSource<int> taskCompletionSource = new TaskCompletionSource<int>();
            var sc = SynchronizationContext.Current;

            Timer timer = new Timer((o) =>
            {
                count++;

                sc.Post(p =>
                {
                    Output($"Exercise 19 {p}");
                }, count);

                if (count == 4)
                {
                    taskCompletionSource.SetResult(4);
                }
            }, null, 500, 500);

            await taskCompletionSource.Task;
            timer.Dispose();

            Output($"Exercise 19 Done");

        }

        private void Output(string message)
        {

            var id = System.Threading.Thread.CurrentThread.ManagedThreadId;

            if (id != 1)
            {
                // Use DispatcherSynchronizationContext.Post for a coule of exercises
                // where SynchronizationContext.Current is set to null
                _backLogMessages.Enqueue($"ThreadID {System.Threading.Thread.CurrentThread.ManagedThreadId} {message}");
            }
            else
            {
                // Show in the correct order - Make it a little more clear
                while (_backLogMessages.TryDequeue(out string result))
                {
                    Messages.Add(result);
                }

                Messages.Add($"UI Thread {message}");
            }

        }

        public void btnClear_Click(object sender, RoutedEventArgs e)
        {
            Messages.Clear();
        }
    }
}
