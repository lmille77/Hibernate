using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hibernate.Models
{
    public class Order_Item
    {
        [Key]
        public int OrderItemsId { get; set; }

        public Order Order { get; set; }
        public int OrderId { get; set; }

        public Item Item { get; set; }
        public int ItemId { get; set; }
    }
}
