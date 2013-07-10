using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OrderModel;

namespace ShardingMvcDemo.ViewModels
{
    public class OrdersHomeViewModel
    {
        public List<Order> Orders { get; set; }

        [Display(Name = "Current page")]
        public int CurrentPage { get; set; }

        [Display(Name = "Items per page")]
        public int ItemsPerPage { get; set; }
    }
}