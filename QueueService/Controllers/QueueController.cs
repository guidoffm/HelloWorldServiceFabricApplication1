using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.ServiceFabric.Data.Collections;

namespace QueueService.Controllers
{

    public class QueueController : ApiController
    {

        private static async Task<IReliableQueue<string>> XGetQueue()
        {

            var reliableQueue = await QueueService.StateManagerInstance.GetOrAddAsync<IReliableQueue<string>>("Queue1");
            return reliableQueue;
            //await Task.Delay(TimeSpan.Zero);
            //return null;
        }

        public async Task<IHttpActionResult> Get()
        {
            var q = await XGetQueue();

            using (var tx = QueueService.StateManagerInstance.CreateTransaction())
            {

                var result = await q.TryDequeueAsync(tx);

                //ServiceEventSource.Current.ServiceMessage(Web.Instance.Context, "Current Counter Value: {0}", result.HasValue ? result.Value : "Value does not exist.");

                if (result.HasValue)
                {
                    await tx.CommitAsync();
                    return Ok(result.Value);

                }

                tx.Abort();
                return NotFound();

                // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                // discarded, and nothing is saved to the secondary replicas.

            }
        }

        // POST api/values 
        public async Task<IHttpActionResult> Post([FromBody]string value)
        {
            //await Task.Delay(TimeSpan.Zero);
            //return null;
            var q = await XGetQueue();

            using (var tx = QueueService.StateManagerInstance.CreateTransaction())
            {

                await q.EnqueueAsync(tx, value);


                await tx.CommitAsync();
                return Ok();

            }
        }
    }
}