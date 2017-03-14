// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Calculator.Client
{
    using System;
    using System.ServiceModel;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
    using System.Fabric;

    internal class Program
    {
        private static void Main(string[] args)
        {
            Uri ServiceName = new Uri("fabric:/CalculatorApplicationWcf/CalculatorServiceWcf");
            ServicePartitionResolver serviceResolver = new ServicePartitionResolver(() => new FabricClient());
            NetTcpBinding binding = CreateClientConnectionBinding();
            //Client calcClient = new Client(new WcfCommunicationClientFactory<ICalculatorService>
            //           (serviceResolver, binding, null), ServiceName);
            Client calcClient = new Client(new WcfCommunicationClientFactory<ICalculatorService>(
                clientBinding: CreateClientConnectionBinding())
                , ServiceName
                );
            Console.WriteLine(calcClient.AddAsync(3, 5).Result);
            Console.ReadKey();
            Console.WriteLine(calcClient.SubtractAsync(19, 10).Result);
            Console.ReadLine();

        }

        private static NetTcpBinding CreateClientConnectionBinding()
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None)
            {
                SendTimeout = TimeSpan.MaxValue,
                ReceiveTimeout = TimeSpan.MaxValue,
                OpenTimeout = TimeSpan.FromSeconds(5),
                CloseTimeout = TimeSpan.FromSeconds(5),
                MaxReceivedMessageSize = 1024 * 1024
            };
            binding.MaxBufferSize = (int)binding.MaxReceivedMessageSize;
            binding.MaxBufferPoolSize = Environment.ProcessorCount * binding.
        MaxReceivedMessageSize;
            return binding;
        }

    }

    /////<summary>
    ///// It uses ClientFactoryBase which in turn provides various features like resolving endpoints during Service Failover  , ExceptionHandling and maintains a cache of communication
    ///// clients and attempts to reuse the clients for requests to the same service endpoint.
    ///// Its using BasicHttpBinding.
    /////  </summary>
    //public class CalculatorClient : ServicePartitionClient<WcfCommunicationClient<ICalculatorService>>, ICalculatorService
    //{
    //    private static ICommunicationClientFactory<WcfCommunicationClient<ICalculatorService>> communicationClientFactory;

    //    static CalculatorClient()
    //    {
    //        communicationClientFactory = new WcfCommunicationClientFactory<ICalculatorService>(
    //            clientBinding: CreateClientConnectionBinding());
    //    }

    //    public CalculatorClient(Uri serviceUri)
    //        : this(serviceUri, ServicePartitionKey.Singleton)
    //    {
    //    }

    //    public CalculatorClient(
    //        Uri serviceUri,
    //        ServicePartitionKey partitionKey)
    //        : base(
    //            communicationClientFactory,
    //            serviceUri,
    //            partitionKey)
    //    {
    //    }

    //    private static NetTcpBinding CreateClientConnectionBinding()
    //    {
    //        NetTcpBinding binding = new NetTcpBinding(SecurityMode.None)
    //        {
    //            SendTimeout = TimeSpan.MaxValue,
    //            ReceiveTimeout = TimeSpan.MaxValue,
    //            OpenTimeout = TimeSpan.FromSeconds(5),
    //            CloseTimeout = TimeSpan.FromSeconds(5),
    //            MaxReceivedMessageSize = 1024 * 1024
    //        };
    //        binding.MaxBufferSize = (int)binding.MaxReceivedMessageSize;
    //        binding.MaxBufferPoolSize = Environment.ProcessorCount * binding.
    //    MaxReceivedMessageSize;
    //        return binding;
    //    }
    //    public Task<string> AddAsync(int n1, int n2)
    //    {
    //        return this.InvokeWithRetry(
    //            (c) => c.Channel.AddAsync(n1, n2));

    //    }

    //    public Task<string> SubtractAsync(int n1, int n2)
    //    {
    //        return this.InvokeWithRetry(
    //            (c) => c.Channel.SubtractAsync(n1, n2));
    //    }
    //}

    public class Client : ServicePartitionClient<WcfCommunicationClient<ICalculatorService>>,
ICalculatorService
    {
        public Client(WcfCommunicationClientFactory<ICalculatorService> clientFactory,
             Uri serviceName)
               : base(clientFactory, serviceName)
        {
        }
        public Task<string> AddAsync(int a, int b)
        {
            return this.InvokeWithRetryAsync(client => client.Channel.AddAsync(a, b));
        }
        public Task<string> SubtractAsync(int a, int b)
        {
            return this.InvokeWithRetryAsync(client => client.Channel.SubtractAsync(a, b));
        }
    }
}