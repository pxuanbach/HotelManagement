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

        #region Top services (Quantity)
        private int _selectedTopQuantity;
        public int SelectedTopQuantity 
        { 
            get { return _selectedTopQuantity; } 
            set { _selectedTopQuantity = value; OnPropertyChanged(); } 
        }

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

        #region Top services (Revenue)
        private int _selectedTopRevenue;
        public int SelectedTopRevenue
        {
            get { return _selectedTopRevenue; }
            set { _selectedTopRevenue = value; OnPropertyChanged(); }
        }

        private List<int> _topsRevenue;
        public List<int> TopsRevenue { get { return _topsRevenue; } set { _topsRevenue = value; OnPropertyChanged(); } }

        private string _selectedModeRevenue;
        public string SelectedModeRevenue
        {
            get { return _selectedModeRevenue; }
            set { _selectedModeRevenue = value; OnPropertyChanged(); }
        }

        private List<string> _modesRevenue;
        public List<string> ModesRevenue { get { return _modesRevenue; } set { _modesRevenue = value; OnPropertyChanged(); } }

        private DateTime _timeReportRevenue;
        public DateTime TimeReportRevenue
        {
            get { return _timeReportRevenue; }
            set { _timeReportRevenue = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ServiceReportItem> _servicesRevenue;
        public ObservableCollection<ServiceReportItem> ServicesRevenue
        {
            get { return _servicesRevenue; }
            set { _servicesRevenue = value; OnPropertyChanged(); }
        }
        #endregion

        #endregion

        #region Command
        public ICommand ReloadQuantityCommand { get; set; }
        public ICommand ReloadRevenueCommand { get; set; }

        #endregion

        public TopServiceViewModel()
        {
            InitProperties();

            ReloadQuantityCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                LoadQuantity();
            });

            ReloadRevenueCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                LoadRevenue();
            });
        }

        void InitProperties()
        {
            #region Quantity
            ServicesQuantity = new ObservableCollection<ServiceReportItem>();

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
            #endregion

            #region Revenue
            ServicesRevenue = new ObservableCollection<ServiceReportItem>();

            TopsRevenue = new List<int>();
            TopsRevenue.Add(5);
            TopsRevenue.Add(10);
            TopsRevenue.Add(20);
            TopsRevenue.Add(50);
            TopsRevenue.Add(100);
            SelectedTopRevenue = 5;

            ModesRevenue = new List<string>();
            ModesRevenue.Add("Month");
            ModesRevenue.Add("Year");
            SelectedModeRevenue = "Month";

            TimeReportRevenue = DateTime.Now.AddMonths(-1);
            LoadRevenue();
            #endregion
        }

        void LoadQuantity()
        {
            if (ServicesQuantity.Count > 0)
                ServicesQuantity.Clear();
            List<ServiceReportItem> Items = new List<ServiceReportItem>();

            Items = LoadServicesQuantity(SelectedModeQuantity, SelectedTopQuantity, TimeReportQuantity);

            int index = 1;
            foreach(var item in Items)
            {
                item.Index = index;
                ServicesQuantity.Add(item);
                index++;
            }    
        }

        void LoadRevenue()
        {
            if (ServicesRevenue.Count > 0)
                ServicesRevenue.Clear();
            List<ServiceReportItem> Items = new List<ServiceReportItem>();

            Items = LoadServicesRevenue(SelectedModeRevenue, SelectedTopRevenue, TimeReportRevenue);

            int index = 1;
            foreach (var item in Items)
            {
                item.Index = index;
                item.Price = SeparateThousands(item.Price);
                item.Revenue = SeparateThousands(item.Revenue);
                ServicesRevenue.Add(item);
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

        List<ServiceReportItem> LoadServicesRevenue(string mode, int selectedTop, DateTime timeReport)
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
                            folio.SERVICE.name, price.ToString(), folio.amount.Value, (price * folio.amount.Value).ToString());

                        serviceReports.Add(item);
                    }
                    else
                    {
                        int price = (int)folio.SERVICE.price.Value;
                        isItemExist.Quantity += folio.amount.Value;
                        isItemExist.Revenue = (price * isItemExist.Quantity).ToString();
                    }
                }
            }

            return serviceReports.Take(selectedTop).OrderByDescending(x => Convert.ToInt32(x.Revenue)).ToList();
        }
    }
}
