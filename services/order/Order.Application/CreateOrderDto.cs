using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application
{
    public class CreateOrderDto
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
