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
            set { 
                _contentSearch = value; 
                OnPropertyChanged(); 
                if (_contentSearch == "")
                    RefreshProperties(false);
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
        private int _numberOfAdmins;
        public int NumberOfAdmins { get { return _numberOfAdmins; } set { _numberOfAdmins = value; OnPropertyChanged(); } }

        private int _numberOfReservations;
        public int NumberOfReservations { get { return _numberOfReservations; } set { _numberOfReservations = value; OnPropertyChanged(); } }

        private int _numberOfReceptionists;
        public int NumberOfReceptionists { get { return _numberOfReceptionists; } set { _numberOfReceptionists = value; OnPropertyChanged(); } }

        private int _numberOfCashiers;
        public int NumberOfCashiers { get { return _numberOfCashiers; } set { _numberOfCashiers = value; OnPropertyChanged(); } }
        
        private int _numberOfUndefined;
        public int NumberOfUndefined { get { return _numberOfUndefined; } set { _numberOfUndefined = value; OnPropertyChanged(); } }
        #endregion

        private ObservableCollection<ACCOUNT> _accounts;
        public ObservableCollection<ACCOUNT> Accounts
        {
            get { return _accounts; }
            set { _accounts = value; OnPropertyChanged(); }
        }
        #endregion

        #region Command
        public ICommand AddNewAccountCommand { get; set; }
        public ICommand SearchAccountCommand { get; set; }
        public ICommand SaveAccountCommand { get; set; }
        public ICommand EditAccountCommand { get; set; }

        #endregion

        public AccountViewModel()
        {
            InitProperties();

            AddNewAccountCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                IsOpenDialog = true;
                DialogPropertiesChanged(null);
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

            EditAccountCommand = new RelayCommand<ACCOUNT>((p) =>
            {
                return Accounts.Count > 0;
            }, (p) =>
            {
                IsOpenDialog = true;
                DialogPropertiesChanged(p);
            });

            SearchAccountCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(ContentSearch))
                    return false;
                return true;
            }, (p) =>
            {
                Search();
            });
        }

        void InitProperties()
        {
            InitRoleCount();

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
            NumberOfAdmins = DataProvider.Instance.DB.ACCOUNTs.Where(x => x.permission == "Admin").Count();
            NumberOfReservations = DataProvider.Instance.DB.ACCOUNTs.Where(x => x.permission == "Reservation").Count();
            NumberOfReceptionists = DataProvider.Instance.DB.ACCOUNTs.Where(x => x.permission == "Receptionist").Count();
            NumberOfCashiers = DataProvider.Instance.DB.ACCOUNTs.Where(x => x.permission == "Cashier").Count();
            NumberOfUndefined = DataProvider.Instance.DB.ACCOUNTs.Where(x => x.permission == "Undefined").Count();
        }

        void RefreshProperties(bool isRoleCount)
        {
            IsOpenDialog = false;
            Accounts = new ObservableCollection<ACCOUNT>(
                DataProvider.Instance.DB.ACCOUNTs.Where(x => x.permission != "Admin"));

            if (isRoleCount)
                InitRoleCount();
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
            RefreshProperties(true);
        }

        void Search()
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
    }
}
