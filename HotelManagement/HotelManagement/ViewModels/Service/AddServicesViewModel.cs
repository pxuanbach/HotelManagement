using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HotelManagement.ViewModels
{
    class AddServicesViewModel : BaseViewModel
    {
        private string folioTotalMoney;
        public string FolioTotalMoney { get { return folioTotalMoney; } set { folioTotalMoney = value; OnPropertyChanged(); } }

        private string roomName;
        public string RoomName
        {
            get => roomName;
            set
            {
                roomName = value;
                OnPropertyChanged();
            }
        }

        public ICommand CloseWindowCommand { get; set; }

        public AddServicesViewModel()
        {
            CloseWindowCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                Application.Current.Shutdown();
            });
        }
        public void getRoomName(string roomBookedName)
        {
            RoomName = roomBookedName;
        }
    }
}
