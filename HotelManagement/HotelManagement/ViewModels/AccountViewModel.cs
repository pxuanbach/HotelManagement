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

namespace HotelManagement.ViewModels
{
    class AccountViewModel : BaseViewModel
    {
        #region Properties
        private string _username;
        public string Username { get { return _username; } set { _username = value; OnPropertyChanged(); } }

        private List<string> _roles;
        public List<string> Roles { get { return _roles; } set { _roles = value; OnPropertyChanged(); } }

        private string _selectedRole;
        public string SelectedRole { get { return _selectedRole; } set { _selectedRole = value; OnPropertyChanged(); } }

        private bool _isOpenDialog;
        public bool IsOpenDialog { get { return _isOpenDialog; } set { _isOpenDialog = value; OnPropertyChanged(); } }

        private ObservableCollection<ACCOUNT> _accounts;
        public ObservableCollection<ACCOUNT> Accounts
        {
            get { return _accounts; }
            set { _accounts = value; OnPropertyChanged(); }
        }
        #endregion

        #region Command
        public ICommand AddNewAccountCommand { get; set; }
        public ICommand SaveNewAccountCommand { get; set; }
        public ICommand SearchAccountCommand { get; set; }
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
            });

            SaveNewAccountCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(Username))
                    return false;
                return true;
            }, (p) =>
            {
                Save(p);
            });

            EditAccountCommand = new RelayCommand<ACCOUNT>((p) =>
            {
                return true;
            }, (p) =>
            {
                IsOpenDialog = true;
                Username = p.username;
                SelectedRole = p.permission;
            });
        }

        void InitProperties()
        {
            Accounts = new ObservableCollection<ACCOUNT>();
            var objList = DataProvider.Instance.DB.ACCOUNTs.Where(p => p.permission != "Admin").ToList();
            foreach (var obj in objList)
            {
                ACCOUNT acc = new ACCOUNT();
                acc = obj;
                Accounts.Add(acc);
            }    

            IsOpenDialog = false;

            Roles = new List<string>();
            Roles.Add("Reservation");
            Roles.Add("Receptionist");
            Roles.Add("Cashier");
            Roles.Add("Undefined");

            SelectedRole = "Undefined";
        }

        void Save(object p)
        {
            IsOpenDialog = false;
        }
    }
}
