using HotelManagement.Models;
using HotelManagement.Resources.UC;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HotelManagement.ViewModels
{
    class ReportsViewModel : BaseViewModel
    {
        public string Title { get; } = "Reports";

        private object _currentReportView;
        public object CurrentReportView
        {
            get => _currentReportView;
            set
            {
                _currentReportView = value;
                OnPropertyChanged();
            }
        }

        public TopServiceViewModel TopServiceViewModel { get; set; }
        public GuestChart GuestChart { get; set; }

        public ICommand RevenueCommand { get; set; }
        public ICommand GuestChartCommnad { get; set; }
        public ICommand TopServiceCommand { get; set; }

        public ReportsViewModel()
        {
            TopServiceViewModel = new TopServiceViewModel();
            GuestChart = new GuestChart();

            //CurrentReportView = TopServiceViewModel;

            RevenueCommand = new RelayCommand<object>((para) => true,
                (para) => CurrentReportView = null);

            GuestChartCommnad = new RelayCommand<object>((para) => true, 
                (para) => CurrentReportView = GuestChart);

            TopServiceCommand = new RelayCommand<object>((para) => true,
                (para) => CurrentReportView = TopServiceViewModel);
        }
    }
}
