﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApi.Controllers
{

    /// <summary>
    /// Async Await is NOT Fire and Forget
    /// </summary>
    public class MissingAwaitController : ApiController
    {

        private static object CountLock = new object();
        private static int Count = 0;

        private static List<Task> tasks = new List<Task>();

        private async Task ExampleAsyncMethod()
        {
            await Task.Delay(100);
            lock (CountLock)
            {
                Count++;
            }
        }

        [HttpPost]
        [Route("NoAwait_ManyTasks")]
        public async Task<IHttpActionResult> NoWait()
        {
            for (var i = 0; i < 50; i++)
            {
                Task.Delay(10).Wait();
                await ExampleAsyncMethod();
            }
            return Ok();
        }

        [HttpGet]
        [Route("GetCount")]
        public IHttpActionResult GetCount()
        {
            return Ok(Count);
        }

        private async Task ExceptionAsyncMethod()
        {
            await Task.Delay(10);
            throw new Exception("To Infinity..and beyond!!");
            // Something Important is Missed 
        }

        [HttpPost]
        [Route("NoAwait_Exception")]
        public async Task<IHttpActionResult> NoAwaitException()
        {
            await ExceptionAsyncMethod();
            Task.Delay(100).Wait();
            return Ok();
        }

    }
}
