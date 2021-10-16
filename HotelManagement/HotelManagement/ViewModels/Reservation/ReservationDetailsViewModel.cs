using HotelManagement.Models;
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
        public GuestViewModel GuestInformation { get; set; }

        public ReservationViewModel StayInformation { get; set; }

        public ObservableCollection<RoomViewModel> BookedRooms { get; set; }

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
                return _removeSharerCommand ?? (_removeSharerCommand = new RelayCommand<GuestViewModel>((p) => true, (p) => RemoveSelectedSharer(p)));
            }
        }

        private ICommand _removeRoomCommand;
        public ICommand RemoveRoomCommand
        {
            get
            {
                return _removeRoomCommand ?? (_removeRoomCommand = new RelayCommand<RoomViewModel>((p) => true, (p) => RemoveSelectedRoom(p)));
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

        public ReservationDetailsViewModel(int ResID)
        {
            Sharers = new ObservableCollection<GuestViewModel>();
            BookedRooms = new ObservableCollection<RoomViewModel>();

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
            Sharers.Remove(sharer);
            if (sharer.ID == GuestInformation.ID) BeASharer = false;
        }

        public void RemoveSelectedRoom(RoomViewModel room)
        {
            BookedRooms.Remove(room);
        }

        void LoadReservationDetails(int ResID)
        {
            var db = new HotelManagementEntities();

            var reservation = (from res in db.RESERVATIONs where res.id == ResID select res).First();

            var mainGuest = (from g in db.GUESTs where reservation.main_guest == g.id select g).First();

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
                Arrival = (DateTime)reservation.arrival,
                Departure = (DateTime)reservation.departure,
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
    }
}
