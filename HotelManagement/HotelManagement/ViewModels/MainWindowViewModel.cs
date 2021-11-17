using HotelManagement.Models;
using HotelManagement.ViewModels.Service;
using HotelManagement.Views;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary> 

    class MainWindowViewModel : BaseViewModel
    {
        public MainWindow MainWindow {get;set;}

        #region Data Template
        private object dataTemplate;
        public object DataTemplate
        {
            get => dataTemplate;
            set
            {
                dataTemplate = value;
                OnPropertyChanged();
            }
        }

        public DashBoardViewModel DashBoardViewModel { get; set; }
        public DashBoardView DashBoardView { get; set; } 
        public ReservationListViewModel ReservationListViewModel { get; set; }
        public CalendarViewModel CalendarViewModel { get; set; }
        public GuestsViewModel GuestsViewModel { get; set; }
        public RoomsViewModels RoomsViewModels { get; set; }
        public InvoiceViewModel InvoiceViewModel { get; set; }
        public ReportsViewModel ReportsViewModel { get; set; }
        public AccountViewModel AccountViewModel { get; set; }
        public ServicesViewModel ServicesViewModel { get; set; }
        #endregion

        public ICommand DashBoardViewCommmand { get; set; }
        public ICommand ReservationListViewCommand { get; set; }
        public ICommand CalendarViewCommmand { get; set; }
        public ICommand RoomsViewCommmand { get; set; }
        public ICommand GuestsViewCommmand { get; set; }
        public ICommand InvoicesViewCommmand { get; set; }
        public ICommand ReportsViewCommmand { get; set; }
        public ICommand AccountsViewCommmand { get; set; }
        public ICommand ServicesViewCommmand { get; set; }

        public ICommand CloseWindowCommand { get; set; }

        public MainWindowViewModel()
        {

            DashBoardViewModel = new DashBoardViewModel();
            ReservationListViewModel = new ReservationListViewModel();
            CalendarViewModel = new CalendarViewModel();
            RoomsViewModels = new RoomsViewModels();
            GuestsViewModel = new GuestsViewModel();
            InvoiceViewModel = new InvoiceViewModel();
            ReportsViewModel = new ReportsViewModel();
            AccountViewModel = new AccountViewModel();
            ServicesViewModel = new ServicesViewModel();
            DataTemplate = DashBoardViewModel;

            CloseWindowCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                Application.Current.Shutdown();
            });

            #region Navigation
            DashBoardViewCommmand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                DataTemplate = DashBoardViewModel;
            });

            ReservationListViewCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                DataTemplate = ReservationListViewModel;
            });

            CalendarViewCommmand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                DataTemplate = CalendarViewModel;
            });

            RoomsViewCommmand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                DataTemplate = RoomsViewModels;
            });

            GuestsViewCommmand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                DataTemplate = GuestsViewModel;
            });

            InvoicesViewCommmand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                DataTemplate = InvoiceViewModel;
            });

            ReportsViewCommmand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                DataTemplate = ReportsViewModel;
            });

            AccountsViewCommmand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                DataTemplate = AccountViewModel;
            });

            ServicesViewCommmand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                DataTemplate = ServicesViewModel;
            });
            #endregion

        }
    }
}
