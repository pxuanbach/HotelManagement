using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelManagement.Views;
using HotelManagement.Models;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Runtime;

namespace HotelManagement.ViewModels
{
    class ServicesViewModel : BaseViewModel
    {
        public string Title { get; } = "Service";
        #region Item Source
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
        #endregion

        #region Selected Services
        private Service selectedService;
        public Service SelectedService
        {
            get { return selectedService; }
            set
            {
                selectedService = value;
                OnPropertyChanged();
                DataProvider.Instance.DB.SaveChanges();
            }
        }
        #endregion

        #region Dialog Properties
        private bool isOpenDialog;
        public bool IsOpenDialog
        {
            get { return isOpenDialog; }
            set
            {
                isOpenDialog = value;
                OnPropertyChanged();
            }
        }
        private bool isReadOnlyServiceName;
        public bool IsReadOnlyServiceName
        {
            get { return isReadOnlyServiceName; }
            set
            {
                isReadOnlyServiceName = value;
                OnPropertyChanged();
            }
        }
        private string dialogTitle;
        public string DialogTitle
        {
            get { return dialogTitle; }
            set
            {
                dialogTitle = value;
                OnPropertyChanged();
            }
        }
        private string serviceName;
        public string ServiceName
        {
            get { return serviceName; }
            set
            {
                serviceName = value;
                OnPropertyChanged();
            }
        }

        private decimal servicePrice;
        public decimal ServicePrice
        {
            get { return servicePrice; }
            set
            {
                servicePrice = value;
                OnPropertyChanged();
            }
        }

        private string errorMessage;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                errorMessage = value;
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

        #region Command
        public ServicesView ServicesView { get; set; }

        public ICommand SearchServiceCommand { get; set; }
        public ICommand DeleteServiceCommand { get; set; }
        public ICommand EditServiceCommand { get; set; }
        public ICommand AddNewServiceCommand { get; set; }
        public ICommand SaveServiceCommand { get; set; }

        
        public ServicesViewModel()
        {
            IsOpenDialog = false;
            LoadServices();
            SearchServiceCommand = new RelayCommand<ServicesView>((p) => true, (p) => Search(p));
            DeleteServiceCommand = new RelayCommand<object>((p) => true, (p) => Delete());
            AddNewServiceCommand = new RelayCommand<object>((p) => true, (p) => { IsOpenDialog = true; DialogPropertiesChanged(); });
            SaveServiceCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                SaveService();
            });
        }
        #endregion

        #region New Service
        public void DialogPropertiesChanged()
        {
            ErrorMessage = "";
            DialogTitle = "New service";
            IsReadOnlyServiceName = false;
            ServiceName = "";
            ServicePrice = 0;
            
        }

        public void SaveService()
        {
            var serviceMount = DataProvider.Instance.DB.SERVICEs.Where(x => x.name == ServiceName && x.isActive == true).Count();
            if (serviceMount > 0)
            {
                ErrorMessage = "\"" + ServiceName + "\"" + " has already existed";
                ServiceName = "";
                return;
            }
            else if (String.IsNullOrEmpty(ServiceName))
            {
                ErrorMessage = "Please enter service name!";
                return;
            }
            else
            {
                SERVICE service = new SERVICE()
                {
                    name = ServiceName,
                    price = ServicePrice,
                    isActive = true,
                };
                DataProvider.Instance.DB.SERVICEs.Add(service);
                DataProvider.Instance.DB.SaveChanges();
            }
            LoadServices();
        }
        #endregion

        #region Delete
        private void Delete()
        {
            Service service = SelectedService;
            SERVICE sERVICE = DataProvider.Instance.DB.SERVICEs.Where(x => x.id == service.ID).FirstOrDefault();
            string message = "Do you want to delete this service?";
            string caption = "Delete service";
            DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes) 
            {
                sERVICE.isActive = false;
                DataProvider.Instance.DB.SaveChanges();
                LoadServices();
            }
            else
            {
                return;
            }
            
        }
        #endregion

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
                        x => x.name.ToLower().Contains(ContentSearch.ToLower()) ));
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
            IsOpenDialog = false;
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

        private List<SERVICE> GetSERVICEs()
        {
            List<SERVICE> res = new List<SERVICE>();
            res = DataProvider.Instance.DB.SERVICEs.Where(x => x.isActive == true).ToList<SERVICE>();
            return res;
        }
        #endregion
    }
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
