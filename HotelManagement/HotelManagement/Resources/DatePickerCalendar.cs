using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace HotelManagement.Resources
{
    public static class DatePickerCalendar
    {
        public static readonly DependencyProperty IsMonthYearProperty =
            DependencyProperty.RegisterAttached("IsMonthYear", typeof(bool), typeof(DatePickerCalendar),
                                                new FrameworkPropertyMetadata(false, OnIsMonthYearChanged));

        public static readonly DependencyProperty IsYearProperty =
            DependencyProperty.RegisterAttached("IsYear", typeof(bool), typeof(DatePickerCalendar),
                                                new FrameworkPropertyMetadata(false, OnIsYearChanged));

        public static bool GetIsMonthYear(DependencyObject dobj)
        {
            return (bool)dobj.GetValue(IsMonthYearProperty);
        }

        public static bool GetIsYear(DependencyObject dobj)
        {
            return (bool)dobj.GetValue(IsYearProperty);
        }

        public static void SetIsMonthYear(DependencyObject dobj, bool value)
        {
            dobj.SetValue(IsMonthYearProperty, value);
        }

        public static void SetIsYear(DependencyObject dobj, bool value)
        {
            dobj.SetValue(IsYearProperty, value);
        }

        private static void OnIsMonthYearChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
        {
            var datePicker = (DatePicker)dobj;

            Application.Current.Dispatcher
                .BeginInvoke(DispatcherPriority.Loaded,
                             new Action<DatePicker, DependencyPropertyChangedEventArgs>(SetCalendarMonthEventHandlers),
                             datePicker, e);
        }

        private static void OnIsYearChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
        {
            var datePicker = (DatePicker)dobj;

            Application.Current.Dispatcher
                .BeginInvoke(DispatcherPriority.Loaded,
                             new Action<DatePicker, DependencyPropertyChangedEventArgs>(SetCalendarYearEventHandlers),
                             datePicker, e);
        }

        private static void SetCalendarMonthEventHandlers(DatePicker datePicker, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
                return;

            if ((bool)e.NewValue)
            {
                datePicker.CalendarOpened += DatePickerOnCalendarOpened;
                datePicker.CalendarClosed += DatePickerOnCalendarClosed;
            }
            else
            {
                datePicker.CalendarOpened -= DatePickerOnCalendarOpened;
                datePicker.CalendarClosed -= DatePickerOnCalendarClosed;
            }
        }
        
        private static void SetCalendarYearEventHandlers(DatePicker datePicker, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
                return;

            if ((bool)e.NewValue)
            {
                datePicker.CalendarOpened += DatePickerOnCalendarYearOpened;
                datePicker.CalendarClosed += DatePickerOnCalendarYearClosed;
            }
            else
            {
                datePicker.CalendarOpened -= DatePickerOnCalendarYearOpened;
                datePicker.CalendarClosed -= DatePickerOnCalendarYearClosed;
            }
        }

        private static void DatePickerOnCalendarOpened(object sender, RoutedEventArgs routedEventArgs)
        {
            var calendar = GetDatePickerCalendar(sender);
            calendar.DisplayMode = CalendarMode.Year;

            calendar.DisplayModeChanged += CalendarOnDisplayModeChanged;

            calendar.KeyDown += Calendar_KeyDown;
        }

        private static void DatePickerOnCalendarYearOpened(object sender, RoutedEventArgs routedEventArgs)
        {
            var calendar = GetDatePickerCalendar(sender);
            calendar.DisplayMode = CalendarMode.Decade;

            calendar.DisplayModeChanged += CalendarYearOnDisplayModeChanged;

            calendar.KeyDown += Calendar_KeyDown;
        }

        private static void DatePickerOnCalendarClosed(object sender, RoutedEventArgs routedEventArgs)
        {
            var datePicker = (DatePicker)sender;
            var calendar = GetDatePickerCalendar(sender);
            if (calendar.SelectedDate.HasValue)
            {
                // warning, this might not be what you want, it's a pretty aggressive selection, where the selected date is changed even when keyboard navigating to a new date and then trying to cancel the selection
                calendar.SelectedDate = GetSelectedCalendarDate(calendar.DisplayDate);
            }
            datePicker.SelectedDate = calendar.SelectedDate;

            calendar.DisplayModeChanged -= CalendarOnDisplayModeChanged;

            calendar.KeyDown -= Calendar_KeyDown;
        }

        private static void DatePickerOnCalendarYearClosed(object sender, RoutedEventArgs routedEventArgs)
        {
            var datePicker = (DatePicker)sender;
            var calendar = GetDatePickerCalendar(sender);
            if (calendar.SelectedDate.HasValue)
            {
                // warning, this might not be what you want, it's a pretty aggressive selection, where the selected date is changed even when keyboard navigating to a new date and then trying to cancel the selection
                calendar.SelectedDate = GetSelectedCalendarDate(calendar.DisplayDate);
            }
            datePicker.SelectedDate = calendar.SelectedDate;

            calendar.DisplayModeChanged -= CalendarYearOnDisplayModeChanged;

            calendar.KeyDown -= Calendar_KeyDown;
        }

        private static void Calendar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                var c = (Calendar)sender;
                c.SelectedDate = GetSelectedCalendarDate(c.DisplayDate);
            }
        }

        private static void CalendarOnDisplayModeChanged(object sender, CalendarModeChangedEventArgs e)
        {
            var calendar = (Calendar)sender;
            if (calendar.DisplayMode != CalendarMode.Month)
                return;

            calendar.SelectedDate = GetSelectedCalendarDate(calendar.DisplayDate);

            var datePicker = GetCalendarsDatePicker(calendar);
            datePicker.IsDropDownOpen = false;
        }

        private static void CalendarYearOnDisplayModeChanged(object sender, CalendarModeChangedEventArgs e)
        {
            var calendar = (Calendar)sender;
            if (calendar.DisplayMode != CalendarMode.Year)
                return;

            calendar.SelectedDate = GetSelectedCalendarYearDate(calendar.DisplayDate);

            var datePicker = GetCalendarsDatePicker(calendar);
            datePicker.IsDropDownOpen = false;
        }

        private static Calendar GetDatePickerCalendar(object sender)
        {
            var datePicker = (DatePicker)sender;
            var popup = (Popup)datePicker.Template.FindName("PART_Popup", datePicker);
            return ((Calendar)popup.Child);
        }

        private static DatePicker GetCalendarsDatePicker(FrameworkElement child)
        {
            var parent = (FrameworkElement)child.Parent;
            if (parent.Name == "PART_Root")
                return (DatePicker)parent.TemplatedParent;
            return GetCalendarsDatePicker(parent);
        }

        private static DateTime? GetSelectedCalendarDate(DateTime? selectedDate)
        {
            if (!selectedDate.HasValue)
                return null;
            return new DateTime(selectedDate.Value.Year, selectedDate.Value.Month, 1);
        }

        private static DateTime? GetSelectedCalendarYearDate(DateTime? selectedDate)
        {
            if (!selectedDate.HasValue)
                return null;
            return new DateTime(selectedDate.Value.Year, 1, 1);
        }
    }
}
