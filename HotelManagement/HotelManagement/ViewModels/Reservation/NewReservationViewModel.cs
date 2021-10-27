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
                return _beASharerCommand ?? (_beASharerCommand = new RelayCommand<object>((p) => FilledGuestInformation && !BeASharer, (p) => ReserveLikeASharer()));
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
                return _removeSharerCommand ?? (_removeSharerCommand = new RelayCommand<GuestViewModel>((p) => Sharers.Count > 1, (p) => RemoveSelectedSharer(p)));
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
            SelectedRooms.CollectionChanged += SelectedRooms_CollectionChanged;

            BeASharer = false;
            GuestInformation.Birthday = DateTime.Parse("01-01-2000");
            StayInformation.Arrival = DateTime.Today;
            StayInformation.Departure = DateTime.Today.AddDays(1);
        }

        private void SelectedRooms_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            StayInformation.Rooms = SelectedRooms.Count;
        }

        private void StayInformation_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ReservationViewModel.Arrival) ||
                e.PropertyName == nameof(ReservationViewModel.Departure))
            {
                var StayInfo = sender as ReservationViewModel;
                if (StayInfo.Arrival < DateTime.Today)
                    StayInformation.Arrival = DateTime.Today;
                if (StayInfo.Departure < DateTime.Today.AddDays(1))
                    StayInformation.Departure = DateTime.Today.AddDays(1);
                if ((int)(StayInfo.Departure - StayInfo.Arrival).TotalDays < 1)
                {
                    if (e.PropertyName == nameof(ReservationViewModel.Arrival)) StayInformation.Arrival = DateTime.Today;
                    if (e.PropertyName == nameof(ReservationViewModel.Departure)) StayInformation.Departure = DateTime.Today.AddDays(1);
                }
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
                if (Sharers.Count == 0) return false;
                if (StayInformation.Rooms == 0) return false;
                foreach (var row in Sharers)
                {
                    if (String.IsNullOrEmpty(row.Name) ||
                        String.IsNullOrEmpty(row.ID) ||
                        String.IsNullOrEmpty(row.Gender) ||
                        String.IsNullOrEmpty(row.Address)) 
                        return false;
                }
                return true;
            }
        }

        public void ReserveLikeASharer()
        {
            Sharers.Add(GuestInformation);
            BeASharer = true;
        }

        public void AddSharer()
        {
            GuestViewModel newSharer = new GuestViewModel();
            Sharers.Add(newSharer);
        }

        public void RemoveSelectedSharer(GuestViewModel sharer)
        {
            Sharers.Remove(sharer);
            if (sharer.ID == GuestInformation.ID) BeASharer = false;
        }

        public void Reserve(Window window) 
        {
            using (var context = new HotelManagementEntities())
            {
                // Insert main guest
                if (!context.GUESTs.Any(g => g.id == GuestInformation.ID))
                {
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
                    context.GUESTs.Add(mainGuest);
                    context.SaveChanges();
                }

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
                context.SaveChanges();

                // Insert room_booked
                foreach (var selectedRoom in SelectedRooms)
                {
                    var bookedBoom = new ROOM_BOOKED()
                    {
                        reservation_id = reservation.id,
                        room_id = selectedRoom.RoomID,
                    };
                    context.ROOM_BOOKED.Add(bookedBoom);
                    context.SaveChanges();
                }

                // Insert sharers
                foreach (var sharer in Sharers)
                {
                    if (!context.GUESTs.Any(g => g.id == sharer.ID))
                    {
                        var newGuest = new GUEST()
                        {
                            id = sharer.ID,
                            name = sharer.Name,
                            gender = sharer.Gender,
                            address = sharer.Address,
                        };
                        context.GUESTs.Add(newGuest);
                        context.SaveChanges();
                    }

                    var guestBooking = new GUEST_BOOKING()
                    {
                        reservation_id = reservation.id,
                        guest_id = sharer.ID,
                    };
                    context.GUEST_BOOKING.Add(guestBooking);
                    context.SaveChanges();
                }
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
            if (SelectedRooms.Count > 0) SelectedRooms.Clear();

            var db = new HotelManagementEntities();

            var allrooms = from r in db.ROOMs
                           join rt in db.ROOMTYPEs on r.roomtype_id equals rt.id 
                           join rb in db.ROOM_BOOKED on r.id equals rb.room_id into result
                           from rs in result.DefaultIfEmpty()
                           join res in db.RESERVATIONs on rs.reservation_id equals res.id into result1
                           from rs1 in result1.DefaultIfEmpty()
                           where rt.date_created <= DateTime.Today && (rt.date_updated == null || rt.date_updated >= DateTime.Today) &&
                           r.out_of_service == false && r.dirty == false
                           select new
                           {
                                RoomID = r.id,
                                RoomName = r.name,
                                TypeID = rt.id,
                                TypeName = rt.name,
                                ResID = rs == null ? 0 : rs.reservation_id,
                                Arrival = rs1.arrival,
                                Departure = rs1.departure,
                                Price = rt.price,
                           };

            var excepts = from r in allrooms where !(r.Arrival >= StayInformation.Departure || r.Departure <= StayInformation.Arrival) select r;

            // Thieu truong hop cancelled res va on request res

            var rooms = (from r in allrooms
                         where !excepts.Any(exc => exc.RoomID == r.RoomID) || r.ResID == 0 
                         group r by r.RoomID into rs
                         select rs).ToList();

            foreach (var r in rooms)
            {
                var room = r.FirstOrDefault();
                var obj = new RoomViewModel()
                {
                    RoomID = room.RoomID,
                    RoomType = room.TypeName,
                    RoomName = room.RoomName,
                    RoomTypeID = room.TypeID,
                    Price = SeparateThousands(((long)room.Price).ToString()),
                };

                AvailableRooms.Add(obj);

                AvailableRooms.Last().PropertyChanged += NewReservationViewModel_PropertyChanged;
            }
        }

        private void NewReservationViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RoomViewModel.IsSelected))
            {
                if ((sender as RoomViewModel).IsSelected)
                {
                    SelectedRooms.Add(sender as RoomViewModel);
                }
                else
                {
                    SelectedRooms.Remove(sender as RoomViewModel);
                }
                OnPropertyChanged(nameof(IsAllRoomsSelected));
            }
        }
    }

    class ReservationViewModel : BaseViewModel
    {
        private int _id;
        private string _status;
        private DateTime _date_created;
        private DateTime _arrival;
        private DateTime _departure;
        private int _stays;
        private int _rooms;
        private int _pax;

        public int ID { get { return _id; } set { _id = value; OnPropertyChanged(); } }

        public string Status { get { return _status; } set { _status = value; OnPropertyChanged(); } }

        public DateTime DateCreated { get { return _date_created; } set { _date_created = value; OnPropertyChanged(); } }

        public DateTime Arrival { get { return _arrival; } set { _arrival = value; OnPropertyChanged(); } }

        public DateTime Departure { get { return _departure; } set { _departure = value; OnPropertyChanged(); } }

        public int Stays { get { return _stays; } set { _stays = value; OnPropertyChanged(); } }

        public int Rooms { get { return _rooms; } set { _rooms = value; OnPropertyChanged(); } }

        public int Pax { get { return _pax; } set { _pax = value; OnPropertyChanged(); } }
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

    class RoomViewModel : BaseViewModel
    {
        private bool _isSelected;
        int _room_id;
        int _roomtype_id;
        string _roomType;
        string _roomName;
        string _price;

        public bool IsSelected { get { return _isSelected; } set { _isSelected = value; OnPropertyChanged(); } }

        public int RoomID { get { return _room_id; } set { _room_id = value; OnPropertyChanged(); } }

        public int RoomTypeID { get { return _roomtype_id; } set { _roomtype_id = value; OnPropertyChanged(); } }

        public string RoomType { get { return _roomType; } set { _roomType = value; OnPropertyChanged(); } }

        public string RoomName { get { return _roomName; } set { _roomName = value; OnPropertyChanged(); } }

        public string Price { get { return _price; } set { _price = value; OnPropertyChanged(); } }
    }
}