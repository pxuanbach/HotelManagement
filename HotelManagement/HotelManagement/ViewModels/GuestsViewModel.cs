using HotelManagement.Models;
using HotelManagement.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace HotelManagement.ViewModels
{
    class GuestsViewModel:BaseViewModel
    {
        public string Title { get; } = "Guests";
        #region ItemS ource
        private ObservableCollection<GUEST> itemSource = new ObservableCollection<GUEST>();
        public ObservableCollection<GUEST> ItemSource
        {
            get => itemSource;
            set
            {
                itemSource = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Selected Guests
        private GUEST selectedGuest;
        public GUEST SelectedGuest
        {
            get { return selectedGuest; }
            set
            {
                selectedGuest = value;
                OnPropertyChanged();
                DataProvider.Instance.DB.SaveChanges();
            }
        }
        #endregion

        #region Dialog Properties
        private bool isOpenDialog;
        public bool IsOpenDialog
        {
            get { return isOpenDialog; }
            set
            {
                isOpenDialog = value;
                OnPropertyChanged();
            }
        }

        private string guestID;
        public string GuestID
        {
            get { return guestID; }
            set
            {
                guestID = value;
                OnPropertyChanged();
            }
        }

        private string guestName;
        public string GuestName
        {
            get { return guestName; }
            set
            {
                guestName = value;
                OnPropertyChanged();
            }
        }

        private string guestGender;
        public string GuestGender
        {
            get { return guestGender; }
            set
            {
                guestGender = value;
                OnPropertyChanged();
            }
        }

        private DateTime guestBirthday;
        public DateTime GuestBirthday
        {
            get { return guestBirthday; }
            set
            {
                guestBirthday = value;
                OnPropertyChanged();
            }
        }

        private string guestAddress;
        public string GuestAddress
        {
            get { return guestAddress; }
            set
            {
                guestAddress = value;
                OnPropertyChanged();
            }
        }

        private string guestPhone;
        public string GuestPhone
        {
            get { return guestPhone; }
            set
            {
                guestPhone = value;
                OnPropertyChanged();
            }
        }

        private string guestEmail;
        public string GuestEmail
        {
            get { return guestEmail; }
            set
            {
                guestEmail = value;
                OnPropertyChanged();
            }
        }

        private string errorMessage;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                errorMessage = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Search
        private string contentSearch;
        public string ContentSearch
        {
            get { return contentSearch; }
            set
            {
                contentSearch = value;
                OnPropertyChanged();
            }
        }

        private List<string> _searchTypes;
        public List<string> SearchTypes { get { return _searchTypes; } set { _searchTypes = value; OnPropertyChanged(); } }

        private string _selectedSearchType;
        public string SelectedSearchType
        {
            get { return _selectedSearchType; }
            set { _selectedSearchType = value; OnPropertyChanged(); }
        }
        #endregion

        #region Command
        public GuestsView GuestView { get; set; }

        public ICommand SearchGuestCommand { get; set; }
        public ICommand DeleteGuestCommand { get; set; }
        public ICommand AddNewGuestCommand { get; set; }
        public ICommand SaveGuestCommand { get; set; }
        public ICommand EditCommand { get; set; }


        public GuestsViewModel()
        {
            IsOpenDialog = false;
            LoadGuest();
            guestName = "null";
            GuestBirthday = DateTime.Now;
            SearchGuestCommand = new RelayCommand<GuestsView>((p) => true, (p) => Search());
            DeleteGuestCommand = new RelayCommand<object>((p) => true, (p) => Delete());
            AddNewGuestCommand = new RelayCommand<object>((p) => true, (p) => { IsOpenDialog = true; DialogPropertiesChanged(); });
            SaveGuestCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(GuestName) && string.IsNullOrEmpty(GuestName.ToString()))
                    return false;
                else return true;
            }, (p) =>
            {
                SaveGuest();
            });
            EditCommand = new RelayCommand<object>((p) => true, (p) => { IsOpenDialog = true; DialogPropertiesChanged(); });
        }
        #endregion

        #region New Guest
        public void DialogPropertiesChanged()
        {
            ErrorMessage = "";
            GuestName = "";
            GuestName = "";
        }

        public void SaveGuest()
        {
            var guestCount = DataProvider.Instance.DB.GUESTs.Where(x => x.id == GuestID).Count();
            if (guestCount > 0)
            {
                ErrorMessage = "\"" + guestName + "\"" + " has already existed";
                guestName = "";
                return;
            }
            else
            {
                GUEST guest = new GUEST()
                {
                    id = GuestID,
                    name = GuestName,
                    gender = GuestGender,
                    email = GuestEmail,
                    phone = GuestPhone,
                    address = GuestAddress,
                    birthday = GuestBirthday,
                };
                DataProvider.Instance.DB.GUESTs.Add(guest);
                DataProvider.Instance.DB.SaveChanges();
            }
            LoadGuest();
        }
        #endregion

        #region Delete
        private void Delete()
        {
            GUEST item = SelectedGuest;
            
            DataProvider.Instance.DB.SaveChanges();
            LoadGuest();
        }
        #endregion

        #region Search
        void Search()
        {
            switch (SelectedSearchType)
            {
                case "ID":
                    ItemSource = new ObservableCollection<GUEST>(
                        DataProvider.Instance.DB.GUESTs.Where(
                            x => x.id.ToString().Contains(ContentSearch)));
                    break;
                case "Name":
                    ItemSource = new ObservableCollection<GUEST>(
                        DataProvider.Instance.DB.GUESTs.Where(
                            x => x.name.ToString().Contains(ContentSearch)));
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Load
        private void LoadGuest()
        {
            IsOpenDialog = false;
            this.ItemSource.Clear();
            List<GUEST> list = GetGuests();
            foreach (var item in list)
            {
                ItemSource.Add(item);
            }
            SearchTypes = new List<string>();
            SearchTypes.Add("ID");
            SearchTypes.Add("Name");

            SelectedSearchType = "ID";
        }

        private List<GUEST> GetGuests()
        {
            List<GUEST> res = new List<GUEST>();
            res = DataProvider.Instance.DB.GUESTs.ToList();
            return res;
        }
        #endregion
    }
}
