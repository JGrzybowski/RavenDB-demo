using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ClientDAL;
using OrderModel;
using Services;
using ShardingMvcDemo.Raven;
using ShardingMvcDemo.ViewModels;

namespace ShardingMvcDemo.Controllers
{
    public class OrdersController : Controller
    {
        private readonly RavenDbConnectionManager _rdcm = new RavenDbConnectionManager();

        //
        // GET: /Orders/
        public ActionResult Index(int pageNumber = 1, int itemsPerPage = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (itemsPerPage < 1) itemsPerPage = 1;

            using (var rdbc = new RavenDbConnection(_rdcm))
            {
                var orderService = new OrderService(rdbc);
                var orders = orderService.GetOrders(pageNumber, itemsPerPage);
                var viewModel = new OrdersHomeViewModel
                {
                    CurrentPage = pageNumber,
                    ItemsPerPage = itemsPerPage,
                    Orders = orders.ToList()
                };
                return View(viewModel);
            }
        }

        //
        // GET: /Orders/AddOrder?clientId=1
        public ActionResult AddOrder(string clientId)
        {
            var viewModel = new OrdersAddViewModel
            {
                ClientId = clientId
            };
            return View(viewModel);
        }

        //
        // POST: /Orders/AddOrder
        [HttpPost]
        public ActionResult AddOrder(OrdersAddViewModel order)
        {
            using (var rdbc = new RavenDbConnection(_rdcm))
            {
                var orderService = new OrderService(rdbc);
                var clientService = new ClientService(rdbc);
                var client = clientService.GetClient(order.ClientId);

                var newOrder = new Order
                {
                    ClientId = client.Id,
                    ClientFirstName = client.FirstName,
                    ClientLastName = client.LastName,
                    ClientCountry = client.Country,
                    Payments = new List<Payment>
                    {
                        new Payment
                        {
                            Amount = order.PaymentAmount,
                            Id = order.PaymentId
                        }
                    },
                    Products = new List<Product>
                    {
                        new Product
                        {
                            Name = order.ProductName,
                            Price = order.ProductPrice
                        }
                    },
                    TimeOfOrder = DateTime.Now
                };
                orderService.AddOrder(newOrder);
            }
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Orders/AddPayment?orderId=orders/1
        public ActionResult AddPayment(string orderId)
        {
            return View(new AddPaymentViewModel
            {
                OrderId = orderId
            });
        }

        //
        // POST: /Orders/AddPayment
        [HttpPost]
        public ActionResult AddPayment(AddPaymentViewModel newPayment)
        {
            using (var rdbc = new RavenDbConnection(_rdcm))
            {
                var orderService = new OrderService(rdbc);
                orderService.AddPayment(newPayment.OrderId, newPayment.PaymentId, newPayment.PaymentAmount);
            }
            return RedirectToAction("Index", "Orders");
        }

        //
        // GET: /Orders/AddProduct?orderId=orders/1
        public ActionResult AddProduct(string orderId)
        {
            return View(new AddProductViewModel
            {
                OrderId = orderId
            });
        }

        //
        // POST: /Orders/AddProduct
        [HttpPost]
        public ActionResult AddProduct(AddProductViewModel newProduct)
        {
            using (var rdbc = new RavenDbConnection(_rdcm))
            {
                var orderService = new OrderService(rdbc);
                orderService.AddProduct(newProduct.OrderId, newProduct.ProductName, newProduct.ProductPrice);
            }
            return RedirectToAction("Index", "Orders");
        }

        //
        // GET: /Orders/DeleteOrder?orderId=1
        public ActionResult DeleteOrder(string orderId)
        {
            using (var rdbc = new RavenDbConnection(_rdcm))
            {
                var orderService = new OrderService(rdbc);
                orderService.DeleteOrder(orderId);
            }
            return RedirectToAction("Index");
        }
    }
}
