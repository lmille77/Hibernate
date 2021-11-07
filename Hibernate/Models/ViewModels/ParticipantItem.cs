using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibernate.Models.ViewModels
{
    public class ParticipantItem
    {
        public IEnumerable<Participant> participants { get; set; }

        public IEnumerable<Order_Item> order_Items { get; set; }
    }
}
