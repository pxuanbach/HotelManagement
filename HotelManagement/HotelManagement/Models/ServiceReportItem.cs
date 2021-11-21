using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Models
{
    class ServiceReportItem
    {
        public int Index { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public string Revenue { get; set; }
        public int Quantity { get; set; }

        public ServiceReportItem(int id, string name, string price)
        {
            Id = id;
            Name = name;
            Price = price;
        }

        public ServiceReportItem(int id, string name, string price, int quantity)
        {
            Id = id;
            Name = name;
            Price = price;
            Quantity = quantity;
        }

        public ServiceReportItem(int id, string name, string price, int quantity, string revenue)
        {
            Id = id;
            Name = name;
            Price = price;
            Quantity = quantity;
            Revenue = revenue;
        }
    }
}
