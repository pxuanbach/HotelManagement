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
        #region Room
        public static int ExactRoomPrice(List<ROOMTYPE> roomTypeList, DateTime dateCreated)
        {
            //in ascending order according to a key
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
                else
                {
                    break;
                }
            }
            return 0;
        }

        public static int ExactRoomPrice(int roomId, DateTime dateCreated)
        {
            var room = DataProvider.Instance.DB.ROOMs.SingleOrDefault(x => x.id == roomId);

            List<ROOMTYPE> roomTypeList = 
                DataProvider.Instance.DB.ROOMTYPEs.Where(x => x.name == room.ROOMTYPE.name).ToList();

            //in ascending order according to a key
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
                else
                {
                    break;
                }
            }
            return 0;
        }

        public static int ExactCapacity(int roomId, DateTime dateCreated)
        {
            var room = DataProvider.Instance.DB.ROOMs.SingleOrDefault(x => x.id == roomId);

            List<ROOMTYPE> roomTypeList =
                DataProvider.Instance.DB.ROOMTYPEs.Where(x => x.name == room.ROOMTYPE.name).ToList();

            //in ascending order according to a key
            List<ROOMTYPE> sortList = roomTypeList.OrderBy(x => x.id).ToList();
            foreach (var item in sortList)
            {
                if (item.date_created <= dateCreated)
                {
                    if (item.date_updated.HasValue)
                    {
                        if (dateCreated <= item.date_updated)
                        {
                            return item.max_guest.Value;
                        }
                    }
                    else
                    {
                        return item.max_guest.Value;
                    }
                }
                else
                {
                    break;
                }    
            }
            return 0;
        }

        public static int TotalRoomPriceOfReservation(RESERVATION reservation)
        {
            int sum = 0;
            foreach (var roomBooked in reservation.ROOM_BOOKED)
            {
                //List room type with the same name
                var roomTypeList = DataProvider.Instance.DB.ROOMTYPEs.Where(x => x.name == roomBooked.ROOM.ROOMTYPE.name).ToList();
                int exactRoomPrice = ExactRoomPrice(roomTypeList, reservation.date_created.Value);
                sum = sum + exactRoomPrice;
            }
            return sum;
        }

        public static int TotalNumOfDays(RESERVATION reservation)
        {
            return (int)(reservation.departure.Value - reservation.arrival.Value).TotalDays;
        }

        public static long RoomTotalMoney(int roomId, RESERVATION reservation)
        {
            var room = DataProvider.Instance.DB.ROOMs.SingleOrDefault(x => x.id == roomId);

            var roomTypeList = DataProvider.Instance.DB.ROOMTYPEs.Where(x => x.name == room.ROOMTYPE.name).ToList();
            int exactRoomPrice = ExactRoomPrice(roomTypeList, reservation.date_created.Value);
            
            return exactRoomPrice * TotalNumOfDays(reservation);
        }

        public static long RoomsTotalMoney(RESERVATION reservation)
        {
            return TotalRoomPriceOfReservation(reservation) * TotalNumOfDays(reservation);
        }
        #endregion

        #region Folio
        public static int FolioTotalOfRoom(FOLIO[] folio)
        {
            int sum = 0;
            for (int i = 0; i < folio.Length; i++)
            {
                sum = sum + (int)folio[i].SERVICE.price * folio[i].amount.Value;
            }
            return sum;
        }

        public static long FolioTotalMoney(RESERVATION reservation)
        {
            long sum = 0;
            foreach(var obj in reservation.ROOM_BOOKED)
            {
                sum = sum + FolioTotalOfRoom(obj.FOLIOs.ToArray());
            }
            return sum;
        }
        #endregion

        public static long TotalMoneyNoFee(RESERVATION reservation)
        {
            return RoomsTotalMoney(reservation) + FolioTotalMoney(reservation);
        }

        public static long TotalMoneyWithFee(RESERVATION reservation)
        {
            var invoice = DataProvider.Instance.DB.INVOICEs.SingleOrDefault(x => x.reservation_id == reservation.id);
            double fee = 0;
            double earlyCheckinFee = 0;
            double lateCheckoutFee = 0;
            double surcharge = 0;
            double totalMoney = 0;

            if (invoice != null)
            {
                return (long)invoice.total_money;
            }
            else
            {
                if (reservation.early_checkin.Value)
                {
                    earlyCheckinFee = DataProvider.Instance.DB.CHARGES.First().early_checkin_fee.Value;
                }
                if (reservation.late_checkout.Value)
                {
                    lateCheckoutFee = DataProvider.Instance.DB.CHARGES.First().late_checkout_fee.Value;
                }
                surcharge = DataProvider.Instance.DB.CHARGES.First().surcharge.Value;
            }

            fee = TotalRoomPriceOfReservation(reservation) * (earlyCheckinFee + lateCheckoutFee) / 100;
            totalMoney = (RoomsTotalMoney(reservation) + FolioTotalMoney(reservation)) * (100 + surcharge) / 100;
            return (long)(totalMoney + fee);
        }
    }
}
