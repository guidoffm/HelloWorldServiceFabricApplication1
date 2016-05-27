using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Actor1.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;

namespace Web.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values 
        public async Task<IHttpActionResult> Get()
        {
            // Create a randomly distributed actor ID
            ActorId actorId = ActorId.CreateRandom();

            // This only creates a proxy object, it does not activate an actor or invoke any methods yet.
            var myActor = ActorProxy.Create<IActor1>(actorId, new Uri("fabric:/HelloWorldServiceFabricApplication1/Actor1ActorService"));

            // This will invoke a method on the actor. If an actor with the given ID does not exist, it will be activated by this method call.


            return Ok(await myActor.GetDateTimeOffset());
        }

        // GET api/values/5 
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values 
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5 
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5 
        public void Delete(int id)
        {
        }
    }
}
