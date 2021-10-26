using HotelManagement.Models;
using HotelManagement.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HotelManagement.ViewModels
{
    class InvoiceViewModel : BaseViewModel
    {
        #region Properties

        #region Search Bar
        private string _contentSearch;
        public string ContentSearch
        {
            get { return _contentSearch; }
            set
            {
                _contentSearch = value;
                OnPropertyChanged();
                if (ContentSearch == "")
                    LoadReservations();
            }
        }

        private List<string> _searchTypes;
        public List<string> SearchTypes { get { return _searchTypes; } set { _searchTypes = value; OnPropertyChanged(); } }

        private string _selectedSearchType;
        public string SelectedSearchType 
        { 
            get { return _selectedSearchType; } 
            set { _selectedSearchType = value; OnPropertyChanged(); } 
        }
        #endregion

        #region Invoice Details
        public int ReservationIdSelected { get; set; }

        private string _arrival;
        public string Arrival { get { return _arrival; } set { _arrival = value; OnPropertyChanged(); } }

        private string _departure;
        public string Departure { get { return _departure; } set { _departure = value; OnPropertyChanged(); } }

        #region Main Guest
        private string _identity;
        public string Identity { get { return _identity; } set { _identity = value; OnPropertyChanged(); } }

        private string _name;
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged(); } }

        private string _phone;
        public string Phone { get { return _phone; } set { _phone = value; OnPropertyChanged(); } }

        private string _email;
        public string Email { get { return _email; } set { _email = value; OnPropertyChanged(); } }
        #endregion

        #region Guests
        private int _guestCount;
        public int GuestCount { get { return _guestCount; } set { _guestCount = value; OnPropertyChanged(); } }

        private ObservableCollection<GUEST> _guests;
        public ObservableCollection<GUEST> Guests
        {
            get { return _guests; }
            set { _guests = value; OnPropertyChanged(); }
        }
        #endregion

        #region Rooms
        private int _roomCount;
        public int RoomCount { get { return _roomCount; } set { _roomCount = value; OnPropertyChanged(); } }

        private ObservableCollection<RoomDisplayItem> _rooms;
        public ObservableCollection<RoomDisplayItem> Rooms
        {
            get { return _rooms; }
            set { _rooms = value; OnPropertyChanged(); }
        }

        private string _roomsTotalMoney;
        public string RoomsTotalMoney { get { return _roomsTotalMoney; } set { _roomsTotalMoney = value; OnPropertyChanged(); } }
        #endregion

        #region Folio
        private int _folioCount;
        public int FolioCount { get { return _folioCount; } set { _folioCount = value; OnPropertyChanged(); } }

        private ObservableCollection<FolioDisplayItem> _folio;
        public ObservableCollection<FolioDisplayItem> Folio
        {
            get { return _folio; }
            set { _folio = value; OnPropertyChanged(); }
        }

        private string _folioTotalMoney;
        public string FolioTotalMoney { get { return _folioTotalMoney; } set { _folioTotalMoney = value; OnPropertyChanged(); } }
        #endregion

        #region Fees + Total Money
        private double _earlyCheckinFee;
        public double EarlyCheckinFee { get { return _earlyCheckinFee; } set { _earlyCheckinFee = value; OnPropertyChanged(); } }

        private double _lateCheckoutFee;
        public double LateCheckoutFee { get { return _lateCheckoutFee; } set { _lateCheckoutFee = value; OnPropertyChanged(); } }

        private double _surcharge;
        public double Surcharge { get { return _surcharge; } set { _surcharge = value; OnPropertyChanged(); } }

        public long TotalMoneyNumber { get; set; }

        private string _totalMoney;
        public string TotalMoney { get { return _totalMoney; } set { _totalMoney = value; OnPropertyChanged(); } }
        #endregion

        #endregion

        #region Dialog Properties
        private bool _isOpenDialog;
        public bool IsOpenDialog
        {
            get { return _isOpenDialog; }
            set { _isOpenDialog = value; OnPropertyChanged(); }
        }

        private string _errorMessageDialog;
        public string ErrorMessageDialog 
        { 
            get { return _errorMessageDialog; } 
            set { _errorMessageDialog = value; OnPropertyChanged(); } 
        }

        private double _surchargeDialog;
        public double SurchargeDialog { get { return _surchargeDialog; } set { _surchargeDialog = value; OnPropertyChanged(); } }

        private double _earlyCheckinFeeDialog;
        public double EarlyCheckinFeeDialog 
        { 
            get { return _earlyCheckinFeeDialog; } 
            set { _earlyCheckinFeeDialog = value; OnPropertyChanged(); } 
        }

        private double _lateCheckoutFeeDialog;
        public double LateCheckoutFeeDialog
        {
            get { return _lateCheckoutFeeDialog; }
            set { _lateCheckoutFeeDialog = value; OnPropertyChanged(); }
        }
        #endregion

        public string StatusSelected { get; set; }

        private ObservableCollection<RESERVATION> _reservations;
        public ObservableCollection<RESERVATION> Reservations 
        { 
            get { return _reservations; } 
            set { _reservations = value; OnPropertyChanged(); } 
        }
        #endregion

        #region Command
        public ICommand SearchReservationCommand { get; set; }
        public ICommand OperationalCommnad { get; set; }
        public ICommand CompletedCommnad { get; set; }
        public ICommand ListviewSelectionChangedCommand { get; set; }
        public ICommand ClearDetailCommand { get; set; }
        public ICommand FolioOfRoomCommand { get; set; }
        public ICommand ShowFeesCommand { get; set; }
        public ICommand SaveFeesCommand { get; set; }
        public ICommand CheckOutCommand { get; set; }
        public ICommand ExportCommand { get; set; }
        #endregion

        public InvoiceViewModel()
        {
            InitProperties();
            InitDialogProperties();

            SearchReservationCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                Search();
            });

            OperationalCommnad = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                if (Reservations.Count > 0)
                    Reservations.Clear();
                ClearDetailProperties();
                StatusSelected = "Operational";
                LoadReservations();
            });

            CompletedCommnad = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                if (Reservations.Count > 0)
                    Reservations.Clear();
                ClearDetailProperties();
                StatusSelected = "Completed";
                LoadReservations();
            });

            ListviewSelectionChangedCommand = new RelayCommand<ListView>((p) =>
            {
                return !p.Items.IsEmpty;
            }, (p) =>
            {
                ClearDetailProperties();
                LoadItemSelected((RESERVATION)p.SelectedItem);
            });

            ClearDetailCommand = new RelayCommand<ListView>((p) =>
            {
                return true;
            }, (p) =>
            {
                ClearDetailProperties();
            });

            FolioOfRoomCommand = new RelayCommand<RoomDisplayItem>((p) =>
            {
                return true;
            }, (p) =>
            {
                OpenFolioOfRoomWindow(p);
            });

            ShowFeesCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                IsOpenDialog = true;
                InitDialogProperties();
            });

            SaveFeesCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                var charge = DataProvider.Instance.DB.CHARGES.First();
                charge.surcharge = SurchargeDialog;
                charge.early_checkin_fee = EarlyCheckinFeeDialog;
                charge.late_checkout_fee = LateCheckoutFeeDialog;
                DataProvider.Instance.DB.SaveChanges();

                IsOpenDialog = false;
            });

            CheckOutCommand = new RelayCommand<ListView>((p) =>
            {
                if (StatusSelected != "Operational")
                {
                    return false; 
                }    
                if (p != null)
                {
                    if (p.SelectedIndex == -1)
                        return false;
                }
                return true;
            }, (p) =>
            {
                ClearDetailProperties();
                CheckOut((RESERVATION)p.SelectedItem);
                LoadReservations();
            });

            ExportCommand = new RelayCommand<ListView>((p) =>
            {
                if (p != null)
                {
                    if (p.SelectedIndex == -1)
                        return false;
                }
                return true;
            }, (p) =>
            {
                string path = @"invoice.pdf";
                ExportInvoice export = new ExportInvoice();
                export.Export(path, (RESERVATION)p.SelectedItem);
            });
        }

        void InitProperties()
        {
            Guests = new ObservableCollection<GUEST>();
            Rooms = new ObservableCollection<RoomDisplayItem>();
            Folio = new ObservableCollection<FolioDisplayItem>();
            SearchTypes = new List<string>();
            SearchTypes.Add("ID");
            SearchTypes.Add("Main Guest");

            StatusSelected = "Operational";
            LoadReservations();
            SelectedSearchType = "ID";
        }

        void InitDialogProperties()
        {
            var charge = DataProvider.Instance.DB.CHARGES.First();
            ErrorMessageDialog = "";
            SurchargeDialog = charge.surcharge.Value;
            EarlyCheckinFeeDialog = charge.early_checkin_fee.Value;
            LateCheckoutFeeDialog = charge.late_checkout_fee.Value;
        }

        void ClearDetailProperties()
        {
            ReservationIdSelected = 0;
            Arrival = "";
            Departure = "";

            //Main Guest
            Identity = "";
            Name = "";
            Phone = "";
            Email = "";

            //Guests
            GuestCount = 0;
            if (Guests.Count > 0)
                Guests.Clear();

            //Rooms
            RoomCount = 0;
            RoomsTotalMoney = "";
            if (Rooms.Count > 0)
                Rooms.Clear();

            //Folio
            FolioCount = 0;
            FolioTotalMoney = "";
            if (Folio.Count > 0)
                Folio.Clear();

            //Fees + Total Money
            EarlyCheckinFee = 0;
            LateCheckoutFee = 0;
            Surcharge = 0;
            TotalMoneyNumber = 0;
            TotalMoney = "";
        }

        #region Load Function
        void LoadReservations()
        {
            Reservations = new ObservableCollection<RESERVATION>(
                DataProvider.Instance.DB.RESERVATIONs.Where(x => x.status == StatusSelected));

            if (StatusSelected == "Operational")
            {
                foreach (var reservation in Reservations)
                {
                    if ((reservation.departure.Value - DateTime.Now).TotalDays < 0)
                    {
                        reservation.late_checkout = true;
                        DataProvider.Instance.DB.SaveChanges();
                    }    
                }    
            }    
        }

        void LoadItemSelected(RESERVATION p)
        {
            long sumRoomPrice = 0;
            long folioTotalMoney = 0;
            var mainGuest = DataProvider.Instance.DB.GUESTs.Where(x => x.id == p.main_guest).SingleOrDefault();
            List<GUEST_BOOKING> guestBookingList = 
                DataProvider.Instance.DB.GUEST_BOOKING.Where(x => x.reservation_id == p.id).ToList();
            List<ROOM_BOOKED> roomBookedList =
                DataProvider.Instance.DB.ROOM_BOOKED.Where(x => x.reservation_id == p.id).ToList();

            //Load Guests
            foreach (GUEST_BOOKING obj in guestBookingList)
            {
                var guest = DataProvider.Instance.DB.GUESTs.SingleOrDefault(x => x.id == obj.guest_id);
                Guests.Add(guest);
            }

            //Load Rooms + Folio
            foreach (ROOM_BOOKED obj in roomBookedList)
            {
                //Rooms
                var room = DataProvider.Instance.DB.ROOMs.SingleOrDefault(x => x.id == obj.room_id);
                var roomType = DataProvider.Instance.DB.ROOMTYPEs.SingleOrDefault(x => x.id == room.roomtype_id);

                //List room type with the same name
                var roomTypeList = DataProvider.Instance.DB.ROOMTYPEs.Where(x => x.name == roomType.name).ToList();
                int exactRoomPrice = GetExactRoomPriceOfReservation(roomTypeList, p.date_created.Value);

                RoomDisplayItem roomDisplayItem = new RoomDisplayItem(
                    room.id, room.name, roomType.name);
                Rooms.Add(roomDisplayItem);

                sumRoomPrice = sumRoomPrice + exactRoomPrice;

                //Folio
                List<FOLIO> folio = DataProvider.Instance.DB.FOLIOs.Where(x => x.room_booked_id == obj.id).ToList();
                foreach (FOLIO item in folio)
                {
                    var service = DataProvider.Instance.DB.SERVICEs.SingleOrDefault(x => x.id == item.service_id);
                    var folioItem = Folio.FirstOrDefault(x => x.Id == service.id);

                    if (folioItem == null)
                    {
                        FolioDisplayItem folioDisplayItem = new FolioDisplayItem(service.id, service.name, item.amount.Value);
                        Folio.Add(folioDisplayItem);

                        folioTotalMoney = folioTotalMoney + (long)service.price.Value * item.amount.Value;
                    }
                    else
                    {
                        folioItem.Amount += (int)item.amount;

                        folioTotalMoney = folioTotalMoney + (int)service.price.Value * item.amount.Value;
                    }
                }
            }

            ReservationIdSelected = p.id;
            Arrival = p.arrival.Value.ToString("dd/MM/yyyy");
            Departure = p.departure.Value.ToString("dd/MM/yyyy");
            Identity = p.main_guest;
            Name = mainGuest.name;
            Phone = mainGuest.phone;
            Email = mainGuest.email;
            GuestCount = guestBookingList.Count();
            RoomCount = roomBookedList.Count();
            FolioCount = Folio.Count();

            //Calculate money
            long roomsTotalMoney = CalculateRoomsTotalMoney(sumRoomPrice, p.arrival.Value, p.departure.Value);
            RoomsTotalMoney = SeparateThousands(roomsTotalMoney.ToString());
            FolioTotalMoney = SeparateThousands(folioTotalMoney.ToString());

            LoadFeeByStatus(p);

            TotalMoneyNumber = CalculateTotalMoney(sumRoomPrice, roomsTotalMoney, folioTotalMoney);
            TotalMoney = SeparateThousands(TotalMoneyNumber.ToString());
        }

        void LoadFeeByStatus(RESERVATION p)
        {
            if (StatusSelected == "Operational")
            {
                if (p.early_checkin.Value)
                {
                    EarlyCheckinFee = DataProvider.Instance.DB.CHARGES.First().early_checkin_fee.Value;
                }
                if (p.late_checkout.Value)
                {
                    LateCheckoutFee = DataProvider.Instance.DB.CHARGES.First().late_checkout_fee.Value;
                }  
                
                Surcharge = DataProvider.Instance.DB.CHARGES.First().surcharge.Value;
            }
            if (StatusSelected == "Completed")
            {
                var invoice = DataProvider.Instance.DB.INVOICEs.SingleOrDefault(x => x.reservation_id == p.id);

                EarlyCheckinFee = invoice.early_checkin_fee.Value;
                LateCheckoutFee = invoice.late_checkout_fee.Value;
                Surcharge = invoice.surcharge.Value;
            }
        }
        #endregion
        
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

        long CalculateRoomsTotalMoney(long sumRoomPrice, DateTime arrival, DateTime departure)
        {
            return sumRoomPrice * (int)(departure - arrival).TotalDays;
        }

        long CalculateTotalMoney(long sumRoomPrice, long roomsTotalMoney, long folioTotalMoney)
        {
            double fee = sumRoomPrice * (EarlyCheckinFee + LateCheckoutFee)/100;
            double totalMoney = (roomsTotalMoney + folioTotalMoney) * (100 + Surcharge)/100;
            return (long)(totalMoney + fee);
        }

        void Search()
        {
            switch (SelectedSearchType)
            {
                case "ID":
                    Reservations = new ObservableCollection<RESERVATION>(
                        DataProvider.Instance.DB.RESERVATIONs.Where(
                            x => x.id.ToString().Contains(ContentSearch) && x.status == StatusSelected));
                    break;
                case "Main Guest":
                    Reservations = new ObservableCollection<RESERVATION>(
                        DataProvider.Instance.DB.RESERVATIONs.Where(
                            x => x.main_guest.ToString().Contains(ContentSearch) && x.status == StatusSelected));
                    break;
                default:
                    break;
            }
        }

        void OpenFolioOfRoomWindow(RoomDisplayItem room)
        {
            FolioOfRoomWindow wd = new FolioOfRoomWindow();
            FolioOfRoomViewModel vm = new FolioOfRoomViewModel();
            vm.ReservationId = ReservationIdSelected;
            vm.RoomId = room.Id;
            vm.InitProperties();
            wd.DataContext = vm;
            wd.ShowDialog();
        }

        void CheckOut(RESERVATION p)
        {
            p.status = "Completed";
            INVOICE invoice = new INVOICE()
            {
                reservation_id = p.id,
                total_money = Convert.ToDecimal(TotalMoneyNumber),
                surcharge = Surcharge,
                early_checkin_fee = EarlyCheckinFee,
                late_checkout_fee = LateCheckoutFee,
            };
            DataProvider.Instance.DB.INVOICEs.Add(invoice);
            DataProvider.Instance.DB.SaveChanges();
        }
    }
}
