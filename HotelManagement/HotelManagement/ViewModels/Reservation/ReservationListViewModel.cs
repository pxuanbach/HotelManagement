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
            Reservations = new ObservableCollection<ReservationItemViewModel>();
            LoadAllReservations();
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

            var reservations = (from res in db.RESERVATIONs select res).ToList();

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
                    Arrival = (!res.arrival.HasValue) ? DateTime.Now : (DateTime)res.arrival,
                    Departure = (!res.departure.HasValue) ? DateTime.Now : (DateTime)res.departure,
                    Pax = res.GUEST_BOOKING.Count,
                };

                Reservations.Add(obj);
            }
        }
    }

    class ReservationItemViewModel : BaseViewModel
    {
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

        private ICommand _command;
        public ICommand Command
        {
            get
            {
                return _command ?? (_command = new RelayCommand<object>((p) => true, (p) => { MessageBox.Show("PHAI TOI TOI DAM CHO MAY NHAT!!!"); }));
            }
        }

        public void OpenReservationDetailsWindow()
        {
            var wd = new ReservationDetailsWindow();
            wd.DataContext = new ReservationDetailsViewModel(ID);
            wd.Show();
        }

        private void InitializePopup()
        {
            Options = new ObservableCollection<Option>();
            var option1 = new Option()
            {
                Content = "Details",
                Command = DetailsCommand,
            };
            Options.Add(option1);
            var option2 = new Option()
            {
                Content = "Check In",
                Command = Command,
            };
            Options.Add(option2);
            var option3 = new Option()
            {
                Content = "Check Out",
                Command = Command,
            };
            Options.Add(option3);
        }
        #endregion

        public ReservationItemViewModel()
        {
            InitializePopup();
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
