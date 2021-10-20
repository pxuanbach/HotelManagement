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
        private bool _canEdit;
        public bool CanEdit { get { return _canEdit; } set { _canEdit = value; OnPropertyChanged(); } }

        public GuestViewModel GuestInformation { get; set; }

        public ReservationViewModel StayInformation { get; set; }

        public ObservableCollection<GuestViewModel> Sharers { get; set; }

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
                return _canEditCommand ?? (_canEditCommand = new RelayCommand<object>((p) => true, (p) => { CanEdit = !CanEdit; }));
            }
        }

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

        private ICommand _saveDataCommand;
        public ICommand SaveDataCommand
        {
            get
            {
                return _saveDataCommand ?? (_saveDataCommand = new RelayCommand<Window>((p) => CanSaveData, (p) => SaveDataChange(p)));
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

        private ICommand _confirmAddRoomCommand;
        public ICommand ConfirmAddRoomCommand
        {
            get
            {
                return _confirmAddRoomCommand ?? (_confirmAddRoomCommand = new RelayCommand<Window>((p) => true, (p) => AddRoom(p)));
            }
        }
        #endregion

        public ReservationDetailsViewModel(int ResID)
        {
            CanEdit = false;
            Sharers = new ObservableCollection<GuestViewModel>();
            BookedRooms = new ObservableCollection<RoomViewModel>();
            SelectedRooms = new ObservableCollection<RoomViewModel>();

            LoadReservationDetails(ResID);
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
                BookedRooms.Add(room);
                var bookedBoom = new ROOM_BOOKED()
                {
                    reservation_id = StayInformation.ID,
                    room_id = room.RoomID,
                };
                db.ROOM_BOOKED.Add(bookedBoom);

                var invoice = db.INVOICEs.SingleOrDefault(inv => inv.reservation_id == StayInformation.ID);
                invoice.total_money += room.Price;

            }
            db.SaveChanges();
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
                BookedRooms.Remove(room);

                var invoice = db.INVOICEs.SingleOrDefault(inv => inv.reservation_id == StayInformation.ID);
                invoice.total_money -= room.Price;

                db.SaveChanges();

            }
            else MessageBox.Show("Cannot remove booked room which registed a folio!!!", "[ERROR]");
        }

        void LoadReservationDetails(int ResID)
        {
            var db = new HotelManagementEntities();

            var reservation = (from res in db.RESERVATIONs where res.id == ResID select res).SingleOrDefault();

            var mainGuest = (from g in db.GUESTs where reservation.main_guest == g.id select g).SingleOrDefault();

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

            var sharers = (from g in db.GUESTs
                           join gb in db.GUEST_BOOKING on g.id equals gb.guest_id
                           where gb.reservation_id == ResID
                           select g).ToList();

            StayInformation = new ReservationViewModel()
            {
                ID = reservation.id,
                Arrival = (DateTime)reservation.arrival,
                Departure = (DateTime)reservation.departure,
                Total = (decimal)(from inv in db.INVOICEs where inv.reservation_id == ResID select inv).SingleOrDefault().total_money,
                Status = reservation.status,
            };

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

            foreach (var room in rooms)
            {
                var obj = new RoomViewModel()
                {
                    RoomID = room.RoomID,
                    RoomName = room.RoomName,
                    RoomType = room.TypeName,
                    Price = Decimal.Round((decimal)room.Price),
                };
                BookedRooms.Add(obj);
            }

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

        public bool CanSaveData
        {
            get
            {
                if (!FilledGuestInformation) return false;
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

                // Update sharer information
                foreach (var sharer in Sharers)
                {
                    var existing_guest = context.GUESTs.SingleOrDefault(g => g.id == sharer.ID);
                    if (existing_guest == null)
                    {
                        var guest = new GUEST()
                        {
                            id = sharer.ID,
                            name = sharer.Name,
                            gender = sharer.Gender,
                            address = sharer.Address,
                        };
                        context.GUESTs.Add(guest);
                    }
                    else
                    {
                        existing_guest.name = sharer.Name;
                        existing_guest.gender = sharer.Gender;
                        existing_guest.address = sharer.Address;
                    }
                    context.SaveChanges();

                    if (!context.GUEST_BOOKING.Any(gb => gb.reservation_id == reservation.id && gb.guest_id == sharer.ID))
                    {
                        var guestBooking = new GUEST_BOOKING()
                        {
                            reservation_id = reservation.id,
                            guest_id = sharer.ID,
                        };
                        context.GUEST_BOOKING.Add(guestBooking);
                        context.SaveChanges();
                    }
                }
            }

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
                               RT_DateCreated = rt.date_created,
                               RT_DateUpdated = rt.date_updated,
                               OOS = r.out_of_service,
                           };

            var excepts = from r in allrooms where !(r.Arrival >= StayInformation.Departure || r.Departure <= StayInformation.Arrival) select r;

            var rooms = (from r in allrooms
                         where !excepts.Any(exc => exc.RoomID == r.RoomID) || r.ResID == 0 &&
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
                    //StayInformation.Rooms++;
                    //StayInformation.Total += (sender as RoomViewModel).Price;
                    SelectedRooms.Add(sender as RoomViewModel);
                }
                else
                {
                    //StayInformation.Rooms--;
                    //StayInformation.Total -= (sender as RoomViewModel).Price;
                    SelectedRooms.Remove(sender as RoomViewModel);
                }
                OnPropertyChanged(nameof(IsAllRoomsSelected));
            }
        }
        #endregion
    }
}
