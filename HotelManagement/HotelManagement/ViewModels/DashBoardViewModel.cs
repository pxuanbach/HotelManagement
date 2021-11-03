using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;
using HotelManagement.Views;
using HotelManagement.Models;
using HotelManagement.Resources.UC;
using System.Windows.Input;
using System.Windows.Media;

namespace HotelManagement.ViewModels
{
    class DashBoardViewModel : BaseViewModel
    {
        public string Title { get; } = "Dash Board";

        private ObservableCollection<ROOMTYPE> itemRoomTypes = new ObservableCollection<ROOMTYPE>();
        public ObservableCollection<ROOMTYPE> ItemRoomTypes
        {
            get => itemRoomTypes;
            set
            {
                itemRoomTypes = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<RESERVATION> itemReservationsCheckin = new ObservableCollection<RESERVATION>();
        public ObservableCollection<RESERVATION> ItemReservationsCheckin
        {
            get => itemReservationsCheckin;
            set
            {
                itemReservationsCheckin = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<RESERVATION> itemReservationsCheckout = new ObservableCollection<RESERVATION>();
        public ObservableCollection<RESERVATION> ItemReservationsCheckout
        {
            get => itemReservationsCheckout;
            set
            {
                itemReservationsCheckout = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<RESERVATION> itemReserGuest = new ObservableCollection<RESERVATION>();
        public ObservableCollection<RESERVATION> ItemReserGuest
        {
            get => itemReserGuest;
            set
            {
                itemReserGuest = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ROOM> itemROOMsToday = new ObservableCollection<ROOM>();
        public ObservableCollection<ROOM> ItemROOMsToday
        {
            get => itemROOMsToday;
            set
            {
                itemROOMsToday = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ROOM> itemROOMsAvaiToday = new ObservableCollection<ROOM>();
        public ObservableCollection<ROOM> ItemROOMsAvaiToday
        {
            get => itemROOMsAvaiToday;
            set
            {
                itemROOMsAvaiToday = value;
                OnPropertyChanged();
            }
        }

        public DashBoardView DashBoardView { get; set; }
        public ICommand InitRoomStatusCommand { get; set; }
        public ICommand InitTodayStatusCommand { get; set; }

        public DashBoardViewModel()
        {
            LoadRoomType();
            InitRoomStatusCommand = new RelayCommand<DashBoardView>((p) => true, (p) => LoadRoomTypeSide(p));
            InitTodayStatusCommand = new RelayCommand<DashBoardView>((p) => true, (p) => LoadTodayStatus(p));
        }

        private void LoadTodayStatus(DashBoardView dashBoardView)
        {
            LoadRESERVATIONs();
            LoadROOMs();
            this.DashBoardView = dashBoardView;
            this.DashBoardView.txbCheckinAmount.Text = ItemReservationsCheckin.Count().ToString();
            this.DashBoardView.txbCheckoutAmount.Text = ItemReservationsCheckout.Count().ToString();
            this.DashBoardView.txbRoomUsed.Text = ItemROOMsAvaiToday.Count().ToString() + " of " + ItemROOMsToday.Count().ToString();
            this.DashBoardView.txbGuest.Text = ItemReserGuest.Count.ToString();
        }

        private void LoadRESERVATIONs()
        {
            this.ItemReservationsCheckin.Clear();
            this.ItemReservationsCheckout.Clear();
            List<RESERVATION> rESERVATIONsCheckin = GetRESERVATIONsCheckin();
            foreach(var res in rESERVATIONsCheckin)
            {
                ItemReservationsCheckin.Add(res);
            }
            List<RESERVATION> rESERVATIONsCheckout = GetRESERVATIONsCheckout();
            foreach (var res in rESERVATIONsCheckout)
            {
                ItemReservationsCheckout.Add(res);
            }
            List<RESERVATION> rESERVATIONs = GetRESERVATIONs();
            foreach (var res in rESERVATIONs)
            {
                ItemReserGuest.Add(res);
            }
        }

        private void LoadROOMs()
        {
            this.ItemROOMsToday.Clear();
            this.ItemROOMsAvaiToday.Clear();
            List<ROOM> rOOMs = GetROOMs();
            foreach(var res in rOOMs)
            {
                ItemROOMsToday.Add(res);
                List<ROOM_BOOKED> listR = DataProvider.Instance.DB.ROOM_BOOKED.Where(x => x.room_id == res.id).ToList();
                foreach (ROOM_BOOKED booked in listR)
                {
                    RESERVATION reser = DataProvider.Instance.DB.RESERVATIONs.Where(x => x.id == booked.reservation_id).FirstOrDefault();
                    if (reser.arrival <= DateTime.Now && reser.departure >= DateTime.Now)
                    {
                        string status = reser.status;
                        switch (status)
                        {
                            case "Completed":
                            case "Cancelled":
                                if (ItemROOMsAvaiToday.IndexOf(res) == -1)
                                    ItemROOMsAvaiToday.Add(res);
                                break;
                            default:
                                break;
                        }
                        break;
                    }
                    else
                    {
                        if (ItemROOMsAvaiToday.IndexOf(res) == -1)
                            ItemROOMsAvaiToday.Add(res);
                    }
                }
            }
        }

        #region Load Room Type Side
        private void LoadRoomTypeSide(DashBoardView dashBoardView)
        {
            this.DashBoardView = dashBoardView;
            this.DashBoardView.stackRoomStatus.Children.Clear();
            foreach (var ROOMTYPE in ItemRoomTypes)
            {
                UC_RoomStatus uC_RoomStatus = new UC_RoomStatus();
                uC_RoomStatus.expanderRoomStatus.Header = ROOMTYPE.name;
                this.DashBoardView.stackRoomStatus.Children.Add(uC_RoomStatus);
                List<ROOM> rOOMs = ROOMTYPE.ROOMs.Where(x => x.isActive == true).ToList();
                LoadRoom(rOOMs, uC_RoomStatus);
            }
        }
        private void LoadRoom(List<ROOM> rOOMs, UC_RoomStatus uC_RoomStatus)
        {
            uC_RoomStatus.stackRoom.Children.Clear();
            foreach(var item in rOOMs)
            {
                UC_Room room = new UC_Room();
                room.txbName.Text = item.name;
                room.txbType.Text = item.ROOMTYPE.name;
                room.txbID.Text = item.id.ToString();
                room.Width = 100;
                room.Height = 100;
                room.IsEnabled = false;

                List<ROOM_BOOKED> listR = DataProvider.Instance.DB.ROOM_BOOKED.Where(x => x.room_id == item.id).ToList();
                foreach (ROOM_BOOKED booked in listR)
                {
                    RESERVATION reser = DataProvider.Instance.DB.RESERVATIONs.Where(x => x.id == booked.reservation_id).FirstOrDefault();
                    if (reser.arrival <= DateTime.Now && reser.departure >= DateTime.Now)
                    {
                        string status = reser.status;
                        switch (status)
                        {
                            case "Operational":
                                if (reser.departure.Value.Day == DateTime.Now.Day && reser.departure.Value.Month == DateTime.Now.Month && reser.departure.Value.Year == DateTime.Now.Year)
                                {
                                    room.txbStatus.Text = "Due Out";
                                    room.Background = (Brush)new BrushConverter().ConvertFrom("#F70006");
                                }
                                else
                                {
                                    room.txbStatus.Text = "Occupied";
                                    room.Background = (Brush)new BrushConverter().ConvertFrom("#2C9244");
                                }
                                break;
                            case "No Show":
                                room.txbStatus.Text = "No Show";
                                room.Background = (Brush)new BrushConverter().ConvertFrom("#FCDA2D");
                                break;
                            case "Completed":
                            case "Cancelled":
                                room.txbStatus.Text = "Available";
                                room.Background = (Brush)new BrushConverter().ConvertFrom("#76FF03");
                                break;
                            case "Confirmed":
                            case "On Request":
                                room.txbStatus.Text = "Reserved";
                                room.Background = (Brush)new BrushConverter().ConvertFrom("#45DCBD");
                                break;
                            default:
                                break;
                        }
                        break;
                    }
                    else
                    {
                        room.txbStatus.Text = "Available";
                        room.Background = (Brush)new BrushConverter().ConvertFrom("#76FF03");
                    }
                }
                uC_RoomStatus.stackRoom.Children.Add(room);
            }
        }
        private void LoadRoomType()
        {
            this.ItemRoomTypes.Clear();
            List<ROOMTYPE> listROOMTYPEs = GetROOMTYPEs();
            foreach (var ROOMTYPE in listROOMTYPEs)
            {
                ItemRoomTypes.Add(ROOMTYPE);
            }
        }
        #endregion


        private List<ROOMTYPE> GetROOMTYPEs()
        {
            List<ROOMTYPE> res = new List<ROOMTYPE>();
            res = DataProvider.Instance.DB.ROOMTYPEs.Where(x => x.date_updated == null).ToList<ROOMTYPE>();
            return res;
        }

        private List<RESERVATION> GetRESERVATIONsCheckin()
        {
            List<RESERVATION> res = new List<RESERVATION>();
            res = DataProvider.Instance.DB.RESERVATIONs.Where(x => x.arrival == DateTime.Now).ToList();
            return res;
        }
        private List<RESERVATION> GetRESERVATIONsCheckout()
        {
            List<RESERVATION> res = new List<RESERVATION>();
            res = DataProvider.Instance.DB.RESERVATIONs.Where(x => x.departure == DateTime.Now).ToList();
            return res;
        }
        private List<RESERVATION> GetRESERVATIONs()
        {
            List<RESERVATION> res = new List<RESERVATION>();
            res = DataProvider.Instance.DB.RESERVATIONs.Where(x => x.departure >= DateTime.Now).ToList();
            return res;
        }
        private List<ROOM> GetROOMs()
        {
            List<ROOM> res = new List<ROOM>();
            res = DataProvider.Instance.DB.ROOMs.Where(x => x.isActive == true).ToList();
            return res;
        }
    }
}
