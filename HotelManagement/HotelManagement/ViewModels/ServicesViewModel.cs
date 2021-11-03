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
        private SERVICE selectedService;
        public SERVICE SelectedService
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
            EditServiceCommand = new RelayCommand<SERVICE>((p) =>
            {
                return ItemSourceServices.Count > 0;
            }, (p) =>
            {
                IsOpenDialog = true;
                DialogPropertiesChanged(SelectedService);
            });
            AddNewServiceCommand = new RelayCommand<object>((p) => true, (p) => { IsOpenDialog = true; DialogPropertiesChanged(null); });
            SaveServiceCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(ServiceName) && string.IsNullOrEmpty(ServicePrice.ToString()))
                    return false;
                else return true;
            }, (p) =>
            {
                SaveService(IsReadOnlyServiceName);
            });
        }
        #endregion

        #region New Service
        public void DialogPropertiesChanged(SERVICE p)
        {
            ErrorMessage = "";
            if (p == null)
            {
                DialogTitle = "New service";
                IsReadOnlyServiceName = false;
                ServiceName = "";
                ServicePrice = 0;
            }
            else
            {
                DialogTitle = "Edit service";
                IsReadOnlyServiceName = true;
                ServiceName = p.name;
                ServicePrice = (decimal)p.price;
            }
        }

        public void SaveService(bool isEdit)
        {
            if(isEdit)
            {
                SERVICE service = SelectedService;
                service.price = ServicePrice;
                DataProvider.Instance.DB.SaveChanges();
            }
            else
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
            }
            LoadServices();
        }
        #endregion

        #region Delete
        private void Delete()
        {
            SERVICE service = SelectedService;
            string message = "Do you want to delete this service?";
            string caption = "Delete service";
            DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes) 
            {
                service.isActive = false;
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
                        x => x.name.ToLower().Contains(ContentSearch.ToLower()) && x.isActive == true));
            }
        }
        #endregion

        #region Load
        private void LoadServices()
        {
            IsOpenDialog = false;
            this.ItemSourceServices.Clear();
            List<SERVICE> listSERVICEs = GetSERVICEs();
            foreach (var SERVICE in listSERVICEs)
            {
                ItemSourceServices.Add(SERVICE);
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
}
