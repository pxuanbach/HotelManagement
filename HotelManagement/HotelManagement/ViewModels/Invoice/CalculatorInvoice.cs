using HotelManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.ViewModels
{
    class CalculatorInvoice
    {
        public static int GetExactRoomPriceOfReservation(List<ROOMTYPE> roomTypeList, DateTime dateCreated)
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

        public static int CalculateFolioTotalOfRoom(FOLIO[] folio)
        {
            int sum = 0;
            for (int i = 0; i < folio.Length; i++)
            {
                sum = sum + (int)folio[i].SERVICE.price * folio[i].amount.Value;
            }
            return sum;
        }

        public static int SumRoomPrice(RESERVATION reservation)
        {
            int sum = 0;
            foreach (var roomBooked in reservation.ROOM_BOOKED)
            {
                //List room type with the same name
                var roomTypeList = DataProvider.Instance.DB.ROOMTYPEs.Where(x => x.name == roomBooked.ROOM.ROOMTYPE.name).ToList();
                int exactRoomPrice = GetExactRoomPriceOfReservation(roomTypeList, reservation.date_created.Value);
                sum = sum + exactRoomPrice;
            }
            return sum;
        }
    }
}
