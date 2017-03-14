using Common;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SimpleStoreClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri ServiceName = new Uri("fabric:/SimpleStoreApplication/ShoppingCartService");
            ServicePartitionResolver serviceResolver = new ServicePartitionResolver(() => new FabricClient());
            NetTcpBinding binding = CreateClientConnectionBinding();
            //Client calcClient = new Client(new WcfCommunicationClientFactory<ICalculatorService>
            //           (serviceResolver, binding, null), ServiceName);
            Client shoppingClient = new Client(new WcfCommunicationClientFactory<IShoppingCartService>(
                clientBinding: CreateClientConnectionBinding())
                , ServiceName
                );

            shoppingClient.AddItem(new ShoppingCartItem
            {
                ProductName = "XBOX ONE",
                UnitPrice = 329.0,
                Amount = 2
            }).Wait();
            var list = shoppingClient.GetItems().Result;
            foreach (var item in list)
            {
                Console.WriteLine(string.Format("{0}: {1:C2} X {2} = {3:C2}",
                    item.ProductName,
                    item.UnitPrice,
                    item.Amount,
                    item.LineTotal));
            }
            Console.ReadKey();
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
            binding.MaxBufferPoolSize = Environment.ProcessorCount * binding.MaxReceivedMessageSize;
            return binding;
        }
    }

    public class Client : ServicePartitionClient<WcfCommunicationClient<IShoppingCartService>>, IShoppingCartService
    {
        public Client(WcfCommunicationClientFactory<IShoppingCartService> clientFactory,
             Uri serviceName)
               : base(clientFactory, serviceName, new ServicePartitionKey(1))
        {
        }

        public Task AddItem(ShoppingCartItem item)
        {
            return this.InvokeWithRetryAsync(client => client.Channel.AddItem(item));
        }

        public Task DeleteItem(string productName)
        {
            return this.InvokeWithRetryAsync(client => client.Channel.DeleteItem(productName));
        }

        public Task<List<ShoppingCartItem>> GetItems()
        {
            return this.InvokeWithRetryAsync(client => client.Channel.GetItems());
        }

    }
}

