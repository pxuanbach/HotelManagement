using HotelManagement.Models;
using HotelManagement.Resources.UC;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HotelManagement.ViewModels
{
    class GuestChart : BaseViewModel
    {
        private List<int> years;
        public List<int> Years
        {
            get => years;
            set
            {
                years = value;
                OnPropertyChanged();
            }
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

        public ICommand InitCommand { get; set; }
        public ICommand ChooseItemCommand { get; set; }
        public ICommand ChangeTypeCommand { get; set; }

        public GuestChart()
        {
            InitCommand = new RelayCommand<UC_GuestChart>((para) => true, (para) => Init(para));
            ChooseItemCommand = new RelayCommand<ListBox>((para) => true, (para) => ChooseItem(para));
            ChangeTypeCommand = new RelayCommand<UC_GuestChart>((para) => true, (para) => ChangeType(para));

            Years = DataProvider.Instance.DB.RESERVATIONs.Select(x => x.arrival.Value.Year).Distinct().ToList();
        }

        private void ChangeType(UC_GuestChart para)
        {
            if (para.cbbChangeType.SelectedIndex == 0)
            {
                LoadChartMonth(para);
                para.txtNameChart.Text = "Chart of Visitors by Month";
                para.grdYears.Visibility = Visibility.Visible;
                para.titleX.Title = "Month";
            }
            else
            {
                LoadChartYear(para);
                para.txtNameChart.Text = "Chart of Visitors by Year";
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
        private void Init(UC_GuestChart para)
        {
            Labels = new List<string>();

            LoadChartMonth(para);

        }

        public void LoadChartMonth(UC_GuestChart para)
        {
            para.lbGuestChart.Items.Clear();
            SeriesCollection = new SeriesCollection();
            foreach (int item in Years)
            {
                para.lbGuestChart.Items.Add(new ListBoxItem() { Content = item.ToString() });
                SeriesCollection.Add(new LineSeries
                {
                    Title = item.ToString(),
                    Values = getValueOfMonth(item)
                });
            }

            foreach (ListBoxItem item in para.lbGuestChart.Items)
            {
                item.IsSelected = true;
            }

            Labels.Clear();
            string[] labelMonth = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            Labels = labelMonth.ToList();

            YFormatter = value => value.ToString();
        }

        public void LoadChartYear(UC_GuestChart para)
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
                values.Add(CountGuestbyYear(item));
            }
            return values;
        }


        private ChartValues<Double> getValueOfMonth(int year)
        {
            return new ChartValues<Double>
                {
                    CountGuestbyMonth(1, year),
                    CountGuestbyMonth(2, year),
                    CountGuestbyMonth(3, year),
                    CountGuestbyMonth(4, year),
                    CountGuestbyMonth(5, year),
                    CountGuestbyMonth(6, year),
                    CountGuestbyMonth(7, year),
                    CountGuestbyMonth(8, year),
                    CountGuestbyMonth(9, year),
                    CountGuestbyMonth(10, year),
                    CountGuestbyMonth(11, year),
                    CountGuestbyMonth(12, year)
                };
        }

        private double CountGuestbyMonth(int month, int year)
        {
            double count = 0;

            List<RESERVATION> listRes = DataProvider.Instance.DB.RESERVATIONs.Where(
                y => y.arrival.Value.Month == month && y.arrival.Value.Year == year).ToList();

            foreach (RESERVATION item in listRes)
            {
                count += DataProvider.Instance.DB.GUEST_BOOKING.Where(x => x.reservation_id == item.id && item.status == "Completed").Count();
            }

            return count;
        }

        private double CountGuestbyYear(int year)
        {
            double count = 0;

            List<RESERVATION> listRes = DataProvider.Instance.DB.RESERVATIONs.Where(y => y.arrival.Value.Year == year).ToList();

            foreach (RESERVATION item in listRes)
            {
                count += DataProvider.Instance.DB.GUEST_BOOKING.Where(x => x.reservation_id == item.id && item.status == "Completed").Count();
            }

            return count;
        }
    }
}

