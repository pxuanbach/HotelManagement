using HotelManagement.Models;
using HotelManagement.Views;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace HotelManagement.ViewModels
{
    class ReservationListViewModel : BaseViewModel
    {
        public PageNavigationViewModel PageNavigationViewModel { get; set; }
        public ObservableCollection<ReservationItemViewModel> Reservations { get; set; }

        private ICommand _newReservationCommand;
        public ICommand NewReservationCommand
        {
            get
            {
                return _newReservationCommand ?? (_newReservationCommand = new RelayCommand<object>((p) => true, (p) => OpenNewReservationWindow()));
            }
        }

        public ReservationListViewModel()
        {
            PageNavigationViewModel = new PageNavigationViewModel();
            PageNavigationViewModel.PageSize = 2;

            var db = new HotelManagementEntities();
            PageNavigationViewModel.SumRecords = db.RESERVATIONs.Count();

            Reservations = new ObservableCollection<ReservationItemViewModel>();

            PageNavigationViewModel.PropertyChanged += PageNavigationViewModel_PropertyChanged;
            PageNavigationViewModel.CurrentPage = 1;
        }

        private void PageNavigationViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PageNavigationViewModel.CurrentPage))
            {
                Console.WriteLine(PageNavigationViewModel.PageTitle);
                LoadAllReservations();
            }
        }

        public void OpenNewReservationWindow()
        {
            var wd = new NewReservationWindow();
            wd.Show();
        }

        private void LoadAllReservations()
        {
            if (Reservations.Count > 0) Reservations.Clear();

            var db = new HotelManagementEntities();

            int selectedRecords = PageNavigationViewModel.SelectedRecords;
            int exceptRecords = PageNavigationViewModel.ExceptRecords;

            Console.WriteLine("Selected: {0}, Except: {1}", selectedRecords, exceptRecords);

            var reservations = (from res in db.RESERVATIONs select res).Take(selectedRecords).ToList();
            reservations.RemoveRange(0, exceptRecords);

            foreach (var res in reservations)
            {
                GUEST mainGuest = (from g in db.GUESTs where res.main_guest.Equals(g.id) select g).SingleOrDefault();

                var obj = new ReservationItemViewModel()
                {
                    ID = res.id,
                    Status = res.status,
                    Rooms = res.ROOM_BOOKED.Count,
                    Guest = mainGuest.name,
                    DateCreated = (DateTime)res.date_created,
                    Arrival = (DateTime)res.arrival,
                    Departure = (DateTime)res.departure,
                    Pax = res.GUEST_BOOKING.Count,
                };

                Console.WriteLine(obj.ID);

                obj.InitializePopup();

                Reservations.Add(obj);
            }
        }
    }

    class ReservationItemViewModel : BaseViewModel
    {
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

        private ICommand _checkoutCommand;
        public ICommand CheckoutCommand
        {
            get
            {
                return _checkoutCommand ?? (_checkoutCommand = new RelayCommand<object>((p) => true, (p) => CheckOut()));
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
            wd.DataContext = new ReservationDetailsViewModel(ID);
            wd.Show();
        }

        private void CheckIn()
        {
            var db = new HotelManagementEntities();
            db.RESERVATIONs.Where(res => res.id == ID).FirstOrDefault().status = "Operational";
            db.SaveChanges();
        }

        private void CancelRes()
        {
            var db = new HotelManagementEntities();
            db.RESERVATIONs.Where(res => res.id == ID).FirstOrDefault().status = "Cancelled";
            db.SaveChanges();
        }

        private void CheckOut()
        {
            var db = new HotelManagementEntities();
            db.RESERVATIONs.Where(res => res.id == ID).FirstOrDefault().status = "Completed";
            db.SaveChanges();
        }

        private void ConfirmGuarantee()
        {
            var db = new HotelManagementEntities();
            db.RESERVATIONs.Where(res => res.id == ID).FirstOrDefault().status = "Confirmed";
            db.SaveChanges();
        }

        public void InitializePopup()
        {
            Options = new ObservableCollection<Option>();
            var option = new Option()
            {
                Content = "Edit details",
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
                option = new Option()
                {
                    Content = "Check in",
                    Command = CheckinCommand,
                };
                Options.Add(option);

                option = new Option()
                {
                    Content = "Cancel reservation",
                    Command = CancelResCommand,
                };
                Options.Add(option);
            }
            
            if (Status == "Operational")
            {
                option = new Option()
                {
                    Content = "Check out",
                    Command = CheckoutCommand,
                };
                Options.Add(option);
            }           
        }
        #endregion
    }

    class Option : BaseViewModel
    {
        private string _content;
        private ICommand _command;

        public string Content { get { return _content; } set { _content = value; OnPropertyChanged(); } }
        public ICommand Command { get { return _command; } set { _command = value; OnPropertyChanged(); } }
    }
}
