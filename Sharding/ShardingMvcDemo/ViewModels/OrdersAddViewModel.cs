using System.ComponentModel.DataAnnotations;

namespace ShardingMvcDemo.ViewModels
{
    public class OrdersAddViewModel
    {
        [Display(Name = "Client ID")]
        public string ClientId { get; set; }

        [Display(Name = "Product name")]
        public string ProductName { get; set; }
        
        [Display(Name = "Product price")]
        public double ProductPrice { get; set; }

        [Display(Name = "Payment amount")]
        public double PaymentAmount { get; set; }

        [Display(Name = "Payment ID")]
        public string PaymentId { get; set; }
    }
}