using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;
using HotelManagement.Models;
using HotelManagement.Views;
using System.Data.Entity.SqlServer;
using System.Windows.Threading;
using System.Windows.Controls;

namespace HotelManagement.ViewModels
{
    class AccountViewModel : BaseViewModel
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
                    Load(false);
            } 
        }

        private List<string> _searchTypes;
        public List<string> SearchTypes { get { return _searchTypes; } set { _searchTypes = value; OnPropertyChanged(); } }

        private string _selectedSearchType;
        public string SelectedSearchType { get { return _selectedSearchType; } set { _selectedSearchType = value; OnPropertyChanged(); } }
        #endregion

        #region Dialog Properties
        private string _dialogTittle;
        public string DialogTittle { get { return _dialogTittle; } set { _dialogTittle = value; OnPropertyChanged(); } }

        private string _subText;
        public string SubText { get { return _subText; } set { _subText = value; OnPropertyChanged(); } }

        private string _username;
        public string Username { get { return _username; } set { _username = value; OnPropertyChanged(); } }

        private string _errorMessage;
        public string ErrorMessage { get { return _errorMessage; } set { _errorMessage = value; OnPropertyChanged(); } }

        private List<string> _roles;
        public List<string> Roles { get { return _roles; } set { _roles = value; OnPropertyChanged(); } }

        private string _selectedRole;
        public string SelectedRole { get { return _selectedRole; } set { _selectedRole = value; OnPropertyChanged(); } }

        private bool _isOpenDialog;
        public bool IsOpenDialog 
        { 
            get { return _isOpenDialog; } 
            set { _isOpenDialog = value; OnPropertyChanged(); } 
        }

        private bool _isReadOnlyUsername;
        public bool IsReadOnlyUsername 
        { 
            get { return _isReadOnlyUsername; } 
            set { _isReadOnlyUsername = value; OnPropertyChanged(); } 
        }
        #endregion

        #region Role Count
        private int _numberOfReservations;
        public int NumberOfReservations { get { return _numberOfReservations; } set { _numberOfReservations = value; OnPropertyChanged(); } }

        private int _numberOfReceptionists;
        public int NumberOfReceptionists { get { return _numberOfReceptionists; } set { _numberOfReceptionists = value; OnPropertyChanged(); } }

        private int _numberOfCashiers;
        public int NumberOfCashiers { get { return _numberOfCashiers; } set { _numberOfCashiers = value; OnPropertyChanged(); } }
        
        private int _numberOfUndefined;
        public int NumberOfUndefined { get { return _numberOfUndefined; } set { _numberOfUndefined = value; OnPropertyChanged(); } }
        #endregion

        public string RoleSelected { get; set; }

        private ObservableCollection<ACCOUNT> _accounts;
        public ObservableCollection<ACCOUNT> Accounts
        {
            get { return _accounts; }
            set { _accounts = value; OnPropertyChanged(); }
        }
        #endregion

        #region Command
        public ICommand SearchAccountCommand { get; set; }
        public ICommand AddNewAccountCommand { get; set; }
        public ICommand SaveAccountCommand { get; set; }
        public ICommand EditAccountCommand { get; set; }
        public ICommand ReloadCommand { get; set; }
        public ICommand AllCommnad { get; set; }
        public ICommand ReservationCommnad { get; set; }
        public ICommand ReceptionistCommnad { get; set; }
        public ICommand CashierCommnad { get; set; }
        public ICommand UndefinedCommnad { get; set; }
        #endregion

        public AccountViewModel()
        {
            InitProperties();

            SearchAccountCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                Search();
            });

            AddNewAccountCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                IsOpenDialog = true;
                DialogPropertiesChanged(null);
            });

            EditAccountCommand = new RelayCommand<ACCOUNT>((p) =>
            {
                return Accounts.Count > 0;
            }, (p) =>
            {
                IsOpenDialog = true;
                DialogPropertiesChanged(p);
            });

            SaveAccountCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(Username))
                    return false;
                return true;
            }, (p) =>
            {
                Save(IsReadOnlyUsername);
            });

            ReloadCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                if (RoleSelected == "All")
                    LoadPropertiesAll();
                else
                    LoadPropertiesByRole();
            });

            AllCommnad = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                RoleSelected = "All";
                LoadPropertiesAll();
            });

            ReservationCommnad = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                RoleSelected = "Reservation";
                LoadPropertiesByRole();
            });

            ReceptionistCommnad = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                RoleSelected = "Receptionist";
                LoadPropertiesByRole();
            });

            CashierCommnad = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                RoleSelected = "Cashier";
                LoadPropertiesByRole();
            });

            UndefinedCommnad = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                RoleSelected = "Undefined";
                LoadPropertiesByRole();
            });
        }

        void InitProperties()
        {
            InitRoleCount();
            RoleSelected = "All";

            IsOpenDialog = false;
            Accounts = new ObservableCollection<ACCOUNT>(
                DataProvider.Instance.DB.ACCOUNTs.Where(x => x.permission != "Admin"));

            SearchTypes = new List<string>();
            SearchTypes.Add("ID");
            SearchTypes.Add("Username");

            SelectedSearchType = "ID";

            Roles = new List<string>();
            Roles.Add("Reservation");
            Roles.Add("Receptionist");
            Roles.Add("Cashier");
            Roles.Add("Undefined");
        }

        void InitRoleCount()
        {
            NumberOfReservations = DataProvider.Instance.DB.ACCOUNTs.Where(x => x.permission == "Reservation").Count();
            NumberOfReceptionists = DataProvider.Instance.DB.ACCOUNTs.Where(x => x.permission == "Receptionist").Count();
            NumberOfCashiers = DataProvider.Instance.DB.ACCOUNTs.Where(x => x.permission == "Cashier").Count();
            NumberOfUndefined = DataProvider.Instance.DB.ACCOUNTs.Where(x => x.permission == "Undefined").Count();
        }

        void DialogPropertiesChanged(ACCOUNT p)
        {
            ErrorMessage = "";
            if (p == null)
            {
                DialogTittle = "New Account";
                IsReadOnlyUsername = false;
                Username = "";
                SelectedRole = "Undefined";
                SubText = "Default password is 1";
            }    
            else
            {
                DialogTittle = "Edit Account";
                IsReadOnlyUsername = true;
                SubText = "ID: " + p.id;
                Username = p.username;
                SelectedRole = p.permission;
            }    
        }

        void Save(bool isEdit)
        {
            if (isEdit)
            {
                //Get id account
                int id = Convert.ToInt32(SubText.Substring(4));
                var account = DataProvider.Instance.DB.ACCOUNTs.Where(x => x.id == id).SingleOrDefault();
                account.permission = SelectedRole;
                DataProvider.Instance.DB.SaveChanges();
            }
            else
            {
                //Check username
                var accCount = DataProvider.Instance.DB.ACCOUNTs
                    .Where(x => x.username == Username).Count();

                if (accCount > 0)
                {
                    ErrorMessage = "\"" + Username + "\"" + " has already existed";
                    Username = "";
                    return;
                }
                else
                {
                    ACCOUNT account = new ACCOUNT()
                    {
                        username = Username,
                        password = HashModule.Hash("1"),
                        permission = SelectedRole,
                    };
                    DataProvider.Instance.DB.ACCOUNTs.Add(account);
                    DataProvider.Instance.DB.SaveChanges();
                }    
            }
            Load(true);
        }

        #region Load Function
        void Load(bool isRoleCount)
        {
            IsOpenDialog = false;

            if (RoleSelected == "All")
            {
                LoadPropertiesAll();
            }
            else
            {
                LoadPropertiesByRole();
            }

            if (isRoleCount)
                InitRoleCount();
        }

        void LoadPropertiesAll()
        {
            Accounts = new ObservableCollection<ACCOUNT>(
                DataProvider.Instance.DB.ACCOUNTs.Where(x => x.permission != "Admin"));
        }

        void LoadPropertiesByRole()
        {
            Accounts = new ObservableCollection<ACCOUNT>(
                DataProvider.Instance.DB.ACCOUNTs.Where(x => x.permission == RoleSelected));
        }
        #endregion

        #region Search Function
        void Search()
        {
            if (RoleSelected == "All")
            {
                SearchAll();
            }
            else
            {
                SearchByRole(RoleSelected);
            } 
        }

        void SearchAll()
        {
            switch (SelectedSearchType)
            {
                case "ID":
                    Accounts = new ObservableCollection<ACCOUNT>(
                        DataProvider.Instance.DB.ACCOUNTs.Where(
                            x => x.id.ToString().Contains(ContentSearch) && x.permission != "Admin"));
                    break;
                case "Username":
                    Accounts = new ObservableCollection<ACCOUNT>(
                        DataProvider.Instance.DB.ACCOUNTs.Where(
                            x => x.username.Contains(ContentSearch) && x.permission != "Admin"));
                    break;
                default:
                    break;
            }
        }

        void SearchByRole(string role)
        {
            switch (SelectedSearchType)
            {
                case "ID":
                    Accounts = new ObservableCollection<ACCOUNT>(
                        DataProvider.Instance.DB.ACCOUNTs.Where(
                            x => x.id.ToString().Contains(ContentSearch) && x.permission == role));
                    break;
                case "Username":
                    Accounts = new ObservableCollection<ACCOUNT>(
                        DataProvider.Instance.DB.ACCOUNTs.Where(
                            x => x.username.Contains(ContentSearch) && x.permission == role));
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}
