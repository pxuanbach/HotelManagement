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

        public string TotalMoney { get; set; }

        public List<FolioDisplayItem> Folio { get; set; }
        #endregion

        #endregion

        public void InitProperties()
        {
            Folio = new List<FolioDisplayItem>();
            //RoomType chưa phù hợp logic sửa thông tin RoomType sau đó lấy giá trị thông tin trong thời gian của phiếu thuê tồn tại
            var reservation = DataProvider.Instance.DB.RESERVATIONs.SingleOrDefault(x => x.id == ReservationId);
            var room = DataProvider.Instance.DB.ROOMs.SingleOrDefault(x => x.id == RoomId);
            var roomType = DataProvider.Instance.DB.ROOMTYPEs.SingleOrDefault(x => x.id == room.roomtype_id);
            var roomBooked = DataProvider.Instance.DB.ROOM_BOOKED.SingleOrDefault(
                x => x.reservation_id == reservation.id && x.room_id == RoomId);
            var folio = DataProvider.Instance.DB.FOLIOs.Where(x => x.room_booked_id == roomBooked.id).ToList();

            long totalMoney = 0;
            foreach (var item in folio)
            {
                var service = DataProvider.Instance.DB.SERVICEs.SingleOrDefault(x => x.id == item.service_id);
                FolioDisplayItem folioDisplayItem = new FolioDisplayItem(
                    service.id, service.name, item.amount.Value, SeparateThousands(Convert.ToInt32(service.price.Value).ToString()));
                Folio.Add(folioDisplayItem);

                //sum total money
                totalMoney = totalMoney + (int)service.price.Value * item.amount.Value;
            }

            FolioCount = folio.Count();
            Arrival = reservation.arrival.Value;
            Departure = reservation.departure.Value;
            RoomName = room.name;
            Notes = room.notes;
            RoomType = roomType.name;
            Price = SeparateThousands(Convert.ToInt32(roomType.price.Value).ToString());
            MaxGuest = roomType.max_guest.Value;
            TotalMoney = SeparateThousands(totalMoney.ToString());
        }
    }
}
