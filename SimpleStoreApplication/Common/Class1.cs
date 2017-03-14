using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public struct ShoppingCartItem
    {
        public string ProductName { get; set; }
        public double UnitPrice { get; set; }
        public int Amount { get; set; }
        public double LineTotal
        {
            get
            {
                return Amount * UnitPrice;
            }
        }
    }

    [ServiceContract]
    public interface IShoppingCartService
    {
        [OperationContract]
        Task AddItem(ShoppingCartItem item);
        [OperationContract]
        Task DeleteItem(string productName);
        [OperationContract]
        Task<List<ShoppingCartItem>> GetItems();
    }
}
