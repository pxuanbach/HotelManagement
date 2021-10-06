using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace HotelManagement.ViewModels
{
    public class ReservationViewModel : BaseViewModel
    {
        private bool _isSelected;
        private string _status;
        private string _guest;
        private string _arrival;
        private string _departure;
        private int _rooms;
        private int _pax;
        private decimal _total;

        public bool IsSelected { get { return _isSelected; } set { _isSelected = value; OnPropertyChanged(); } }

        public string Status { get { return _status; } set { _status = value; OnPropertyChanged(); } }

        public int Rooms { get { return _rooms; } set { _rooms = value; OnPropertyChanged(); } }

        public string Guest { get { return _guest; } set { _guest = value; OnPropertyChanged(); } }

        public string Arrival { get { return _arrival; } set { _arrival = value; OnPropertyChanged(); } }

        public string Departure { get { return _departure; } set { _departure = value; OnPropertyChanged(); } }

        public int Pax { get { return _pax; } set { _pax = value; OnPropertyChanged(); } }

        public decimal Total { get { return _total; } set { _total = value; OnPropertyChanged(); } }
    }
}
