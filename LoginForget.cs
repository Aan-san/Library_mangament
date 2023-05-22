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
    public partial class LoginForget : Form
    {
        public Login log;
        public LoginForget()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            txtbEmail.Clear();
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            txtbUser.Clear();

        }

        private void textBox3_Click(object sender, EventArgs e)
        {
            txtbNewPass.Clear();
        }

        private void textBox4_Click(object sender, EventArgs e)
        {
            txtbNewPass2.Clear();
        }

        private void btEnter_Click(object sender, EventArgs e)
        {
            string email        = txtbEmail.Text.Trim();
            string username     = txtbUser.Text.Trim();
            string pass1        = txtbNewPass.Text.Trim();
            string pass2        = txtbNewPass2.Text.Trim();

            if (email == string.Empty || email == "Enter Email"
                || username == string.Empty || username == "Enter Username"
                || pass1 == string.Empty || pass1 == "Enter New Password"
                || pass2 == string.Empty || pass2 == "Rennter New Password")
            {
                MessageBox.Show("Please input information");
            }
            else
            {
                if (pass1 == pass2)
                {
                    if (Check(email, username, pass1) || LoginDAL.Instance.CheckEmailFormat(email))
                    {
                        MessageBox.Show("Change sucessful");
                        log = new Login();
                        log.ShowDialog();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Pleas check email and username again");
                    }
                }
                else
                {
                    MessageBox.Show("Wrong renter password\nPlease input again");
                }
            }
        }
        bool Check(string email,string username,string password)
        {
            return LoginDAL.Instance.ChangePas(email,username,password);
        }

        private void textBack_Click(object sender, EventArgs e)
        {
            log = new Login();
            this.Hide();
            log.ShowDialog();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
