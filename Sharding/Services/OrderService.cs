using System.Collections.Generic;
using ClientDAL;
using OrderModel;

namespace Services
{
    public class OrderService
    {
        private readonly RavenDbConnection _ravenDbConnection;

        public OrderService(RavenDbConnection rdbc)
        {
            _ravenDbConnection = rdbc;
        }

        public void AddOrder(Order order)
        {
            _ravenDbConnection.AddOrder(order);
        }

        public void AddPayment(string orderId, string paymentId, double paymentAmount)
        {
            _ravenDbConnection.AddPayment(orderId, paymentId, paymentAmount);
        }

        public void AddProduct(string orderId, string productName, double productPrice)
        {
            _ravenDbConnection.AddProduct(orderId, productName, productPrice);
        }

        public void DeleteOrder(string orderId)
        {
            _ravenDbConnection.DeleteOrder(orderId);
        }

        public IEnumerable<Order> GetOrders(int pageNumber, int itemsPerPage)
        {
            return _ravenDbConnection.GetOrders(pageNumber, itemsPerPage);
        }

        public Order GetOrder(string orderId)
        {
            return _ravenDbConnection.GetOrder(orderId);
        }
    }
}
