using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HotelManagement.ViewModels
{
    class PageNavigationViewModel : BaseViewModel
    {
        private string _pageTitle;
        public string PageTitle { get { return _pageTitle; } set { _pageTitle = value; OnPropertyChanged(); } }

        private int _currentPage;
        public int CurrentPage { get { return _currentPage; } set { _currentPage = value; UpdateRecords(); OnPropertyChanged(); } }

        private int _maxPage;
        public int MaxPage { get { return _maxPage; } set { _maxPage = value; OnPropertyChanged(); } }

        private int _pageSize;
        public int PageSize { get { return _pageSize; } set { _pageSize = value; UpdateMaxPage(); OnPropertyChanged(); } }

        private int _sumRecords;
        public int SumRecords { get { return _sumRecords; } set { _sumRecords = value; UpdateMaxPage(); OnPropertyChanged(); } }

        private int _selectedRecords;
        public int SelectedRecords { get { return _selectedRecords; } set { _selectedRecords = value; OnPropertyChanged(); } }

        private int _exceptRecords;
        public int ExceptRecords { get { return _exceptRecords; } set { _exceptRecords = value; OnPropertyChanged(); } }

        private void UpdateMaxPage()
        {
            MaxPage = (SumRecords % PageSize != 0) ? (SumRecords / PageSize) + 1 : SumRecords / PageSize;
        }

        private void UpdateRecords()
        {
            SelectedRecords = PageSize * CurrentPage;
            ExceptRecords = (CurrentPage - 1) * PageSize;
            PageTitle = string.Format("{0} / {1}", CurrentPage, MaxPage);
        }

        private ICommand _firstPageCommand;
        public ICommand FirstPageCommand
        {
            get
            {
                return _firstPageCommand ?? (_firstPageCommand = new RelayCommand<object>((p) => { return CurrentPage != 1; }, (p) => { CurrentPage = 1; }));
            }
        }

        private ICommand _lastPageCommand;
        public ICommand LastPageCommand
        {
            get
            {
                return _lastPageCommand ?? (_lastPageCommand = new RelayCommand<object>((p) => { return CurrentPage != MaxPage; }, (p) => { CurrentPage = MaxPage; }));
            }
        }

        private ICommand _previousPageCommand;
        public ICommand PreviousPageCommand
        {
            get
            {
                return _previousPageCommand ?? (_previousPageCommand = new RelayCommand<object>((p) => { return CurrentPage > 1; }, (p) => { CurrentPage--; }));
            }
        }

        private ICommand _nextPageCommand;
        public ICommand NextPageCommand
        {
            get
            {
                return _nextPageCommand ?? (_nextPageCommand = new RelayCommand<object>((p) => { return CurrentPage < MaxPage; }, (p) => { CurrentPage++; }));
            }
        }
    }
}
