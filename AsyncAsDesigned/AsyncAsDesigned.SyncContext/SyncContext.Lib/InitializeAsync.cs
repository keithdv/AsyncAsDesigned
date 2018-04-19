using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SyncContext.Lib
{
    public class InitializeAsync
    {
        private Task _initializeTask;

        public InitializeAsync()
        {

            // Do NOT do the following. The exception is not handled
            InitializeIncorrect().ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    throw t.Exception;
                }
            });

            // Left commented but the following is an option
            // I would shy away from this
            // But if you really want to start the task in the constructor you can
            // Just ensure that you await that Task instance later
            // as we are doing in DoWork1

            //var t = Initialize(); // Putting it into a variable = no warning

        }

        private async Task InitializeIncorrect()
        {
            await Task.Delay(500);
            throw new Exception("Failure");
        }

        public async Task Initialize()
        {
            async Task _InitializeOnce()
            {
                await Task.Delay(500);
            }

            // This method can be called multiple times put the 
            // _InitializeOnce method will only be called once
            // Again, Task is an instance
            if (_initializeTask == null)
            {
                _initializeTask = _InitializeOnce();
            }

            await _initializeTask;
        }

        // Then either require that Initialized is called before any DoWork methods
        // Or make them all async and ensure Initialize is finished before they execute

        public async Task DoWork()
        {
            await Initialize();
        }

    }
}
