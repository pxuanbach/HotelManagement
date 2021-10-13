using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Models
{
    class CurrentAccount
    {
        private static CurrentAccount _instance;
        public static CurrentAccount Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CurrentAccount();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Permission { get; set; }

        public void GetCurrentAccount(string username)
        {
            ACCOUNT account = new ACCOUNT();
            account = DataProvider.Instance.DB.ACCOUNTs.Where(x => x.username == username).SingleOrDefault();

            Id = account.id;
            Username = account.username;
            Permission = account.permission;
        }

        public void DisposeCurrentAccount()
        {
            Id = -1;
            Username = "";
            Permission = "Undefined";
        }
    }
}
