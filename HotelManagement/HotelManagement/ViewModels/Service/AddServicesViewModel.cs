using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HotelManagement.ViewModels.Service
{
    class AddServicesViewModel : BaseViewModel
    {
        private string folioTotalMoney;
        public string FolioTotalMoney { get { return folioTotalMoney; } set { folioTotalMoney = value; OnPropertyChanged(); } }

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
    }
}
