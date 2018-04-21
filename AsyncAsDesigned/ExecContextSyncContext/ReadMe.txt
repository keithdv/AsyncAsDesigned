
Summary: Exercises to see the behavior of Task, ExecutionContext, SynchronizationContext and ConfigureAwait
See corresponding diagram

Throughout this Task.Delay represents any asynchronous operation 

Exercise 1 - Baseline
  Console Application - AsyncAwait_A(true)
     - Different thread on the continuations because SynchronizationContext.Current == null and thread 1 is stuck at .Wait()
     - All three ThreadLocals loose their value on the new thread
     - AsyncLocal correctly retains its value following the logical execution
	 - Culture code is correctly changed on each continuation
   WPF Application - AsyncAwait_A(true)
     - Same thread on the continuations because SynchronizationContext.Current = DispatcherSynchronizationContext
     - ThreadLocals retained on continuation
     - AsyncLocal correctly "loses" values on continuation
	 - Culture code is correctly changed on each continuation
     
Exercise 2 - Task and Thread Debugger Window
   AsyncAwait_A(pause: 10000) - Pause with enough time to break
   Break during pause
   View the Task Debugger Window
     Note the 5 tasks. Each await creates a task.
         Hover over the tasks - 4 are awaiting on their "child" task
         Note: The WPF application has additional tasks for the busy indicator bar
   View the Thread Debug window
         Console Application 
             - Main Thread - ManualResetEventSlim.Wait - the application can't continue until the Task finishes otherwise it will exit!
         WPF Application 
             - Main Thread - Waiting for message - .EXE doesn't exit until told to so waiting for the next message
             - UI Thread is not blocked
             
Exercise 3 - Don't Await AsyncAwait_A
   View Output Window
   Console Application
      Failure: Application exits before the Task.Delay and the continuations executes.
   WPF Application
      "Appears" to work. No errors and the Task does run because the .EXE stays running.
      However, notice that the Count is incremented immediately - it is not a continuation
      You will see similar behavior in ASP.NET because the ApplicationPool is still running so the Task Continuations may run
      but the request is long over.

Exercise 4 - Don't Await AsyncAwait_A - Throw Exception
   WPF Application - Exception disappears!
   Added to the Task which is never await(ed), .Wait() or .Result

Exercise 5 - Throw Exception - await, .Wait(), .Result
   Each time the exception is thrown
   Remember - Task is an instance

Exercise 6 - .Wait() with Default of ConfigureAwait(true)
    Console Application
      Completes Successfully - No Issues
      Break during Task.Delay - Note the MainThread is in the same holding pattern as Exercise 2 - ManualResetEventSlim.Wait
    WPF Application
      Deadlock!! 
      Note the Task Debug Window - The Task at Execution Location 4 is [Scheduled and Waiting to Run]
      However because of the DispatcherSynchronizationContext it's scheduled to run on the Main Thread
      Which when freed at Task.Delay recieved the .Wait() call
         Hover of the "Location" and you'll see the CallSTack - You see ManualResetEventSlim.Wait then DispatcherSynchronizationContext.Wait
      So the MainThread is waiting for the Task to Complete but the Task is waiting for the MainThread - Deadlock!!

Exercise 7 = .Wait() with ConfigureAwait(false)
    WPF Application
    Deadlock fixed. 
    UI Thread Blocked 
    Note Output Window
     Different thread completes the task (Logical Execution 7+) that the main thread is waiting for to complete alleviating the deadlock         
     ThreadLocal information is gone however AsyncLocal is in tact
    This is why it is recommended to have ConfigureAwait(false) in all libraries - To keep this option available
      
Exercise 8 - SynchronizationContext.Current = Null
    Though it can cause DeadLocks this shows that DispaterSynchronizationContext is important. 
    Without it application doesn't continue on the UI Thread causing an exception when we hit data binding.
    This is why ConfigureAwait(false) was provided in the design.
    
Exercise 9 - - .Wait() fix -> ContinueWith
     Simple DeadLock fix if you dont' have dependant code after the task
 
Exercise 10 - .Wait() fix -> Task.Run
     Sync to Async bridge using an additional thread
     Instead of allowing the Task Continuation to occur on a different thread (.ConfigureAwait(false))
     We are executing the Task and it's continuation on a different thread than the main thread
     and that thread is never blocked, only the main thread, so we don't have a deadlock
     See this in the Output Window - All points are not ThreadID 1 but are the same thread
     When the newly created task is executed the Main Thread is already blocked
     This is wrapping an asyncronous operation with multithreading - this is adding back in thread contention issues

Exercise 11 - Task Queue Logic
     WPF Application
     Hammer on 'Exercise 1' button and you'll notice that the tasks are running in parrallel. The count jumps up
     THis may be good. However, you may want the tasks to wait for each other.
  
Exercise 12 - TaskQueue Method
     System.Thread.Tasks.Task was provided, in part, to implement generic logic around tasks
     This shows how to wrap a task into a TaskQueue so only one task runs at a time
     How to use Task.WhenAny and Task.Delay to implement a timeout
     and how to use CancellationToken to cancel the executing Task
     
Exercise 13 - Constructor Task
     There is no async constructor (and there shouldn't be)
     One possible design is to create an initialize method
     This shows how you can "hide" this initialize

Exercise 14 - Task.WhenAll
    Run multiple Tasks in Parallel
    
Exercise 15 - Task.WhenAll under control
     Likely it's a bad idea to create too many tasks - both for client and server
     Easy to use Task.WhenAll but chunk out the Task Batches
     Yes, this method takes 1 second instead of 100 ms
     but in a real-world situation this is many times wise
     
Exercise 16 - Null SynchronizationContext Warning
     The default ASP.NET Core SynchronizationContext is null
     See Stephen Clearly's page https://blog.stephencleary.com/2017/03/aspnetcore-synchronization-context.html
     He provides a warning under "Beware Implicit Parallelism
     This highlights that warning
     NullSyncContext is sometimes off (<100) because multiple threads are running Count++
     Because there's No SynchronozationContext to continue all Count++
     executions on the same thread
     
     Both NotNullSyncContext and NullSyncContext should produce 100
     However NullSyncContext can return less
     
     Solution: Change the approach. Asynchronous code should be written functionally.
      That's a broad field but worth researching.
      
Exercise 17 - ExecutionContext.Run
     See how AsyncLocal has the same values as AsyncAwait_C
     This is because AsyncLocal is within a container within ExecutionContext
     Execution.Run is part of back-end mechanics of asynchronous forks (await, Task.Run)
