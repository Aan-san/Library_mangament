using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using System.Text.RegularExpressions;


namespace LibraryManager
{
    public partial class Main : MaterialForm
    {
        DBConnect db;
        private readonly MaterialSkinManager materialSkinManager;

        //--------khởi tạo để truyền tài khoản vào-----------
        private Accounts loginAcc;

        public Accounts LoginAcc
        {
            get { return LoginAcc; }
            set { LoginAcc = value; }
        }

        public Main(Accounts acc)
        {
            InitializeComponent();
            this.loginAcc = acc;  // nạp Tài khoản đăng nhập vào form Main
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = true;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Blue300, Primary.Blue500, Primary.Blue300, Accent.LightBlue200, TextShade.WHITE);
            load_database_Home();


        }

        // -----------------------------------------------------------------------------------------------
        // Home tab
        private void load_database_Home()
        {
            db = new DBConnect();
            DataTable task;
            string query = "SELECT B.ID, Title, Author, Name, Status FROM BOOK B JOIN CATEGORY C WHERE CategoryID = C.ID";
            task = db.GetDataTable(query);
            materialHomeListView.Items.Clear();   // Clear old data
            load_DataTable_Home(task);
        }

        private void load_DataTable_Home(DataTable dataTable)
        {
            int i = 1;
            foreach (DataRow r in dataTable.Rows)
            {
                ListViewItem listViewItem = new ListViewItem(i.ToString());
                listViewItem.SubItems.Add(r["ID"].ToString());
                listViewItem.SubItems.Add(r["Title"].ToString());
                listViewItem.SubItems.Add(r["Author"].ToString());
                listViewItem.SubItems.Add(r["Name"].ToString());
                if (r["Status"].ToString() == "0")
                {
                    listViewItem.SubItems.Add("Còn");
                }
                else
                {
                    listViewItem.SubItems.Add("Đã mượn");
                }
                i++;
                materialHomeListView.Items.Add(listViewItem);
            }
        }

        private void tabpageHome_Enter(object sender, EventArgs e)
        {

        }

        private void materialButtonHomeReload_Click(object sender, EventArgs e)
        {
            mtbHomeSearch.Text = "";
            materialComboBoxCategory.Text = "Mã sách";
            load_database_Home(); // Refresh data  table
        }

