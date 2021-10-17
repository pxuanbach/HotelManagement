using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Models
{
    class FolioDisplayItem
    {
        public int Id { get; set; } //Id of service
        public string ServiceName { get; set; }
        public int Amount { get; set; }
        public string Price { get; set; }

        public FolioDisplayItem(int id, string serviceName, int amount)
        {
            Id = id;
            ServiceName = serviceName;
            Amount = amount;
        }

        public FolioDisplayItem(int id, string serviceName, int amount, string price)
        {
            Id = id;
            ServiceName = serviceName;
            Amount = amount;
            Price = price;
        }
    }
}
