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
        public ObservableCollection<ReservationViewModel> Reservations { get; set; }
        public ReservationListViewModel()
        {
            Reservations = new ObservableCollection<ReservationViewModel>();
            LoadAllReservations();

            foreach (var model in Reservations)
            {
                model.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(ReservationViewModel.IsSelected))
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

        private static void SelectAll(bool select, IEnumerable<ReservationViewModel> models)
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

                var obj = new ReservationViewModel()
                {
                    Status = res.status,
                    Rooms = res.ROOM_BOOKED.Count,
                    Guest = mainGuest.name,
                    Arrival = (res.arrival == null) ? DateTime.Now.ToString("dd/MM/yyyy") : ((DateTime)res.arrival).ToString("dd/MM/yyyy"),
                    Departure = (res.departure == null) ? DateTime.Now.ToString("dd/MM/yyyy") : ((DateTime)res.departure).ToString("dd/MM/yyyy"),
                    Pax = res.GUEST_BOOKING.Count
                };

                Reservations.Add(obj);
            }
        }
    }
}
