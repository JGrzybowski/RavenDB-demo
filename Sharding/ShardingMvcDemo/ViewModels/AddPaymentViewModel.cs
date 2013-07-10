using System.ComponentModel.DataAnnotations;

namespace ShardingMvcDemo.ViewModels
{
    public class AddPaymentViewModel
    {
        [Display(Name = "Order ID")]
        public string OrderId { get; set; }

        [Display(Name = "Payment amount")]
        public double PaymentAmount { get; set; }

        [Display(Name = "Payment ID")]
        public string PaymentId { get; set; }
    }
}