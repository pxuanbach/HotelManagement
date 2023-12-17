﻿using HotelManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.ViewModels
{
    class FolioOfRoomViewModel : BaseViewModel
    {
        #region Properties

        #region Reservation
        public int ReservationId { get; set; }

        public DateTime Arrival { get; set; }

        public DateTime Departure { get; set; }
        #endregion

        #region Room
        public int RoomId { get; set; }

        public string RoomName { get; set; }

        public string RoomType { get; set; }

        public string Price { get; set; }

        public int MaxGuest { get; set; }

        public string Notes { get; set; }
        #endregion

        #region Guests
        public int GuestCount { get; set; }

        public List<GUEST> Guests { get; set; }
        #endregion

        #region Folio
        public int FolioCount { get; set; }

        public List<FolioDisplayItem> Folio { get; set; }
        #endregion

        public double OverCapacityFee { get; set; }

        public string OverCapacityFeeMoney { get; set; }

        public string RoomTotalMoney { get; set; }

        public string FolioTotalMoney { get; set; }

        public string TotalMoney { get; set; }

        #endregion

        public void InitProperties()
        {
            Guests = new List<GUEST>();
            Folio = new List<FolioDisplayItem>();
            var reservation = DataProvider.Instance.DB.RESERVATIONs.SingleOrDefault(x => x.id == ReservationId);
            var roomBooked = DataProvider.Instance.DB.ROOM_BOOKED.SingleOrDefault(
                x => x.reservation_id == reservation.id && x.room_id == RoomId);
            var folio = DataProvider.Instance.DB.FOLIOs.Where(x => x.room_booked_id == roomBooked.id).ToList();
            int exactRoomPrice = CalculatorInvoice.ExactRoomPrice(RoomId, reservation.date_created.Value);
            int exactCapacity = CalculatorInvoice.ExactCapacity(RoomId, reservation.date_created.Value);
            long roomTotalMoney = CalculatorInvoice.RoomTotalMoney(RoomId, reservation);
            long folioTotalMoney = CalculatorInvoice.FolioTotalOfRoom(folio.ToArray());

            if (CalculatorInvoice.IsRoomOverCapacity(roomBooked))
                OverCapacityFee = CalculatorInvoice.LoadOverCapacityFee(reservation);
            long overCapacityFeeMoney = (long)(OverCapacityFee/100 * roomTotalMoney);

            foreach (var item in roomBooked.GUEST_BOOKING)
            {
                Guests.Add(item.GUEST);
            }    

            foreach (var item in folio)
            {
                var service = DataProvider.Instance.DB.SERVICEs.SingleOrDefault(x => x.id == item.service_id);
                FolioDisplayItem folioDisplayItem = new FolioDisplayItem(
                    service.id, service.name, item.amount.Value, SeparateThousands(((int)service.price).ToString()));

                var itemExist = Folio.FirstOrDefault(x => x.Id == folioDisplayItem.Id);

                if (itemExist == null)
                {
                    Folio.Add(folioDisplayItem);
                }    
                else
                {
                    itemExist.Amount += folioDisplayItem.Amount;
                }    
                
            }

            FolioCount = folio.Count();
            GuestCount = roomBooked.GUEST_BOOKING.Count();
            Arrival = reservation.arrival.Value;
            Departure = reservation.departure.Value;
            RoomName = roomBooked.ROOM.name;
            Notes = roomBooked.ROOM.notes;
            RoomType = roomBooked.ROOM.ROOMTYPE.name;
            Price = SeparateThousands(exactRoomPrice.ToString());
            MaxGuest = exactCapacity;

            RoomTotalMoney = SeparateThousands(roomTotalMoney.ToString());
            FolioTotalMoney = SeparateThousands(folioTotalMoney.ToString());
            OverCapacityFeeMoney = SeparateThousands(overCapacityFeeMoney.ToString());
            TotalMoney = SeparateThousands((roomTotalMoney + folioTotalMoney + overCapacityFeeMoney).ToString());
        }
    }
}
