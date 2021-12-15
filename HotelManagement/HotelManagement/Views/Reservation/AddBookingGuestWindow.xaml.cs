using HotelManagement.Models;
using HotelManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HotelManagement.Views
{
    /// <summary>
    /// Interaction logic for AddBookingGuestWindow.xaml
    /// </summary>
    public partial class AddBookingGuestWindow : Window
    {
        public AddBookingGuestWindow()
        {
            InitializeComponent();
            SuggestGuestList = DataProvider.Instance.DB.GUESTs.ToList();
        }

        private List<GUEST> suggestGuestList = new List<GUEST>();

        public List<GUEST> SuggestGuestList
        {
            get { return this.suggestGuestList; }
            set { this.suggestGuestList = value; }
        }

        private GUEST SelectedGuest { get; set; }

        private void OpenAutoSuggestionBox()
        {
            try
            {
                if (txtboxGuestID.Text.Length < 10)
                {
                    // Enable.
                    this.autoListPopup.Visibility = Visibility.Visible;
                    this.autoListPopup.IsOpen = true;
                    this.autoList.Visibility = Visibility.Visible;
                }    
                
            }
            catch (Exception ex)
            {
                // Info.
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.Write(ex);
            }
        }

        private void CloseAutoSuggestionBox()
        {
            try
            {
                // Enable.
                this.autoListPopup.Visibility = Visibility.Collapsed;
                this.autoListPopup.IsOpen = false;
                this.autoList.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                // Info.
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.Write(ex);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                // Verification.
                if (string.IsNullOrEmpty(this.txtboxGuestID.Text))
                {
                    // Disable.
                    this.CloseAutoSuggestionBox();

                    // Info.
                    return;
                }

                // Enable.
                this.OpenAutoSuggestionBox();

                // Settings.
                this.autoList.ItemsSource = this.SuggestGuestList.Where(p => p.id.Contains(this.txtboxGuestID.Text)).ToList();
                this.autoList.DisplayMemberPath = "id";
            }
            catch (Exception ex)
            {
                // Info.
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.Write(ex);
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Verification.
                if (this.autoList.SelectedIndex <= -1)
                {
                    // Disable.
                    this.CloseAutoSuggestionBox();

                    // Info.
                    return;
                }

                // Disable.
                this.CloseAutoSuggestionBox();

                // Settings.
                SelectedGuest = (GUEST)this.autoList.SelectedItem;
                if (this.DataContext.GetType() == typeof(ReservationDetailsViewModel))
                {
                    ((ReservationDetailsViewModel)this.DataContext).NewSharer.ID = SelectedGuest.id;
                    ((ReservationDetailsViewModel)this.DataContext).NewSharer.Name = SelectedGuest.name;
                    ((ReservationDetailsViewModel)this.DataContext).NewSharer.Gender = SelectedGuest.gender;
                    ((ReservationDetailsViewModel)this.DataContext).NewSharer.Birthday = (DateTime)SelectedGuest.birthday;
                    ((ReservationDetailsViewModel)this.DataContext).NewSharer.Email = SelectedGuest.email;
                    ((ReservationDetailsViewModel)this.DataContext).NewSharer.Phone = SelectedGuest.phone;
                    ((ReservationDetailsViewModel)this.DataContext).NewSharer.Address = SelectedGuest.address;
                }
                else if (this.DataContext.GetType() == typeof(NewReservationViewModel))
                {
                    ((NewReservationViewModel)this.DataContext).NewSharer.ID = SelectedGuest.id;
                    ((NewReservationViewModel)this.DataContext).NewSharer.Name = SelectedGuest.name;
                    ((NewReservationViewModel)this.DataContext).NewSharer.Gender = SelectedGuest.gender;
                    ((NewReservationViewModel)this.DataContext).NewSharer.Birthday = (DateTime)SelectedGuest.birthday;
                    ((NewReservationViewModel)this.DataContext).NewSharer.Email = SelectedGuest.email;
                    ((NewReservationViewModel)this.DataContext).NewSharer.Phone = SelectedGuest.phone;
                    ((NewReservationViewModel)this.DataContext).NewSharer.Address = SelectedGuest.address;
                }
                this.autoList.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                // Info.
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.Write(ex);
            }
        }
    }
}
