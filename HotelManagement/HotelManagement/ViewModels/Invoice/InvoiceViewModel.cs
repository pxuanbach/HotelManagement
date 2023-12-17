﻿using HotelManagement.Models;
using HotelManagement.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Globalization;
using System.IO;
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

        private bool _isSearchDateCreated;
        public bool IsSearchDateCreated { get { return _isSearchDateCreated; } set { _isSearchDateCreated = value; OnPropertyChanged(); } }

        private DateTime _dateCreatedSearch;
        public DateTime DateCreatedSearch { get { return _dateCreatedSearch; } set { _dateCreatedSearch = value; OnPropertyChanged(); } }

        private bool _isSearchArrDep;
        public bool IsSearchArrDep { get { return _isSearchArrDep; } set { _isSearchArrDep = value; OnPropertyChanged(); } }

        private DateTime _arrivalSearch;
        public DateTime ArrivalSearch { get { return _arrivalSearch; } set { _arrivalSearch = value; OnPropertyChanged(); } }

        private DateTime _departureSearch;
        public DateTime DepartureSearch { get { return _departureSearch; } set { _departureSearch = value; OnPropertyChanged(); } }
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
        private double _overCapacityFee;
        public double OverCapacityFee { get { return _overCapacityFee; } set { _overCapacityFee = value; OnPropertyChanged(); } }

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

        private double _overCapacityFeeDialog;
        public double OverCapacityFeeDialog
        {
            get { return _overCapacityFeeDialog; }
            set { _overCapacityFeeDialog = value; OnPropertyChanged(); }
        }

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
        public ICommand ReloadCommand { get; set; }
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
                ClearDetailProperties();
                Search();
            });

            ReloadCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                ClearDetailProperties();
                LoadReservations();
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

            ListviewSelectionChangedCommand = new RelayCommand<System.Windows.Controls.ListView>((p) =>
            {
                return !p.Items.IsEmpty;
            }, (p) =>
            {
                ClearDetailProperties();
                //Console.WriteLine(((RESERVATION)p.SelectedItem).id);
                LoadItemSelected((RESERVATION)p.SelectedItem);
            });

            ClearDetailCommand = new RelayCommand<System.Windows.Controls.ListView>((p) =>
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
                if (CurrentAccount.Instance.Permission != "Admin")
                    return false;
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
                charge.over_capacity_fee = OverCapacityFeeDialog;
                charge.early_checkin_fee = EarlyCheckinFeeDialog;
                charge.late_checkout_fee = LateCheckoutFeeDialog;
                DataProvider.Instance.DB.SaveChanges();

                IsOpenDialog = false;
                InitDialogProperties();
            });

            CheckOutCommand = new RelayCommand<System.Windows.Controls.ListView>((p) =>
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
                CheckOut((RESERVATION)p.SelectedItem);
                ClearDetailProperties();
                LoadReservations();
            });

            ExportCommand = new RelayCommand<System.Windows.Controls.ListView>((p) =>
            {
                if (p != null)
                {
                    if (p.SelectedIndex == -1)
                        return false;
                }
                return true;
            }, (p) =>
            {
                ExportPdf((RESERVATION)p.SelectedItem);
            });
        }

        void InitProperties()
        {
            Guests = new ObservableCollection<GUEST>();
            Rooms = new ObservableCollection<RoomDisplayItem>();
            Folio = new ObservableCollection<FolioDisplayItem>();
            SearchTypes = new List<string>();
            SearchTypes.Add("ID");
            SearchTypes.Add("Guest");

            StatusSelected = "Operational";
            LoadReservations();
            SelectedSearchType = "ID";
            DateCreatedSearch = DateTime.Now.AddDays(-14);
            ArrivalSearch = DateTime.Now.AddDays(-14);
            DepartureSearch = DateTime.Now;
        }

        void InitDialogProperties()
        {
            var charges = DataProvider.Instance.DB.CHARGES.First();
            ErrorMessageDialog = "";
            SurchargeDialog = charges.surcharge.Value;
            OverCapacityFeeDialog = charges.over_capacity_fee.Value;
            EarlyCheckinFeeDialog = charges.early_checkin_fee.Value;
            LateCheckoutFeeDialog = charges.late_checkout_fee.Value;
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
            OverCapacityFee = 0;
            EarlyCheckinFee = 0;
            LateCheckoutFee = 0;
            Surcharge = 0;
            TotalMoneyNumber = 0;
            TotalMoney = "";
        }

        #region Load Functions
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
            if (p == null)
                return;
            var mainGuest = DataProvider.Instance.DB.GUESTs.SingleOrDefault(x => x.id == p.main_guest);
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
                int exactRoomPrice = CalculatorInvoice.ExactRoomPrice(roomTypeList, p.date_created.Value);

                RoomDisplayItem roomDisplayItem = new RoomDisplayItem(room.id, room.name, roomType.name);
                Rooms.Add(roomDisplayItem);

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
                    }
                    else
                    {
                        folioItem.Amount += (int)item.amount;
                    }
                }
            }

            ReservationIdSelected = p.id;
            Arrival = p.arrival.Value.ToString();
            Departure = p.departure.Value.ToString();
            Identity = p.main_guest;
            Name = mainGuest.name;
            Phone = mainGuest.phone;
            Email = mainGuest.email;
            GuestCount = guestBookingList.Count();
            RoomCount = roomBookedList.Count();
            FolioCount = Folio.Count();

            LoadFeeByStatus(p);

            long roomsTotalMoney = CalculatorInvoice.RoomsTotalMoney(p) 
                + CalculatorInvoice.OverCapacityFeeOfRooms(p, OverCapacityFee);

            //Calculate money
            RoomsTotalMoney = SeparateThousands(roomsTotalMoney.ToString());
            FolioTotalMoney = SeparateThousands(CalculatorInvoice.FolioTotalMoney(p).ToString());
            TotalMoneyNumber = CalculatorInvoice.TotalMoneyWithFee(p);
            TotalMoney = SeparateThousands(TotalMoneyNumber.ToString());
        }

        void LoadFeeByStatus(RESERVATION p)
        {
            if (StatusSelected == "Operational")
            {
                if (CalculatorInvoice.IsReservationContainsRoomOverCapacity(p))
                {
                    OverCapacityFee = DataProvider.Instance.DB.CHARGES.First().over_capacity_fee.Value;
                }    
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

                OverCapacityFee = invoice.over_capacity_fee.Value;
                EarlyCheckinFee = invoice.early_checkin_fee.Value;
                LateCheckoutFee = invoice.late_checkout_fee.Value;
                Surcharge = invoice.surcharge.Value;
            }
        }
        #endregion

        #region Search Functions
        void Search()
        {
            if (IsSearchArrDep == false && IsSearchDateCreated == false)
            {
                SearchWithoutDate();
            }    
            else if (IsSearchArrDep == true && IsSearchDateCreated == false)
            {
                SearchWithDateArrDep();
            }    
            else if (IsSearchArrDep == false && IsSearchDateCreated == true)
            {
                SearchWithDateCreated();
            }    
            else
            {
                SearchWithDateCreateAndArrDep();
            }    
        }

        void SearchWithoutDate()
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

        void SearchWithDateArrDep()
        {
            if (string.IsNullOrEmpty(ContentSearch))
            {
                Reservations = new ObservableCollection<RESERVATION>(
                    DataProvider.Instance.DB.RESERVATIONs.Where(
                        x => x.status == StatusSelected
                        && DbFunctions.TruncateTime(x.departure.Value) <= DbFunctions.TruncateTime(DepartureSearch)
                        && DbFunctions.TruncateTime(x.arrival.Value) >= DbFunctions.TruncateTime(ArrivalSearch)));
            }    
            else
            {
                switch (SelectedSearchType)
                {
                    case "ID":
                        Reservations = new ObservableCollection<RESERVATION>(
                            DataProvider.Instance.DB.RESERVATIONs.Where(
                                x => x.id.ToString().Contains(ContentSearch) && x.status == StatusSelected
                                && DbFunctions.TruncateTime(x.departure.Value) <= DbFunctions.TruncateTime(DepartureSearch)
                                && DbFunctions.TruncateTime(x.arrival.Value) >= DbFunctions.TruncateTime(ArrivalSearch)));
                        break;
                    case "Main Guest":
                        Reservations = new ObservableCollection<RESERVATION>(
                            DataProvider.Instance.DB.RESERVATIONs.Where(
                                x => x.main_guest.ToString().Contains(ContentSearch) && x.status == StatusSelected
                                && DbFunctions.TruncateTime(x.departure.Value) <= DbFunctions.TruncateTime(DepartureSearch)
                                && DbFunctions.TruncateTime(x.arrival.Value) >= DbFunctions.TruncateTime(ArrivalSearch)));
                        break;
                    default:
                        break;
                }
            }    
        }

        void SearchWithDateCreated()
        {
            if (string.IsNullOrEmpty(ContentSearch))
            {
                Reservations = new ObservableCollection<RESERVATION>(
                    DataProvider.Instance.DB.RESERVATIONs.Where(
                        x => x.status == StatusSelected
                        && DbFunctions.DiffDays(x.date_created, DateCreatedSearch) == 0));
            }
            else
            {
                switch (SelectedSearchType)
                {
                    case "ID":
                        Reservations = new ObservableCollection<RESERVATION>(
                            DataProvider.Instance.DB.RESERVATIONs.Where(
                                x => x.id.ToString().Contains(ContentSearch) && x.status == StatusSelected
                                && DbFunctions.DiffDays(x.date_created, DateCreatedSearch) == 0));
                        break;
                    case "Main Guest":
                        Reservations = new ObservableCollection<RESERVATION>(
                            DataProvider.Instance.DB.RESERVATIONs.Where(
                                x => x.main_guest.ToString().Contains(ContentSearch) && x.status == StatusSelected
                                && DbFunctions.DiffDays(x.date_created, DateCreatedSearch) == 0));
                        break;
                    default:
                        break;
                }
            }
        }

        void SearchWithDateCreateAndArrDep()
        {
            if (string.IsNullOrEmpty(ContentSearch))
            {
                Reservations = new ObservableCollection<RESERVATION>(
                    DataProvider.Instance.DB.RESERVATIONs.Where(
                        x => x.status == StatusSelected
                        && DbFunctions.DiffDays(x.date_created, DateCreatedSearch) == 0
                        && DbFunctions.TruncateTime(x.departure.Value) <= DbFunctions.TruncateTime(DepartureSearch)
                        && DbFunctions.TruncateTime(x.arrival.Value) >= DbFunctions.TruncateTime(ArrivalSearch)));
            }
            else
            {
                switch (SelectedSearchType)
                {
                    case "ID":
                        Reservations = new ObservableCollection<RESERVATION>(
                            DataProvider.Instance.DB.RESERVATIONs.Where(
                                x => x.id.ToString().Contains(ContentSearch) && x.status == StatusSelected
                                && DbFunctions.DiffDays(x.date_created, DateCreatedSearch) == 0
                                && DbFunctions.TruncateTime(x.departure.Value) <= DbFunctions.TruncateTime(DepartureSearch)
                                && DbFunctions.TruncateTime(x.arrival.Value) >= DbFunctions.TruncateTime(ArrivalSearch)));
                        break;
                    case "Main Guest":
                        Reservations = new ObservableCollection<RESERVATION>(
                            DataProvider.Instance.DB.RESERVATIONs.Where(
                                x => x.main_guest.ToString().Contains(ContentSearch) && x.status == StatusSelected
                                && DbFunctions.DiffDays(x.date_created, DateCreatedSearch) == 0
                                && DbFunctions.TruncateTime(x.departure.Value) <= DbFunctions.TruncateTime(DepartureSearch)
                                && DbFunctions.TruncateTime(x.arrival.Value) >= DbFunctions.TruncateTime(ArrivalSearch)));
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

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
            //Console.WriteLine(p.id);
            
            p.status = "Completed";
            INVOICE invoice = new INVOICE()
            {
                reservation_id = p.id,
                total_money = Convert.ToDecimal(TotalMoneyNumber),
                surcharge = Surcharge,
                over_capacity_fee = OverCapacityFee,
                early_checkin_fee = EarlyCheckinFee,
                late_checkout_fee = LateCheckoutFee,
            };
            DataProvider.Instance.DB.INVOICEs.Add(invoice);
            DataProvider.Instance.DB.SaveChanges();
            
        }

        void ExportPdf(RESERVATION p)
        {
            ExportInvoice export = new ExportInvoice();
            export.Export(p);
        }
    }
}
