using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;


// Xử lý trong login 
namespace LibraryManager
{
    public class LoginDAL
    {
        private static LoginDAL instance;

        private LoginDAL() { }

        public static LoginDAL Instance
        {
            get { if (instance == null) instance = new LoginDAL(); return instance; }

            private set { instance = value; }
        }

        public bool Login(string username, string password)   // đăng nhập
        {
            string query="Select * From LIBRARIAN Where Username='"+username+"' and Password='"+password+"'";
            DataTable  result = DataConnect.Instance.ExcuteQuery(query);
            return result.Rows.Count>0;
        }

        public bool ChangePas(string email, string username,string password) // hàm đổi pass mới
        {
            string query = "Update LIBRARIAN Set Password='"+password+"' Where Email='" + email + "' and Username='" + username + "'";
            DataTable result = DataConnect.Instance.ExcuteQuery(query);
            return result.Rows.Count>0;
        }

        public bool checkAccbyUser(string username) // hàm check username có tồn tại hay không
        {
            string query = "Select * From LIBRARIAN Where Username='" + username + "' ";
            DataTable result = DataConnect.Instance.ExcuteQuery(query);
            return result.Rows.Count > 0;
        }

        public Accounts GetAcccoutbyUser(string username) // hàm truyền/nạp tài khoản khi đăng nhập
        {
            string query = "Select * from LIBRARIAN Where Username = '" + username + "'";
            DataTable data = DataConnect.Instance.ExcuteQuery(query);
            foreach(DataRow item in data.Rows)
            {
                return new Accounts(item);
            }

            return null;
        }

        public bool CheckEmailFormat(string email) // hàm check format 
        {
            // format email
            string  format1     = "@gm.uit.edu.vn";
            string  format2     = "@gmail.com";

            //check email account
            int     index       = email.IndexOf('@');
            string  accMail     = email.Substring(0, index);

            if (email.Contains(format1)|| email.Contains(format2)||(accMail!=string.Empty))
            {
                return true;
            }
            else
            {
                return false;
            }         
        }
    }
}
