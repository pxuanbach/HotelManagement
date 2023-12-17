using HotelManagement.Models;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HotelManagement.ViewModels
{
    class ServiceByRoomTypeViewModel : BaseViewModel
    {
        #region Properties
        private List<ROOMTYPE> _roomTypes;
        public List<ROOMTYPE> RoomTypes { get { return _roomTypes; } set { _roomTypes = value; OnPropertyChanged(); } }

        private List<int> _years;
        public List<int> Years
        {
            get { return _years; }
            set { _years = value; OnPropertyChanged(); }
        }

        private List<string> labels;
        public List<string> Labels
        {
            get => labels;
            set
            {
                labels = value;
                OnPropertyChanged();
            }
        }

        private Func<double, string> yFormatter;
        public Func<double, string> YFormatter
        {
            get => yFormatter;
            set
            {
                yFormatter = value;
                OnPropertyChanged();
            }
        }

        private SeriesCollection seriesCollection;
        public SeriesCollection SeriesCollection
        {
            get => seriesCollection;
            set
            {
                seriesCollection = value;
                OnPropertyChanged();
            }
        }

        private string _selectedMode;
        public string SelectedMode
        {
            get { return _selectedMode; }
            set { _selectedMode = value; OnPropertyChanged(); }
        }

        private string _visibility;
        public string Visibility
        {
            get { return _visibility; }
            set { _visibility = value; OnPropertyChanged(); }
        }

        private List<string> _modes;
        public List<string> Modes { get { return _modes; } set { _modes = value; OnPropertyChanged(); } }

        private DateTime _timeReport;
        public DateTime TimeReport
        {
            get { return _timeReport; }
            set { _timeReport = value; OnPropertyChanged(); }
        }
        
        private DateTime _dateStart;
        public DateTime DateStart
        {
            get { return _dateStart; }
            set { _dateStart = value; OnPropertyChanged(); }
        }

        private DateTime _dateEnd;
        public DateTime DateEnd
        {
            get { return _dateEnd; }
            set { _dateEnd = value; OnPropertyChanged(); }
        }
        #endregion

        #region Command
        public ICommand CheckedBoxTypeCommand { get; set; }
        public ICommand UncheckedBoxTypeCommand { get; set; }
        public ICommand SelectionChangedCommand { get; set; }
        public ICommand ReloadMonthCommand { get; set; }
        #endregion

        public ServiceByRoomTypeViewModel()
        {
            InitProperties();

            CheckedBoxTypeCommand = new RelayCommand<ROOMTYPE>((p) =>
            {
                return true;
            }, (p) => 
            {
                CheckedBoxType(p);
            });

            UncheckedBoxTypeCommand = new RelayCommand<ROOMTYPE>((p) =>
            {
                return true;
            }, (p) =>
            {
                UncheckedBoxType(p);
            });

            SelectionChangedCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                if (SelectedMode == "Month")
                {
                    Visibility = "Visible";
                    LoadChartMonth(TimeReport.Year);
                }    
                else if (SelectedMode == "Year")
                {
                    Visibility = "Collapsed";
                    LoadChartYear();
                }    
                    
            });

            ReloadMonthCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                LoadChartMonth(TimeReport.Year);
            });
        }

        void InitProperties()
        {
            Labels = new List<string>();
            RoomTypes = new List<ROOMTYPE>(
                DataProvider.Instance.DB.ROOMTYPEs.Where(x => x.date_updated == null).ToList());
            List<int> temp = new List<int>(
                DataProvider.Instance.DB.RESERVATIONs.Select(x => x.departure.Value.Year)
                .Distinct().ToList());
            Years = new List<int>();
            Years = temp.OrderBy(x => x).ToList();

            Modes = new List<string>();
            Modes.Add("Month");
            Modes.Add("Year");
            SelectedMode = "Month";
            Visibility = "Visible";

            if (Years.FirstOrDefault() <= 0)
            {
                TimeReport = new DateTime(1900, 1, 1);
                DateStart = new DateTime(1900, 1, 1);
                DateEnd = new DateTime();
            } else
            {
                TimeReport = new DateTime(Years.FirstOrDefault(), 1, 1);
                DateStart = new DateTime(Years.First(), 1, 1);
                DateEnd = new DateTime(Years[Years.Count - 1], 1, 1);
            }
            LoadChartMonth(TimeReport.Year);
        }

        #region Checkbox room type
        void CheckedBoxType(ROOMTYPE roomType)
        {
            if (SelectedMode == "Month")
            {
                SeriesCollection.Add(new LineSeries
                {
                    Title = roomType.name,
                    Values = GetValueOfMonth(TimeReport.Year, roomType)
                });
            }   
            else if (SelectedMode == "Year")
            {
                SeriesCollection.Add(new LineSeries
                {
                    Title = roomType.name,
                    Values = GetValueOfYear(roomType)
                });
            }    
        }

        void UncheckedBoxType(ROOMTYPE roomType)
        {
            LineSeries removeLine = new LineSeries();
            foreach (LineSeries line in SeriesCollection)
            {
                if (line.Title == roomType.name)
                    removeLine = line;
            }
            SeriesCollection.Remove(removeLine);
        }
        #endregion

        public void LoadChartMonth(int year)
        {
            SeriesCollection = new SeriesCollection();
            foreach (var item in RoomTypes)
            {
                SeriesCollection.Add(new LineSeries
                {
                    Title = item.name,
                    Values = GetValueOfMonth(year, item)
                });
            }

            Labels.Clear();
            string[] labelMonth = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            Labels = labelMonth.ToList();

            YFormatter = value => value.ToString();
        }

        ChartValues<long> GetValueOfMonth(int year, ROOMTYPE roomType)
        {
            ChartValues<long> chartValues = new ChartValues<long>();
            long temp = 0;

            for (int i = 1; i <= 12; i++)
            {
                temp = GetRevenueByMonth(i, year, roomType);
                chartValues.Add(temp);
            }

            return chartValues;
        }

        long GetRevenueByMonth(int month, int year, ROOMTYPE roomType)
        {
            long revenue = 0;
            List<ROOM_BOOKED> roomBookedList = new List<ROOM_BOOKED>(
                DataProvider.Instance.DB.ROOM_BOOKED.Where(
                    x => x.RESERVATION.departure.Value.Year == year 
                    && x.RESERVATION.departure.Value.Month == month
                    && x.RESERVATION.status == "Completed" 
                    && x.ROOM.ROOMTYPE.name == roomType.name).ToList());
            
            foreach (var roomBooked in roomBookedList)
            {
                foreach (var folio in roomBooked.FOLIOs)
                {
                    int price = (int)folio.SERVICE.price.Value;
                    revenue += (price * folio.amount.Value);
                }
            }

            return revenue;
        }

        public void LoadChartYear()
        {
            SeriesCollection = new SeriesCollection();
            foreach (var item in RoomTypes)
            {
                SeriesCollection.Add(new LineSeries
                {
                    Title = item.name,
                    Values = GetValueOfYear(item)
                });
            }

            Labels.Clear();

            if (Years.Count > 0 & Years.Count < 6)
            {
                Labels.Add((Years[0] - 1).ToString());
            }

            foreach (int item in Years)
            {
                Labels.Add(item.ToString());
            }

            YFormatter = value => value.ToString();
        }

        ChartValues<long> GetValueOfYear(ROOMTYPE roomType)
        {
            ChartValues<long> chartValues = new ChartValues<long>();
            long temp = 0;

            if (Years.Count == 0)
            {
                temp = 0;
                chartValues.Add(temp);
            }

            if (Years.Count > 0 & Years.Count < 6)
            {
                temp = GetRevenueByYear(Years[0] - 1, roomType);
                chartValues.Add(temp);
            }

            foreach (int item in Years)
            {
                temp = GetRevenueByYear(item, roomType);
                chartValues.Add(temp);
            }

            return chartValues;
        }

        long GetRevenueByYear(int year, ROOMTYPE roomType)
        {
            long revenue = 0;
            List<ROOM_BOOKED> roomBookedList = new List<ROOM_BOOKED>(
                DataProvider.Instance.DB.ROOM_BOOKED.Where(
                    x => x.RESERVATION.departure.Value.Year == year
                    && x.RESERVATION.status == "Completed"
                    && x.ROOM.ROOMTYPE.name == roomType.name).ToList());

            foreach (var roomBooked in roomBookedList)
            {
                foreach (var folio in roomBooked.FOLIOs)
                {
                    int price = (int)folio.SERVICE.price.Value;
                    revenue += (price * folio.amount.Value);
                }
            }

            return revenue;
        }
    }
}
