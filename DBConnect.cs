using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using System.IO;


namespace LibraryManager
{
    public class DBConnect
    {
        string connectionString;
        private string path;

        //Constructor
        public DBConnect()
        {
            Initialize();
        }

        //Initialize values
        private void Initialize()
        {
            var enviroment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(enviroment).Parent.FullName;
            projectDirectory = projectDirectory.Replace(@"\", @"\\");
            path = $"{projectDirectory}\\lbDatabase.db";
            connectionString = $"Data Source={path}; Version = 3;";
        }

        // Read data
        // Nhận câu truy vấn sql và trả về kết quả dưới dạng datatable . Tự đóng csdl lại

        public DataTable GetDataTable(string query)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SQLiteConnection c = new SQLiteConnection(connectionString))
                {
                    c.Open();
                    if (c.State == ConnectionState.Open)
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(query, c))
                        {
                            using (SQLiteDataReader rdr = cmd.ExecuteReader())
                            {
                                if (rdr.HasRows)
                                {
                                    dt.Load(rdr);
                                }
                                else
                                {
                                    dt = null;
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Connection Failed");
                    }
                }
                return dt;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //Insert - Update - Delete statement
        public bool ExucuteDbCmd(string query)
        {
            try
            {
                bool ifSuccess = false;
                using (SQLiteConnection c = new SQLiteConnection(connectionString))
                {
                    c.Open();
                    if (c.State == ConnectionState.Open)
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(query, c))
                        {
                            cmd.ExecuteNonQuery();
                            ifSuccess = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Connection Failed");
                    }
                }
                return ifSuccess;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        //Select statement
        public List<string>[] Select()
        {
            return null;
        }

        //Count statement
        public int Count()
        {
            return 0;
        }

        //Backup
        public void Backup()
        {

        }

        //Restore
        public void Restore()
        {

        }

        public DataTable Search_Book(string value, string column)
        {
            string query = $"SELECT B.ID, Title, Author, Name, Status FROM BOOK B JOIN CATEGORY C WHERE CategoryID = C.ID AND B.{column} = '{value}'";
            DataTable task = GetDataTable(query);
            return task;
        }
        public DataTable Search_Book_Category(string value)
        {
            string query = $"SELECT B.ID, Title, Author, Name, Status FROM BOOK B JOIN CATEGORY C WHERE CategoryID = C.ID AND C.Name = '{value}'";
            DataTable task = GetDataTable(query);
            return task;
        }

        public DataTable Search_Reader(string code)
        {
            string query = $"SELECT * FROM READER WHERE ID = '{code}'";
            DataTable task = GetDataTable(query);
            return task;
        }

        public bool CheckID(string id, string table)
        {
            string query = $"SELECT id FROM {table} WHERE id = '{id}'";
            DataTable task = GetDataTable(query);
            if (task == null)
                return false;
            return true;
        }

        public string CheckBorrow(string id)
        {
            string query = $"SELECT * FROM BOOK_BORROW WHERE UserID = '{id}'";
            DataTable task = GetDataTable(query);
            if (task == null)
                return "";
            return task.Rows[0]["UserID"].ToString();
        }

        public string GetReaderID(string id)
        {
            string query = $"SELECT * FROM BOOK_BORROW WHERE BookID = '{id}'";
            DataTable task = GetDataTable(query);
            if (task == null)
                return "";
            return task.Rows[0]["UserID"].ToString();
        }

        public bool CheckName(string name, string table)
        {
            string query = $"SELECT id FROM {table} WHERE Name = '{name}'";
            DataTable task = GetDataTable(query);
            if (task == null)
                return false;
            return true;
        }

        public int CheckCategory(string id)
        {
            string query = $"SELECT * FROM BOOK WHERE CategoryID = '{id}'";
            DataTable task = GetDataTable(query);
            if (task == null)
                return -1;
            return task.Rows.Count;
        }
        public string CategoryGetID(string name)
        {
            string query = $"SELECT * FROM CATEGORY WHERE Name = '{name}'";
            DataTable task = GetDataTable(query);
            if (task == null)
                return "";
            return task.Rows[0]["ID"].ToString();
        }
    }
}
