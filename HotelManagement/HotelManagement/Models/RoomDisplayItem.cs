using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Models
{
    class RoomDisplayItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Price { get; set; }
        public List<FolioDisplayItem> folio { get; set; }

        public RoomDisplayItem(int id, string name, string type)
        {
            Id = id;
            Name = name;
            Type = type;
        }

        public RoomDisplayItem(int id, string name, string type, string price)
        {
            Id = id;
            Name = name;
            Type = type;
            Price = price;
        }
    }
}
