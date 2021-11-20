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

        #region Top services used
        private int _selectedTop;
        public int SelectedTop { get { return _selectedTop; } set { _selectedTop = value; OnPropertyChanged(); } }

        private List<int> _tops;
        public List<int> Tops { get { return _tops; } set { _tops = value; OnPropertyChanged(); } }

        private string _selectedMode;
        public string SelectedMode { get { return _selectedMode; } set { _selectedMode = value; OnPropertyChanged(); } }

        private List<string> _modes;
        public List<string> Modes { get { return _modes; } set { _modes = value; OnPropertyChanged(); } }

        private DateTime _timeReport;
        public DateTime TimeReport { get { return _timeReport; } set { _timeReport = value; OnPropertyChanged(); } }

        private string _priceOrRevenue;
        public string PriceOrRevenue { get { return _priceOrRevenue; } set { _priceOrRevenue = value; OnPropertyChanged(); } }

        private ObservableCollection<ServiceReportItem> _services;
        public ObservableCollection<ServiceReportItem> Services { get { return _services; } set { _services = value; OnPropertyChanged(); } }
        #endregion

        #region Revenue of service
        private List<string> _labels;
        public List<string> Labels
        {
            get => _labels;
            set
            {
                _labels = value;
                OnPropertyChanged();
            }
        }

        private Func<double, string> _yFormatter;
        public Func<double, string> YFormatter
        {
            get => _yFormatter;
            set
            {
                _yFormatter = value;
                OnPropertyChanged();
            }
        }

        private SeriesCollection _seriesCollection;
        public SeriesCollection SeriesCollection
        {
            get => _seriesCollection;
            set
            {
                _seriesCollection = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #endregion

        #region Command
        public ICommand QuantityCommand { get; set; }
        public ICommand RevenueCommand { get; set; }
        public ICommand ReloadCommand { get; set; }
        #endregion

        public TopServiceViewModel()
        {
            InitProperties();

            QuantityCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                PriceOrRevenue = "Price";
                LoadService();
            });

            RevenueCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                PriceOrRevenue = "Revenue";
                LoadService();
            });

            ReloadCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                LoadService();
            });
        }

        void InitProperties()
        {
            Services = new ObservableCollection<ServiceReportItem>();
            PriceOrRevenue = "Price";

            Tops = new List<int>();
            Tops.Add(5);
            Tops.Add(10);
            Tops.Add(20);
            Tops.Add(50);
            Tops.Add(100);
            SelectedTop = 5;

            Modes = new List<string>();
            Modes.Add("Month");
            Modes.Add("Year");
            SelectedMode = "Month";

            TimeReport = DateTime.Now.AddMonths(-1);
            LoadService();
        }

        void LoadService()
        {
            if (Services.Count > 0)
                Services.Clear();
            List<ServiceReportItem> Items = new List<ServiceReportItem>();

            if (PriceOrRevenue == "Price")
                Items = LoadServicesQuantity(SelectedMode, SelectedTop, TimeReport);
            else
                Items = LoadServicesRevenue(SelectedMode, SelectedTop, TimeReport);

            int index = 1;
            foreach(var item in Items)
            {
                item.Index = index;
                item.Price = SeparateThousands(item.Price);

                Services.Add(item);
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
                    x => DbFunctions.DiffMonths(x.RESERVATION.departure, timeReport) == 0).ToList();
            }   
            else if (mode == "Year")
            {
                roomBookedList = DataProvider.Instance.DB.ROOM_BOOKED.Where(
                    x => DbFunctions.DiffYears(x.RESERVATION.departure, timeReport) == 0).ToList();
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
                            folio.SERVICE.name, price.ToString(), folio.amount.Value);

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
                    x => DbFunctions.DiffMonths(x.RESERVATION.departure, timeReport) == 0).ToList();
            }
            else if (mode == "Year")
            {
                roomBookedList = DataProvider.Instance.DB.ROOM_BOOKED.Where(
                    x => DbFunctions.DiffYears(x.RESERVATION.departure, timeReport) == 0).ToList();
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
                            folio.SERVICE.name, (price * folio.amount.Value).ToString(), folio.amount.Value);

                        serviceReports.Add(item);
                    }
                    else
                    {
                        int price = (int)folio.SERVICE.price.Value;
                        isItemExist.Quantity += folio.amount.Value;
                        isItemExist.Price = (price * isItemExist.Quantity).ToString();
                    }
                }
            }

            return serviceReports.Take(selectedTop).OrderByDescending(x => Convert.ToInt32(x.Price)).ToList();
        }

        public void LoadChartMonth(DateTime timeReport)
        {
            SeriesCollection = new SeriesCollection();
            //foreach (int item in Years)
            {
                //para.lbGuestChart.Items.Add(new ListBoxItem() { Content = item.ToString() });
                SeriesCollection.Add(new LineSeries
                {
                    //Title = item.ToString(),
                    //Values = getValueOfMonth(item)
                });
            }

            Labels.Clear();
            string[] labelMonth = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            Labels = labelMonth.ToList();

            YFormatter = value => value.ToString();
        }
    }
}
