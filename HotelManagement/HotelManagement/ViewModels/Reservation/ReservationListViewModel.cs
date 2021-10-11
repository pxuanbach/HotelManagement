using HotelManagement.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace HotelManagement.ViewModels
{
    class ReservationListViewModel : BaseViewModel
    {
        public ObservableCollection<ReservationItemViewModel> Reservations { get; set; }
        public ReservationListViewModel()
        {
            Reservations = new ObservableCollection<ReservationItemViewModel>();
            LoadAllReservations();

            foreach (var model in Reservations)
            {
                model.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(ReservationItemViewModel.IsSelected))
                        OnPropertyChanged(nameof(IsAllReservationsSelected));
                };
            }
        }

        public bool? IsAllReservationsSelected
        {
            get
            {
                var selected = Reservations.Select(item => item.IsSelected).Distinct().ToList();
                return selected.Count == 1 ? selected.Single() : (bool?)null;
            }
            set
            {
                if (value.HasValue)
                {
                    SelectAll(value.Value, Reservations);
                    OnPropertyChanged();
                }
            }
        }

        private static void SelectAll(bool select, IEnumerable<ReservationItemViewModel> models)
        {
            foreach (var model in models)
            {
                model.IsSelected = select;
            }
        }

        private void LoadAllReservations()
        {
            if (Reservations.Count > 0) Reservations.Clear();

            var db = new HotelManagementEntities();

            var reservations = (from res in db.RESERVATIONs 
                                join rb in db.ROOM_BOOKED on res.id equals rb.reservation_id
                                join r in db.ROOMs on rb.room_id equals r.id
                                join rt in db.ROOMTYPEs on r.roomtype_id equals rt.id
                                where res.date_created >= rt.date_created && (rt.date_updated == null || res.date_created <= rt.date_updated)
                                group res by res.id into g
                                select g).ToList();

            foreach (var r in reservations)
            {
                var res = r.First();
                GUEST mainGuest = (from g in db.GUESTs where res.main_guest.Equals(g.id) select g).ToList()[0];

                var obj = new ReservationItemViewModel()
                {
                    ID = res.id,
                    Status = res.status,
                    Rooms = res.ROOM_BOOKED.Count,
                    Guest = mainGuest.name,
                    DateCreated = (res.date_created.HasValue) ? DateTime.Now : (DateTime)res.date_created,
                    Arrival = (res.arrival.HasValue) ? DateTime.Now : (DateTime)res.arrival,
                    Departure = (res.departure.HasValue) ? DateTime.Now : (DateTime)res.departure,
                    Pax = res.GUEST_BOOKING.Count
                };

                Reservations.Add(obj);
            }
        }
    }

    class ReservationItemViewModel : BaseViewModel
    {
        private bool _isSelected;
        private int _id;
        private string _status;
        private string _guest;
        private DateTime _date_created;
        private DateTime _arrival;
        private DateTime _departure;
        private int _rooms;
        private int _pax;
        private decimal _total;

        public bool IsSelected { get { return _isSelected; } set { _isSelected = value; OnPropertyChanged(); } }

        public int ID { get { return _id; } set { _id = value; OnPropertyChanged(); } }

        public string Status { get { return _status; } set { _status = value; OnPropertyChanged(); } }

        public int Rooms { get { return _rooms; } set { _rooms = value; OnPropertyChanged(); } }

        public string Guest { get { return _guest; } set { _guest = value; OnPropertyChanged(); } }

        public DateTime DateCreated { get { return _date_created; } set { _date_created = value; OnPropertyChanged(); } }

        public DateTime Arrival { get { return _arrival; } set { _arrival = value; OnPropertyChanged("Arrival"); } }

        public DateTime Departure { get { return _departure; } set { _departure = value; OnPropertyChanged(); } }

        public int Pax { get { return _pax; } set { _pax = value; OnPropertyChanged(); } }

        public decimal Total { get { return _total; } set { _total = value; OnPropertyChanged(); } }
    }
}
