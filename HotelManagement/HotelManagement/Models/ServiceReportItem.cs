using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Models
{
    class ServiceReportItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public int Amount { get; set; }

        public ServiceReportItem(int id, string name, string price)
        {
            Id = id;
            Name = name;
            Price = price;
        }

        public ServiceReportItem(int id, string name, string price, int amount)
        {
            Id = id;
            Name = name;
            Price = price;
            Amount = amount;
        }
    }
}
