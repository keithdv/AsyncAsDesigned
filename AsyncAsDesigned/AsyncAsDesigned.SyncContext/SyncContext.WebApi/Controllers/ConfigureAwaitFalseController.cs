using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SyncContext.Lib;

namespace SyncContext.WebApi.Controllers
{
    public class ConfigureAwaitFalseController : Controller
    {

        // GET api/values/5
        [HttpGet]
        public int Increment(int count)
        {
            // This blocks in ASP.NET MVC w/ .Net Framework
            // Doesn't block here because SyncronizationContext.Current is Null
            return ConfigureAwaitFalse.Increment(count).Result;
        }

        [HttpGet]
        public async Task<int> IncrementAsync(int count)
        {
            return await ConfigureAwaitFalse.Increment(count);
        }

    }
}
