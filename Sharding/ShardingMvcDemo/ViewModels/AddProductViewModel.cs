using System.ComponentModel.DataAnnotations;

namespace ShardingMvcDemo.ViewModels
{
    public class AddProductViewModel
    {
        [Display(Name = "Order ID")]
        public string OrderId { get; set; }

        [Display(Name = "Product name")]
        public string ProductName { get; set; }

        [Display(Name = "Product price")]
        public double ProductPrice { get; set; }
    }
}