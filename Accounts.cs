using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace LibraryManager
{
    public class Accounts
    {
        private static Accounts instance;

        private Accounts() { }

        public static Accounts Instance
        {
            get { if (instance == null) instance = new Accounts(); return instance; }

            private set { instance = value; }
        }
        public Accounts(string username,string password,string email,string hoten) 
        {
            this.username = username;
            this.password = password;
            this.email = email;
            this.hoten = hoten;

        }

        public Accounts(DataRow row)
        {
            this.username = row["Username"].ToString();
            this.password = row["Password"].ToString();
            this.email = row["Email"].ToString();
            this.hoten = row["Fullname"].ToString();
        }

        private string username;
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        private string hoten;
        public string Hoten
        {
            get { return hoten; }
            set { hoten = value; }
        }
        private string email;
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public bool InsertAcc(string username,string password,string email,string displayname)
        {
            string query = "Insert into LIBRARIAN(Username,Password,Email,FullName) Values('" + username + "','" + password + "','" + email + "','" + displayname + "')";
            int result = DataConnect.Instance.ExcuteNonQuery(query);
            return result > 0;
        }
    }
}
