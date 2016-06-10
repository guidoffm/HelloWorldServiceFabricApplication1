using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;


namespace QueueService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class QueueService : StatefulService
    {
        public static IReliableStateManager StateManagerInstance { get; private set; }

        public QueueService(StatefulServiceContext context)
            : base(context)
        {
            StateManagerInstance = StateManager;
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see http://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            //ServiceEventSource.Current.CreateCommunicationListener(ServiceEventSourceName);

            return new[]
            {
                new ServiceReplicaListener(initParams => new OwinCommunicationListener(new Startup(StateManager), initParams))
            };
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            IReliableQueue<string> reliableQueue = await StateManager.GetOrAddAsync<IReliableQueue<string>>("Queue1");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = StateManager.CreateTransaction())
                {
                    //var result = await reliableQueue.TryDequeueAsync(tx);

                    //ServiceEventSource.Current.ServiceMessage(this, "Current Counter Value: {0}", result.HasValue ? result.Value : "Value does not exist.");

                    //if (result.HasValue)
                    //{
                    //    await reliableQueue.EnqueueAsync(tx, result.Value);

                    //}

                    await reliableQueue.EnqueueAsync(tx, DateTimeOffset.Now.ToString());

                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is saved to the secondary replicas.
                    await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
