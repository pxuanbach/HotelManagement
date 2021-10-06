using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HotelManagement.ViewModels
{
    class NewReservationViewModel : BaseViewModel
    {
        public ObservableCollection<BookedRoom> BookedRooms { get; set; }

        public ObservableCollection<Sharer> Sharers { get; set; }

        public GuestInformation GuestInformation { get; set; }

        public StayInformation StayInformation { get; set; }

        public IEnumerable<string> AvailableRoomTypes => new[] { "STD", "DBL", "DLX", "SUP" };

        public IEnumerable<string> AvailableRooms => new[] { "101", "102", "103", "104" };

        public IEnumerable<string> Gender => new[] { "Male", "Female", "Other" };

        #region Command
        private ICommand _addRoomCommand;
        public ICommand AddRoomCommand 
        {
            get
            {
                return _addRoomCommand ?? (_addRoomCommand = new RelayCommand<object>((p) => CanAddRoom, (p) => AddRoom()));
            }
        }

        private ICommand _addSharerCommand;
        public ICommand AddSharerCommand
        {
            get
            {
                return _addSharerCommand ?? (_addSharerCommand = new RelayCommand<object>((p) => CanAddSharer, (p) => AddSharer()));
            }
        }

        private ICommand _removeRoomCommand;
        public ICommand RemoveRoomCommand
        {
            get
            {
                return _removeRoomCommand ?? (_removeRoomCommand = new RelayCommand<BookedRoom>((p) => true, (p) => RemoveSelectedRoom(p)));
            }
        }

        private ICommand _removeSharerCommand;
        public ICommand RemoveSharerCommand
        {
            get
            {
                return _removeSharerCommand ?? (_removeSharerCommand = new RelayCommand<Sharer>((p) => true, (p) => RemoveSelectedSharer(p)));
            }
        }

        private ICommand _reserveCommand;
        public ICommand ReserveCommand
        {
            get
            {
                return _reserveCommand ?? (_reserveCommand = new RelayCommand<object>((p) => CanReserve, (p) => Reserve()));
            }
        }
        #endregion

        public NewReservationViewModel()
        {
            BookedRooms = new ObservableCollection<BookedRoom>();
            Sharers = new ObservableCollection<Sharer>();
            GuestInformation = new GuestInformation();
            StayInformation = new StayInformation();

            BookedRooms.CollectionChanged += BookedRooms_CollectionChanged;
            Sharers.CollectionChanged += Sharers_CollectionChanged;
        }

        private void Sharers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            StayInformation.Sharers = Sharers.Count;
        }

        private void BookedRooms_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            StayInformation.Rooms = BookedRooms.Count;
        }

        public bool FilledGuestInformation
        {
            get
            {
                return true;
            }
        }

        public bool CanAddRoom
        {
            get
            {
                if (BookedRooms.Count == 0)
                {
                    return true;
                }
                else
                {
                    foreach (var row in BookedRooms)
                    {
                        if (row.RoomType == null || row.RoomName == null)
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
        }

        public bool CanAddSharer
        {
            get
            {
                if (Sharers.Count == 0)
                {
                    return true;
                }
                else
                {
                    foreach (var row in Sharers)
                    {
                        if (row.Name == null || row.Gender == null || row.Address == null)
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
        }

        public bool CanReserve
        {
            get
            {
                if (!CanAddSharer || Sharers.Count == 0) return false;
                if (!CanAddRoom || BookedRooms.Count == 0) return false;
                return true;
            }
        }

        public void AddRoom()
        {
            BookedRoom bookedRoom = new BookedRoom();
            BookedRooms.Add(bookedRoom);
        }

        public void RemoveSelectedRoom(BookedRoom bookedRoom)
        {
            BookedRooms.Remove(bookedRoom);
        }

        public void AddSharer()
        {
            Sharer newSharer = new Sharer();
            Sharers.Add(newSharer);
        }

        public void RemoveSelectedSharer(Sharer sharer)
        {
            Sharers.Remove(sharer);
        }

        public void Reserve() { }
    }

    class GuestInformation : BaseViewModel
    {
        string _name;
        string _gender;
        string _birthday;
        string _address;
        string _email;
        string _phone;

        public string Name { get { return _name; } set { _name = value; OnPropertyChanged(); } }

        public string Gender { get { return _gender; } set { _gender = value; OnPropertyChanged(); } }

        public string Birthday { get { return _birthday; } set { _birthday = value; OnPropertyChanged(); } }

        public string Address { get { return _address; } set { _address = value; OnPropertyChanged(); } }

        public string Email { get { return _email; } set { _email = value; OnPropertyChanged(); } }

        public string Phone { get { return _phone; } set { _phone = value; OnPropertyChanged(); } }
    }

    class StayInformation : BaseViewModel
    {
        DateTime _arrival;
        DateTime _departure;
        string _gender;
        bool _guarantee;
        int _sharers;
        int _stays;
        int _rooms;

        public DateTime Arrival { get { return _arrival; } set { _arrival = value; OnPropertyChanged(); } }

        public DateTime Departure { get { return _departure; } set { _departure = value; OnPropertyChanged(); } }

        public string Gender { get { return _gender; } set { _gender = value; OnPropertyChanged(); } }

        public bool Guarantee { get { return _guarantee; } set { _guarantee = value; OnPropertyChanged(); } }

        public int Sharers { get { return _sharers; } set { _sharers = value; OnPropertyChanged(); } }

        public int Stays { get { return _stays; } set { _stays = value; OnPropertyChanged(); } }

        public int Rooms { get { return _rooms; } set { _rooms = value; OnPropertyChanged(); } }
    }

    class BookedRoom : BaseViewModel
    {
        string _roomType;
        string _roomName;

        public string RoomType { get { return _roomType; } set { _roomType = value; OnPropertyChanged(); } }

        public string RoomName { get { return _roomName; } set { _roomName = value; OnPropertyChanged(); } }
    }

    class Sharer : BaseViewModel
    {
        string _name;
        string _gender;
        string _address;

        public string Name { get { return _name; } set { _name = value; OnPropertyChanged(); } }

        public string Gender { get { return _gender; } set { _gender = value; OnPropertyChanged(); } }

        public string Address { get { return _address; } set { _address = value; OnPropertyChanged(); } }
    }
}
