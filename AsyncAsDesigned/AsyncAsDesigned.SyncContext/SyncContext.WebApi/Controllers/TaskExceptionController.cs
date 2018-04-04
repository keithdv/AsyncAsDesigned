using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SyncContext.Lib;

namespace SyncContext.WebApi.Controllers
{

    public class TaskExceptionController : Controller
    {

        [HttpGet()]
        public void Method()
        {

            // Very bad senario - Exception is lost
            // Exception is thrown within an unawaited async and SynchronizationContext.Current is null
            // So there is no task to assign the exception thru and no SynchronizationContext to account for it either
            // So the response is "OK"

            TaskException.Method();
        }

        [HttpGet()]
        public async Task MethodAsync()
        {
            await TaskException.MethodAsync();
        }


    }
}
