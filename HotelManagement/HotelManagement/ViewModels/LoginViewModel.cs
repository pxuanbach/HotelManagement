using HotelManagement.Models;
using HotelManagement.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HotelManagement.ViewModels
{
    class LoginViewModel : BaseViewModel
    {
        #region Properties
        private string _errorMessage;
        public string ErrorMessage { get { return _errorMessage; } set { _errorMessage = value; OnPropertyChanged(); } }

        private string _username;
        public string Username { get { return _username; } set { _username = value; OnPropertyChanged(); } }

        private string _password;
        public string Password { get { return _password; } set { _password = value; OnPropertyChanged(); } }
        #endregion

        #region Command
        public ICommand CloseWindowCommand { get; set; }
        public ICommand PasswordChangedCommand { get; set; }
        public ICommand LoginCommand { get; set; }
        public ICommand RegisterCommand { get; set; }
        #endregion

        public LoginViewModel()
        {
            InitProperties();

            CloseWindowCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                Application.Current.Shutdown();
            });

            PasswordChangedCommand = new RelayCommand<PasswordBox>((p) =>
            {
                return true;
            }, (p) =>
            {
                Password = p.Password;
                ErrorMessage = "";
            });

            LoginCommand = new RelayCommand<Window>((p) =>
            {
                return true;
            }, (p) =>
            {
                Login(p);
            });

            RegisterCommand = new RelayCommand<Window>((p) =>
            {
                return true;
            }, (p) =>
            {
                Register(p);
            });
        }

        void InitProperties()
        {
            Username = "";
            Password = "";
        }

        void Login(Window p)
        {
            if (p == null)
                return;

            string hashPassword = HashModule.Hash(Password);
            var accCount = DataProvider.Instance.DB.ACCOUNTs
                .Where(x => x.username == Username && x.password == hashPassword).Count();

            if (accCount > 0)
            {
                CurrentAccount.Instance.GetCurrentAccount(Username);
                p.Hide();
                (new MainWindow()).Show();
                p.Close();
            }
            else
            {
                ErrorMessage = "Invalid username or password";
            }
        }

        void Register(Window p)
        {
            if (p == null)
                return;

            p.Hide();
            (new RegisterWindow()).Show();
            p.Close();
        }
    }
}