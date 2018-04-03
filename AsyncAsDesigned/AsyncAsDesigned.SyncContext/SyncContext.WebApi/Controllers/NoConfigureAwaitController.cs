using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SyncContext.Lib;

namespace SyncContext.WebApi.Controllers
{

    public class NoConfigureAwaitController : Controller
    {

        [HttpGet()]
        public int Increment(int count)
        {
            // This blocks in ASP.NET MVC w/ .Net Framework
            var result = NoConfigureAwait.Increment(count).Result;
            return result;
        }

        [HttpGet()]
        public async Task<int> IncrementAsync(int count)
        {
            // This blocks in ASP.NET MVC w/ .Net Framework
            var result = await NoConfigureAwait.Increment(count);
            return result;
        }


    }
}
