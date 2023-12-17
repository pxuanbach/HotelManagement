using HotelManagement.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace HotelManagement.ViewModels
{
    class CalendarViewModel:BaseViewModel
    {
        public string Title { get; } = "Calendar";

        public ICommand LoadCommand { get; set; }
        public ICommand DateChangeCommand { get; set; }
        public ICommand MonthCommand { get; set; }
        public ICommand WeekCommand { get; set; }

        public CalendarViewModel()
        {
            LoadCommand =  new RelayCommand<CalendarView>((p) => true, (p) => Load(p));
            DateChangeCommand = new RelayCommand<CalendarView>((p) => true, (p) => DateChange(p));
            WeekCommand = new RelayCommand<CalendarView>((p) => true, (p) => WeekOption(p));
            MonthCommand = new RelayCommand<CalendarView>((p) => true, (p) => MonthOption(p));
        }

        private void MonthOption(CalendarView p)
        {
            DateTime selectedDate = p.datePciker.SelectedDate.Value;

            p.SC_Scheduler.VisibleDateTimeInterval = new TagBites.WinSchedulers.TimeSchedulerInterval(selectedDate.AddDays(-15), selectedDate.AddDays(15));
            p.SC_Scheduler.TimeScroller.Scale = TimeSpan.FromDays(0.5);

            p.SC_Scheduler.ScrollTo(selectedDate, TagBites.WinSchedulers.Drawing.Alignment.Center);
        }

        private void WeekOption(CalendarView p)
        {
            DateTime selectedDate = p.datePciker.SelectedDate.Value;

            p.SC_Scheduler.VisibleDateTimeInterval = new TagBites.WinSchedulers.TimeSchedulerInterval(selectedDate.AddDays(-3), selectedDate.AddDays(3));
            p.SC_Scheduler.TimeScroller.Scale = TimeSpan.FromDays(0.2);

            p.SC_Scheduler.ScrollTo(selectedDate, TagBites.WinSchedulers.Drawing.Alignment.Center);
        }

        private void DateChange(CalendarView p)
        {
            if (p.btnWeek.IsChecked == true)
            {
                WeekOption(p);
            }
            else
            {
                MonthOption(p);
            }
        }

        private void Load(CalendarView p)
        {
            p.SC_Scheduler.VisibleDateTimeInterval = new TagBites.WinSchedulers.TimeSchedulerInterval(DateTime.Today.AddDays(-3), DateTime.Today.AddDays(3));
        }
    }
}
