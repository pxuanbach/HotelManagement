using HotelManagement.Models;
using HotelManagement.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HotelManagement.ViewModels
{
    class RegisterViewModel : BaseViewModel
    {
        #region Properties
        private string _errorMessage;
        public string ErrorMessage { get { return _errorMessage; } set { _errorMessage = value; OnPropertyChanged(); } }

        private string _username;
        public string Username { get { return _username; } set { _username = value; OnPropertyChanged(); } }

        private string _password;
        public string Password { get { return _password; } set { _password = value; OnPropertyChanged(); } }

        private string _confirmPassword;
        public string ConfirmPassword { get { return _confirmPassword; } set { _confirmPassword = value; OnPropertyChanged(); } }
        #endregion

        #region Command
        public ICommand CloseWindowCommand { get; set; }
        public ICommand PasswordChangedCommand { get; set; }
        public ICommand ConfirmPasswordChangedCommand { get; set; }
        public ICommand RegisterCommand { get; set; }
        public ICommand BackCommand { get; set; }
        #endregion

        public RegisterViewModel()
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

            ConfirmPasswordChangedCommand = new RelayCommand<PasswordBox>((p) =>
            {
                return true;
            }, (p) =>
            {
                ConfirmPassword = p.Password;
                ErrorMessage = "";
            });

            RegisterCommand = new RelayCommand<RegisterWindow>((p) =>
            {
                if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password)
                    || string.IsNullOrEmpty(ConfirmPassword))
                    return false;
                return true;
            }, (p) =>
            {
                Register(p);
            });

            BackCommand = new RelayCommand<Window>((p) =>
            {
                return true;
            }, (p) =>
            {
                Back(p);
            });
        }

        void InitProperties()
        {
            Username = "";
            Password = "";
            ConfirmPassword = "";
        }

        void Register(RegisterWindow p)
        {
            if (p == null)
                return;

            if (!CheckVietKey(Username))
            {
                p.txtUsername.Text = "";
                ErrorMessage = "Username contains vietkey";
                return;
            }    

            //Check username
            var accCount = DataProvider.Instance.DB.ACCOUNTs
                .Where(x => x.username == Username).Count();
            if (accCount > 0)
            {
                p.txtUsername.Text = "";
                ErrorMessage = "\"" + Username + "\"" + " has already existed";
                return;
            }

            if (!CheckVietKey(Password) || !CheckVietKey(ConfirmPassword))
            {
                p.txtPassword.Password = "";
                p.txtConfirmPassword.Password = "";
                ErrorMessage = "Password or confirm password contains vietkey";
                return;
            }

            //Check two passwords are the same  
            if (Password != ConfirmPassword)
            {
                p.txtPassword.Password = "";
                p.txtConfirmPassword.Password = "";
                ErrorMessage = "Password and confirm password are different";
            }
            else
            {
                ACCOUNT account = new ACCOUNT()
                {
                    username = Username,
                    password = HashModule.Hash(Password),
                    permission = "Undefined", //default role is "undefined"
                };

                DataProvider.Instance.DB.ACCOUNTs.Add(account);
                DataProvider.Instance.DB.SaveChanges();

                ErrorMessage = "Registration success";
            }
        }

        void Back(Window p)
        {
            if (p == null)
                return;

            p.Hide();
            (new LoginWindow()).Show();
            p.Close();
        }
    }
}