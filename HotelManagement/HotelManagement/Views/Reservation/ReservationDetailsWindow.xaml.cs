using HotelManagement.Models;
using HotelManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for NewReservationWindow.xaml
    /// </summary>
    public partial class ReservationDetailsWindow : Window
    {
        public ReservationDetailsWindow()
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
                if (swEdit.IsChecked == true)
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
                if (string.IsNullOrEmpty(this.autoTextBox.Text))
                {
                    // Disable.
                    this.CloseAutoSuggestionBox();

                    // Info.
                    return;
                }

                // Enable.
                this.OpenAutoSuggestionBox();

                // Settings.
                this.autoList.ItemsSource = this.SuggestGuestList.Where(p => p.id.Contains(this.autoTextBox.Text)).ToList();
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
                ((ReservationDetailsViewModel)this.DataContext).GuestInformation.ID = SelectedGuest.id;
                ((ReservationDetailsViewModel)this.DataContext).GuestInformation.Name = SelectedGuest.name;
                ((ReservationDetailsViewModel)this.DataContext).GuestInformation.Gender = SelectedGuest.gender;
                ((ReservationDetailsViewModel)this.DataContext).GuestInformation.Birthday = (DateTime)SelectedGuest.birthday;
                ((ReservationDetailsViewModel)this.DataContext).GuestInformation.Email = SelectedGuest.email;
                ((ReservationDetailsViewModel)this.DataContext).GuestInformation.Phone = SelectedGuest.phone;
                ((ReservationDetailsViewModel)this.DataContext).GuestInformation.Address = SelectedGuest.address;  
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
