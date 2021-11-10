using HotelManagement.Models;
using HotelManagement.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace HotelManagement.ViewModels
{
    class ReservationListViewModel : BaseViewModel
    {
        public IEnumerable<string> ResStatusList => new[] { "All", "On Request", "Confirmed", "Operational", "No Show", "Completed", "Cancelled" };
        private string _selectedStatus;
        public string SelectedStatus { get { return _selectedStatus; } set { _selectedStatus = value; OnPropertyChanged(); } }

        private DateTime _selectedArrival;
        public DateTime SelectedArrival { get { return _selectedArrival; } set { _selectedArrival = value; OnPropertyChanged(); } }

        private DateTime _selectedDeparture;
        public DateTime SelectedDeparture { get { return _selectedDeparture; } set { _selectedDeparture = value; OnPropertyChanged(); } }
        public PageNavigationViewModel PageNavigationViewModel { get; set; }
        public List<RESERVATION> AllReservations { get; set; }

        public ObservableCollection<ReservationItemViewModel> CurrentPageReservations { get; set; }

        bool CanCreateNewReservation
        {
            get
            {
                if (CurrentAccount.Instance.Permission == "Admin" ||
                    CurrentAccount.Instance.Permission == "Reservation")
                    return true;
                return false;
            }
        }

        private ICommand _newReservationCommand;
        public ICommand NewReservationCommand
        {
            get
            {
                return _newReservationCommand ?? (_newReservationCommand = new RelayCommand<object>((p) => CanCreateNewReservation, (p) => OpenNewReservationWindow()));
            }
        }

        private ICommand _searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                return _searchCommand ?? (_searchCommand = new RelayCommand<object>((p) => true, (p) => LoadReservations()));
            }
        }

        private ICommand _resetCommand;
        public ICommand ResetCommand
        {
            get
            {
                return _resetCommand ?? (_resetCommand = new RelayCommand<object>((p) => true, (p) => ResetFilter()));
            }
        }

        public ReservationListViewModel()
        {
            AllReservations = new List<RESERVATION>();
            CurrentPageReservations = new ObservableCollection<ReservationItemViewModel>();

            PageNavigationViewModel = new PageNavigationViewModel();
            PageNavigationViewModel.PageSize = 2;

            PageNavigationViewModel.PropertyChanged += PageNavigationViewModel_PropertyChanged;
            ResetFilter();
        }

        private void PageNavigationViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PageNavigationViewModel.CurrentPage))
            {
                LoadReservationsOnCurrentPage();
            }
        }

        private void OpenNewReservationWindow()
        {
            var wd = new NewReservationWindow();
            wd.DataContext = new NewReservationViewModel(this);
            wd.Show();
        }

        public void ResetFilter()
        {
            SelectedArrival = DateTime.Today.AddMonths(-6);
            SelectedDeparture = DateTime.Today.AddMonths(6);
            SelectedStatus = "All";
            LoadReservations();
        }

        public void LoadReservations()
        {
            if (SelectedStatus == "All")
            {
                AllReservations = DataProvider.Instance.DB.RESERVATIONs.Where(res => res.arrival >= SelectedArrival &&
                            res.departure <= SelectedDeparture).ToList();
            }
            else
            {
                AllReservations = DataProvider.Instance.DB.RESERVATIONs.Where(res => res.arrival >= SelectedArrival &&
                            res.departure <= SelectedDeparture && res.status == SelectedStatus).ToList();
            }
            
            PageNavigationViewModel.SumRecords = AllReservations.Count();
            if (PageNavigationViewModel.SumRecords > 0)
                PageNavigationViewModel.CurrentPage = 1;
            else PageNavigationViewModel.CurrentPage = 0;
        }

        private void LoadReservationsOnCurrentPage()
        {
            if (CurrentPageReservations.Count > 0) CurrentPageReservations.Clear();

            int selectedRecords = PageNavigationViewModel.SelectedRecords;
            int exceptRecords = PageNavigationViewModel.ExceptRecords;

            var resCurrentPage = AllReservations.OrderBy(res => res.id).Take(selectedRecords).Skip(exceptRecords);

            foreach (var res in resCurrentPage)
            {
                GUEST mainGuest = DataProvider.Instance.DB.GUESTs.Where(g => g.id == res.main_guest).SingleOrDefault();

                var obj = new ReservationItemViewModel(this)
                {
                    ID = res.id,
                    Status = res.status,
                    Rooms = (from rb in DataProvider.Instance.DB.ROOM_BOOKED where rb.reservation_id == res.id select rb).Count(),
                    Guest = mainGuest.name,
                    DateCreated = (DateTime)res.date_created,
                    Arrival = (DateTime)res.arrival,
                    Departure = (DateTime)res.departure,
                    Pax = (from gb in DataProvider.Instance.DB.GUEST_BOOKING where gb.reservation_id == res.id select gb).Count(),
                };

                obj.InitializePopup();

                CurrentPageReservations.Add(obj);
            }
        }
    }

    class ReservationItemViewModel : BaseViewModel
    {
        private ReservationListViewModel Instance { get; set; } 

        #region Property
        private int _id;
        private string _status;
        private string _guest;
        private DateTime _date_created;
        private DateTime _arrival;
        private DateTime _departure;
        private int _rooms;
        private int _pax;

        public int ID { get { return _id; } set { _id = value; OnPropertyChanged(); } }

        public string Status { get { return _status; } set { _status = value; OnPropertyChanged(); } }

        public int Rooms { get { return _rooms; } set { _rooms = value; OnPropertyChanged(); } }

        public string Guest { get { return _guest; } set { _guest = value; OnPropertyChanged(); } }

        public DateTime DateCreated { get { return _date_created; } set { _date_created = value; OnPropertyChanged(); } }

        public DateTime Arrival { get { return _arrival; } set { _arrival = value; OnPropertyChanged(); } }

        public DateTime Departure { get { return _departure; } set { _departure = value; OnPropertyChanged(); } }

        public int Pax { get { return _pax; } set { _pax = value; OnPropertyChanged(); } }
        #endregion

        #region Option Popup
        public ObservableCollection<Option> Options { get; set; }

        private ICommand _detailsCommand;
        public ICommand DetailsCommand
        {
            get
            {
                return _detailsCommand ?? (_detailsCommand = new RelayCommand<object>((p) => true, (p) => OpenReservationDetailsWindow()));
            }
        }

        private ICommand _checkinCommand;
        public ICommand CheckinCommand
        {
            get
            {
                return _checkinCommand ?? (_checkinCommand = new RelayCommand<object>((p) => true, (p) => CheckIn()));
            }
        }

        private ICommand _cancelResCommand;
        public ICommand CancelResCommand
        {
            get
            {
                return _cancelResCommand ?? (_cancelResCommand = new RelayCommand<object>((p) => true, (p) => CancelRes()));
            }
        }

        private ICommand _confirmGuaranteeCommand;
        public ICommand ConfirmGuaranteeCommand
        {
            get
            {
                return _confirmGuaranteeCommand ?? (_confirmGuaranteeCommand = new RelayCommand<object>((p) => true, (p) => ConfirmGuarantee()));
            }
        }

        private void OpenReservationDetailsWindow()
        {
            var wd = new ReservationDetailsWindow();
            wd.DataContext = new ReservationDetailsViewModel(ID, Instance);
            wd.Show();
        }

        private void CheckIn()
        {
            DataProvider.Instance.DB.RESERVATIONs.Where(res => res.id == ID).FirstOrDefault().status = "Operational";
            DataProvider.Instance.DB.SaveChanges();
            Instance.LoadReservations();
        }

        private void CancelRes()
        {
            DataProvider.Instance.DB.RESERVATIONs.Where(res => res.id == ID).FirstOrDefault().status = "Cancelled";
            DataProvider.Instance.DB.SaveChanges();
            Instance.LoadReservations();
        }

        private void ConfirmGuarantee()
        {
            DataProvider.Instance.DB.RESERVATIONs.Where(res => res.id == ID).FirstOrDefault().status = "Confirmed";
            DataProvider.Instance.DB.SaveChanges();
            Instance.LoadReservations();
        }

        public void InitializePopup()
        {
            Options = new ObservableCollection<Option>();
            var option = new Option()
            {
                Content = "Details",
                Command = DetailsCommand,
            };
            Options.Add(option);

            if (Status == "On Request")
            {
                option = new Option()
                {
                    Content = "Confirm guarantee",
                    Command = ConfirmGuaranteeCommand,
                };
                Options.Add(option);
            }

            if (Status == "On Request" || Status == "Confirmed" || Status == "No Show")
            {
                if ((Status != "No Show" && Arrival == DateTime.Today) ||
                    (Status == "No Show"))
                {
                    option = new Option()
                    {
                        Content = "Check in",
                        Command = CheckinCommand,
                    };
                    Options.Add(option);
                }

                option = new Option()
                {
                    Content = "Cancel reservation",
                    Command = CancelResCommand,
                };
                Options.Add(option);
            }      
        }
        #endregion

        public ReservationItemViewModel(ReservationListViewModel _instance)
        {
            Instance = _instance;
        }
    }

    class Option : BaseViewModel
    {
        private string _content;
        private ICommand _command;

        public string Content { get { return _content; } set { _content = value; OnPropertyChanged(); } }
        public ICommand Command { get { return _command; } set { _command = value; OnPropertyChanged(); } }
    }
}
