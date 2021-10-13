using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelManagement.Views;
using HotelManagement.Models;
using System.Windows.Input;

namespace HotelManagement.ViewModels
{
    class ServicesViewModel : BaseViewModel
    {
        public string Title { get; } = "Services";
        public ServicesView ServicesView { get; set; }

        //public class Service : BaseViewModel
        //{
        //    private int id;
        //    private string name;
        //    private string price;

        //    public int Id { get { return id; } set { id = value; OnPropertyChanged(); } }
        //    public string Name { get { return name; } set { name = value; OnPropertyChanged(); } }
        //    public string Price { get { return price; } set { price = value; OnPropertyChanged(); } }
        //}

       

        private ObservableCollection<SERVICE> itemSourceServices = new ObservableCollection<SERVICE>();
        public ObservableCollection<SERVICE> ItemSourceServices
        {
            get => itemSourceServices;
            set
            {
                itemSourceServices = value;
                OnPropertyChanged();
            }
        }
        public ServicesViewModel()
        {
            LoadServices(ServicesView);
        }

        private void LoadServices(ServicesView servicesView)
        {
            this.ItemSourceServices.Clear();
            List<SERVICE> listSERVICEs = GetSERVICEs();
            foreach (var SERVICE in listSERVICEs)
            {
                //if(SERVICE.isActive == true)
                //{
                //    Service service = new Service();
                //    service.Id = SERVICE.id;
                //    service.Name = SERVICE.name;
                //    service.Price = ConvertToString(((long?)SERVICE.price));
                //    ItemSourceServices.Add(service);
                //}
                ItemSourceServices.Add(SERVICE);
            }    
        }

        private List<SERVICE> GetSERVICEs()
        {
            List<SERVICE> res = new List<SERVICE>();
            res = DataProvider.Instance.DB.SERVICEs.ToList<SERVICE>();
            return res;
        }
    }
}
