using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.Windows.Forms;
using System.IO;
//  mô hình Singleton--Class xử lý lấy dữ liệu từ database--Dung cho LOGIN 
//là một mẫu thiết kế phần mềm để hạn chế sự khởi tạo của lớp đối tượng
//Đảm bảo rằng một class chỉ có duy nhất một instance,Và cung cấp một cáchs global để truy cập tới instance đó

//     1 class chỉ có thể có duy nhất 1 instance
//              + Private constructor của class đó để đảm bảo rằng class lớp khác không thể truy cập vào constructor và tạo ra instance mới
//              + Tạo một biến private static là thể hiện của class đó để đảm bảo rằng nó là duy nhất và chỉ được tạo ra trong class đó thôi
//      ccung cấp một cáchs toàn cầu để truy cấp tới instance 
///             + Tạo một public static menthod trả về instance vừa khởi tạo bên trên,-->cách duy nhất để các class khác có thể truy cập vào instance của class này
/// 
/// 
namespace LibraryManager
{
    public class DataConnect
    {        
        private string ConnectionStr;
        private string path;
        private SQLiteConnection connect = new SQLiteConnection();
        // Tạo một biến private static là thể hiện của class đó để đảm bảo rằng nó là duy nhất và chỉ được tạo ra trong class đó thôi
        private static DataConnect instance;


        //Private constructor của class đó để đảm bảo rằng class lớp khác không thể truy cập vào constructor và tạo ra instance mới
        private DataConnect()
        {
            var enviroment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(enviroment).Parent.FullName;
            projectDirectory = projectDirectory.Replace(@"\", @"\\");
            path = $"{projectDirectory}\\lbDatabase.db";
            ConnectionStr = $"Data Source={path}; Version = 3;";
        }

        //public static menthod trả về instance vừa khởi tạo bên trên,-->cách duy nhất để các class khác có thể truy cập vào instance của class này
        public static DataConnect Instance
        {
            get
            {
                if (instance == null)  instance = new DataConnect();
                return instance;
            }
            private set
            {
                DataConnect.instance = value;
            }
        }

        // hàm truy vấn tìm kiếm 
        public DataTable ExcuteQuery(string query)  
        {
            DataTable data = new DataTable();

            using (connect = new SQLiteConnection(ConnectionStr))
            {
                connect.Open();
                SQLiteCommand command = new SQLiteCommand(query, connect);
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                adapter.Fill(data);
                connect.Close();

            }
            return data;

        } 
        
        
        //  hàm truy vấn Update,Insert,Delete 
        //  tra ve int
        public int ExcuteNonQuery(string query) 
        {
            int data = 0;

            using (connect = new SQLiteConnection(ConnectionStr))
            {
                connect.Open();
                SQLiteCommand command = new SQLiteCommand(query, connect);

                data = command.ExecuteNonQuery();
                connect.Close();
                
            }
            return data;

        }
        //    tra ve object
        public object ExcuteScalar(string query)
        {
            object data = 0;

            using (connect = new SQLiteConnection(ConnectionStr))
            {
                connect.Open();
                SQLiteCommand command = new SQLiteCommand(query, connect);

                data = command.ExecuteScalar();
                connect.Close();

            }
            return data;

        }
    }
 }
    

