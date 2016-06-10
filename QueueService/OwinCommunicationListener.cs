using System;
using System.Diagnostics;
using System.Fabric;
using System.Fabric.Description;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace QueueService
{
    public class OwinCommunicationListener : ICommunicationListener
    {
        private readonly ServiceContext _serviceContext;

        /// <summary>
        /// OWIN server handle.
        /// </summary>
        private IDisposable _serverHandle;

        private readonly IOwinAppBuilder _startup;
        private string _publishAddress;
        private string _listeningAddress;
        private readonly string _appRoot;

        public OwinCommunicationListener(IOwinAppBuilder startup, ServiceContext serviceContext)
            : this(null, startup, serviceContext)
        {
        }

        public OwinCommunicationListener(string appRoot, IOwinAppBuilder startup, ServiceContext serviceContext)
        {
            this._startup = startup;
            this._appRoot = appRoot;
            this._serviceContext = serviceContext;
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            Trace.WriteLine("Initialize");

            EndpointResourceDescription serviceEndpoint = _serviceContext.CodePackageActivationContext.GetEndpoint("ServiceEndpoint");
            int port = serviceEndpoint.Port;

            if (_serviceContext is StatefulServiceContext)
            {
                //_listeningAddress = $"http://+:{port}/{_serviceContext.PartitionId}/{((StatefulServiceContext) _serviceContext).ReplicaId}/{Guid.NewGuid()}/";
                _listeningAddress = $"http://+:{port}/";
            }
            else if (_serviceContext is StatelessServiceContext)
            {
                _listeningAddress = $"http://+:{port}/{(string.IsNullOrWhiteSpace(_appRoot) ? "" : _appRoot.TrimEnd('/') + '/')}";
            }
            else
            {
                throw new InvalidOperationException();
            }

            this._publishAddress = this._listeningAddress.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);

            Trace.WriteLine($"Opening on {this._publishAddress}");

            try
            {
                Trace.WriteLine($"Starting web server on {this._listeningAddress}");

                this._serverHandle = WebApp.Start(this._listeningAddress, appBuilder => this._startup.Configuration(appBuilder));

                return Task.FromResult(this._publishAddress);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);

                this.StopWebServer();

                throw;
            }
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            Trace.WriteLine("Close");

            this.StopWebServer();

            return Task.FromResult(true);
        }

        public void Abort()
        {
            Trace.WriteLine("Abort");

            this.StopWebServer();
        }

        private void StopWebServer()
        {
            if (this._serverHandle != null)
            {
                try
                {
                    this._serverHandle.Dispose();
                }
                catch (ObjectDisposedException)
                {
                    // no-op
                }
            }
        }
    }
}