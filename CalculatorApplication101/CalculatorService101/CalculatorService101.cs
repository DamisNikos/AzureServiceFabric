using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;

namespace CalculatorService101
{
    public interface ICalculatorService : IService
    {
        Task<string> AddAsync(int b, int a);
        Task<string> SubtractAsync(int a, int b);
    }
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class CalculatorService101 : StatelessService, ICalculatorService
    {
        public CalculatorService101(StatelessServiceContext context)
            : base(context)
        { }
        public Task<string> AddAsync(int b, int a)
        {
            return Task.FromResult<string>(string.Format("Instance {0} returns: {1}",
        this.Context.InstanceId, a + b));
        }
        public Task<string> SubtractAsync(int a, int b) {
            return Task.FromResult<string>(string.Format("Instance {0} returns: {1}",
        this.Context.InstanceId, a - b));
        }
        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[] { new ServiceInstanceListener(context =>
            this.CreateServiceRemotingListener(context)) };
        }

       
    }
}
