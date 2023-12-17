using HotelManagement.Models;
using HotelManagement.Resources.UC;
using HotelManagement.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace HotelManagement.ViewModels
{
    class RoomsViewModels : BaseViewModel
    {
        public string Title { get; } = "Rooms";
        public RoomsView roomView { get; set; }
        private List<ROOM> listRooms;

        private ObservableCollection<ROOMTYPE> itemsSourceType = new ObservableCollection<ROOMTYPE>();
        public ObservableCollection<ROOMTYPE> ItemsSourceType
        {
            get => itemsSourceType;
            set
            {
                itemsSourceType = value;
                OnPropertyChanged();
            }
        }
        private readonly Color[] m_colors =
        {
            Color.FromRgb(178, 191, 229),
            Color.FromRgb(178,223, 229),
            Color.FromRgb(178, 229, 203),
            Color.FromRgb(184, 229, 178),
            Color.FromRgb(197, 178, 229),
            Color.FromRgb(216, 229, 178),
            Color.FromRgb(229, 178, 178),
            Color.FromRgb(229,178,197),
            Color.FromRgb(229, 178, 229),
            Color.FromRgb(229, 210, 178),
        };

        #region icommand
        public ICommand LoadRoomCommand { get; set; }
        public ICommand ChosenRoomCommand { get; set; }
        public ICommand MouseRightCommand { get; set; }
        public ICommand SearchRoomCommand { get; set; }
        public ICommand ResetSearchCommand { get; set; }
        public ICommand ShowRoomCommand { get; set; }
        public ICommand ShowTypeCommand { get; set; }
        public ICommand OpenAddRoomCommand { get; set; }
        public ICommand OpenAddTypeCommand { get; set; }
        public ICommand OpenAddTypefromRoomCommand { get; set; }
        public ICommand EditTypeCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand OpenEditRoomCommand { get; set; }
        public ICommand SaveRoomCommand { get; set; }
        public ICommand SaveTypeCommand {get; set;}
        public ICommand DeleteTypeCommand { get; set; }
        public ICommand DeleteRoomCommand { get; set; }
        public ICommand ToggleDirtyCommand { get; set; }
        public ICommand ToggleOutServiceCommand { get; set; }
        private ICommand _newReservationCommand;
        public ICommand NewReservationCommand
        {
            get
            {
                return _newReservationCommand ?? (_newReservationCommand = new RelayCommand<object>((p) => true, (p) => OpenNewReservationWindow()));
            }
        }

        private void OpenNewReservationWindow()
        {
            var wd = new NewReservationWindow();
            wd.DataContext = new NewReservationViewModel();
            wd.Show();
        }

        #endregion
        public RoomsViewModels()
        {
            LoadRoomCommand = new RelayCommand<RoomsView>((para) => true, (para) => LoadRoom(para));
            ChosenRoomCommand = new RelayCommand<UC_Room>((para) => true, (para) => LoadDataRoom(para));
            MouseRightCommand = new RelayCommand<UC_Room>((para) => true, (para) => MouseRightRoom(para));
            SearchRoomCommand = new RelayCommand<RoomsView>((para) => true, (para) => SearchRoom(para));
            ResetSearchCommand = new RelayCommand<RoomsView>((para) => true, (para) => ResetSearchRoom(para));
            ShowRoomCommand = new RelayCommand<RoomsView>((para) => true, (para) => ShowRoom(para));
            ShowTypeCommand = new RelayCommand<RoomsView>((para) => true, (para) => ShowType(para));
            OpenAddRoomCommand = new RelayCommand<RoomsView>((para) => true, (para) => OpenAddRoom(para));
            OpenAddTypeCommand = new RelayCommand<RoomsView>((para) => true, (para) => OpenAddType(para));
            OpenAddTypefromRoomCommand = new RelayCommand<AddRoomWindow>((para) => true, (para) => OpenAddRoomType(para));
            EditTypeCommand = new RelayCommand<UC_RoomType>((para) => true, (para) => EditType(para));
            CloseCommand = new RelayCommand<Window>((para) => true, (para) => para.Close());
            OpenEditRoomCommand = new RelayCommand<RoomsView>((para) => true, (para) => EditRoom(para));
            SaveRoomCommand = new RelayCommand<AddRoomWindow>((para) => true, (para) => SaveRoom(para));
            SaveTypeCommand = new RelayCommand<AddTypeWindow>((para) => true, (para) => SaveType(para));
            DeleteTypeCommand = new RelayCommand<UC_RoomType>((para) => true, (para) => DeleteType(para));
            DeleteRoomCommand = new RelayCommand<RoomsView>((para) => true, (para) => DeleteRoom(para));
            ToggleDirtyCommand = new RelayCommand<ToggleButton>((para) => true, (para) => ExcuteDirty(para));
            ToggleOutServiceCommand = new RelayCommand<ToggleButton>((para) => true, (para) => ExcuteOutService(para));
        }

        private void ExcuteOutService(ToggleButton para)
        {
            Grid grid = (Grid)para.Parent;
            Grid grid1 = (Grid)grid.Parent;
            this.roomView = (RoomsView)grid1.Parent;
            if (this.roomView.txbNameRoom.Text != "")
            {
                int id = int.Parse(this.roomView.txb_id.Text);
                ROOM room = DataProvider.Instance.DB.ROOMs.Where(x => x.id == id).FirstOrDefault();
                if (para.IsChecked == true)
                {
                    room.out_of_service = true;
                }
                else
                {
                    room.out_of_service = false;
                }

                DataProvider.Instance.DB.ROOMs.AddOrUpdate(room);
                DataProvider.Instance.DB.SaveChanges();

                foreach (UC_Room item in this.roomView.stkRoom.Children)
                {
                    if (item.txbID.Text == id.ToString())
                    {
                        item.Background = loadColor(item.txbStatus.Text);
                        if (room.dirty == true)
                        {
                            item.Background = (Brush)new BrushConverter().ConvertFrom("#FDD835");
                        }
                        
                        if (room.out_of_service == true)
                        {
                            item.Background = (Brush)new BrushConverter().ConvertFrom("#EF5350");
                        }
                    }
                }
            }
        }

        private void ExcuteDirty(ToggleButton para)
        {
            Grid grid = (Grid)para.Parent;
            Grid grid1 = (Grid)grid.Parent;
            this.roomView = (RoomsView)grid1.Parent;
            if (this.roomView.txbNameRoom.Text != "")
            {
                int id = int.Parse(this.roomView.txb_id.Text);
                ROOM room = DataProvider.Instance.DB.ROOMs.Where(x => x.id == id).FirstOrDefault();
                if (para.IsChecked == true)
                {
                    room.dirty = true;
                }
                else
                {
                    room.dirty = false;
                }

                DataProvider.Instance.DB.ROOMs.AddOrUpdate(room);
                DataProvider.Instance.DB.SaveChanges();

                foreach (UC_Room item in this.roomView.stkRoom.Children)
                {
                    if (item.txbID.Text == id.ToString())
                    {
                        item.Background = loadColor(item.txbStatus.Text);
                        if (room.dirty == true)
                        {
                            item.Background = (Brush)new BrushConverter().ConvertFrom("#FDD835");
                        }
                        if (room.out_of_service == true)
                        {
                            item.Background = (Brush)new BrushConverter().ConvertFrom("#EF5350");
                        }
                    }
                }
            }
        }

        private void OpenAddRoomType(AddRoomWindow para)
        {
            AddTypeWindow wd = new AddTypeWindow(para.rooms);
            try
            {
                ROOMTYPE type = DataProvider.Instance.DB.ROOMTYPEs.Where(x => x.date_updated == null).ToList().Last();
                wd.txtIDType.Text = (type.id + 1).ToString();
            }
            catch
            {
                wd.txtIDType.Text = "100";
            }
            finally
            {
                wd.ShowDialog();
            }
        }

        private void DeleteType(UC_RoomType para)
        {
            MessageBoxResult res = MessageBox.Show("Are you sure?", "Notify", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                int id = int.Parse(para.txbID.Text);

                ROOMTYPE type = DataProvider.Instance.DB.ROOMTYPEs.Where(x => x.id == id).FirstOrDefault();

                type.date_updated = DateTime.Now;

                DataProvider.Instance.DB.ROOMTYPEs.AddOrUpdate(type);
                DataProvider.Instance.DB.SaveChanges();

                StackPanel stk = (StackPanel)para.Parent;
                stk.Children.Remove(para);
            }
        }

        private void DeleteRoom(RoomsView para)
        {
            if (para.txbNameRoom.Text != "")
            {
                MessageBoxResult res = MessageBox.Show("Are you sure?", "Notify", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    int id = int.Parse(para.txb_id.Text);

                    ROOM room = DataProvider.Instance.DB.ROOMs.Where(x => x.id == id).FirstOrDefault();

                    room.isActive = false;

                    DataProvider.Instance.DB.ROOMs.AddOrUpdate(room);
                    DataProvider.Instance.DB.SaveChanges();

                    foreach (UC_Room item in para.stkRoom.Children)
                    {
                        if (item.txbID.Text == id.ToString())
                        {
                            para.stkRoom.Children.Remove(item);
                            break;
                        }
                    }
                }
            }
        }

        private void SaveType(AddTypeWindow para)
        {
            if (CheckSaveType(para))
            {
                if (para.title.Text == "NEW TYPE")
                {
                    ROOMTYPE type = new ROOMTYPE();

                    type.name = para.txtName.Text;
                    type.price = decimal.Parse(para.txtPrice.Text);
                    type.max_guest = int.Parse(para.txtMax.Text);
                    type.date_created = DateTime.Now;
                    type.date_updated = null;

                    DataProvider.Instance.DB.ROOMTYPEs.Add(type);
                    DataProvider.Instance.DB.SaveChanges();

                    MessageBox.Show("Completed!!!", "Notify", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    UC_RoomType item = new UC_RoomType();
                    item.txbID.Text = para.txtIDType.Text;
                    item.txbName.Text = para.txtName.Text;
                    item.txbPrice.Text = para.txtPrice.Text;
                    item.txbMax.Text = para.txtMax.Text;
                    item.txbDateCreate.Text = type.date_created.ToString();
                    item.txbcOUNT.Text = "0";

                    item.Height = 40;
                    item.Width = para.rooms.stkType.Width;

                    para.rooms.stkType.Children.Add(item);
                    para.Close();


                }
                else
                {
                    ROOMTYPE type = new ROOMTYPE();
                    int id = int.Parse(para.txtIDType.Text);

                    type = DataProvider.Instance.DB.ROOMTYPEs.Where(x => x.id == id).FirstOrDefault();
                    type.date_updated = DateTime.Now;

                    DataProvider.Instance.DB.ROOMTYPEs.AddOrUpdate(type);
                    DataProvider.Instance.DB.SaveChanges();

                    foreach (UC_RoomType uc in para.rooms.stkType.Children)
                    {
                        if (uc.txbID.Text == para.txtIDType.Text)
                        {
                            para.rooms.stkType.Children.Remove(uc);
                            break;
                        }
                    }

                    ROOMTYPE newType = new ROOMTYPE();
                    newType.name = para.txtName.Text;
                    newType.price = decimal.Parse(para.txtPrice.Text);
                    newType.max_guest = int.Parse(para.txtMax.Text);
                    newType.date_created = DateTime.Now;
                    newType.date_updated = null;

                    DataProvider.Instance.DB.ROOMTYPEs.Add(newType);
                    DataProvider.Instance.DB.SaveChanges();

                    List<ROOM> rooms = DataProvider.Instance.DB.ROOMs.Where(x => x.roomtype_id == id).ToList();
                    foreach (ROOM room in rooms)
                    {
                        room.roomtype_id = newType.id;

                        DataProvider.Instance.DB.ROOMs.AddOrUpdate(room);
                        DataProvider.Instance.DB.SaveChanges();
                    }

                    MessageBox.Show("Completed!!!", "Notify", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    UC_RoomType item = new UC_RoomType();
                    item.txbID.Text = para.txtIDType.Text;
                    item.txbName.Text = para.txtName.Text;
                    item.txbPrice.Text = para.txtPrice.Text;
                    item.txbMax.Text = para.txtMax.Text;
                    item.txbDateCreate.Text = type.date_created.ToString();
                    item.txbcOUNT.Text = "0";

                    item.Height = 40;
                    item.Width = para.rooms.stkType.Width;

                    para.rooms.stkType.Children.Add(item);
                    para.Close();
                }

                para.rooms.stkType.Children.Clear();
                LoadType(para.rooms);
            }
        }

        private void SaveRoom(AddRoomWindow para)
        {
            if (CheckSaveRoom(para))
            {
                if (para.title.Text == "NEW ROOM")
                {
                    ROOM room = new ROOM();

                    room.name = para.txtNameRoom.Text;
                    room.roomtype_id = DataProvider.Instance.DB.ROOMTYPEs.Where(x => x.name == para.cbbType.Text && x.date_updated == null).FirstOrDefault().id;
                    room.notes = para.txtNote.Text;
                    room.dirty = false;
                    room.out_of_service = false;
                    room.isActive = true;

                    DataProvider.Instance.DB.ROOMs.Add(room);
                    DataProvider.Instance.DB.SaveChanges();

                    MessageBox.Show("Completed!!!", "Notify", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    UC_Room item = new UC_Room();
                    item.txbName.Text = room.name;
                    item.txbID.Text = para.txtIDRoom.Text;
                    item.txbType.Text = para.cbbType.Text;
                    item.txbStatus.Text = "Available";
                    item.Width = 100;
                    item.Height = 100;

                    para.rooms.stkRoom.Children.Add(item);
                    para.Close();
                }
                else
                {
                    ROOM room = new ROOM();
                    int id = int.Parse(para.txtIDRoom.Text);

                    room = DataProvider.Instance.DB.ROOMs.Where(x => x.id == id).FirstOrDefault();
                    room.name = para.txtNameRoom.Text;
                    room.roomtype_id = DataProvider.Instance.DB.ROOMTYPEs.Where(x => x.name == para.cbbType.Text && x.date_updated == null).FirstOrDefault().id;
                    room.notes = para.txtNote.Text;
                    DataProvider.Instance.DB.ROOMs.AddOrUpdate(room);
                    DataProvider.Instance.DB.SaveChanges();

                    MessageBox.Show("Completed!!!", "Notify", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    foreach (UC_Room item in para.rooms.stkRoom.Children)
                    {
                        if (item.txbID.Text == para.txtIDRoom.Text)
                        {
                            item.txbName.Text = para.txtNameRoom.Text;
                            item.txbType.Text = para.cbbType.Text;
                        }
                    }
                    para.Close();
                }
            }
        }

        private void EditRoom(RoomsView para)
        {
            if (para.txbNameRoom.Text != "")
            {
                AddRoomWindow wd = new AddRoomWindow(para);

                List<ROOMTYPE> listType = DataProvider.Instance.DB.ROOMTYPEs.Where(x => x.date_updated == null).ToList();
                foreach (ROOMTYPE type in listType)
                {
                    wd.cbbType.Items.Add(type.name);
                }
                wd.txtIDRoom.Text = DataProvider.Instance.DB.ROOMs.Where(x => x.name == para.txbNameRoom.Text).FirstOrDefault().id.ToString();
                wd.title.Text = "EDIT ROOM";
                wd.txtNameRoom.Text = para.txbNameRoom.Text;
                wd.txtPrice.Text = para.txbPriceRoom.Text;
                wd.txtNote.Text = para.txbNoteRoom.Text;
                wd.cbbType.Text = para.txbTypeRoom.Text;

                wd.ShowDialog();
            }
        }

        private void EditType(UC_RoomType para)
        {
            StackPanel stk = (StackPanel)para.Parent;
            ScrollViewer viewer = (ScrollViewer)stk.Parent;
            Grid grid1 = (Grid)viewer.Parent;
            Grid grid2 = (Grid)grid1.Parent;
            Grid grid3 = (Grid)grid2.Parent;
            this.roomView = (RoomsView)grid3.Parent;
            AddTypeWindow wd = new AddTypeWindow(this.roomView);

            wd.title.Text = "EDIT TYPE";
            wd.txtIDType.Text = para.txbID.Text;
            wd.txtName.Text = para.txbName.Text;
            wd.txtPrice.Text = para.txbPrice.Text;
            wd.txtMax.Text = para.txbMax.Text;
            wd.txtDateCreate.Text = para.txbDateCreate.Text;

            wd.ShowDialog();
        }

        private void OpenAddType(RoomsView para)
        {
            AddTypeWindow wd = new AddTypeWindow(para);
            try
            {
                ROOMTYPE type = DataProvider.Instance.DB.ROOMTYPEs.Where(x => x.date_updated == null).ToList().Last();
                wd.txtIDType.Text = (type.id + 1).ToString();
            }
            catch
            {
                wd.txtIDType.Text = "100";
            }
            finally
            {
                wd.ShowDialog();
            }

        }

        private void OpenAddRoom(RoomsView para)
        {
            AddRoomWindow wd = new AddRoomWindow(para);
            try
            {
                ROOM room = DataProvider.Instance.DB.ROOMs.Where(x => x.isActive == true).ToList().Last();
                wd.txtIDRoom.Text = (room.id + 1).ToString();
            }
            catch
            {
                wd.txtIDRoom.Text = "100";
            }
            finally
            {
                List<ROOMTYPE> listType = DataProvider.Instance.DB.ROOMTYPEs.Where(x => x.date_updated == null).ToList();
                foreach (ROOMTYPE type in listType)
                {
                    wd.cbbType.Items.Add(type.name);
                }
                wd.ShowDialog();
            }
        }

        private void ShowType(RoomsView para)
        {
            para.stkType.Visibility = System.Windows.Visibility.Visible;
            para.btnAddType.Visibility = System.Windows.Visibility.Visible;
            para.btnReset.IsEnabled = false;
            para.btnSearch.IsEnabled = false;
            para.stkRoom.Visibility = System.Windows.Visibility.Collapsed;
            para.scrRoom.Visibility = Visibility.Collapsed;
            para.btnAddRoom.Visibility = System.Windows.Visibility.Collapsed;

            para.grdStatus.Visibility = Visibility.Collapsed;
            ResetInfo(para);
        }

        private void LoadType(RoomsView para)
        {
            List<ROOMTYPE> listType = DataProvider.Instance.DB.ROOMTYPEs.Where(x => x.date_updated == null).ToList();

            UC_RoomType title = new UC_RoomType();
            title.Width = para.stkType.Width;
            title.btnDelete.Visibility = System.Windows.Visibility.Hidden;
            title.btnEdit.Visibility = System.Windows.Visibility.Hidden;
            para.stkType.Children.Add(title);
            foreach (ROOMTYPE item in listType)
            {
                UC_RoomType uc = new UC_RoomType();
                uc.txbID.Text = item.id.ToString();
                uc.txbName.Text = item.name;
                uc.txbPrice.Text = SeparateThousands(((long)item.price).ToString());
                uc.txbMax.Text = item.max_guest.ToString();
                uc.txbDateCreate.Text = item.date_created.ToString();
                uc.txbcOUNT.Text = DataProvider.Instance.DB.ROOMs.Where(x => x.roomtype_id == item.id).Count().ToString();

                uc.Height = 40;
                uc.Width = para.stkType.Width;

                para.stkType.Children.Add(uc);
            }
        }

        private void ShowRoom(RoomsView para)
        {
            para.stkType.Visibility = System.Windows.Visibility.Collapsed;
            para.btnAddType.Visibility = System.Windows.Visibility.Collapsed;
            para.btnReset.IsEnabled = true;
            para.btnSearch.IsEnabled = true;
            para.stkRoom.Visibility = System.Windows.Visibility.Visible;
            para.scrRoom.Visibility = Visibility.Visible;
            para.btnAddRoom.Visibility = System.Windows.Visibility.Visible;
            para.grdStatus.Visibility = Visibility.Visible;
        }

        private void ResetSearchRoom(RoomsView para)
        {
            para.cbbStatus.SelectedIndex = 0;
            para.cbbType.SelectedIndex = 0;
            para.cbbFloor.SelectedIndex = 0;

            foreach (UC_Room item in para.stkRoom.Children)
            {
                item.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void InitItemsSourceType()
        {
            this.ItemsSourceType.Clear();

            ROOMTYPE item = new ROOMTYPE();
            item.name = "All";
            this.ItemsSourceType.Add(item);

            List<ROOMTYPE> list = DataProvider.Instance.DB.ROOMTYPEs.Where(x => x.date_updated== null).ToList();
            foreach (ROOMTYPE type in list)
            {
                this.itemsSourceType.Add(type);
            }
        }

        private void InitItemsSourceFloor(RoomsView para)
        {
            para.cbbFloor.Items.Clear();
            this.listRooms = DataProvider.Instance.DB.ROOMs.Where(x => x.isActive == true).ToList();

            List<string> listFloor = new List<string> { "All" };
            foreach (var item in listRooms)
            {
                for (int i = 0; i < listFloor.Count; i++)
                {
                    if (item.name[0].ToString() == listFloor[i])
                        break;
                    else if (i == listFloor.Count - 1)
                        listFloor.Add(item.name[0].ToString());
                }
            }

            foreach (var item in listFloor)
            {
                para.cbbFloor.Items.Add(item);
            }
        }

        private void SearchRoom(RoomsView para)
        {
            bool flag = true;
            if (para.cbbType.Text == "")
                para.cbbType.SelectedIndex = 0;
            if (para.cbbFloor.Text == "")
                para.cbbFloor.SelectedIndex = 0; 
            if (para.cbbStatus.Text == "")
                para.cbbStatus.SelectedIndex = 0;
            foreach (UC_Room item in para.stkRoom.Children)
            {
                flag = true;

                if (item.txbType.Text != para.cbbType.Text && para.cbbType.Text != "All")
                    flag = false;

                if (item.txbName.Text[0].ToString() != para.cbbFloor.Text && para.cbbFloor.Text != "All")
                    flag = false;

                if (item.txbStatus.Text != para.cbbStatus.Text && para.cbbStatus.Text != "All")
                {
                        flag = false;
                }

                if (flag == true)
                    item.Visibility = System.Windows.Visibility.Visible;
                else
                    item.Visibility = System.Windows.Visibility.Collapsed;

            }
        }

        private void MouseRightRoom(UC_Room para)
        {

        }

        private void LoadDataRoom(UC_Room para)
        {
            WrapPanel wrap = (WrapPanel)para.Parent;
            ScrollViewer viewer = (ScrollViewer)wrap.Parent;
            Grid grid1 = (Grid)viewer.Parent;
            Grid grid2 = (Grid)grid1.Parent;
            Grid grid3 = (Grid)grid2.Parent;
            this.roomView = (RoomsView)grid3.Parent;
            ResetInfoCustom(roomView);
            this.roomView.txb_id.Text = para.txbID.Text;
            int id = int.Parse(para.txbID.Text);
            ROOM room = DataProvider.Instance.DB.ROOMs.Where(x => x.id == id).FirstOrDefault();
            List<ROOM_BOOKED> listR = DataProvider.Instance.DB.ROOM_BOOKED.Where(x => x.room_id == id).ToList();
            foreach (ROOM_BOOKED item in listR)
            {
                RESERVATION reser = DataProvider.Instance.DB.RESERVATIONs.Where(x => x.id == item.reservation_id).FirstOrDefault();
                if (reser.arrival <= DateTime.Now && reser.departure >= DateTime.Now)
                {
                    GUEST guest = DataProvider.Instance.DB.GUESTs.Where(x => x.id.ToString() == reser.main_guest).FirstOrDefault();
                    this.roomView.txbNameCustom.Text = guest.name;
                    roomView.txbGenderCustom.Text = guest.gender;
                    roomView.txbEmailCustom.Text = guest.email;
                    roomView.txbIDCustom.Text = guest.id;
                    roomView.txbPhoneCustom.Text = guest.phone;
                    roomView.txbArrivalCustom.Text = reser.arrival.ToString();
                    roomView.txbDepCustom.Text = reser.departure.ToString();
                    roomView.txbStatusRoom.Text = reser.status;
                    roomView.txbCountRoom.Text = DataProvider.Instance.DB.GUEST_BOOKING.Where(x => x.reservation_id == reser.id && x.room_booked_id == item.id).Count().ToString();
                    break;
                }
            }
            ROOMTYPE type = DataProvider.Instance.DB.ROOMTYPEs.Where(x => x.id == room.roomtype_id).FirstOrDefault();

            roomView.txbTypeRoom.Text = type.name;
            roomView.txbPriceRoom.Text = SeparateThousands(((long)type.price).ToString());
            roomView.txbMaxRoom.Text = type.max_guest.ToString();
            roomView.txbNoteRoom.Text = room.notes;
            roomView.txbNameRoom.Text = room.name;
            roomView.togDirty.IsChecked = room.dirty;
            roomView.togOutService.IsChecked = room.out_of_service;
        }

        private void LoadRoom(RoomsView para)
        {
            this.roomView = para;
            this.listRooms = DataProvider.Instance.DB.ROOMs.Where(x => x.isActive == true).ToList();

            foreach (ROOM item in this.listRooms)
            {
                UC_Room room = new UC_Room();
                room.txbName.Text = item.name;
                room.txbType.Text = item.ROOMTYPE.name;
                room.txbID.Text = item.id.ToString();
                room.Width = 100;
                room.Height = 100;

                List<ROOM_BOOKED> listR = DataProvider.Instance.DB.ROOM_BOOKED.Where(x => x.room_id == item.id).ToList();
                foreach (ROOM_BOOKED booked in listR)
                {
                    RESERVATION reser = DataProvider.Instance.DB.RESERVATIONs.Where(x => x.id == booked.reservation_id).FirstOrDefault();
                    if (reser.arrival <= DateTime.Now && reser.departure >= DateTime.Now)
                    {
                        string status = reser.status;
                        switch (status)
                        {
                            case "Operational":
                                if (reser.departure.Value.Day == DateTime.Now.Day && reser.departure.Value.Month == DateTime.Now.Month && reser.departure.Value.Year == DateTime.Now.Year)
                                {
                                    room.txbStatus.Text = "Due Out";
                                    room.Background = (Brush)new BrushConverter().ConvertFrom("#2196F3");
                                } else
                                {
                                    room.txbStatus.Text = "Occupied";
                                    room.Background = (Brush)new BrushConverter().ConvertFrom("#AB47BC");
                                }
                                break;
                            case "No Show":
                                room.txbStatus.Text = "No Show";
                                room.Background = (Brush)new BrushConverter().ConvertFrom("#FF3D00");
                                break;
                            case "Completed":
                            case "Cancelled":
                                room.txbStatus.Text = "Available";
                                room.Background = (Brush)new BrushConverter().ConvertFrom("#FFD0D0D0");
                                break;
                            case "Confirmed":
                            case "On Request":
                                room.txbStatus.Text = "Reserved";
                                room.Background = (Brush)new BrushConverter().ConvertFrom("#7CB342");
                                break;
                            default:
                                break;
                        }
                        break;
                    }
                    else
                        room.txbStatus.Text = "Available";
                }
                if (item.dirty == true)
                {
                    room.Background = (Brush)new BrushConverter().ConvertFrom("#FDD835");
                }
                if (item.out_of_service == true)
                {
                    room.Background = (Brush)new BrushConverter().ConvertFrom("#EF5350");
                }
                para.stkRoom.Children.Add(room);
            }
            InitItemsSourceType();
            InitItemsSourceFloor(para);
            LoadType(para);
            permissionLogin(roomView);
            para.stkType.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void ResetInfoCustom(RoomsView para)
        {
            para.txbNameCustom.Text = "";
            para.txbGenderCustom.Text = "";
            para.txbEmailCustom.Text = "";
            para.txbIDCustom.Text = "";
            para.txbPhoneCustom.Text = "";
            para.txbArrivalCustom.Text = "";
            para.txbDepCustom.Text = "";
            para.txbStatusRoom.Text = "";
            para.txbCountRoom.Text = "";
        }

        private void ResetInfo(RoomsView para)
        {
            para.txbNameCustom.Text = "";
            para.txbGenderCustom.Text = "";
            para.txbEmailCustom.Text = "";
            para.txbIDCustom.Text = "";
            para.txbPhoneCustom.Text = "";
            para.txbArrivalCustom.Text = "";
            para.txbDepCustom.Text = "";
            para.txbStatusRoom.Text = "";
            para.txbCountRoom.Text = "";
            para.txbMaxRoom.Text = "";
            para.txbNameRoom.Text = "";
            para.txbNoteRoom.Text = "";
            para.txbPriceRoom.Text = "";
            para.txbStatusRoom.Text = "";
            para.txbTypeRoom.Text = "";
        }

        private void permissionLogin(RoomsView view)
        {
            string permission = CurrentAccount.Instance.Permission;

            switch (permission)
            {
                case "Admin":
                default:
                    return;
                case "Reservation":
                    view.btnAddType.IsEnabled = false;
                    editTypePermission(view);
                    return;
                case "Receptionist":
                case "Cashier":
                case "Undefined":
                    view.btnAddRoom.IsEnabled = false;
                    view.btnAddType.IsEnabled = false;
                    view.btnMoreRoom.IsEnabled = false;
                    view.btnDelRoom.IsEnabled = false;
                    editTypePermission(view);
                    return;
            }
        }

        void editTypePermission(RoomsView para)
        {
            foreach (UC_RoomType item in para.stkType.Children)
            {
                item.btnEdit.IsEnabled = false;
                item.btnDelete.IsEnabled = false;
            }
        }

        private Brush loadColor(string status)
        {
            switch (status)
            {
                case "Due out":
                    return (Brush)new BrushConverter().ConvertFrom("#2196F3");
                case "Occupied":
                    return (Brush)new BrushConverter().ConvertFrom("#AB47BC");
                case "No Show":
                    return (Brush)new BrushConverter().ConvertFrom("#FF3D00");
                case "Available":
                    return (Brush)new BrushConverter().ConvertFrom("#FFD0D0D0");
                case "Reserved":
                    return (Brush)new BrushConverter().ConvertFrom("#7CB342");
            }
            return null;
        }

        public bool CheckSaveRoom(AddRoomWindow para)
        {
            if (string.IsNullOrEmpty(para.txtNameRoom.Text) || string.IsNullOrEmpty(para.cbbType.Text))
            {
                para.txbError.Visibility = Visibility.Visible;
                return false;
            }
            else
                para.txbError.Visibility = Visibility.Hidden;
            return true;

        }

        public bool CheckSaveType(AddTypeWindow para)
        {
            if (string.IsNullOrEmpty(para.txtName.Text) || string.IsNullOrEmpty(para.txtPrice.Text) || string.IsNullOrEmpty(para.txtMax.Text))
            {
                para.txbError.Visibility = Visibility.Visible;
                return false;
            }
            else
                para.txbError.Visibility = Visibility.Hidden;
            return true;

        }

    }
}
