using HotelManagement.Models;
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

        #region Folio
        public int FolioCount { get; set; }

        public List<FolioDisplayItem> Folio { get; set; }
        #endregion

        public string RoomTotalMoney { get; set; }

        public string FolioTotalMoney { get; set; }

        public string TotalMoney { get; set; }

        #endregion

        public void InitProperties()
        {
            Folio = new List<FolioDisplayItem>();
            var reservation = DataProvider.Instance.DB.RESERVATIONs.SingleOrDefault(x => x.id == ReservationId);
            var room = DataProvider.Instance.DB.ROOMs.SingleOrDefault(x => x.id == RoomId);
            var roomType = DataProvider.Instance.DB.ROOMTYPEs.SingleOrDefault(x => x.id == room.roomtype_id);
            var roomBooked = DataProvider.Instance.DB.ROOM_BOOKED.SingleOrDefault(
                x => x.reservation_id == reservation.id && x.room_id == RoomId);
            var folio = DataProvider.Instance.DB.FOLIOs.Where(x => x.room_booked_id == roomBooked.id).ToList();

            //List room type with the same name
            var roomTypeList = DataProvider.Instance.DB.ROOMTYPEs.Where(x => x.name == roomType.name).ToList();

            long folioTotalMoney = 0;
            foreach (var item in folio)
            {
                var service = DataProvider.Instance.DB.SERVICEs.SingleOrDefault(x => x.id == item.service_id);
                FolioDisplayItem folioDisplayItem = new FolioDisplayItem(
                    service.id, service.name, item.amount.Value, SeparateThousands(Convert.ToInt32(service.price.Value).ToString()));
                Folio.Add(folioDisplayItem);

                //sum folio total money
                folioTotalMoney = folioTotalMoney + (int)service.price.Value * item.amount.Value;
            }

            FolioCount = folio.Count();
            Arrival = reservation.arrival.Value;
            Departure = reservation.departure.Value;
            RoomName = room.name;
            Notes = room.notes;
            RoomType = roomType.name;
            Price = SeparateThousands(GetExactRoomPriceOfReservation(roomTypeList, reservation.date_created.Value).ToString());
            MaxGuest = roomType.max_guest.Value;
            long roomTotalMoney = (long)roomType.price.Value * (int)(Departure - Arrival).TotalDays;
            RoomTotalMoney = SeparateThousands(roomTotalMoney.ToString());
            FolioTotalMoney = SeparateThousands(folioTotalMoney.ToString());
            TotalMoney = SeparateThousands((roomTotalMoney + folioTotalMoney).ToString());
        }

        int GetExactRoomPriceOfReservation(List<ROOMTYPE> roomTypeList, DateTime dateCreated)
        {
            List<ROOMTYPE> sortList = roomTypeList.OrderBy(x => x.id).ToList();
            foreach (var item in sortList)
            {
                if (item.date_created <= dateCreated)
                {
                    if (item.date_updated.HasValue)
                    {
                        if (dateCreated <= item.date_updated)
                        {
                            return (int)item.price;
                        }
                    }
                    else
                    {
                        return (int)item.price;
                    }
                }
            }
            return 0;
        }
    }
}
