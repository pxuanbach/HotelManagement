using HotelManagement.Models;
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
        private int _selectedTop;
        public int SelectedTop { get { return _selectedTop; } set { _selectedTop = value; OnPropertyChanged(); } }

        private List<int> _tops;
        public List<int> Tops { get { return _tops; } set { _tops = value; OnPropertyChanged(); } }
        
        private DateTime _timeReport;
        public DateTime TimeReport { get { return _timeReport; } set { _timeReport = value; OnPropertyChanged(); } }

        private string _selectedMode;
        public string SelectedMode { get { return _selectedMode; } set { _selectedMode = value; OnPropertyChanged(); } }

        private ObservableCollection<ServiceReportItem> _services;
        public ObservableCollection<ServiceReportItem> Services { get { return _services; } set { _services = value; OnPropertyChanged(); } }
        #endregion

        #region Command
        public ICommand MonthCommand { get; set; }
        public ICommand YearCommand { get; set; }
        public ICommand ReloadCommand { get; set; }
        #endregion

        public TopServiceViewModel()
        {
            InitProperties();

            MonthCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                SelectedMode = "Month";
                LoadService();
            });

            YearCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                SelectedMode = "Year";
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
            SelectedMode = "Month";

            Tops = new List<int>();
            Tops.Add(5);
            Tops.Add(10);
            Tops.Add(20);
            Tops.Add(50);
            Tops.Add(100);
            SelectedTop = 5;

            TimeReport = DateTime.Now.AddMonths(-1);
            LoadService();
        }

        void LoadService()
        {
            if (Services.Count > 0)
                Services.Clear();

            if (SelectedMode == "Month")
            {
                LoadServiceByMonth();
            } 
            else if (SelectedMode == "Year")
            {
                LoadServiceByYear();
            }    
        }

        void LoadServiceByMonth()
        {
            var roomBookedList = DataProvider.Instance.DB.ROOM_BOOKED.Where(
                    x => DbFunctions.DiffMonths(x.RESERVATION.departure, TimeReport) == 0).ToList();
            ObservableCollection<ServiceReportItem> serviceReports = new ObservableCollection<ServiceReportItem>();

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
                        isItemExist.Amount += folio.amount.Value;
                    }
                }
            }

            foreach (var item in serviceReports.Take(SelectedTop).OrderByDescending(x => x.Amount))
            {
                Services.Add(item);
            }
        }

        void LoadServiceByYear()
        {
            var roomBookedList = DataProvider.Instance.DB.ROOM_BOOKED.Where(
                    x => DbFunctions.DiffYears(x.RESERVATION.departure, TimeReport) == 0).ToList();
            ObservableCollection<ServiceReportItem> serviceReports = new ObservableCollection<ServiceReportItem>();

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
                        isItemExist.Amount += folio.amount.Value;
                    }
                }
            }

            foreach (var item in serviceReports.Take(SelectedTop).OrderByDescending(x => x.Amount))
            {
                Services.Add(item);
            }
        }
    }
}
