using HotelManagement.Models;
using HotelManagement.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace HotelManagement.ViewModels
{
    class NewReservationViewModel : BaseViewModel
    {
        private ReservationListViewModel Instance { get; set; }

        public GuestViewModel GuestInformation { get; set; }

        public ReservationViewModel StayInformation { get; set; }

        public GuestViewModel NewSharer { get; set; }

        public ObservableCollection<RoomViewModel> AvailableRooms { get; set; }

        public ObservableCollection<RoomViewModel> SelectedRooms { get; set; }

        public ObservableCollection<GuestViewModel> Sharers { get; set; }

        private bool _beWalkIn;
        public bool BeWalkIn { get { return _beWalkIn; } set { _beWalkIn = value; OnPropertyChanged(); } }

        public bool BeASharer { get; set; }

        public bool Guaranteed { get; set; }

        public IEnumerable<string> Gender => new[] { "Male", "Female" };

        #region Command
        // Reserve as sharer
        private bool CanReserveAsSharer
        {
            get
            {
                if (GuestInformation.FilledGuestInformation == false) return false;
                if (BeASharer == true) return false;
                if (StayInformation.Pax >= StayInformation.MaxPax) return false;
                return true;
            }
        }
        public void ReserveLikeASharer()
        {
            Sharers.Add(GuestInformation);
            BeASharer = true;
        }

        private ICommand _beASharerCommand;
        public ICommand BeASharerCommand
        {
            get
            {
                return _beASharerCommand ?? (_beASharerCommand = new RelayCommand<object>((p) => CanReserveAsSharer, (p) => ReserveLikeASharer()));
            }
        }

        // Open add sharer window
        private bool CanAddSharer
        {
            get
            {
                if (Sharers.Count < StayInformation.MaxPax)
                    return true;
                else return false;
            }
        }
        public void OpenAddSharerWindow()
        {
            var wd = new AddBookingGuestWindow();
            NewSharer = new GuestViewModel();
            NewSharer.Birthday = DateTime.Parse("01-01-2000");
            wd.DataContext = this;
            wd.ShowDialog();
        }

        private ICommand _addSharerCommand;
        public ICommand AddSharerCommand
        {
            get
            {
                return _addSharerCommand ?? (_addSharerCommand = new RelayCommand<object>((p) => CanAddSharer, (p) => OpenAddSharerWindow()));
            }
        }

        // Remove sharer
        public void RemoveSelectedSharer(GuestViewModel sharer)
        {
            Sharers.Remove(sharer);
            if (sharer.ID == GuestInformation.ID) BeASharer = false;
        }

        private ICommand _removeSharerCommand;
        public ICommand RemoveSharerCommand
        {
            get
            {
                return _removeSharerCommand ?? (_removeSharerCommand = new RelayCommand<GuestViewModel>((p) => Sharers.Count > 1, (p) => RemoveSelectedSharer(p)));
            }
        }

        // Confirm add sharer
        public void AddSharer(Window wd)
        {
            if (wd.Title == "Add new sharer") Sharers.Add(NewSharer);
            wd.Close();
        }

        private ICommand _confirmAddSharerCommand;
        public ICommand ConfirmAddSharerCommand
        {
            get
            {
                return _confirmAddSharerCommand ?? (_confirmAddSharerCommand = new RelayCommand<Window>((p) => NewSharer.FilledGuestInformation, (p) => AddSharer(p)));
            }
        }

        // Open edit sharer window
        public void OpenEditSharerWindow(GuestViewModel guest)
        {
            var wd = new AddBookingGuestWindow();
            NewSharer = guest;
            wd.Title = "Edit sharer information";
            wd.txtboxGuestID.IsEnabled = false;
            wd.DataContext = this;
            wd.ShowDialog();
        }

        private ICommand _editSharerCommand;
        public ICommand EditSharerCommand
        {
            get
            {
                return _editSharerCommand ?? (_editSharerCommand = new RelayCommand<GuestViewModel>((p) => true, (p) => OpenEditSharerWindow(p)));
            }
        }

        // Confirm to reserve
        public bool CanReserve
        {
            get
            {
                if (!GuestInformation.FilledGuestInformation) return false;
                if (Sharers.Count == 0) return false;
                if (StayInformation.Rooms == 0) return false;
                foreach (var row in Sharers)
                {
                    if (String.IsNullOrEmpty(row.Name) ||
                        String.IsNullOrEmpty(row.ID) ||
                        String.IsNullOrEmpty(row.Gender) ||
                        String.IsNullOrEmpty(row.Address) ||
                        row.Room == null)
                        return false;
                }
                return true;
            }
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
                    status = StayInformation.Status,
                    early_checkin = StayInformation.EarlyCheckin,
                    late_checkout = false,
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
                            birthday = sharer.Birthday,
                            email = sharer.Email,
                            phone = sharer.Phone,
                            address = sharer.Address,
                        };
                        context.GUESTs.Add(newGuest);
                        context.SaveChanges();
                    }

                    var guestBooking = new GUEST_BOOKING()
                    {
                        reservation_id = reservation.id,
                        guest_id = sharer.ID,
                        room_booked_id = context.ROOM_BOOKED.Where(rb => rb.reservation_id == reservation.id && 
                                            rb.room_id == sharer.Room.RoomID).FirstOrDefault().id,
                    };
                    context.GUEST_BOOKING.Add(guestBooking);
                    context.SaveChanges();
                }
            }
            if (Instance != null)
                Instance.LoadReservations();
            window.Close();
        }

        private ICommand _reserveCommand;
        public ICommand ReserveCommand
        {
            get
            {
                return _reserveCommand ?? (_reserveCommand = new RelayCommand<Window>((p) => CanReserve, (p) => Reserve(p)));
            }
        }

        // Cancel reserving
        private ICommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new RelayCommand<Window>((p) => true, (p) => p.Close()));
            }
        }
        #endregion

        public NewReservationViewModel(ReservationListViewModel _instance)
        {
            Instance = _instance;

            Sharers = new ObservableCollection<GuestViewModel>();
            GuestInformation = new GuestViewModel();
            StayInformation = new ReservationViewModel();
            AvailableRooms = new ObservableCollection<RoomViewModel>();
            SelectedRooms = new ObservableCollection<RoomViewModel>(); 

            StayInformation.PropertyChanged += StayInformation_PropertyChanged;
            GuestInformation.PropertyChanged += GuestInformation_PropertyChanged;
            Sharers.CollectionChanged += Sharers_CollectionChanged;
            SelectedRooms.CollectionChanged += SelectedRooms_CollectionChanged;

            BeASharer = false;
            GuestInformation.Birthday = DateTime.Parse("01-01-2000");
            StayInformation.Arrival = DateTime.Today;
            StayInformation.Departure = DateTime.Today.AddDays(1);
        }

        public NewReservationViewModel()
        {
            Sharers = new ObservableCollection<GuestViewModel>();
            GuestInformation = new GuestViewModel();
            StayInformation = new ReservationViewModel();
            AvailableRooms = new ObservableCollection<RoomViewModel>();
            SelectedRooms = new ObservableCollection<RoomViewModel>();

            StayInformation.PropertyChanged += StayInformation_PropertyChanged;
            GuestInformation.PropertyChanged += GuestInformation_PropertyChanged;
            Sharers.CollectionChanged += Sharers_CollectionChanged;
            SelectedRooms.CollectionChanged += SelectedRooms_CollectionChanged;

            BeASharer = false;
            GuestInformation.Birthday = DateTime.Parse("01-01-2000");
            StayInformation.Arrival = DateTime.Today;
            StayInformation.Departure = DateTime.Today.AddDays(1);
        }

        private void GuestInformation_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GuestViewModel.Birthday))
            {
                if ((sender as GuestViewModel).Age < 21)
                {
                    GuestInformation.Birthday = DateTime.Parse("01-01-2000");
                    MessageBox.Show("Guest must be at least 21 years of age for reserving.", "WALKIN / RESERVATION POLICY");
                }
            }
        }

        private void SelectedRooms_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            StayInformation.Rooms = SelectedRooms.Count;
        }

        private void StayInformation_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var StayInfo = sender as ReservationViewModel;
            if (e.PropertyName == nameof(ReservationViewModel.Arrival))
            {
                if (StayInfo.Arrival < DateTime.Today)
                    StayInformation.Arrival = DateTime.Today;

                if (StayInfo.Arrival == DateTime.Today)
                {
                    BeWalkIn = true;
                    StayInformation.Status = "Operational";
                    if (DateTime.Now.TimeOfDay < new TimeSpan(12, 0, 0))
                        StayInformation.EarlyCheckin = true;
                    else StayInformation.EarlyCheckin = false;
                }
                else
                {
                    BeWalkIn = false;
                    if (Guaranteed) StayInformation.Status = "Confirmed";
                    else StayInformation.Status = "On Request";
                }

                if (StayInfo.Departure != DateTime.Parse("01-01-0001"))
                {
                    if ((int)(StayInfo.Departure - StayInfo.Arrival).TotalDays < 1)
                        StayInformation.Arrival = DateTime.Today;

                    StayInformation.Stays = (int)(StayInformation.Departure - StayInformation.Arrival).TotalDays;
                }

                LoadAvailableRooms();
            }

            if (e.PropertyName == nameof(ReservationViewModel.Departure))
            {
                if (StayInfo.Departure < DateTime.Today.AddDays(1))
                    StayInformation.Departure = DateTime.Today.AddDays(1);
                
                if (StayInfo.Arrival != DateTime.Parse("01-01-0001"))
                {
                    if ((int)(StayInfo.Departure - StayInfo.Arrival).TotalDays < 1)
                        StayInformation.Departure = DateTime.Today.AddDays(1);

                    StayInformation.Stays = (int)(StayInformation.Departure - StayInformation.Arrival).TotalDays;
                }
                    
                LoadAvailableRooms();
            }
        }

        private void Sharers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            StayInformation.Pax = Sharers.Count;
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
                           join rb in db.ROOM_BOOKED on r.id equals rb.room_id into result
                           from rs in result.DefaultIfEmpty() 
                           select new
                           {
                               RoomID = r.id,
                               RoomName = r.name,
                               OOS = r.out_of_service,
                               Dirty = r.dirty,
                               TypeID = r.roomtype_id,
                               TypeName = r.ROOMTYPE.name,
                               CreatedRT = r.ROOMTYPE.date_created,
                               UpdatedRT = r.ROOMTYPE.date_updated,
                               ResID = rs == null ? 0 : rs.reservation_id,
                               Arrival = rs.RESERVATION.arrival,
                               Departure = rs.RESERVATION.departure,
                               Price = r.ROOMTYPE.price,
                               Capacity = r.ROOMTYPE.max_guest,
                           };

            var booked = from r in allrooms where !(r.Arrival >= StayInformation.Departure || r.Departure <= StayInformation.Arrival) select r;

            var availables = from r in allrooms
                             where r.CreatedRT <= DateTime.Today && (r.UpdatedRT == null || r.UpdatedRT >= DateTime.Today) &&
                             r.OOS == false && r.Dirty == false &&
                             !booked.Any(exc => exc.RoomID == r.RoomID) || r.ResID == 0
                             group r by r.RoomID into result select result;

            // TODO: Thieu truong hop cancelled res va on request res

            foreach (var r in availables)
            {
                var room = r.FirstOrDefault();
                var obj = new RoomViewModel()
                {
                    RoomID = room.RoomID,
                    RoomType = room.TypeName,
                    RoomName = room.RoomName,
                    RoomTypeID = room.TypeID,
                    Price = SeparateThousands(((long)room.Price).ToString()),
                    Capacity = (int)room.Capacity,
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
                    StayInformation.MaxPax += (sender as RoomViewModel).Capacity;
                }
                else
                {
                    SelectedRooms.Remove(sender as RoomViewModel);
                    StayInformation.MaxPax -= (sender as RoomViewModel).Capacity;
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
        private bool _early_checkin;

        private int _stays;
        private int _rooms;
        private int _pax;

        public int ID { get { return _id; } set { _id = value; OnPropertyChanged(); } }

        public string Status { get { return _status; } set { _status = value; OnPropertyChanged(); } }

        public DateTime DateCreated { get { return _date_created; } set { _date_created = value; OnPropertyChanged(); } }

        public DateTime Arrival { get { return _arrival; } set { _arrival = value; OnPropertyChanged(); } }

        public DateTime Departure { get { return _departure; } set { _departure = value; OnPropertyChanged(); } }

        public bool EarlyCheckin { get { return _early_checkin; } set { _early_checkin = value; OnPropertyChanged(); } }

        public int Stays { get { return _stays; } set { _stays = value; OnPropertyChanged(); } }

        public int Rooms { get { return _rooms; } set { _rooms = value; OnPropertyChanged(); } }

        public int Pax { get { return _pax; } set { _pax = value; OnPropertyChanged(); } }

        public int MaxPax { get; set; }
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

        int _age;

        RoomViewModel _room;

        public string ID { get { return _id; } set { _id = value; OnPropertyChanged(); } }

        public string Name { get { return _name; } set { _name = value; OnPropertyChanged(); } }

        public string Gender { get { return _gender; } set { _gender = value; OnPropertyChanged(); } }

        public DateTime Birthday { get { return _birthday; } set { _birthday = value; CalculateAge(); OnPropertyChanged(); } }

        public string Address { get { return _address; } set { _address = value; OnPropertyChanged(); } }

        public string Email { get { return _email; } set { _email = value; OnPropertyChanged(); } }

        public string Phone { get { return _phone; } set { _phone = value; OnPropertyChanged(); } }

        public int Age { get { return _age; } set { _age = value; OnPropertyChanged(); } }

        public RoomViewModel Room { get { return _room; } set { _room = value; OnPropertyChanged(); } }

        public bool FilledGuestInformation
        {
            get
            {
                if (String.IsNullOrEmpty(ID) ||
                    String.IsNullOrEmpty(Gender) ||
                    String.IsNullOrEmpty(Name) ||
                    String.IsNullOrEmpty(Email) ||
                    String.IsNullOrEmpty(Phone) ||
                    String.IsNullOrEmpty(Address))
                    return false;
                return true;
            }
        }

        public void CalculateAge()
        {
            Age = DateTime.Today.Year - Birthday.Year;
            if (DateTime.Now.DayOfYear < Birthday.DayOfYear)
                Age = Age - 1;
        }

        static public int CalculateAge(DateTime birthday)
        {
            int age = DateTime.Today.Year - birthday.Year;
            if (DateTime.Now.DayOfYear < birthday.DayOfYear)
                age = age - 1;
            return age;
        }
    }

    class RoomViewModel : BaseViewModel
    {
        private bool _isSelected;
        int _room_id;
        int _roomtype_id;
        string _roomType;
        string _roomName;
        string _price;
        int _capacity;

        public bool IsSelected { get { return _isSelected; } set { _isSelected = value; OnPropertyChanged(); } }

        public int RoomID { get { return _room_id; } set { _room_id = value; OnPropertyChanged(); } }

        public int RoomTypeID { get { return _roomtype_id; } set { _roomtype_id = value; OnPropertyChanged(); } }

        public string RoomType { get { return _roomType; } set { _roomType = value; OnPropertyChanged(); } }

        public string RoomName { get { return _roomName; } set { _roomName = value; OnPropertyChanged(); } }

        public string Price { get { return _price; } set { _price = value; OnPropertyChanged(); } }

        public int Capacity { get { return _capacity; } set { _capacity = value; OnPropertyChanged(); } }
    }
}