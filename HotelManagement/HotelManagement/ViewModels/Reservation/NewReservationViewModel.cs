using HotelManagement.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace HotelManagement.ViewModels
{
    class NewReservationViewModel : BaseViewModel
    {
        public GuestViewModel GuestInformation { get; set; }

        public ReservationViewModel StayInformation { get; set; }

        public ObservableCollection<RoomViewModel> AvailableRooms { get; set; }

        public ObservableCollection<RoomViewModel> SelectedRooms { get; set; }

        public ObservableCollection<GuestViewModel> Sharers { get; set; }

        public bool BeASharer { get; set; }

        public bool Guaranteed { get; set; }

        public IEnumerable<string> Gender => new[] { "Male", "Female", "Other" };

        #region Command
        private ICommand _beASharerCommand;
        public ICommand BeASharerCommand
        {
            get
            {
                return _beASharerCommand ?? (_beASharerCommand = new RelayCommand<object>((p) => true, (p) => ReserveLikeASharer()));
            }
        }

        private ICommand _addSharerCommand;
        public ICommand AddSharerCommand
        {
            get
            {
                return _addSharerCommand ?? (_addSharerCommand = new RelayCommand<object>((p) => CanAddSharer, (p) => AddSharer()));
            }
        }

        private ICommand _removeSharerCommand;
        public ICommand RemoveSharerCommand
        {
            get
            {
                return _removeSharerCommand ?? (_removeSharerCommand = new RelayCommand<GuestViewModel>((p) => true, (p) => RemoveSelectedSharer(p)));
            }
        }

        private ICommand _reserveCommand;
        public ICommand ReserveCommand
        {
            get
            {
                return _reserveCommand ?? (_reserveCommand = new RelayCommand<Window>((p) => CanReserve, (p) => Reserve(p)));
            }
        }

        private ICommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new RelayCommand<Window>((p) => true, (p) => p.Close()));
            }
        }
        #endregion

        public NewReservationViewModel()
        {
            Sharers = new ObservableCollection<GuestViewModel>();
            GuestInformation = new GuestViewModel();
            StayInformation = new ReservationViewModel();
            AvailableRooms = new ObservableCollection<RoomViewModel>();
            SelectedRooms = new ObservableCollection<RoomViewModel>(); 

            StayInformation.PropertyChanged += StayInformation_PropertyChanged;
            Sharers.CollectionChanged += Sharers_CollectionChanged;

            StayInformation.Total = 0;
            StayInformation.Arrival = DateTime.Today;
            StayInformation.Departure = DateTime.Today.AddDays(1);
        }

        private void StayInformation_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ReservationViewModel.Arrival) ||
                e.PropertyName == nameof(ReservationViewModel.Departure))
            {
                StayInformation.Stays = (int)(StayInformation.Departure - StayInformation.Arrival).TotalDays;
                LoadAvailableRooms();
            }
        }

        private void Sharers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            StayInformation.Pax = Sharers.Count;
        }

        public bool FilledGuestInformation
        {
            get
            {
                if (String.IsNullOrEmpty(GuestInformation.ID) ||
                    String.IsNullOrEmpty(GuestInformation.Gender) ||
                    String.IsNullOrEmpty(GuestInformation.Name) ||
                    String.IsNullOrEmpty(GuestInformation.Email) ||
                    String.IsNullOrEmpty(GuestInformation.Phone) ||
                    String.IsNullOrEmpty(GuestInformation.Address))
                    return false;
                return true;
            }
        }

        public bool CanAddSharer
        {
            get
            {
                if (Sharers.Count == 0)
                {
                    return true;
                }
                else
                {
                    foreach (var row in Sharers)
                    {
                        if (String.IsNullOrEmpty(row.Name) ||
                            String.IsNullOrEmpty(row.ID) ||
                            String.IsNullOrEmpty(row.Gender) || 
                            String.IsNullOrEmpty(row.Address))
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
        }

        public bool CanReserve
        {
            get
            {
                if (!FilledGuestInformation) return false;
                if (StayInformation.Pax == 0) return false;
                if (StayInformation.Rooms == 0) return false;
                return true;
            }
        }

        public void ReserveLikeASharer()
        {
            BeASharer = !BeASharer;
            if (BeASharer)
            {
                StayInformation.Pax++;
            }
            else
            {
                StayInformation.Pax--;
            }
        }

        public void AddSharer()
        {
            GuestViewModel newSharer = new GuestViewModel();
            Sharers.Add(newSharer);
        }

        public void RemoveSelectedSharer(GuestViewModel sharer)
        {
            Sharers.Remove(sharer);
        }

        public void Reserve(Window window) 
        {
            using (var context = new HotelManagementEntities())
            {
                // Insert reservation
                var reservation = new RESERVATION()
                {
                    date_created = DateTime.Today,
                    arrival = StayInformation.Arrival,
                    departure = StayInformation.Departure,
                    main_guest = GuestInformation.ID,
                    status = Guaranteed ? "Confirmed" : "On Request",
                };
                context.RESERVATIONs.Add(reservation);

                // Insert main guest
                var mainGuest = new GUEST()
                {
                    id = GuestInformation.ID,
                    name = GuestInformation.Name,
                    gender = GuestInformation.Gender,
                    birthday = GuestInformation.Birthday,
                    email = GuestInformation.Email,
                    phone = GuestInformation.Phone,
                    address = GuestInformation.Address,
                };

                if (!context.GUESTs.Any(g => g.id == mainGuest.id))
                {
                    context.GUESTs.Add(mainGuest);
                }
                else
                {
                    Console.WriteLine("[ERROR] Guest ID {0} existed in Guest Database!!!", mainGuest.id);
                }

                if (BeASharer)
                {
                    var guestBooking = new GUEST_BOOKING()
                    {
                        reservation_id = reservation.id,
                        guest_id = mainGuest.id,
                    };
                    context.GUEST_BOOKING.Add(guestBooking);
                }

                // Insert room_booked
                foreach (var selectedRoom in SelectedRooms)
                {
                    var bookedBoom = new ROOM_BOOKED()
                    {
                        reservation_id = reservation.id,
                        room_id = selectedRoom.RoomID,
                    };
                    context.ROOM_BOOKED.Add(bookedBoom);
                }

                // Insert sharers
                foreach (var sharer in Sharers)
                {
                    var newGuest = new GUEST()
                    {
                        id = sharer.ID,
                        name = sharer.Name,
                        gender = sharer.Gender,
                        address = sharer.Address,
                    };
                    if (!context.GUESTs.Any(g => g.id == newGuest.id))
                    {
                        context.GUESTs.Add(newGuest);
                    }
                    else
                    {
                        Console.WriteLine("[ERROR] Guest ID {0} is existing in Guest Database!!!", newGuest.id);
                    }

                    var guestBooking = new GUEST_BOOKING()
                    {
                        reservation_id = reservation.id,
                        guest_id = newGuest.id,
                    };
                    context.GUEST_BOOKING.Add(guestBooking);
                }

                context.SaveChanges();
            }

            window.Close();
        }

        public bool? IsAllRoomsSelected
        {
            get
            {
                var selected = AvailableRooms.Select(item => item.IsSelected).Distinct().ToList();
                return selected.Count == 1 ? selected.Single() : (bool?)null;
            }
            set
            {
                if (value.HasValue)
                {
                    SelectAll(value.Value, AvailableRooms);
                    OnPropertyChanged();
                }
            }
        }

        private static void SelectAll(bool select, IEnumerable<RoomViewModel> models)
        {
            foreach (var model in models)
            {
                if (model.IsSelected != select) model.IsSelected = select;
            }
        }

        public void LoadAvailableRooms()
        {
            if (AvailableRooms.Count > 0) AvailableRooms.Clear();

            var db = new HotelManagementEntities();

            var allrooms = from r in db.ROOMs
                           join rt in db.ROOMTYPEs on r.roomtype_id equals rt.id
                           join rb in db.ROOM_BOOKED on r.id equals rb.room_id into result
                           from rs in result.DefaultIfEmpty()      
                           select new
                           {
                                RoomID = r.id,
                                RoomName = r.name,
                                TypeID = rt.id,
                                TypeName = rt.name,
                                ResID = rs == null ? 0 : rs.reservation_id,
                                Price = rt.price,
                                RT_DateCreated = rt.date_created,
                                RT_DateUpdated = rt.date_updated,
                                OOS = r.out_of_service,
                           };

            var rooms = (from r in allrooms
                         join res in db.RESERVATIONs on r.ResID equals res.id into result
                         from rs in result.DefaultIfEmpty()
                         where (rs.arrival >= StayInformation.Departure || rs.departure <= StayInformation.Arrival || r.ResID == 0) &&
                         r.RT_DateCreated <= DateTime.Today && (r.RT_DateUpdated == null || r.RT_DateUpdated >= DateTime.Today) &&
                         r.OOS == false
                         select r).ToList();

            foreach (var room in rooms)
            {
                var obj = new RoomViewModel()
                {
                    RoomID = room.RoomID,
                    RoomType = room.TypeName,
                    RoomName = room.RoomName,
                    RoomTypeID = room.TypeID,
                    Price = Decimal.Round((decimal)room.Price),
                };
                Console.WriteLine(room.RoomName + "\t" + room.ResID);

                AvailableRooms.Add(obj);

                AvailableRooms.Last().PropertyChanged += NewReservationViewModel_PropertyChanged;
            }

            Console.WriteLine(AvailableRooms.Count);
        }

        private void NewReservationViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RoomViewModel.IsSelected))
            {
                if ((sender as RoomViewModel).IsSelected)
                {
                    StayInformation.Rooms++;
                    StayInformation.Total += (sender as RoomViewModel).Price;
                    SelectedRooms.Add(sender as RoomViewModel);
                }
                else
                {
                    StayInformation.Rooms--;
                    StayInformation.Total -= (sender as RoomViewModel).Price;
                    SelectedRooms.Remove(sender as RoomViewModel);
                }
                OnPropertyChanged(nameof(IsAllRoomsSelected));
            }
        }
    }

    class ReservationViewModel : BaseViewModel
    {
        private int _id;
        private DateTime _date_created;
        private DateTime _arrival;
        private DateTime _departure;
        private int _stays;
        private int _rooms;
        private int _pax;
        private decimal _total;

        public int ID { get { return _id; } set { _id = value; OnPropertyChanged(); } }

        public DateTime DateCreated { get { return _date_created; } set { _date_created = value; OnPropertyChanged(); } }

        public DateTime Arrival 
        { 
            get { return _arrival; } 
            set 
            {
                if (value >= DateTime.Today && (int)(_departure - value).TotalDays > 0) _arrival = value;
                else _arrival = DateTime.Today;
                OnPropertyChanged(); 
            } 
        }

        public DateTime Departure 
        {
            get { return _departure; }
            set 
            {
                if (value >= DateTime.Today.AddDays(1) && (int)(value - _arrival).TotalDays > 0) _departure = value;
                else _departure = DateTime.Today.AddDays(1);
                OnPropertyChanged(); 
            } 
        }

        public int Stays { get { return _stays; } set { _stays = value; OnPropertyChanged(); } }

        public int Rooms { get { return _rooms; } set { _rooms = value; OnPropertyChanged(); } }

        public int Pax { get { return _pax; } set { _pax = value; OnPropertyChanged(); } }

        public decimal Total { get { return _total; } set { _total = value; OnPropertyChanged(); } }
    }

    class GuestViewModel : BaseViewModel
    {
        string _id;
        string _name;
        string _gender;
        DateTime _birthday;
        string _address;
        string _email;
        string _phone;

        public string ID { get { return _id; } set { _id = value; OnPropertyChanged(); } }

        public string Name { get { return _name; } set { _name = value; OnPropertyChanged(); } }

        public string Gender { get { return _gender; } set { _gender = value; OnPropertyChanged(); } }

        public DateTime Birthday { get { return _birthday; } set { _birthday = value; OnPropertyChanged(); } }

        public string Address { get { return _address; } set { _address = value; OnPropertyChanged(); } }

        public string Email { get { return _email; } set { _email = value; OnPropertyChanged(); } }

        public string Phone { get { return _phone; } set { _phone = value; OnPropertyChanged(); } }
    }

    public class RoomViewModel : BaseViewModel
    {
        private bool _isSelected;
        int _room_id;
        int _roomtype_id;
        string _roomType;
        string _roomName;
        decimal _price;

        public bool IsSelected { get { return _isSelected; } set { _isSelected = value; OnPropertyChanged(); } }

        public int RoomID { get { return _room_id; } set { _room_id = value; OnPropertyChanged(); } }

        public int RoomTypeID { get { return _roomtype_id; } set { _roomtype_id = value; OnPropertyChanged(); } }

        public string RoomType { get { return _roomType; } set { _roomType = value; OnPropertyChanged(); } }

        public string RoomName { get { return _roomName; } set { _roomName = value; OnPropertyChanged(); } }

        public decimal Price { get { return _price; } set { _price = value; OnPropertyChanged(); } }
    }
}