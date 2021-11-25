using HotelManagement.Models;
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

        //Visible
        private string _roomsVisible;
        public string RoomsVisible { get { return _roomsVisible; } set { _roomsVisible = value; OnPropertyChanged(); } }

        private string _bookingVisible;
        public string BookingVisible { get { return _bookingVisible; } set { _bookingVisible = value; OnPropertyChanged(); } }

        private string _guestsVisible;
        public string GuestsVisible { get { return _guestsVisible; } set { _guestsVisible = value; OnPropertyChanged(); } }

        private string _servicesVisible;
        public string ServicesVisible { get { return _servicesVisible; } set { _servicesVisible = value; OnPropertyChanged(); } }

        private string _invoicesVisible;
        public string InvoicesVisible { get { return _invoicesVisible; } set { _invoicesVisible = value; OnPropertyChanged(); } }

        private string _reportsVisible;
        public string ReportsVisible { get { return _reportsVisible; } set { _reportsVisible = value; OnPropertyChanged(); } }
        
        private string _accountsVisible;
        public string AccountsVisible { get { return _accountsVisible; } set { _accountsVisible = value; OnPropertyChanged(); } }

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
        public ICommand LogOutCommand { get; set; }

        public MainWindowViewModel()
        {
            SwitchNavigationBar();

            DataTemplate = DashBoardViewModel;

            CloseWindowCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                Application.Current.Shutdown();
            });

            LogOutCommand = new RelayCommand<MainWindow>((p) =>
            {
                return true;
            }, (p) =>
            {
                CurrentAccount.Instance.DisposeCurrentAccount();
                LoginWindow wd = new LoginWindow();
                p.Close();
                wd.Show();
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

        void SwitchNavigationBar()
        {
            DashBoardViewModel = new DashBoardViewModel();
            CalendarViewModel = new CalendarViewModel();
            
            switch (CurrentAccount.Instance.Permission)
            {
                case "Reservation":
                    RoomsViewModels = new RoomsViewModels();
                    ReservationListViewModel = new ReservationListViewModel();
                    GuestsVisible = "Collapsed";
                    ServicesVisible = "Collapsed";
                    InvoicesVisible = "Collapsed";
                    ReportsVisible = "Collapsed";
                    AccountsVisible = "Collapsed";
                    break;
                case "Receptionist":
                    ReservationListViewModel = new ReservationListViewModel();
                    GuestsViewModel = new GuestsViewModel();
                    ServicesViewModel = new ServicesViewModel();
                    RoomsVisible = "Collapsed";
                    InvoicesVisible = "Collapsed";
                    ReportsVisible = "Collapsed";
                    AccountsVisible = "Collapsed";
                    break;
                case "Cashier":
                    InvoiceViewModel = new InvoiceViewModel();
                    ReportsViewModel = new ReportsViewModel();
                    RoomsVisible = "Collapsed";
                    BookingVisible = "Collapsed";
                    GuestsVisible = "Collapsed";
                    ServicesVisible = "Collapsed";
                    AccountsVisible = "Collapsed";
                    break;
                case "Undefined":
                    RoomsVisible = "Collapsed";
                    BookingVisible = "Collapsed";
                    GuestsVisible = "Collapsed";
                    ServicesVisible = "Collapsed";
                    InvoicesVisible = "Collapsed";
                    ReportsVisible = "Collapsed";
                    AccountsVisible = "Collapsed";
                    break;
                case "Admin":
                    RoomsViewModels = new RoomsViewModels();
                    ReservationListViewModel = new ReservationListViewModel();
                    GuestsViewModel = new GuestsViewModel();
                    ServicesViewModel = new ServicesViewModel();
                    InvoiceViewModel = new InvoiceViewModel();
                    ReportsViewModel = new ReportsViewModel();
                    AccountViewModel = new AccountViewModel();
                    break;
                default:
                    break;
            }
        }
    }
}
