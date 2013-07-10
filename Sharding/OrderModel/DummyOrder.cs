using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderModel
{
    public class DummyOrder
    {
        public string ClientId { get; set; }
        public string ClientFirstName { get; set; }
        public string ClientLastName { get; set; }
        public string ClientCountry { get; set; }
        public List<Payment> Payments { get; set; }
        public List<Product> Products { get; set; }
        public DateTime TimeOfOrder { get; set; }

        public DummyOrder()
        {
            ClientId = "unknown";
            Payments = new List<Payment>();
            Products = new List<Product>();
            TimeOfOrder = DateTime.Now;
        }
        public DummyOrder(string clientId, string clientFirstName, string clientLastName, string clientCountry,
            List<Payment> payments, List<Product> products, DateTime timeOfOrder)
        {
            ClientId = clientId;
            ClientFirstName = clientFirstName;
            ClientLastName = clientLastName;
            ClientCountry = clientCountry;
            Payments = payments ?? new List<Payment>();
            Products = products ?? new List<Product>();
            TimeOfOrder = timeOfOrder;
        }

        public override string ToString()
        {
            return "Order [] for client [" + ClientId + "]";
        }

        public double Balance
        {
            get
            {
                double balance = 0;
                foreach (var product in Products)
                {
                    balance -= product.Price;
                }
                foreach (var payment in Payments)
                {
                    balance += payment.Amount;
                }
                return Math.Round(balance, 2);
            }
        }
        public void AddPayment(Payment payment)
        {
            Payments.Add(payment);
        }
        public void AddPayment(string paymentId, double amount)
        {
            Payments.Add(new Payment() { Amount = amount, Id = paymentId });
        }
        public void AddProduct(Product product)
        {
            Products.Add(product);
        }
        public void AddProduct(string name, double price)
        {
            Products.Add(new Product() { Name = name, Price = price });
        }
    }
}
