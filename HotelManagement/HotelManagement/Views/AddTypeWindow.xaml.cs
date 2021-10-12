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
    /// Interaction logic for AddTypeWindow.xaml
    /// </summary>
    public partial class AddTypeWindow : Window
    {
        public RoomsView rooms { get; set; }
        public AddTypeWindow(RoomsView room)
        {
            this.rooms = room;
            InitializeComponent();
        }

        private void wdAddType_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
