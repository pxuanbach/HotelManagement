using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelManagement.Views;
using HotelManagement.Models;
using HotelManagement.Resources.UC;
using System.Windows.Input;
using System.Runtime;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace HotelManagement.ViewModels
{
    class RevenueChart : BaseViewModel
    {

        private List<int> years;
        public List<int> Years { get => years; set { years = value; OnPropertyChanged(); } }

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

        public ICommand InitCommand { get; set; }
        public ICommand ChooseItemCommand { get; set; }
        public ICommand ChangeTypeCommand { get; set; }

        public RevenueChart()
        {
            InitCommand = new RelayCommand<UC_RevenueChart>((para) => true, (para) => Init(para));
            ChooseItemCommand = new RelayCommand<ListBox>((para) => true, (para) => ChooseItem(para));
            ChangeTypeCommand = new RelayCommand<UC_RevenueChart>((para) => true, (para) => ChangeType(para));

            Years = DataProvider.Instance.DB.RESERVATIONs.Select(x => x.arrival.Value.Year).Distinct().ToList();
        }
        private void ChangeType(UC_RevenueChart para)
        {
            if (para.cbbChangeType.SelectedIndex == 0)
            {
                LoadChartMonth(para);
                para.txtNameChart.Text = "Chart of Revenue by Month";
                para.grdYears.Visibility = Visibility.Visible;
                para.titleX.Title = "Month";
            }
            else
            {
                LoadChartYear(para);
                para.txtNameChart.Text = "Chart of Revenue by Year";
                para.grdYears.Visibility = Visibility.Collapsed;
                para.titleX.Title = "Year";
            }
        }

        private void ChooseItem(ListBox para)
        {
            foreach (ListBoxItem item in para.Items)
            {
                if (item.IsSelected == false)
                {
                    LineSeries removeLine = new LineSeries();
                    foreach (LineSeries line in SeriesCollection)
                    {
                        if (line.Title == item.Content.ToString())
                            removeLine = line;
                    }
                    SeriesCollection.Remove(removeLine);
                }
                else
                {
                    if (!checkChart(item.Content.ToString()))
                    {
                        SeriesCollection.Add(new LineSeries
                        {
                            Title = item.Content.ToString(),
                            Values = getValueOfMonth(int.Parse(item.Content.ToString()))
                        });
                    }
                }
            }
        }

        private bool checkChart(string year)
        {
            foreach (LineSeries item in SeriesCollection)
            {
                if (item.Title == year)
                    return true;
            }
            return false;
        }
        private void Init(UC_RevenueChart para)
        {
            Labels = new List<string>();

            LoadChartMonth(para);

        }

        public void LoadChartMonth(UC_RevenueChart para)
        {
            para.lbRevenueChart.Items.Clear();
            SeriesCollection = new SeriesCollection();
            foreach (int item in Years)
            {
                para.lbRevenueChart.Items.Add(new ListBoxItem() { Content = item.ToString() });
                SeriesCollection.Add(new LineSeries
                {
                    Title = item.ToString(),
                    Values = getValueOfMonth(item)
                });
            }

            foreach (ListBoxItem item in para.lbRevenueChart.Items)
            {
                item.IsSelected = true;
            }

            Labels.Clear();
            string[] labelMonth = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            Labels = labelMonth.ToList();

            YFormatter = value => value.ToString();
        }

        public void LoadChartYear(UC_RevenueChart para)
        {
            SeriesCollection = new SeriesCollection();
            SeriesCollection.Add(new LineSeries
            {
                Title = "",
                Values = getValueOfYears()
            });
            Labels.Clear();
            foreach (int item in Years)
            {
                Labels.Add(item.ToString());
            }

            YFormatter = value => value.ToString();
        }
        private ChartValues<Double> getValueOfYears()
        {
            ChartValues<Double> values = new ChartValues<double>();
            foreach (int item in Years)
            {
                values.Add(GetRevenueByYear(item));
            }
            return values;
        }

        private ChartValues<Double> getValueOfMonth(int year)
        {
            return new ChartValues<Double>
                {
                    GetRevenueByMonth(1, year),
                    GetRevenueByMonth(2, year),
                    GetRevenueByMonth(3, year),
                    GetRevenueByMonth(4, year),
                    GetRevenueByMonth(5, year),
                    GetRevenueByMonth(6, year),
                    GetRevenueByMonth(7, year),
                    GetRevenueByMonth(8, year),
                    GetRevenueByMonth(9, year),
                    GetRevenueByMonth(10, year),
                    GetRevenueByMonth(11, year),
                    GetRevenueByMonth(12, year),
                };
        }

        private double GetRevenueByMonth(int month, int year)
        {
            double total = 0;
            List<RESERVATION> listRES = DataProvider.Instance.DB.RESERVATIONs.Where(
                x => x.arrival.Value.Year == year && x.arrival.Value.Month == month 
                && x.status == "Completed").ToList();
            List<RESERVATION> listRESconfirmed = DataProvider.Instance.DB.RESERVATIONs.Where(
                x => x.arrival.Value.Year == year && x.arrival.Value.Month == month
                && x.status == "Confirmed Cancelled").ToList();
            List<RESERVATION> listRESnoshow = DataProvider.Instance.DB.RESERVATIONs.Where(
                x => x.arrival.Value.Year == year && x.arrival.Value.Month == month
                && x.status == "No Show Cancelled").ToList();

            foreach (var res in listRES)
            {
                List<INVOICE> listInvoices = DataProvider.Instance.DB.INVOICEs.Where(x => x.reservation_id == res.id).ToList();
                foreach (var invoice in listInvoices)
                {
                    total += (double)invoice.total_money;
                }
            }

            //Confirmed Cancelled: tổng đơn giá phòng (1 ngày).
            foreach (var res in listRESconfirmed)
            {
                total += CalculatorInvoice.TotalRoomPriceOfReservation(res);
            }

            //No Show Cancelled: totalDays * tổng đơn giá phòng.
            foreach (var res in listRESnoshow)
            {
                total += CalculatorInvoice.RoomsTotalMoney(res);
            }

            return total;
        }

        private double GetRevenueByYear(int year)
        {
            double total = 0;
            List<RESERVATION> listRES = DataProvider.Instance.DB.RESERVATIONs.Where(
                x => x.arrival.Value.Year == year && x.status == "Completed").ToList();
            List<RESERVATION> listRESconfirmed = DataProvider.Instance.DB.RESERVATIONs.Where(
                x => x.arrival.Value.Year == year && x.status == "Confirmed Cancelled").ToList();
            List<RESERVATION> listRESnoshow = DataProvider.Instance.DB.RESERVATIONs.Where(
                x => x.arrival.Value.Year == year && x.status == "No Show Cancelled").ToList();

            foreach (var res in listRES)
            {
                List<INVOICE> listInvoices = DataProvider.Instance.DB.INVOICEs.Where(x => x.reservation_id == res.id).ToList();
                foreach (var invoice in listInvoices)
                {
                    total += (double)invoice.total_money;
                }
            }

            //Confirmed Cancelled: tổng đơn giá phòng (1 ngày).
            foreach (var res in listRESconfirmed)
            {
                total += CalculatorInvoice.TotalRoomPriceOfReservation(res);
            }

            //No Show Cancelled: totalDays * tổng đơn giá phòng.
            foreach (var res in listRESnoshow)
            {
                total += CalculatorInvoice.RoomsTotalMoney(res);
            }

            return total;
        }
    }
}
