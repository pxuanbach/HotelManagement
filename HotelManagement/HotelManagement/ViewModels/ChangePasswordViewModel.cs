using HotelManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace HotelManagement.ViewModels
{
    class ChangePasswordViewModel : BaseViewModel
    {
        #region Properties
        //label notice when change password error
        private string _changePasswordErrorMessage;
        public string ChangePasswordErrorMessage
        {
            get
            {
                return _changePasswordErrorMessage;
            }
            set
            {
                _changePasswordErrorMessage = value;
                OnPropertyChanged();
            }
        }

        private string _currentPassword;
        public string CurrentPassword { get { return _currentPassword; } set { _currentPassword = value; OnPropertyChanged(); } }

        private string _newPassword;
        public string NewPassword { get { return _newPassword; } set { _newPassword = value; OnPropertyChanged(); } }

        private string _confirmPassword;
        public string ConfirmPassword { get { return _confirmPassword; } set { _confirmPassword = value; OnPropertyChanged(); } }

        //label notice special char Password
        private string _specialCharCurrentPassword;
        public string SpecialCharCurrentPassword { get { return _specialCharCurrentPassword; } set { _specialCharCurrentPassword = value; OnPropertyChanged(); } }

        //label notice special char New Password
        private string _specialCharNewPassword;
        public string SpecialCharNewPassword
        {
            get
            {
                return _specialCharNewPassword;
            }
            set
            {
                _specialCharNewPassword = value;
                OnPropertyChanged();
            }
        }

        //label notice special char Confirm Password
        private string _specialCharConfirmPassword;
        public string SpecialCharConfirmPassword
        {
            get
            {
                return _specialCharConfirmPassword;
            }
            set
            {
                _specialCharConfirmPassword = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Command
        public ICommand ChangePasswordCommand { get; set; }
        public ICommand PasswordChangedCommand { get; set; }
        public ICommand NewPasswordChangedCommand { get; set; }
        public ICommand ConfirmPasswordChangedCommand { get; set; }
        #endregion

        public ChangePasswordViewModel()
        {
            InitProperties();

            PasswordChangedCommand = new RelayCommand<PasswordBox>((p) =>
            {
                return true;
            }, (p) =>
            {
                CurrentPasswordChanged(p);
            });

            NewPasswordChangedCommand = new RelayCommand<PasswordBox>((p) =>
            {
                return true;
            }, (p) =>
            {
                NewPasswordChanged(p);
            });

            ConfirmPasswordChangedCommand = new RelayCommand<PasswordBox>((p) =>
            {
                return true;
            }, (p) =>
            {
                ConfirmPasswordChanged(p);
            });

            /// <summary>
            /// ChangePasswordCommand have 3 parameter (object[])
            /// 0 => <include file='ProfileView.xaml' path='[@ElementName="txtPassword"]'/>
            /// 1 => <include file='ProfileView.xaml' path='[@ElementName="txtNewPassword"]'/>
            /// 2 => <include file='ProfileView.xaml' path='[@ElementName="txtConfirmPassword"]'/>
            /// </summary>
            ChangePasswordCommand = new RelayCommand<object[]>((p) =>
            {
                if (string.IsNullOrEmpty(CurrentPassword) || string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(ConfirmPassword))
                    return false;
                if (SpecialCharCurrentPassword != "" || SpecialCharNewPassword != "" || SpecialCharConfirmPassword != "")
                    return false;
                return true;
            }, (p) =>
            {
                ChangePassword(p);
            });
        }

        void InitProperties()
        {
            ChangePasswordErrorMessage = "";
            SpecialCharCurrentPassword = "";
            SpecialCharNewPassword = "";
            SpecialCharConfirmPassword = "";
        }

        void CurrentPasswordChanged(PasswordBox p)
        {
            CurrentPassword = p.Password;

            if (p.Password.Length > 0)  //remove message when re-type confirm password
            {
                ChangePasswordErrorMessage = "";
            }
            if (!CheckVietKey(p.Password))
            {
                SpecialCharCurrentPassword = "Password contains vietkey character";
            }
            else
            {
                SpecialCharCurrentPassword = "";
            }
        }

        void NewPasswordChanged(PasswordBox p)
        {
            NewPassword = p.Password;

            if (p.Password.Length > 0)  //remove message when re-type new password
            {
                ChangePasswordErrorMessage = "";
            }
            if (!CheckVietKey(p.Password))
            {
                SpecialCharNewPassword = "Password contains vietkey character";
            }
            else
            {
                SpecialCharNewPassword = "";
            }
        }

        void ConfirmPasswordChanged(PasswordBox p)
        {
            ConfirmPassword = p.Password;

            if (p.Password.Length > 0)  //remove message when re-type confirm password
            {
                ChangePasswordErrorMessage = "";
            }
            if (!CheckVietKey(p.Password))
            {
                SpecialCharConfirmPassword = "Password contains vietkey character";
            }
            else
            {
                SpecialCharConfirmPassword = "";
            }
        }

        void ChangePassword(object[] p)
        {
            //Get array parameter
            var values = (object[])p;
            PasswordBox txtPassword = values[0] as PasswordBox;
            PasswordBox txtNewPassword = values[1] as PasswordBox;
            PasswordBox txtConfirmPassword = values[2] as PasswordBox;
            var account = DataProvider.Instance.DB.ACCOUNTs.FirstOrDefault(x => x.id == CurrentAccount.Instance.Id);

            if (account == null)
            {
                ChangePasswordErrorMessage = "Something wrong! Please, reset application.";
                txtPassword.Password = "";
                return;
            }    

            if (account.password != HashModule.Hash(CurrentPassword))   //Current Password wrong
            {
                ChangePasswordErrorMessage = "Current password is wrong";
                txtPassword.Password = "";
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                ChangePasswordErrorMessage = "New password and confirm password are different";
                txtNewPassword.Password = "";
                txtConfirmPassword.Password = "";
                return;
            }

            account.password = HashModule.Hash(NewPassword);
            DataProvider.Instance.DB.SaveChanges();

            ChangePasswordErrorMessage = "Password change success!";
            txtPassword.Password = "";
            txtNewPassword.Password = "";
            txtConfirmPassword.Password = "";
        }
    }
}
