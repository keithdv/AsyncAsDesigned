using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using SyncContext.Lib;

namespace SyncContext.WebApiNetFramework.Controllers
{

    public class TaskExceptionController : ApiController
    {

        [HttpGet()]
        public void Method()
        {
            TaskException.Method();
        }

        [HttpGet()]
        public async Task MethodAsync()
        {
            await TaskException.MethodAsync();
        }


    }
}
