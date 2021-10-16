using HotelManagement.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace HotelManagement.ViewModels
{
    class InvoiceViewModel : BaseViewModel
    {
        #region Properties

        #region Invoice Details
        private string _arrival;
        public string Arrival { get { return _arrival; } set { _arrival = value; OnPropertyChanged(); } }

        private string _departure;
        public string Departure { get { return _departure; } set { _departure = value; OnPropertyChanged(); } }

        #region Main Guest
        private string _identity;
        public string Identity { get { return _identity; } set { _identity = value; OnPropertyChanged(); } }

        private string _name;
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged(); } }

        private string _phone;
        public string Phone { get { return _phone; } set { _phone = value; OnPropertyChanged(); } }

        private string _email;
        public string Email { get { return _email; } set { _email = value; OnPropertyChanged(); } }
        #endregion

        #region Guests
        private int _guestCount;
        public int GuestCount { get { return _guestCount; } set { _guestCount = value; OnPropertyChanged(); } }

        private ObservableCollection<GUEST> _guests;
        public ObservableCollection<GUEST> Guests
        {
            get { return _guests; }
            set { _guests = value; OnPropertyChanged(); }
        }
        #endregion

        #region Rooms
        private int _roomCount;
        public int RoomCount { get { return _roomCount; } set { _roomCount = value; OnPropertyChanged(); } }

        #endregion

        #endregion

        public string StatusSelected { get; set; }
        
        private ObservableCollection<RESERVATION> _reservations;
        public ObservableCollection<RESERVATION> Reservations 
        { 
            get { return _reservations; } 
            set { _reservations = value; OnPropertyChanged(); } 
        }
        #endregion

        #region Command
        public ICommand OperationalCommnad { get; set; }
        public ICommand CompletedCommnad { get; set; }
        public ICommand ListviewSelectionChangedCommand { get; set; }
        public ICommand ClearDetailCommand { get; set; }
        #endregion

        public InvoiceViewModel()
        {
            InitProperties();

            OperationalCommnad = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                if (Reservations.Count > 0)
                    Reservations.Clear();
                ClearDetailProperties();
                StatusSelected = "Operational";
                LoadReservations();
            });

            CompletedCommnad = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                if (Reservations.Count > 0)
                    Reservations.Clear();
                ClearDetailProperties();
                StatusSelected = "Completed";
                LoadReservations();
            });

            ListviewSelectionChangedCommand = new RelayCommand<ListView>((p) =>
            {
                return !p.Items.IsEmpty;
            }, (p) =>
            {
                LoadItemSelected((RESERVATION)p.SelectedItem);
            });

            ClearDetailCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                ClearDetailProperties();
            });
        }

        void InitProperties()
        {
            StatusSelected = "Operational";
            LoadReservations();
        }

        void ClearDetailProperties()
        {
            Arrival = "";
            Departure = "";

            //Main Guest
            Identity = "";
            Name = "";
            Phone = "";
            Email = "";

            //Guests
            GuestCount = 0;
            if (Guests.Count > 0)
                Guests.Clear();

            //Rooms
            RoomCount = 0;
        }

        #region Load Function
        void LoadReservations()
        {
            Reservations = new ObservableCollection<RESERVATION>(
                DataProvider.Instance.DB.RESERVATIONs.Where(x => x.status == StatusSelected));
        }

        void LoadItemSelected(RESERVATION p)
        {
            Arrival = p.arrival.Value.ToString("dd/MM/yyyy");
            Departure = p.departure.Value.ToString("dd/MM/yyyy");

            //Main Guest
            var mainGuest = DataProvider.Instance.DB.GUESTs.Where(x => x.id == p.main_guest).SingleOrDefault();
            Identity = p.main_guest;
            Name = mainGuest.name;
            Phone = mainGuest.phone;
            Email = mainGuest.email;

            //Guests
            Guests = new ObservableCollection<GUEST>();
            List<GUEST_BOOKING> guestBookingList = 
                DataProvider.Instance.DB.GUEST_BOOKING.Where(x => x.reservation_id == p.id).ToList();
            
            GuestCount = guestBookingList.Count();
            foreach (GUEST_BOOKING obj in guestBookingList)
            {
                var guest = DataProvider.Instance.DB.GUESTs.SingleOrDefault(x => x.id == obj.guest_id);
                Guests.Add(guest);
            }    
            
            //Rooms
            RoomCount = DataProvider.Instance.DB.ROOM_BOOKED.Where(x => x.reservation_id == p.id).Count();
        }
        #endregion


    }
}
