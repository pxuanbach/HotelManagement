using HotelManagement.Models;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HotelManagement.ViewModels
{
    class TopServiceViewModel : BaseViewModel
    {
        #region Properties
        private int _selectedTopQuantity;
        public int SelectedTopQuantity 
        { 
            get { return _selectedTopQuantity; } 
            set { _selectedTopQuantity = value; OnPropertyChanged(); } 
        }

        private string _selectedType;
        public string SelectedType
        {
            get { return _selectedType; }
            set { _selectedType = value; OnPropertyChanged(); }
        }

        private List<string> _types;
        public List<string> Types { get { return _types; } set { _types = value; OnPropertyChanged(); } }

        private List<int> _topsQuantity;
        public List<int> TopsQuantity { get { return _topsQuantity; } set { _topsQuantity = value; OnPropertyChanged(); } }

        private string _selectedModeQuantity;
        public string SelectedModeQuantity 
        { 
            get { return _selectedModeQuantity; } 
            set { _selectedModeQuantity = value; OnPropertyChanged(); } 
        }

        private List<string> _modesQuantity;
        public List<string> ModesQuantity { get { return _modesQuantity; } set { _modesQuantity = value; OnPropertyChanged(); } }

        private DateTime _timeReportQuantity;
        public DateTime TimeReportQuantity 
        { 
            get { return _timeReportQuantity; } 
            set { _timeReportQuantity = value; OnPropertyChanged(); } 
        }

        private ObservableCollection<ServiceReportItem> _servicesQuantity;
        public ObservableCollection<ServiceReportItem> ServicesQuantity 
        { 
            get { return _servicesQuantity; } 
            set { _servicesQuantity = value; OnPropertyChanged(); } 
        }

        #endregion

        #region Command
        public ICommand ReloadCommand { get; set; }
        #endregion

        public TopServiceViewModel()
        {
            InitProperties();

            ReloadCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                LoadQuantity();
            });
        }

        void InitProperties()
        {
            ServicesQuantity = new ObservableCollection<ServiceReportItem>();

            Types = new List<string>(
                DataProvider.Instance.DB.ROOMTYPEs.Where(x => x.date_updated == null).Select(x => x.name).ToList());
            Types.Add("All");
            SelectedType = "All";

            TopsQuantity = new List<int>();
            TopsQuantity.Add(5);
            TopsQuantity.Add(10);
            TopsQuantity.Add(20);
            TopsQuantity.Add(50);
            TopsQuantity.Add(100);
            SelectedTopQuantity = 5;

            ModesQuantity = new List<string>();
            ModesQuantity.Add("Month");
            ModesQuantity.Add("Year");
            SelectedModeQuantity = "Month";

            TimeReportQuantity = DateTime.Now.AddMonths(-1);
            LoadQuantity();
        }

        void LoadQuantity()
        {
            if (ServicesQuantity.Count > 0)
                ServicesQuantity.Clear();
            List<ServiceReportItem> Items = new List<ServiceReportItem>();

            if (SelectedType == "All")
                Items = LoadServicesQuantity(SelectedModeQuantity, SelectedTopQuantity, TimeReportQuantity);
            else
                Items = LoadServicesQuantityByType(SelectedModeQuantity, SelectedTopQuantity, TimeReportQuantity, SelectedType);

            int index = 1;
            foreach(var item in Items)
            {
                item.Index = index;
                ServicesQuantity.Add(item);
                index++;
            }    
        }

        List<ServiceReportItem> LoadServicesQuantity(string mode, int selectedTop, DateTime timeReport)
        {
            List<ROOM_BOOKED> roomBookedList = new List<ROOM_BOOKED>();
            List<ServiceReportItem> serviceReports = new List<ServiceReportItem>();

            if (mode == "Month")
            {
                roomBookedList = DataProvider.Instance.DB.ROOM_BOOKED.Where(
                    x => DbFunctions.DiffMonths(x.RESERVATION.departure, timeReport) == 0
                    && x.RESERVATION.status == "Completed").ToList();
            }   
            else if (mode == "Year")
            {
                roomBookedList = DataProvider.Instance.DB.ROOM_BOOKED.Where(
                    x => DbFunctions.DiffYears(x.RESERVATION.departure, timeReport) == 0
                    && x.RESERVATION.status == "Completed").ToList();
            }    

            foreach (var roomBooked in roomBookedList)
            {
                foreach (var folio in roomBooked.FOLIOs)
                {
                    var isItemExist = serviceReports.FirstOrDefault(x => x.Id == folio.SERVICE.id);
                    if (isItemExist == null)
                    {
                        int price = (int)folio.SERVICE.price.Value;
                        ServiceReportItem item = new ServiceReportItem(folio.SERVICE.id,
                            folio.SERVICE.name, SeparateThousands(price.ToString()), folio.amount.Value);

                        serviceReports.Add(item);
                    }
                    else
                    {
                        isItemExist.Quantity += folio.amount.Value;
                    }
                }
            }

            return serviceReports.Take(selectedTop).OrderByDescending(x => x.Quantity).ToList();
        }

        List<ServiceReportItem> LoadServicesQuantityByType(string mode, int selectedTop, DateTime timeReport, string type)
        {
            List<ROOM_BOOKED> roomBookedList = new List<ROOM_BOOKED>();
            List<ServiceReportItem> serviceReports = new List<ServiceReportItem>();

            if (mode == "Month")
            {
                roomBookedList = DataProvider.Instance.DB.ROOM_BOOKED.Where(
                    x => DbFunctions.DiffMonths(x.RESERVATION.departure, timeReport) == 0
                    && x.ROOM.ROOMTYPE.name == type
                    && x.RESERVATION.status == "Completed").ToList();
            }
            else if (mode == "Year")
            {
                roomBookedList = DataProvider.Instance.DB.ROOM_BOOKED.Where(
                    x => DbFunctions.DiffYears(x.RESERVATION.departure, timeReport) == 0
                    && x.ROOM.ROOMTYPE.name == type
                    && x.RESERVATION.status == "Completed").ToList();
            }

            foreach (var roomBooked in roomBookedList)
            {
                foreach (var folio in roomBooked.FOLIOs)
                {
                    var isItemExist = serviceReports.FirstOrDefault(x => x.Id == folio.SERVICE.id);
                    if (isItemExist == null)
                    {
                        int price = (int)folio.SERVICE.price.Value;
                        ServiceReportItem item = new ServiceReportItem(folio.SERVICE.id,
                            folio.SERVICE.name, SeparateThousands(price.ToString()), folio.amount.Value);

                        serviceReports.Add(item);
                    }
                    else
                    {
                        isItemExist.Quantity += folio.amount.Value;
                    }
                }
            }

            return serviceReports.Take(selectedTop).OrderByDescending(x => x.Quantity).ToList();
        }
    }
}
