using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _06_WebApiWithSqlDb.Models
{
    public class Order
    {
        public Order()
        {
            this.OrderItems = new List<Product>();
        }

        public int Id { get; set; }

        public string CustomerName { get; set; }

        public DateTime OrderDate { get; set; }

        public string Email { get; set; }

        public double Amount { get; set; }

        public string Status { get; set; }
        
        public virtual IEnumerable<Product> OrderItems { get; set; }

        
    }
}
