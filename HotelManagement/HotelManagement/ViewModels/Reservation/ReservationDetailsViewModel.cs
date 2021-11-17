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

        private bool _canEditFolio;
        public bool CanEditFolio { get { return _canEditFolio; } set { _canEditFolio = value; OnPropertyChanged(); } }

        private DateTime _limitArrival;
        public DateTime LimitArrival { get { return _limitArrival; } set { _limitArrival = value;OnPropertyChanged(); } }

        private DateTime _limitDeparture;
        public DateTime LimitDeparture { get { return _limitDeparture; } set { _limitDeparture = value; OnPropertyChanged(); } }

        public GuestViewModel GuestInformation { get; set; }

        public ReservationViewModel StayInformation { get; set; }

        public ObservableCollection<GuestViewModel> Sharers { get; set; }

        public GuestViewModel NewSharer { get; set; }

        public ObservableCollection<RoomViewModel> BookedRooms { get; set; }

        public ObservableCollection<RoomViewModel> AvailableRooms { get; set; }

        public ObservableCollection<RoomViewModel> SelectedRooms { get; set; }

        public bool BeASharer { get; set; }

        public bool Guaranteed { get; set; }

        public IEnumerable<string> Gender => new[] { "Male", "Female" };

        #region Command
        // Switch between editable and read only mode
        private bool CanExecuteEditCommand
        {
            get
            {
                if (StayInformation.Status != "Completed" && StayInformation.Status != "Cancelled") return true;
                return false;
            }
        }
        void SetPermission()
        {
            if (CurrentAccount.Instance.Permission == "Admin" ||
                CurrentAccount.Instance.Permission == "Reservation")
                CanEdit = !CanEdit;
            if (CurrentAccount.Instance.Permission == "Admin" ||
                CurrentAccount.Instance.Permission == "Reservation" ||
                CurrentAccount.Instance.Permission == "Receptionist")
                CanEditFolio = !CanEditFolio;
        }
        private ICommand _canEditCommand;
        public ICommand CanEditCommand
        {
            get
            {
                return _canEditCommand ?? (_canEditCommand = new RelayCommand<object>((p) =>
                CanExecuteEditCommand, (p) => SetPermission()));
            }
        }

        // Reserve as sharer
        private bool CanReserveAsSharer
        {
            get
            {
                if (GuestInformation.FilledGuestInformation == false) return false;
                if (BeASharer == true) return false;
                if (StayInformation.MaxPax >= StayInformation.Pax) return false;
                return true;
            }
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
            wd.cbbSelectRoom.ItemsSource = BookedRooms;
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
            var db = new HotelManagementEntities();
            if (!db.GUEST_BOOKING.Any(gb => gb.reservation_id == StayInformation.ID && gb.guest_id == NewSharer.ID))
            {
                var existing_guest = db.GUESTs.Where(g => g.id == NewSharer.ID);
                if (existing_guest.Count() == 0)
                {
                    var guest = new GUEST()
                    {
                        id = NewSharer.ID,
                        name = NewSharer.Name,
                        gender = NewSharer.Gender,
                        birthday = NewSharer.Birthday,
                        email = NewSharer.Email,
                        phone = NewSharer.Phone,
                        address = NewSharer.Address,
                    };
                    db.GUESTs.Add(guest);
                }
                else
                {
                    // TODO: Autofill new sharer by id
                    // Now describe: If this guest information is existing in database, overriding 
                    var guest = existing_guest.FirstOrDefault();
                    guest.name = NewSharer.Name;
                    guest.gender = NewSharer.Gender;
                    guest.birthday = NewSharer.Birthday;
                    guest.email = NewSharer.Email;
                    guest.phone = NewSharer.Phone;
                    guest.address = NewSharer.Address;
                }
                db.SaveChanges();

                var guestBooking = new GUEST_BOOKING()
                {
                    reservation_id = StayInformation.ID,
                    guest_id = NewSharer.ID,
                    room_booked_id = db.ROOM_BOOKED.Where(rb => rb.reservation_id == StayInformation.ID &&
                                            rb.room_id == NewSharer.Room.RoomID).FirstOrDefault().id,
                };
                db.GUEST_BOOKING.Add(guestBooking);
                db.SaveChanges();
            }

            LoadGuests(StayInformation.ID);
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
            wd.cbbSelectRoom.ItemsSource = BookedRooms;
            wd.btnConfirm.Command = ConfirmEditSharerCommand;
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

        // Confirm edit sharer
        public void EditSharer(Window wd)
        {
            var db = new HotelManagementEntities();
            if (db.GUEST_BOOKING.Any(gb => gb.reservation_id == StayInformation.ID && gb.guest_id == NewSharer.ID))
            {
                var guest = db.GUESTs.Where(g => g.id == NewSharer.ID).FirstOrDefault();
                guest.name = NewSharer.Name;
                guest.gender = NewSharer.Gender;
                guest.birthday = NewSharer.Birthday;
                guest.email = NewSharer.Email;
                guest.phone = NewSharer.Phone;
                guest.address = NewSharer.Address;

                db.SaveChanges();

                var guestBooking = db.GUEST_BOOKING.Where(gb => gb.reservation_id == StayInformation.ID && 
                                gb.guest_id == NewSharer.ID).FirstOrDefault();
                var roomBooked = db.ROOM_BOOKED.Where(rb => rb.id == guestBooking.room_booked_id).FirstOrDefault();
                roomBooked.room_id = NewSharer.Room.RoomID;

                db.SaveChanges();

                MessageBox.Show("Edit sharer information successfully");
            }

            LoadGuests(StayInformation.ID);
            wd.Close();
        }

        private ICommand _confirmEditSharerCommand;
        public ICommand ConfirmEditSharerCommand
        {
            get
            {
                return _confirmEditSharerCommand ?? (_confirmEditSharerCommand = new RelayCommand<Window>((p) => NewSharer.FilledGuestInformation, (p) => EditSharer(p)));
            }
        }

        // Open add room window
        private bool CanBookRoom
        {
            get
            {
                if (StayInformation.Status == "Operational" ||
                    StayInformation.Status == "No Show") return false;
                return true;
            }
        }
        public void OpenAddRoomWindow()
        {
            var wd = new AddBookedRoomWindow();
            AvailableRooms = new ObservableCollection<RoomViewModel>();
            LoadAvailableRooms();

            wd.DataContext = this;
            wd.ShowDialog();
        }

        private ICommand _addRoomCommand;
        public ICommand AddRoomCommand
        {
            get
            {
                return _addRoomCommand ?? (_addRoomCommand = new RelayCommand<object>((p) => CanBookRoom, (p) => OpenAddRoomWindow()));
            }
        }

        // Remove booked room
        private bool CanRemoveBooked
        {
            get
            {
                if (BookedRooms.Count <= 1) return false;
                if (StayInformation.Status == "Operational" ||
                    StayInformation.Status == "No Show") return false;
                if (CurrentAccount.Instance.Permission != "Reservation" &&
                    CurrentAccount.Instance.Permission != "Admin") return false;
                return true;
            }
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

        private ICommand _removeRoomCommand;
        public ICommand RemoveRoomCommand
        {
            get
            {
                return _removeRoomCommand ?? (_removeRoomCommand = new RelayCommand<RoomViewModel>((p) => CanRemoveBooked, (p) => RemoveSelectedRoom(p)));
            }
        }

        // Confirm add room
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

        private ICommand _confirmAddRoomCommand;
        public ICommand ConfirmAddRoomCommand
        {
            get
            {
                return _confirmAddRoomCommand ?? (_confirmAddRoomCommand = new RelayCommand<Window>((p) => SelectedRooms.Count > 0, (p) => AddRoom(p)));
            }
        }

        // Show Folio Window
        bool CanOpenFolioWindow
        {
            get
            {
                if (CurrentAccount.Instance.Permission == "Receptionist" ||
                    CurrentAccount.Instance.Permission == "Admin") return true;
                return false;
            }
        }
        public void OpenFolioWindow(RoomViewModel room)
        {
            // TODO: QUANGGGGGGGGGGGGGGGGGGGG
            // room duoc truyen vao la phong duoc click
        }

        private ICommand _showFolioCommand;
        public ICommand ShowFolioCommand 
        { 
            get 
            {
                return _showFolioCommand ?? (_showFolioCommand = new RelayCommand<RoomViewModel>((p) => CanOpenFolioWindow, (p) => OpenFolioWindow(p)));
            } 
        }

        // Save data
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

        private ICommand _saveDataCommand;
        public ICommand SaveDataCommand
        {
            get
            {
                return _saveDataCommand ?? (_saveDataCommand = new RelayCommand<Window>((p) => GuestInformation.FilledGuestInformation, (p) => SaveDataChange(p)));
            }
        }

        // Close reservation details window
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
            LimitDeparture = DateTime.Today.AddYears(1);

            GuestInformation.PropertyChanged += GuestInformation_PropertyChanged;
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
            LoadLimitChangeStays();
        }

        #region Load data
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

            LoadBookedRooms(ResID);
            LoadGuests(ResID);
        }

        void LoadBookedRooms(int ResID)
        {
            if (BookedRooms.Count > 0) BookedRooms.Clear();
            StayInformation.MaxPax = 0;

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
                             Capacity = rt.max_guest,
                         }).ToList();

            foreach (var room in rooms)
            {
                var obj = new RoomViewModel()
                {
                    RoomID = room.RoomID,
                    RoomName = room.RoomName,
                    RoomType = room.TypeName,
                    Price = SeparateThousands(((long)room.Price).ToString()),
                    Capacity = (int)room.Capacity,
                };
                BookedRooms.Add(obj);

                StayInformation.MaxPax += obj.Capacity;
            }
        }

        void LoadGuests(int ResID)
        {
            if (Sharers.Count > 0) Sharers.Clear();

            var db = new HotelManagementEntities();
            var sharers = (from g in db.GUESTs
                           join gb in db.GUEST_BOOKING on g.id equals gb.guest_id
                           where gb.reservation_id == ResID
                           select new 
                           {
                               ID = g.id,
                               Name = g.name,
                               Gender = g.gender,
                               Birthday = g.birthday,
                               Email = g.email,
                               Phone = g.phone,
                               Address = g.address,
                               RoomBooked = gb.room_booked_id
                           }).ToList();

            foreach (var sharer in sharers)
            {
                if (sharer.ID == GuestInformation.ID) BeASharer = true;

                var obj = new GuestViewModel()
                {
                    ID = sharer.ID,
                    Name = sharer.Name,
                    Birthday = (DateTime)sharer.Birthday,
                    Gender = sharer.Gender,
                    Email = sharer.Email,
                    Phone = sharer.Phone,
                    Address = sharer.Address,
                    Age = GuestViewModel.CalculateAge((DateTime)sharer.Birthday),
                };

                foreach (var room in BookedRooms)
                {
                    int RoomID = db.ROOM_BOOKED.Where(rb => rb.id == sharer.RoomBooked).FirstOrDefault().room_id;
                    if (room.RoomID == RoomID) obj.Room = room;
                }

                Sharers.Add(obj);
            }
        }

        void LoadLimitChangeStays()
        {
            var db = new HotelManagementEntities();

            // Find limit of changing arrival and departure date
            foreach (var room in BookedRooms)
            {
                var nearestArrival = (from res in db.RESERVATIONs
                                      join rb in db.ROOM_BOOKED on res.id equals rb.reservation_id
                                      where rb.room_id == room.RoomID && res.departure < StayInformation.Arrival
                                      orderby res.departure descending
                                      select res);

                var nearestDeparture = (from res in db.RESERVATIONs
                                        join rb in db.ROOM_BOOKED on res.id equals rb.reservation_id
                                        where rb.room_id == room.RoomID && res.arrival > StayInformation.Departure
                                        orderby res.arrival ascending
                                        select res);

                if (nearestArrival.ToList().Count != 0)
                {
                    if (LimitArrival == DateTime.Parse("01-01-0001") || nearestArrival.FirstOrDefault().departure >= LimitArrival)
                        LimitArrival = (DateTime)nearestArrival.FirstOrDefault().departure;
                }

                if (nearestDeparture.ToList().Count != 0)
                {
                    if (nearestDeparture.FirstOrDefault().arrival <= LimitDeparture)
                        LimitDeparture = (DateTime)nearestDeparture.FirstOrDefault().arrival;
                }
            }          
        }
        #endregion

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
                             group r by r.RoomID into result
                             select result;

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
