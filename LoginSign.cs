using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryManager
{
    public partial class LoginSign : Form
    {
        public Login log;


        public LoginSign()
        {
            InitializeComponent();
        }
       

        private void label2_Click(object sender, EventArgs e)
        {
            log = new Login();
            this.Hide();
            log.ShowDialog();
        }

        private void txbUser_Click(object sender, EventArgs e)
        {
            txbUser.Clear();
        }

        private void txbPass_Click(object sender, EventArgs e)
        {
            txbPass.Clear();
        }

        private void txbEmail_Click(object sender, EventArgs e)
        {
            txbEmail.Clear();
        }

        private void txbName_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbName_Click(object sender, EventArgs e)
        {
            txbName.Clear();
        }

        private void btnSign_MouseHover(object sender, EventArgs e)
        {
            btnSign.BackColor = Color.FromArgb(51, 145, 222);
        }

        private void btnSign_Click(object sender, EventArgs e)
        {
            string username     = txbUser.Text.Trim();
            string password     = txbPass.Text.Trim();
            string email        = txbEmail.Text.Trim();
            string name         = txbName.Text.Trim();

            if (email == string.Empty   || email == "Email"
            || username == string.Empty || username == "username"
            || password == string.Empty || password == "password"
            || name == string.Empty     || name == "display name") 
            {
                MessageBox.Show("Please input information");
            }
            else
            {
                //---Check Email type but just check only 2 type is edu.vn and gmail
                if (LoginDAL.Instance.CheckEmailFormat(email))
                {
                    if (!CheckUser(username))
                    {
                        CreACC(username, password, email, name);// Tạo account mới 
                    }
                    else MessageBox.Show("This Username already existed");
                }
                else MessageBox.Show("Check email again\nPlease use @gm.uit.edu.vn hoặc @gmail.com");

            }
        }
        bool CheckUser(string username)
        {
            return LoginDAL.Instance.checkAccbyUser(username);
        }
        void CreACC(string username,string password,string email,string name)
        {
            if (Accounts.Instance.InsertAcc(username, password, email, name))
                MessageBox.Show("Sign up success");
            else 
                MessageBox.Show("Fail to sign up\nPlease check all information again");

        }

        private void btnSign_MouseLeave(object sender, EventArgs e)
        {
            btnSign.BackColor = Color.FromArgb(0, 117, 214);
        }
    }
}
