using HotelManagement.Models;
using HotelManagement.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HotelManagement.ViewModels
{
    class ReservationDetailsViewModel : BaseViewModel
    {
        private ReservationListViewModel Instance { get; set; }

        private bool _canEdit;
        public bool CanEdit { get { return _canEdit; } set { _canEdit = value; OnPropertyChanged(); } }

        public GuestViewModel GuestInformation { get; set; }

        public ReservationViewModel StayInformation { get; set; }

        public ObservableCollection<GuestViewModel> Sharers { get; set; }

        public GuestViewModel NewSharer { get; set; }

        public ObservableCollection<RoomViewModel> BookedRooms { get; set; }

        public ObservableCollection<RoomViewModel> AvailableRooms { get; set; }

        public ObservableCollection<RoomViewModel> SelectedRooms { get; set; }

        public bool BeASharer { get; set; }

        public bool Guaranteed { get; set; }

        public IEnumerable<string> Gender => new[] { "Male", "Female", "Other" };

        #region Command
        private ICommand _canEditCommand;
        public ICommand CanEditCommand
        {
            get
            {
                return _canEditCommand ?? (_canEditCommand = new RelayCommand<object>((p) => StayInformation.Status != "Completed", (p) => { CanEdit = !CanEdit; }));
            }
        }

        private ICommand _beASharerCommand;
        public ICommand BeASharerCommand
        {
            get
            {
                return _beASharerCommand ?? (_beASharerCommand = new RelayCommand<object>((p) => GuestInformation.FilledGuestInformation && !BeASharer, (p) => ReserveLikeASharer()));
            }
        }

        private ICommand _addSharerCommand;
        public ICommand AddSharerCommand
        {
            get
            {
                return _addSharerCommand ?? (_addSharerCommand = new RelayCommand<object>((p) => true, (p) => OpenAddSharerWindow()));
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

        private ICommand _confirmAddSharerCommand;
        public ICommand ConfirmAddSharerCommand
        {
            get
            {
                return _confirmAddSharerCommand ?? (_confirmAddSharerCommand = new RelayCommand<Window>((p) => NewSharer.FilledGuestInformation, (p) => AddSharer(p)));
            }
        }

        private ICommand _addRoomCommand;
        public ICommand AddRoomCommand
        {
            get
            {
                return _addRoomCommand ?? (_addRoomCommand = new RelayCommand<object>((p) => true, (p) => OpenAddRoomWindow()));
            }
        }

        private ICommand _removeRoomCommand;
        public ICommand RemoveRoomCommand
        {
            get
            {
                return _removeRoomCommand ?? (_removeRoomCommand = new RelayCommand<RoomViewModel>((p) => BookedRooms.Count > 1, (p) => RemoveSelectedRoom(p)));
            }
        }

        private ICommand _confirmAddRoomCommand;
        public ICommand ConfirmAddRoomCommand
        {
            get
            {
                return _confirmAddRoomCommand ?? (_confirmAddRoomCommand = new RelayCommand<Window>((p) => SelectedRooms.Count > 0, (p) => AddRoom(p)));
            }
        }

        private ICommand _saveDataCommand;
        public ICommand SaveDataCommand
        {
            get
            {
                return _saveDataCommand ?? (_saveDataCommand = new RelayCommand<Window>((p) => GuestInformation.FilledGuestInformation, (p) => SaveDataChange(p)));
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

        public ReservationDetailsViewModel(int ResID, ReservationListViewModel _instance)
        {
            Instance = _instance;

            CanEdit = false;
            Sharers = new ObservableCollection<GuestViewModel>();
            BookedRooms = new ObservableCollection<RoomViewModel>();
            SelectedRooms = new ObservableCollection<RoomViewModel>();

            Sharers.CollectionChanged += Sharers_CollectionChanged;
            BookedRooms.CollectionChanged += BookedRooms_CollectionChanged;

            LoadReservationDetails(ResID);
        }

        private void StayInformation_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var StayInfo = sender as ReservationViewModel;
            if (e.PropertyName == nameof(ReservationViewModel.Arrival))
            {
                if (StayInfo.Departure != DateTime.Parse("01-01-0001"))
                {
                    if ((int)(StayInfo.Departure - StayInfo.Arrival).TotalDays < 1)
                        StayInformation.Arrival = StayInformation.Departure.AddDays(-1);

                    StayInformation.Stays = (int)(StayInformation.Departure - StayInformation.Arrival).TotalDays;
                }
            }

            if (e.PropertyName == nameof(ReservationViewModel.Departure))
            {
                if (StayInfo.Arrival != DateTime.Parse("01-01-0001"))
                {
                    if ((int)(StayInfo.Departure - StayInfo.Arrival).TotalDays < 1)
                        StayInformation.Departure = StayInformation.Arrival.AddDays(1);

                    StayInformation.Stays = (int)(StayInformation.Departure - StayInformation.Arrival).TotalDays;
                }
            }
        }

        private void Sharers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            StayInformation.Pax = Sharers.Count;
        }

        private void BookedRooms_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            StayInformation.Rooms = BookedRooms.Count;
        }

        public void ReserveLikeASharer()
        {
            var db = new HotelManagementEntities();
            if (!db.GUEST_BOOKING.Any(gb => gb.reservation_id == StayInformation.ID && gb.guest_id == GuestInformation.ID))
            {
                var guestBooking = new GUEST_BOOKING()
                {
                    reservation_id = StayInformation.ID,
                    guest_id = GuestInformation.ID,
                };
                db.GUEST_BOOKING.Add(guestBooking);
                db.SaveChanges();
            }

            LoadGuests(StayInformation.ID);
            BeASharer = true;
        }

        public void OpenAddSharerWindow()
        {
            var wd = new AddBookingGuestWindow();
            NewSharer = new GuestViewModel();
            NewSharer.Birthday = DateTime.Parse("01-01-2000");
            wd.DataContext = this;
            wd.ShowDialog();
        }

        public void AddSharer(Window wd)
        {
            var db = new HotelManagementEntities();
            if (!db.GUEST_BOOKING.Any(gb => gb.reservation_id == StayInformation.ID && gb.guest_id == NewSharer.ID))
            {
                var existing_guest = db.GUESTs.SingleOrDefault(g => g.id == NewSharer.ID);
                if (existing_guest == null)
                {
                    var guest = new GUEST()
                    {
                        id = NewSharer.ID,
                        name = NewSharer.Name,
                        gender = NewSharer.Gender,
                        address = NewSharer.Address,
                    };
                    db.GUESTs.Add(guest);
                }
                else
                {
                    existing_guest.name = NewSharer.Name;
                    existing_guest.gender = NewSharer.Gender;
                    existing_guest.address = NewSharer.Address;
                }
                db.SaveChanges();

                var guestBooking = new GUEST_BOOKING()
                {
                    reservation_id = StayInformation.ID,
                    guest_id = NewSharer.ID,
                };
                db.GUEST_BOOKING.Add(guestBooking);
                db.SaveChanges();
            }

            LoadGuests(StayInformation.ID);
            wd.Close();
        }

        public void RemoveSelectedSharer(GuestViewModel sharer)
        {
            if (sharer.ID != null)
            {
                var db = new HotelManagementEntities();
                var guest_booking = db.GUEST_BOOKING.SingleOrDefault(gb => gb.reservation_id == StayInformation.ID &&
                gb.guest_id == sharer.ID);
                db.GUEST_BOOKING.Remove(guest_booking);
                db.SaveChanges();
            }

            Sharers.Remove(sharer);
            if (sharer.ID == GuestInformation.ID) BeASharer = false;
        }

        public void OpenAddRoomWindow()
        {
            var wd = new AddBookedRoomWindow();
            AvailableRooms = new ObservableCollection<RoomViewModel>();
            LoadAvailableRooms();

            wd.DataContext = this;
            wd.ShowDialog();
        }

        public void AddRoom(Window wd)
        {
            var db = new HotelManagementEntities();
            foreach (var room in SelectedRooms)
            {
                var bookedBoom = new ROOM_BOOKED()
                {
                    reservation_id = StayInformation.ID,
                    room_id = room.RoomID,
                };
                db.ROOM_BOOKED.Add(bookedBoom);
            }
            db.SaveChanges();
            LoadBookedRooms(StayInformation.ID);
            Instance.LoadReservations();
            wd.Close();
        }

        public void RemoveSelectedRoom(RoomViewModel room)
        {
            var db = new HotelManagementEntities();
            var room_booked = db.ROOM_BOOKED.SingleOrDefault(rb => rb.reservation_id == StayInformation.ID &&
            rb.room_id == room.RoomID);
            if (room_booked.FOLIOs.Count() == 0)
            {
                db.ROOM_BOOKED.Remove(room_booked);
                db.SaveChanges();
                LoadBookedRooms(StayInformation.ID);
                Instance.LoadReservations();
            }
            else MessageBox.Show("Cannot remove booked room which registed a folio!!!", "[ERROR]");
        }

        void LoadReservationDetails(int ResID)
        {
            var db = new HotelManagementEntities();

            var reservation = (from res in db.RESERVATIONs where res.id == ResID select res).SingleOrDefault();

            var mainGuest = (from g in db.GUESTs where reservation.main_guest == g.id select g).SingleOrDefault();

            StayInformation = new ReservationViewModel();
            StayInformation.PropertyChanged += StayInformation_PropertyChanged;
            StayInformation.ID = reservation.id;
            StayInformation.Arrival = (DateTime)reservation.arrival;
            StayInformation.Departure = (DateTime)reservation.departure;
            StayInformation.Status = reservation.status;
            StayInformation.EarlyCheckin = (bool)reservation.early_checkin;

            GuestInformation = new GuestViewModel()
            {
                ID = mainGuest.id,
                Name = mainGuest.name,
                Gender = mainGuest.gender,
                Birthday = (DateTime)mainGuest.birthday,
                Email = mainGuest.email,
                Address = mainGuest.address,
                Phone = mainGuest.phone,
            };

            LoadGuests(ResID);
            LoadBookedRooms(ResID);
        }

        void LoadBookedRooms(int ResID)
        {
            if (BookedRooms.Count > 0) BookedRooms.Clear();

            var db = new HotelManagementEntities();
            var rooms = (from r in db.ROOMs
                         join rt in db.ROOMTYPEs on r.roomtype_id equals rt.id
                         join rb in db.ROOM_BOOKED on r.id equals rb.room_id
                         where rb.reservation_id == ResID
                         select new
                         {
                             RoomID = r.id,
                             RoomName = r.name,
                             TypeID = rt.id,
                             TypeName = rt.name,
                             Price = rt.price,
                         }).ToList();

            foreach (var room in rooms)
            {
                var obj = new RoomViewModel()
                {
                    RoomID = room.RoomID,
                    RoomName = room.RoomName,
                    RoomType = room.TypeName,
                    Price = SeparateThousands(((long)room.Price).ToString()),
                };
                BookedRooms.Add(obj);
            }
        }

        void LoadGuests(int ResID)
        {
            if (Sharers.Count > 0) Sharers.Clear();

            var db = new HotelManagementEntities();
            var sharers = (from g in db.GUESTs
                           join gb in db.GUEST_BOOKING on g.id equals gb.guest_id
                           where gb.reservation_id == ResID
                           select g).ToList();

            foreach (var sharer in sharers)
            {
                if (sharer.id == GuestInformation.ID) BeASharer = true;

                var obj = new GuestViewModel()
                {
                    ID = sharer.id,
                    Name = sharer.name,
                    Gender = sharer.gender,
                    Address = sharer.address,
                };
                Sharers.Add(obj);
            }
        }

        void SaveDataChange(Window window)
        {
            using (var context = new HotelManagementEntities())
            {
                // Update main guest information
                var main_guest = context.GUESTs.SingleOrDefault(mg => mg.id == GuestInformation.ID);
                main_guest.name = GuestInformation.Name;
                main_guest.gender = GuestInformation.Gender;
                main_guest.birthday = GuestInformation.Birthday;
                main_guest.email = GuestInformation.Email;
                main_guest.phone = GuestInformation.Phone;
                main_guest.address = GuestInformation.Address;
                context.SaveChanges();

                // Update reservation information
                var reservation = context.RESERVATIONs.SingleOrDefault(res => res.id == StayInformation.ID);
                reservation.arrival = StayInformation.Arrival;
                reservation.departure = StayInformation.Departure;
                context.SaveChanges();
            }

            Instance.LoadReservations();

            window.Close();
        }

        #region AddRoomWindow
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

                AvailableRooms.Last().PropertyChanged += ReservationDetailsViewModel_PropertyChanged;
            }
        }

        private void ReservationDetailsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
        #endregion
    }
}
