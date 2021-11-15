using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HotelManagement.ViewModels
{
    class TopServiceViewModel : BaseViewModel
    {
        #region Properties
        private string _selectedType;
        public string SelectedType { get { return _selectedType; } set { _selectedType = value; OnPropertyChanged(); } }

        private List<string> _reportTypes;
        public List<string> ReportTypes { get { return _reportTypes; } set { _reportTypes = value; OnPropertyChanged(); } }

        #endregion

        #region Command

        #endregion

        public TopServiceViewModel()
        {
            InitProperties();
        }

        void InitProperties()
        {
            ReportTypes = new List<string>();
            ReportTypes.Add("Month");
            ReportTypes.Add("Year");

            SelectedType = "Month";
        }
    }
}
