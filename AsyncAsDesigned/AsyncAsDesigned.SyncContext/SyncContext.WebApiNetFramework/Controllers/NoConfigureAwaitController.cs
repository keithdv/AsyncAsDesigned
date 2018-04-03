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

    [AllowAnonymous]
    public class NoConfigureAwaitController : ApiController
    {

        [HttpGet]
        public int Increment(int count)
        {
            return NoConfigureAwait.Increment(count).Result;
        }

        [HttpGet]
        public async Task<int> IncrementAsync(int count)
        {
            return await NoConfigureAwait.Increment(count);
        }
    }
}
