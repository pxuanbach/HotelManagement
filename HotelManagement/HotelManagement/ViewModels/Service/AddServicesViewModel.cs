using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using HotelManagement.Models;
using HotelManagement.Views;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;

namespace HotelManagement.ViewModels
{
    class AddServicesViewModel : BaseViewModel
    {
        #region Folio
        private int folioCount;
        public int FolioCount { get { return folioCount; } set { folioCount = value; OnPropertyChanged(); } }

        private ObservableCollection<FolioDisplayItem> folio = new ObservableCollection<FolioDisplayItem>();
        public ObservableCollection<FolioDisplayItem> Folio
        {
            get { return folio; }
            set { folio = value; OnPropertyChanged(); }
        }

        private string folioTotalMoney;
        public string FolioTotalMoney { get { return folioTotalMoney; } set { folioTotalMoney = value; OnPropertyChanged(); } }

        private decimal totalMoney = 0;
        public decimal TotalMoney { get => totalMoney; set { totalMoney = value; OnPropertyChanged(); } }
        #endregion

        private ObservableCollection<Service> services = new ObservableCollection<Service>();
        public ObservableCollection<Service> Services
        {
            get => services;
            set
            {
                services = value;
                OnPropertyChanged();
            }
        }

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

        #region Selected Services
        private Service selectedService;
        public Service SelectedService
        {
            get { return selectedService; }
            set
            {
                selectedService = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Search
        private string contentSearch;
        public string ContentSearch
        {
            get { return contentSearch; }
            set
            {
                contentSearch = value;
                OnPropertyChanged();
            }
        }
        #endregion

        private string roomName;
        public string RoomName
        {
            get => roomName;
            set
            {
                roomName = value;
                OnPropertyChanged();
            }
        }

        private int roomID;
        public int RoomID
        {
            get => roomID;
            set
            {
                roomID = value;
                OnPropertyChanged();
            }
        }

        #region Command
        public ServicesView ServicesView { get; set; }

        public ICommand SearchServiceCommand { get; set; }
        public ICommand CloseWindowCommand { get; set; }
        public ICommand AddServiceCommand { get; set; }
        public ICommand RemoveServiceCommand { get; set; }
        public ICommand SaveFolioCommnad { get; set; }
        #endregion

        public AddServicesViewModel()
        {
            LoadServices();
            SearchServiceCommand = new RelayCommand<ServicesView>((p) => true, (p) => Search(p));
            CloseWindowCommand = new RelayCommand<Window>((p) =>
            {
                return true;
            }, (p) =>
            {
                p.Close();
            });
            AddServiceCommand = new RelayCommand<Service>((p) => true, (p) => AddService(p));
            RemoveServiceCommand = new RelayCommand<Service>((p) => true, (p) => RemoveSerivce(p));
            SaveFolioCommnad = new RelayCommand<Window>((p) => true, (p) => SaveFolio(p));
        }

        #region Search
        private void Search(ServicesView servicesView)
        {
            this.ServicesView = servicesView;
            if (ContentSearch == "")
            {
                LoadServices();
            }
            else
            {
                ItemSourceServices = new ObservableCollection<SERVICE>(
                    DataProvider.Instance.DB.SERVICEs.Where(
                        x => x.name.ToLower().Contains(ContentSearch.ToLower())));
                Services.Clear();
                foreach (var SERVICE in ItemSourceServices)
                {
                    Service service = new Service();
                    service.ID = SERVICE.id;
                    service.Name = SERVICE.name;
                    service.IsActive = (bool)SERVICE.isActive;
                    service.Price = ConvertToString((long)SERVICE.price);
                    Services.Add(service);
                }
            }
        }
        #endregion

        #region Load
        private void LoadServices()
        {
            this.ItemSourceServices.Clear();
            this.Services.Clear();
            List<SERVICE> listSERVICEs = GetSERVICEs();
            foreach (var SERVICE in listSERVICEs)
            {
                ItemSourceServices.Add(SERVICE);
                Service service = new Service();
                service.ID = SERVICE.id;
                service.Name = SERVICE.name;
                service.IsActive = (bool)SERVICE.isActive;
                service.Price = ConvertToString((long)SERVICE.price);
                Services.Add(service);
            }
        }

        private void AddService(Service p)
        {
            var service = DataProvider.Instance.DB.SERVICEs.SingleOrDefault(x => x.id == SelectedService.ID);
            var folioItem = Folio.FirstOrDefault(x => x.Id == service.id);

            if (folioItem == null)
            {
                FolioDisplayItem folioDisplayItem = new FolioDisplayItem(service.id, service.name, 1);
                Folio.Add(folioDisplayItem);
            } else
            {
                Folio.Remove(folioItem);
                folioItem.Amount += 1;
                Folio.Add(folioItem);
            }
            TotalMoney += (decimal)service.price;
            FolioTotalMoney = ConvertToString((long)TotalMoney);
        }

        private void RemoveSerivce(Service p)
        {
            var service = DataProvider.Instance.DB.SERVICEs.SingleOrDefault(x => x.id == SelectedService.ID);
            var folioItem = Folio.FirstOrDefault(x => x.Id == service.id);

            if (folioItem == null)
            {
                return;
            }
            else
            {
                Folio.Remove(folioItem);
                folioItem.Amount -= 1;
                if (folioItem.Amount == 0)
                {
                    return;
                }
                else 
                {
                    Folio.Add(folioItem); 
                }
            }
            if (TotalMoney > 0)
            {
                TotalMoney -= (decimal)service.price;
            }
            else
            {
                TotalMoney = 0;
            }
            FolioTotalMoney = ConvertToString((long)TotalMoney);
        }

        private void SaveFolio(Window p)
        {
            foreach(var item in Folio)
            {
                FOLIO folio = new FOLIO()
                {
                    service_id = item.Id,
                    amount = item.Amount,
                    room_booked_id = RoomID,
                };
                DataProvider.Instance.DB.FOLIOs.Add(folio);
                DataProvider.Instance.DB.SaveChanges();
            }
            string message = "Register to use the service successfully";
            string caption = "Register service";
            DialogResult result = System.Windows.Forms.MessageBox.Show(message, caption, MessageBoxButtons.OK);
            if (result == DialogResult.OK)
            {
                p.Close();
            }
            else
            {
                p.Close();
            }
        }

        private List<SERVICE> GetSERVICEs()
        {
            List<SERVICE> res = new List<SERVICE>();
            res = DataProvider.Instance.DB.SERVICEs.Where(x => x.isActive == true).ToList<SERVICE>();
            return res;
        }

        public void getRoomName(string roomBookedName)
        {
            RoomName = roomBookedName;
        }

        public void getRoomBookedId(int roombookedID)
        {
            RoomID = roombookedID;
        }
        #endregion
    }
}
