using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.ViewModels.Service
{
    class Service
    {
        private int id;
        private string name;
        private string price;
        private bool isActive;

        public int ID { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Price { get => price; set => price = value; }
        public bool IsActive { get => isActive; set => isActive = value; }
    }
}
