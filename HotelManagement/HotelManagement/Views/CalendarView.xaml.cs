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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TagBites.WinSchedulers;
using TagBites.WinSchedulers.Drawing;

namespace HotelManagement.Views
{
    /// <summary>
    /// Interaction logic for CalendarView.xaml
    /// </summary>
    public partial class CalendarView : UserControl
    {
        public CalendarView()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            SC_Scheduler.DataSource = new SchedulerDataSource();

            SC_Scheduler.ViewOptions.ExactTaskDates = true;
            SC_Scheduler.ViewOptions.InnerLines = true;
            SC_Scheduler.ViewOptions.FadeNotSelectedOrNotConnected = true;

            SC_Scheduler.TimeScroller.Scale = TimeSpan.FromDays(0.2);
            SC_Scheduler.ResourceScroller.HeaderSize = 75;

            SC_Scheduler.VerticalTimeLine = false;
          
            SC_Scheduler.ScrollTo(DateTime.Now, Alignment.Center);
        }
    }
}