        private void listView1_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            //e.Cancel = true;
            //e.NewWidth = this.listView1.Columns[e.ColumnIndex].Width;
        }

        private void DataView_Closed()
        {
            materialHomeListView.Items.Clear();
        }

        private void txtbSearch_TextChanged(object sender, EventArgs e)
        {

        }

        private void mbtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string type = materialComboBoxCategory.Text.Trim();//căt khoảng trắng ở đàu và cuối
                DataTable task;
                string text = mtbHomeSearch.Text.Trim();
                switch (type)
                {
                    case "Book ID":
                        task = db.Search_Book(text, "ID");
                        if (task != null)
                        {
                            DataView_Closed();
                            load_DataTable_Home(task);
                        }
                        else
                        {
                            MessageBox.Show("Not found");
                        }
                        break;
                    case "Title":
                        text = text.ToUpper();
                        task = db.Search_Book(text, "Title");
                        if (task != null)
                        {
                            DataView_Closed();
                            load_DataTable_Home(task);
                        }
                        else
                        {
                            MessageBox.Show("Not found");
                        }
                        break;
                    case "Author":
                        text = Regex.Replace(text, @"\b[a-z]", m=>m.Value.ToUpper());
                        Console.WriteLine(text);
                        task = db.Search_Book(text, "Author");
                        if (task != null)
                        {
                            DataView_Closed();
                            load_DataTable_Home(task);
                        }
                        else
                        {
                            MessageBox.Show("Not found");
                        }
                        break;
                    case "Category":
                        text = Regex.Replace(text, @"\b[a-z]", m => m.Value.ToUpper());
                        Console.WriteLine(text);
                        task = db.Search_Book_Category(text);
                        if (task != null)
                        {
                            DataView_Closed();
                            load_DataTable_Home(task);
                        }
                        else
                        {
                            MessageBox.Show("Not found");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void materialHomeListView_DoubleClick(object sender, EventArgs e)
        {
            materialTabControl1.SelectedTab = tabPageDetail;
            string book_id = materialHomeListView.SelectedItems[0].SubItems[1].Text.Trim();
            materialTextBoxDetailID.Text = book_id;
            materialTextBoxDetailTitle.Text = materialHomeListView.SelectedItems[0].SubItems[2].Text.Trim();
            materialTextBoxDetailAuthor.Text = materialHomeListView.SelectedItems[0].SubItems[3].Text.Trim();
            materialTextBoxDetailCategory.Text = materialHomeListView.SelectedItems[0].SubItems[4].Text.Trim();
            materialTextBoxDetailStatus.Text = materialHomeListView.SelectedItems[0].SubItems[5].Text.Trim();
            if (materialHomeListView.SelectedItems[0].SubItems[5].Text == "Còn")
            {
                materialTextBoxDetailReaderID.Text = "";
                materialTextBoxDetailReaderName.Text = "";
            }
            else
            {
                string reader_id = db.GetReaderID(book_id);
                if (reader_id != "")
                {
                    DataTable task2 = db.Search_Reader(reader_id);
                    DataRow row2 = task2.Rows[0];
                    materialTextBoxDetailReaderID.Text = row2["ID"].ToString();
                    materialTextBoxDetailReaderName.Text = row2["FullName"].ToString();
                }
            }
        }

        // -----------------------------------------------------------------------------------------------
        //
        //
        // Detail tab
        private void materialButtonDetailBorrow_Click(object sender, EventArgs e)
        {
            materialTabControl1.SelectedTab = tabPageBorrow;
            textmasach.Text = materialTextBoxDetailID.Text.Trim();
            button1_Click(sender, e);
        }

        private void materialButtonDetailReturn_Click(object sender, EventArgs e)
        {
            materialTabControl1.SelectedTab = tabPageHome;
        }

        private void materialButtonDetailAdd_Click(object sender, EventArgs e)
        {
            string id = materialTextBoxDetailID.Text.Trim();
            string title = materialTextBoxDetailTitle.Text.Trim();
            string author = materialTextBoxDetailAuthor.Text.Trim();
            string category = materialComboBoxDetailCategory.Text.Trim();
            int status = 0;

            if (db.CheckID(id, "BOOK"))
            {
                MessageBox.Show("Duplicated ID");
            }
            else
            {
                if (category != "")
                {
                    string category_id = db.CategoryGetID(category);
                    string query = $"INSERT INTO BOOK VALUES({id}, '{title}', '{author}', '{category_id}', '{status}')";
                    bool book_result = db.ExucuteDbCmd(query);
                    if (book_result)
                    {
                        MessageBox.Show("Add book successful");
                        Reload_All(sender, e);
                    }
                    else
                    {
                        MessageBox.Show("Fail to addd");
                    }
                }
                else
                {
                    MessageBox.Show("Please choose category");
                }
            }
        }

        private string bookid;
        private string booktitle;
        private string bookauthor;
        private string booktcategory;

        private void materialButtonDetailEdit_Click(object sender, EventArgs e)
        {
            string id = materialTextBoxDetailID.Text.Trim();
            string title = materialTextBoxDetailTitle.Text.Trim();
            string author = materialTextBoxDetailAuthor.Text.Trim();
            string category = materialComboBoxDetailCategory.Text.Trim();
            string status = materialTextBoxDetailStatus.Text.Trim();

            if (status == "Đã mượn")
            {
                MessageBox.Show("Book already borrowed");
            }
            else
            {
                if (db.CheckID(id, "BOOK"))
                {
                    if (category != "")
                    {
                        string category_id = db.CategoryGetID(category);
                        string query = $"UPDATE BOOK SET Title = '{title}', Author = '{author}', CategoryID = '{category_id}' WHERE ID = {id}";
                        bool category_result = db.ExucuteDbCmd(query);
                        if (category_result)
                        {
                            MessageBox.Show("Update book successful");
                            Reload_All(sender, e);
                        }
                        else
                        {
                            MessageBox.Show("Fail to update");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please choose category");
                    }
                }
                else
                {
                    DialogResult dialogResult = MessageBox.Show("Are you sure to change ID", "New ID detected", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        if (category != "")
                        {
                            string category_id = db.CategoryGetID(category);
                            string query = $"UPDATE BOOK SET ID = '{id}', Title = '{title}', Author = '{author}', CategoryID = '{category_id}' WHERE ID = {bookid}";
                            bool book_result = db.ExucuteDbCmd(query);
                            if (book_result)
                            {
                                MessageBox.Show("Update successful");
                                Reload_All(sender, e);
                            }
                            else
                            {
                                MessageBox.Show("Fail to update");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please choose category");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Fail to edit");
                    }
                }
            }
        }

        private void materialButtonDetailDelete_Click(object sender, EventArgs e)
        {
            string id = materialTextBoxDetailID.Text.Trim();
            string status = materialTextBoxDetailStatus.Text.Trim();

            if (status == "Đã mượn")
            {
                MessageBox.Show("Book already borrowed");
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure to delete this book", "Delete out of Library", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string query = $"DELETE FROM BOOK WHERE ID = {id}";
                    bool book_result = db.ExucuteDbCmd(query);
                    if (book_result)
                    {
                        MessageBox.Show("Delete successful");
                        Reload_All(sender, e);
                    }
                    else
                    {
                        MessageBox.Show("Fail to delete");
                    }
                }
                else
                {
                    MessageBox.Show("Fate to edit");
                }
            }
        }

        private void materialButtonDetailEditMode_Click(object sender, EventArgs e)
        {
            materialCardDetail3.Hide();
            materialTextBoxDetailCategory.Hide();

            materialCardDetail4.Show();
            materialComboBoxDetailCategory.Show();

            materialTextBoxDetailID.ReadOnly = false;
            materialTextBoxDetailTitle.ReadOnly = false;
            materialTextBoxDetailAuthor.ReadOnly = false;
            materialTextBoxDetailCategory.ReadOnly = false;

            string check = "SELECT Name FROM CATEGORY";
            DataTable task = db.GetDataTable(check);
            foreach (DataRow r in task.Rows)
            {
                materialComboBoxDetailCategory.Items.Add(r["Name"].ToString());
            }

            bookid = materialTextBoxDetailID.Text.Trim();
            booktitle = materialTextBoxDetailTitle.Text.Trim();
            bookauthor = materialTextBoxDetailAuthor.Text.Trim();
            booktcategory = materialTextBoxDetailCategory.Text.Trim();
        }

        private void materialButtonDetailViewMode_Click(object sender, EventArgs e)
        {
            materialCardDetail4.Hide();
            materialComboBoxDetailCategory.Hide();

            materialCardDetail3.Show();
            materialTextBoxDetailCategory.Show();

            materialTextBoxDetailID.ReadOnly = true;
            materialTextBoxDetailTitle.ReadOnly = true;
            materialTextBoxDetailAuthor.ReadOnly = true;
            materialTextBoxDetailCategory.ReadOnly = true;

            materialTextBoxDetailID.Text = bookid;
            materialTextBoxDetailTitle.Text = booktitle;
            materialTextBoxDetailAuthor.Text = bookauthor;
            materialTextBoxDetailCategory.Text = booktcategory;
        }

        private void tabPageDetail_Leave(object sender, EventArgs e)
        {
            
        }

        private void materialButtonDetailReload_Click(object sender, EventArgs e)
        {
            materialButtonDetailViewMode_Click(sender, e);
            materialTextBoxDetailID.Text = "";
            materialTextBoxDetailTitle.Text = "";
            materialTextBoxDetailAuthor.Text = "";
            materialTextBoxDetailCategory.Text = "";
            materialTextBoxDetailStatus.Text = "";
            materialTextBoxDetailReaderID.Text = "";
            materialTextBoxDetailReaderName.Text = "";
        }


        // end detail tab

        // -----------------------------------------------------------------------------------------------
        //
        //
        // Borrow tab

        //CHECK QUÁ HẠN
        public double Check_Date(string date_br, string date_rt)
        {
            DateTime muon = Convert.ToDateTime(date_br);
            DateTime tra = Convert.ToDateTime(date_rt);
            double sl;
            sl = (tra - muon).TotalDays;
            return sl;
        }

        private void button1_Click(object sender, EventArgs e)//Borrow_search_BOOK_ID
        {
            DataTable task;
            string maso = textmasach.Text.Trim();
            if (string.IsNullOrWhiteSpace(maso))
            {
                MessageBox.Show("Please input ID");
            }
            else
            {
                string query = "SELECT B.ID, Title, Author, Name, Status FROM BOOK B JOIN CATEGORY C WHERE CategoryID = C.ID AND B.ID = " + maso;
                task = db.GetDataTable(query);
                if (task != null)//Kiêmr tra sách có tồn tại hay ko
                {
                    texttensach.Text = task.Rows[0]["ID"].ToString();
                    comboBox1.Text = task.Rows[0]["Name"].ToString();
                    texttacgia.Text = task.Rows[0]["Author"].ToString();
                }
                else
                {
                    MessageBox.Show("Book not found");
                    return;
                }
            }
        }
        //BORROW_SEARCH_USER_ID
        private void materialButton1_Click(object sender, EventArgs e)
        {
            DataTable temp;
            string id = textBox3.Text.Trim();// lay ma doc gia
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    MessageBox.Show("Please input ID");
                }
                else
                {
                    string query = "SELECT * FROM READER where ID = " + id;
                    temp = db.GetDataTable(query);
                    if (temp != null)// Doc gia co ton tai hay khong
                    {
                        textBox2.Text = temp.Rows[0]["Fullname"].ToString();//họ tên
                        textBox5.Text = temp.Rows[0]["Email"].ToString();//Email
                        load_database_BR();
                    }
                    else//Neu doc gia khoong ton tai
                    {
                        MessageBox.Show("Author not found!!");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                //  Block of code to handle errors
                throw new Exception(ex.Message);
            }
        }
        //Hiển thị ra dataGV_BORROW
        private void load_DataTable_BR(DataTable dataTable)
        {
            int i = 1;

            foreach (DataRow r in dataTable.Rows)
            {
                ListViewItem listViewItem = new ListViewItem(i.ToString());
                listViewItem.SubItems.Add(r["UserID"].ToString());
                listViewItem.SubItems.Add(r["BOOkID"].ToString());
                listViewItem.SubItems.Add(r["BorrowDay"].ToString());
                listViewItem.SubItems.Add(r["ReturnDay"].ToString());
                if (r["Status"].ToString() == "0")
                    listViewItem.SubItems.Add("Còn");
                else
                    listViewItem.SubItems.Add("Đã mượn");
                i++;
                materialListView2.Items.Add(listViewItem);
            }
        }

        private void load_database_BR()
        {
            string id = textBox3.Text.Trim();
            if (id != "")
            {
                string query = "SELECT UserID, BookID,BorrowDay,ReturnDay, Status FROM BOOK_BORROW bb join BOOK b on b.ID= bb.BookID WHERE UserID= " + id;
                DataTable task = db.GetDataTable(query);
                materialListView2.Items.Clear();   // Clear old data
                if (task != null)
                {
                    load_DataTable_BR(task);
                }
            }
            else
                MessageBox.Show("Reader not found");
        }

        //Hàm mượn chính
        private void cho_muon_Click(object sender, EventArgs e)
        {
            string masach = textmasach.Text.Trim();//Ma sach
            string id = textBox3.Text.Trim(); //Ma doc gia
            string tensach = texttensach.Text.Trim();// Teen sach
            string HoTen = textBox2.Text.Trim();  //Ho ten
            string Email = textBox5.Text.Trim();  //email
            string NgayMuon = dateTimePicker2.Value.ToShortDateString(); //Ngay muon
            string NgayTra = dateTimePicker1.Value.ToShortDateString();  //Ngay Tra
            try//ys oong laf sao nãy tui thấy ở trên list có 2 cái nó trùng cả id sách và id user
            {
                //Kiểm tra xem quyển sách muốn mượn đã từng mượn chưa -->Tránh mượn trùng
                //Vaf tình trạng quyển sách có ==0 (chưa mượn) hay chưa
                string sql = "SELECT * FROM BOOK_BORROW bb join BOOK b on bb.BookID=b.ID where bb.UserID = " + id + " AND bb.BooKID=" + "'" + masach + "'" ;
                DataTable kq = db.GetDataTable(sql); // = null nêus quyển sách chưa được mượn

                if (kq != null)
                {
                    MessageBox.Show("You already borrow this book or another reader!!!");
                    return;
                }
                else
                {
                    //Lấy số sách mà sinh viên đó mượn
                    string sql1 = "SELECT BooKID from BOOK_BORROW where  UserID = " + textBox3.Text.Trim();
                    DataTable kq1 = db.GetDataTable(sql1);
                    int num_book;
                    if (kq1 != null)
                    {
                        num_book = kq1.Rows.Count;
                    }
                    else
                    {
                        num_book = 0;
                    }

                    if (num_book <= 3)//Nếu mượn và sl mượn <=3 và KHÔNG QUÁ HẠN  thì mới tính tiếp
                    {
                        //Lưu vào csdl thông tin người mượn và mã sách
                        string sql2 = "INSERT INTO BOOK_BORROW VALUES(" + id + "," + "'" + masach + "'" + "," + "'" + NgayMuon + "'" + "," + "'" + NgayTra + "'" + ")";
                        bool kq2 = db.ExucuteDbCmd(sql2);
                        //Lưu vào csdl BOOK.Status==1 --> update đã được mượn
                        string sql3 = " UPDATE BOOK SET Status = 1 WHERE ID=" + masach;
                        bool kq3 = db.ExucuteDbCmd(sql3);

                        // Neu chen vaf cap nhat thanh cong thif xuat ra grid
                        if (kq2 == true && kq3 == true)
                        {
                            //Xuất ra bảng: lay het cac ma sach con dang muon
                            string query = "SELECT UserID, BookID,BorrowDay,ReturnDay, Status FROM BOOK_BORROW bb join BOOK b on b.ID= bb.BookID WHERE UserID= " + id; //Status = '1' and
                            DataTable Data2 = db.GetDataTable(query);
                            materialListView2.Items.Clear();
                            load_DataTable_BR(Data2);
                            MessageBox.Show("Đặt mượn thành công");
                        }
                        else
                        {
                            MessageBox.Show("Fail to edit");
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Reach limitation");
                        return;
                    }
                }
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        private void materialListView2_Click(object sender, EventArgs e)
        {
            string id = materialListView2.SelectedItems[0].SubItems[2].Text.Trim();
            DataTable task = db.Search_Book(id, "ID");
            DataRow dataRow = task.Rows[0];
            textmasach.Text = id;
            texttensach.Text = dataRow["Title"].ToString();
            comboBox1.Text = dataRow["Name"].ToString();
            texttacgia.Text = dataRow["Author"].ToString();
            dateTimePicker2.Value = Convert.ToDateTime(materialListView2.SelectedItems[0].SubItems[3].Text.Trim());
            dateTimePicker1.Value = Convert.ToDateTime(materialListView2.SelectedItems[0].SubItems[4].Text.Trim());
        }

        private void materialListView2_DoubleClick(object sender, EventArgs e)
        {
            string book_id = materialListView2.SelectedItems[0].SubItems[2].Text.Trim();
            DataTable task = db.Search_Book(book_id, "ID");
            DataRow row = task.Rows[0];
            materialTabControl1.SelectedTab = tabPageDetail;
            materialTextBoxDetailID.Text = book_id;
            materialTextBoxDetailTitle.Text = row["Title"].ToString();
            materialTextBoxDetailAuthor.Text = row["Author"].ToString();
            materialTextBoxDetailCategory.Text = row["Name"].ToString();
            materialTextBoxDetailStatus.Text = row["Status"].ToString();

            string reader_id = materialListView2.SelectedItems[0].SubItems[1].Text.Trim();
            DataTable task2 = db.Search_Reader(reader_id);
            DataRow row2 = task2.Rows[0];
            materialTextBoxDetailReaderID.Text = row2["ID"].ToString();
            materialTextBoxDetailReaderName.Text = row2["FullName"].ToString();
        }

        private void materialButtonBorrowReload_Click(object sender, EventArgs e)
        {
            textmasach.Text = "";
            texttensach.Text = "";
            comboBox1.Text = "";
            texttacgia.Text = "";
            textBox3.Text = "";
            textBox2.Text = "";
            textBox5.Text = "";
            dateTimePicker2.Value = DateTime.Now;
            dateTimePicker1.Value = DateTime.Now;
            materialComboBoxDetailCategory.Text = "";
            materialListView2.Items.Clear();
        }

        // end borrow tab

        //-------------------------------------------------------------------------------------------------------------------
        //
        //
        //
        //  return tab


        private void load_DataTable_Search(DataTable dataTable)
        {
            int i = 0;
            if (dataTable == null)
            {
                return;
            }
            foreach (DataRow r in dataTable.Rows)
            {
                ListViewItem listViewItem = new ListViewItem(i.ToString());
                listViewItem.SubItems.Add(r["UserID"].ToString());
                listViewItem.SubItems.Add(r["BookID"].ToString());
                listViewItem.SubItems.Add(r["BorrowDay"].ToString());
                listViewItem.SubItems.Add(r["ReturnDay"].ToString());
                if (r["Status"].ToString() == "0")
                    listViewItem.SubItems.Add("Còn");
                else
                    listViewItem.SubItems.Add("Đã mượn");
                if (Check_Date(r["ReturnDay"].ToString(), DateTime.Now.ToShortDateString()) > 0)
                    listViewItem.SubItems.Add(Check_Date(r["ReturnDay"].ToString(), DateTime.Now.ToShortDateString()).ToString());
                else
                    listViewItem.SubItems.Add((0).ToString());
                i++;
                materialListView3.Items.Add(listViewItem);
            }

        }
        private void button1_a_Click(object sender, EventArgs e)
        {
            //tim
            //Nhap ma sinh vien--> Hien thi ra thong tin sinh vien vaf cac cuon sach da muon
            //Ho va ten
            //Dia chi
            //Email
            //MSSV
            //Ngay sinh

            materialTextBoxDateReturn.Text = DateTime.Now.ToShortDateString();

            DataTable Table;
            DataTable Data;
            string id = textBox1_a.Text.Trim();// lay ma doc gia
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    MessageBox.Show("Please input ID");
                }
                else
                {
                    string query = "SELECT * FROM READER where ID = " + id;
                    string query2 = "SELECT UserID, BookID,BorrowDay,ReturnDay, Status FROM BOOK_BORROW bb join BOOK b on b.ID= bb.BookID WHERE UserID= " + id;
                    Table = db.GetDataTable(query);
                    Data = db.GetDataTable(query2);
                    if (Table != null)
                    {
                        //HIen thi thong tin sinh vien
                        textBox4_a.Text = Table.Rows[0][0].ToString();  //MSSV
                        textBox2_a.Text = Table.Rows[0][1].ToString();  //Ho va ten
                        textBox6_a.Text = Table.Rows[0][2].ToString();  //Ngay sinh
                        textBox5_a.Text = Table.Rows[0][3].ToString();  //Dia chi
                        textBox3_a.Text = Table.Rows[0][4].ToString();  //Email
                       //Hien thi thong tin muon sach sinh vien ra bang
                        load_DataTable_Search(Data);
                    }
                    else
                    {
                        MessageBox.Show("Wrong information");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                //  Block of code to handle errors
                throw new Exception(ex.Message);
            }
        }
        private void load_DataTable_Return(DataTable dataTable)
        {
            int i = 0;
            if (dataTable == null)
            {
                return;
            }
            foreach (DataRow r in dataTable.Rows)
            {
                ListViewItem listViewItem = new ListViewItem(i.ToString());
                listViewItem.SubItems.Add(r["UserID"].ToString());
                listViewItem.SubItems.Add(r["BookID"].ToString());
                listViewItem.SubItems.Add(r["BorrowDay"].ToString());
                listViewItem.SubItems.Add(r["ReturnDay"].ToString());
                if (r["Status"].ToString() == "0")
                    listViewItem.SubItems.Add("Còn");
                else
                    listViewItem.SubItems.Add("Đã mượn");
                listViewItem.SubItems.Add(Check_Date(r["ReturnDay"].ToString(), DateTime.Now.ToShortDateString()).ToString());
                i++;
                materialListView3.Items.Add(listViewItem);
            }
        }
        private void button4_aa_Click(object sender, EventArgs e)
        {
            materialTextBoxDateReturn.Text = DateTime.Now.ToShortDateString();

            ///Nut tra
            //Lay ma sach can tra
            //Kiem tra no co trong sach muon hay khong
            //xóa dữ liệu trong csdl
            //Hiển thị lại dữ liệu
            bool Data2;
            DataTable Data3;
            DataTable Data;
            string masach = textBox7_a.Text.Trim(); // lay ma sach can tra
            string mssv = textBox1_a.Text.Trim();   //Lay mssv
            //Lấy ngày trả
            string ngay_tra = DateTime.Now.ToShortDateString();//

            try
            {
                if (string.IsNullOrWhiteSpace(masach))
                {
                    MessageBox.Show("Please input book ID");
                }
                else
                {
                    //Kiem tra no co trong sach muon hay khong
                    string query = "SELECT * from BOOK_BORROW where UserID = " + mssv + " AND BookID= " + masach;
                    Data = db.GetDataTable(query);

                    // Lay ngay muon tra cua quyen sach tinh qua han
                    string QuaHan = "SELECT ReturnDay FROM BOOK_BORROW WHERE UserID = " + mssv + " AND BookID= " + masach;
                    DataTable QH = db.GetDataTable(QuaHan);
                    double dem;
                    string date_rt;
                    if (QH != null && QH.Rows.Count != 0)
                    {
                        //Ngày hẹn trả
                        date_rt = QH.Rows[0]["ReturnDay"].ToString();
                        dem = Check_Date(date_rt, ngay_tra);
                    }
                    else
                    {
                        MessageBox.Show("Book not found !!");
                        return;
                    }




                    string Xoa;
                    string sql_update;
                    bool result_ne;
                    int num = 0;

                    if (Data != null)//Neu quyen sach dang duoc muon
                    {
                       if (dem <= num)// num la so ngay quy dinh
                        {
                            // Xoa quyen sach ra khoi muon sach
                            Xoa = "DELETE FROM BOOK_BORROW where UserID = " + mssv + " AND BookID= " + masach;
                            Data2 = db.ExucuteDbCmd(Xoa);

                            //Cap nhat quyen sach ve tinh trang chua muon status==0 --> Update da tra roi
                            sql_update = " UPDATE BOOK SET Status = 0 WHERE ID=" + masach;
                            result_ne = db.ExucuteDbCmd(sql_update);

                            if (Data2 == true && result_ne == true)// Xóa thành công và cập nhật lại tình trạng sách
                            {
                                //Hiển thị lại dữ liệu
                                materialListView3.Items.Clear();   // Clear old data
                                string data_new = "SELECT UserID, BookID,BorrowDay,ReturnDay, Status FROM BOOK_BORROW bb join BOOK b on b.ID= bb.BookID WHERE UserID= " + mssv;
                                Data3 = db.GetDataTable(data_new);
                                load_DataTable_Return(Data3);
                                MessageBox.Show("Give back successful");
                            }
                            else
                            {
                                MessageBox.Show("Fail to delete or book already borrowed ");
                                return;
                            }
                        }
                        else
                        {
                            //Nếu số ngày mượn  > num ngày vẫn xóa nhưng  --> tính phạt

                            //VẪN XÓA
                            // Xoa quyen sach ra khoi muon sach
                            Xoa = "DELETE FROM BOOK_BORROW where UserID = " + mssv + " AND BookID= " + masach;
                            Data2 = db.ExucuteDbCmd(Xoa);

                            //Cap nhat quyen sach ve tinh trang chua muon status==0 --> Update da tra roi
                            sql_update = " UPDATE BOOK SET Status = 0 WHERE ID=" + masach;
                            result_ne = db.ExucuteDbCmd(sql_update);

                            if (Data2 == true && result_ne == true)// Xóa thành công và cập nhật lại tình trạng sách
                            {
                                //Hiển thị lại dữ liệu
                                materialListView3.Items.Clear();   // Clear old data
                                string data_new = "SELECT UserID, BookID,BorrowDay,ReturnDay, Status FROM BOOK_BORROW bb join BOOK b on b.ID= bb.BookID WHERE UserID= " + mssv;
                                Data3 = db.GetDataTable(data_new);
                                load_DataTable_Return(Data3);
                            }
                            else
                            {
                                MessageBox.Show("Fail to delete or book already borrowed ");
                                return;
                            }
                            ///XUẤT PHẠT
                            MessageBox.Show("Expire : " + dem + "days. Got fired!");
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Wrong input");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                //  Block of code to handle errors
                throw new Exception(ex.Message);
            }
        }


        private void materialListView3_Click(object sender, EventArgs e)
        {
            string id = materialListView3.SelectedItems[0].SubItems[2].Text.Trim();
            textBox7_a.Text = id;
        }

        private void materialButtonReturnReload_Click(object sender, EventArgs e)
        {
            textBox1_a.Text = "";
            textBox2_a.Text = "";
            textBox3_a.Text = "";
            textBox4_a.Text = "";
            textBox5_a.Text = "";
            textBox6_a.Text = "";
            textBox7_a.Text = "";
            materialTextBoxDateReturn.Text = DateTime.Now.ToShortDateString();
            materialListView3.Items.Clear();
        }

        // end borow tab

        // -----------------------------------------------------------------------------------------------
        //
        //
        // Reader tab
        private void load_DataTable_Reader(DataTable dataTable)
        {
            int i = 1;
            foreach (DataRow r in dataTable.Rows)
            {
                ListViewItem listViewItem = new ListViewItem(i.ToString());
                listViewItem.SubItems.Add(r["ID"].ToString());
                listViewItem.SubItems.Add(r["Fullname"].ToString());
                listViewItem.SubItems.Add(r["DateOfBirth"].ToString());
                listViewItem.SubItems.Add(r["Address"].ToString());
                listViewItem.SubItems.Add(r["Email"].ToString());
                listViewItem.SubItems.Add(r["DateCreated"].ToString());
                i++;
                materialReaderListView.Items.Add(listViewItem);
            }
        }

        private void load_database_Reader()
        {
            DataTable task;
            string query = "SELECT * FROM READER";
            task = db.GetDataTable(query);
            DataReaderView_Closed();   // Clear old data
            load_DataTable_Reader(task);
        }


        private void tabPageReader_Enter(object sender, EventArgs e)
        {
            load_database_Reader();   // Refresh data  table
        }

        private void DataReaderView_Closed()
        {
            materialReaderListView.Items.Clear();
        }

        private void materialButtonReaderSearch_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable task = db.Search_Reader(materialTextBoxReaderID.Text.Trim());
                if (task != null)
                {
                    DataReaderView_Closed();
                    load_DataTable_Reader(task);

                    DataRow dataRow = task.Rows[0];
                    materialTextBoxReaderID.Text = dataRow["ID"].ToString();
                    materialTextBoxReaderName.Text = dataRow["FullName"].ToString();
                    dateTimePickerReader.Value = Convert.ToDateTime(dataRow["DateOfBirth"]);
                    materialTextBoxReaderAddress.Text = dataRow["Address"].ToString();
                    materialTextBoxReaderEmail.Text = dataRow["Email"].ToString();
                }
                else
                {
                    MessageBox.Show("Reader not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        private void Reader_ClearAll()
        {
            materialTextBoxReaderID.Text = "";
            materialTextBoxReaderName.Text = "";
            dateTimePickerReader.Value = DateTime.Now;
            materialTextBoxReaderAddress.Text = "";
            materialTextBoxReaderEmail.Text = "";
        }

        private void materialButtonReaderReload_Click(object sender, EventArgs e)
        {
            load_database_Reader();
            Reader_ClearAll();

        }

        private void materialButtonReaderAdd_Click(object sender, EventArgs e)
        {
            string id = materialTextBoxReaderID.Text.Trim();
            string name = materialTextBoxReaderName.Text.Trim();
            string datetime = dateTimePickerReader.Value.ToShortDateString();
            string address = materialTextBoxReaderAddress.Text.Trim();
            string email = materialTextBoxReaderEmail.Text.Trim();
            string datecreate = DateTime.Now.ToShortDateString();

            if (db.CheckID(id, "READER"))
            {
                MessageBox.Show("Duplicated ID");
            }
            else
            {
                string query = $"INSERT INTO READER VALUES({id}, '{name}', '{datetime}', '{address}', '{email}', '{datecreate}')";
                bool reader_insert = db.ExucuteDbCmd(query);
                if (reader_insert)
                {
                    MessageBox.Show("Add successful");
                    Reload_All(sender, e);
                }
                else
                {
                    MessageBox.Show("Fail to add");
                }
            }
        }

        private void materialReaderListView_Click(object sender, EventArgs e)
        {
            materialTextBoxReaderID.Text = materialReaderListView.SelectedItems[0].SubItems[1].Text.Trim();
            materialTextBoxReaderName.Text = materialReaderListView.SelectedItems[0].SubItems[2].Text.Trim();
            dateTimePickerReader.Value = Convert.ToDateTime(materialReaderListView.SelectedItems[0].SubItems[3].Text.Trim());
            materialTextBoxReaderAddress.Text = materialReaderListView.SelectedItems[0].SubItems[4].Text.Trim();
            materialTextBoxReaderEmail.Text = materialReaderListView.SelectedItems[0].SubItems[5].Text.Trim();
            dateTimePickerReader2.Value = Convert.ToDateTime(materialReaderListView.SelectedItems[0].SubItems[6].Text.Trim());
        }

        private void materialButtonReaderEdit_Click(object sender, EventArgs e)
        {
            if (materialReaderListView.SelectedItems.Count > 0)
            {
                string id = materialReaderListView.SelectedItems[0].SubItems[1].Text.Trim();
                string check_reader = db.CheckBorrow(id);
                if (check_reader != "")
                {
                    MessageBox.Show("Reader have book that not give back yet ");
                }
                else
                {
                    string reader_id = materialTextBoxReaderID.Text.Trim();
                    string name = materialTextBoxReaderName.Text.Trim();
                    string date_birth = dateTimePickerReader.Value.ToShortDateString();
                    string address = materialTextBoxReaderAddress.Text.Trim();
                    string email = materialTextBoxReaderEmail.Text.Trim();
                    //string date_create = DateTime.Now.ToShortDateString();

                    if (db.CheckID(reader_id, "READER"))
                    {
                        string query = $"UPDATE READER SET FullName = '{name}', DateOfBirth = '{date_birth}', Address = '{address}', Email = '{email}' WHERE ID = {id}";
                        bool result = db.ExucuteDbCmd(query);
                        if (result)
                        {
                            MessageBox.Show("Edit success");
                            Reload_All(sender, e);
                        }
                        else
                        {
                            MessageBox.Show("Fail to edit");
                        }
                    }
                    else
                    {
                        DialogResult dialogResult = MessageBox.Show("Are you sure to change ID", "New ID detected", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            string query = $"UPDATE READER SET ID = {reader_id}, FullName = '{name}', DateOfBirth = '{date_birth}', Address = '{address}', Email = '{email}' WHERE ID = {id}";
                            bool result = db.ExucuteDbCmd(query);
                            if (result)
                            {
                                MessageBox.Show("Edit success");
                                Reload_All(sender, e);
                            }
                            else
                            {
                                MessageBox.Show("Fail to edit");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Fail to edit");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please choose reader");
            }

        }


        private void materialButtonReaderDelete_Click(object sender, EventArgs e)
        {
            if (materialReaderListView.SelectedItems.Count > 0)
            {
                string id = materialReaderListView.SelectedItems[0].SubItems[1].Text.Trim();
                string check_reader = db.CheckBorrow(id);
                if (check_reader != "")
                {
                    MessageBox.Show("Reader have book that not give back yet");
                }
                else
                {
                    string query = $"DELETE FROM READER WHERE ID = {id}";
                    bool result = db.ExucuteDbCmd(query);
                    if (result)
                    {
                        MessageBox.Show("Delete successful");
                        Reload_All(sender, e);
                    }
                    else
                    {
                        MessageBox.Show("Fail to delete");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please choose reader");
            }

        }

        // end reader tab

        // -----------------------------------------------------------------------------------------------
        //
        //
        // Category tab

        private void load_DataTable_Category(DataTable dataTable)
        {
            foreach (DataRow r in dataTable.Rows)
            {
                ListViewItem listViewItem = new ListViewItem(r["ID"].ToString());
                listViewItem.SubItems.Add(r["Name"].ToString());
                materialListViewCategory.Items.Add(listViewItem);
            }
        }

        private void DataCategoryView_Closed()
        {
            materialListViewCategory.Items.Clear();
        }

        private void load_database_Category()
        {
            DataTable task;
            string query = "SELECT * FROM CATEGORY";
            task = db.GetDataTable(query);
            DataCategoryView_Closed();   // Clear old data
            load_DataTable_Category(task);
        }

        private void materialListViewCategory_Click(object sender, EventArgs e)
        {
            materialTextBoxCategoryName.Text =  materialListViewCategory.SelectedItems[0].SubItems[1].Text.Trim();
        }

        private void tabPageCategory_Enter(object sender, EventArgs e)
        {
            load_database_Category();
        }

        private void materialButtonCategoryAdd_Click(object sender, EventArgs e)
        {
            string name = materialTextBoxCategoryName.Text.Trim();
            if (db.CheckName(name, "CATEGORY"))
            {
                MessageBox.Show("Existed");
            }
            else
            {
                string check = "SELECT Name FROM CATEGORY";
                DataTable task = db.GetDataTable(check);
                string id;
                if (task.Rows.Count > 0)
                {
                    id = (task.Rows.Count + 1).ToString();
                }
                else
                {
                    id = (1).ToString();
                }
                string query = $"INSERT INTO CATEGORY VALUES({id}, '{name}')";
                bool category_insert = db.ExucuteDbCmd(query);
                if (category_insert)
                {
                    MessageBox.Show("Add successful");
                    Reload_All(sender, e);
                }
                else
                {
                    MessageBox.Show("Fail to add");
                }
            }
        }

        private void materialButtonCategoryEdit_Click(object sender, EventArgs e)
        {
            if (materialListViewCategory.SelectedItems.Count > 0)
            {
                string id = materialListViewCategory.SelectedItems[0].SubItems[0].Text.Trim();
                string name = materialTextBoxCategoryName.Text.Trim();
                int check_category = db.CheckCategory(id);
                if (check_category != -1)
                {
                    DialogResult dialogResult = MessageBox.Show($"Are you sure to change cagetory\n {check_category} tag will be changed", "New cagetory detected", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        string query = $"UPDATE CATEGORY SET Name = '{name}' WHERE ID = {id}";
                        bool result = db.ExucuteDbCmd(query);
                        if (result)
                        {
                            MessageBox.Show($"Edit successful\n{check_category} tag is changed");
                            Reload_All(sender, e);
                        }
                        else
                        {
                            MessageBox.Show("Fail to edit");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Fail to edit");
                    }
                }
                else
                {
                    string query = $"UPDATE CATEGORY SET Name = '{name}' WHERE ID = {id}";
                    bool result = db.ExucuteDbCmd(query);
                    if (result)
                    {
                        MessageBox.Show("Edit successful");
                        Reload_All(sender, e);
                    }
                    else
                    {
                        MessageBox.Show("Fail to edit");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please choose cagetory");
            }
        }

        private void materialButtonCategoryDelete_Click(object sender, EventArgs e)
        {
            if (materialListViewCategory.SelectedItems.Count > 0)
            {
                string id = materialListViewCategory.SelectedItems[0].SubItems[0].Text.Trim();
                string name = materialTextBoxCategoryName.Text.Trim();
                int check_category = db.CheckCategory(id);
                if (check_category != -1)
                {
                     MessageBox.Show($"Cant delete this cagetory\n {check_category} tag have book");
                }
                else
                {
                    string query = $"DELETE FROM CATEGORY WHERE ID = {id}";
                    bool result = db.ExucuteDbCmd(query);
                    if (result)
                    {
                        MessageBox.Show("Delete successful");
                        Reload_All(sender, e);
                    }
                    else
                    {
                        MessageBox.Show("Fail to delete");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please choose cagetory");
            }
        }

        private void materialButtonCategoryReload_Click(object sender, EventArgs e)
        {
            materialTextBoxCategoryName.Text = "";
            load_database_Category();
        }

        // end category tab

        // -----------------------------------------------------------------------------------------------
        //
        //
        // Exit tab
        private void tabPageExit_Enter(object sender, EventArgs e)
        {
            Close();
        }
        // end exit tab

        // -----------------------------------------------------------------------------------------------

        private void ngaytra_Click(object sender, EventArgs e)
        {

        }

        private void Main_Load(object sender, EventArgs e)
        {

        }

        private void materialLabel4_Click(object sender, EventArgs e)
        {

        }

        private void materialTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void materialTextBox4_TextChanged(object sender, EventArgs e)
        {

        }





        private void textBox5_a_TextChanged(object sender, EventArgs e)
        {

        }

        private void loaisach_Click(object sender, EventArgs e)
        {

        }

        private void Reload_All(object sender, EventArgs e)
        {
            materialButtonHomeReload_Click(sender, e);
            materialButtonDetailReload_Click(sender, e);
            materialButtonBorrowReload_Click(sender, e);
            materialButtonReturnReload_Click(sender, e);
            materialButtonReaderReload_Click(sender, e);
            materialButtonCategoryReload_Click(sender, e);
        }

        private void txtbCode_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabPageDetail_Click(object sender, EventArgs e)
        {

        }

        private void tabReturn_Enter(object sender, EventArgs e)
        {
            materialTextBoxDateReturn.Text = DateTime.Now.ToShortDateString();
        }

        private void materialListView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
