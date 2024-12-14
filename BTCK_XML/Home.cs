using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BTCK_XML
{
    public partial class Home : Form
    {
        private string strCon = "Data Source=LAPTOP-HF76ABDE\\BINH;Initial Catalog=dbQUANLYCUAHANGGAUBONG;Integrated Security=True";
        private TaoXML taoXML = new TaoXML();
        private string fileXML = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Nhanvien.xml");
        public Home()
        {
            InitializeComponent();
        }

        private void Home_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
        }

        private void LoadComboBoxData()
        {
            // Danh sách cột mà người dùng có thể tìm kiếm
            List<string> columns = new List<string> { "Magaubong", "Tengaubong", "Gia" };

            // Xóa các mục cũ nếu có
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();

            // Thêm các cột vào ComboBox
            foreach (string column in columns)
            {
                comboBox1.Items.Add(column);
                comboBox2.Items.Add(column);
            }

            // Đặt mục mặc định
            if (comboBox1.Items.Count > 0) comboBox1.SelectedIndex = 0;
            if (comboBox2.Items.Count > 0) comboBox2.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Lấy giá trị từ ComboBox và TextBox
            string column1 = comboBox1.SelectedItem.ToString(); // Cột 1
            string value1 = textBox1.Text.Trim(); // Giá trị nhập trong TextBox 1
            string column2 = comboBox2.SelectedItem.ToString(); // Cột 2
            string value2 = textBox2.Text.Trim(); // Giá trị nhập trong TextBox 2

            // Kiểm tra loại tìm kiếm (AND/OR)
            string condition = radioButtonAnd.Checked ? "AND" : (radioButtonOr.Checked ? "OR" : "");

            if (string.IsNullOrEmpty(condition))
            {
                MessageBox.Show("Vui lòng chọn loại tìm kiếm: AND hoặc OR.");
                return;
            }

            // Thực hiện tìm kiếm
            TimKiem(column1, value1, column2, value2, condition);
        }

        private void TimKiem(string column1, string value1, string column2, string value2, string condition)
        {
            using (SqlConnection con = new SqlConnection(strCon))
            {
                try
                {
                    // Mở kết nối
                    con.Open();

                    // Xây dựng câu lệnh SQL cơ bản
                    string query = "SELECT Magaubong, Tengaubong, Madanhmuc, Soluong, Gia, Mota FROM GauBong WHERE 1=1";

                    // Điều kiện 1 (ComboBox1 và TextBox1)
                    string condition1 = "";
                    if (!string.IsNullOrEmpty(value1))
                    {
                        if (column1 == "Gia") // Xử lý giá trị kiểu số cho cột "Gia"
                        {
                            if (decimal.TryParse(value1, out decimal giaValue))
                            {
                                condition1 = $"{column1} = {giaValue}";
                            }
                            else
                            {
                                MessageBox.Show("Giá trị nhập cho cột 'Gia' không hợp lệ. Vui lòng nhập số.");
                                return;
                            }
                        }
                        else // Các cột khác
                        {
                            condition1 = $"{column1} LIKE N'%{value1}%'";
                        }
                    }

                    // Điều kiện 2 (ComboBox2 và TextBox2)
                    string condition2 = "";
                    if (!string.IsNullOrEmpty(value2))
                    {
                        if (column2 == "Gia") // Xử lý giá trị kiểu số cho cột "Gia"
                        {
                            if (decimal.TryParse(value2, out decimal giaValue))
                            {
                                condition2 = $"{column2} = {giaValue}";
                            }
                            else
                            {
                                MessageBox.Show("Giá trị nhập cho cột 'Gia' không hợp lệ. Vui lòng nhập số.");
                                return;
                            }
                        }
                        else // Các cột khác
                        {
                            condition2 = $"{column2} LIKE N'%{value2}%'";
                        }
                    }

                    // Kết hợp các điều kiện
                    if (!string.IsNullOrEmpty(condition1) && !string.IsNullOrEmpty(condition2))
                    {
                        query += $" AND ({condition1} {condition} {condition2})";
                    }
                    else if (!string.IsNullOrEmpty(condition1))
                    {
                        query += $" AND ({condition1})";
                    }
                    else if (!string.IsNullOrEmpty(condition2))
                    {
                        query += $" AND ({condition2})";
                    }

                    // Kiểm tra truy vấn SQL
                    Console.WriteLine($"Generated Query: {query}");

                    // Tạo SqlCommand và thực thi câu lệnh SQL
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Hiển thị kết quả tìm kiếm vào DataGridView
                    dataGridView1.DataSource = dt;
                    dataGridView1.Refresh(); // Làm mới DataGridView

                    // Thông báo nếu không có kết quả
                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("Không tìm thấy kết quả!");
                    }
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi nếu xảy ra
                    MessageBox.Show("Lỗi khi tìm kiếm: " + ex.Message);
                }
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
        private void tabPage2_Load(object sender, EventArgs e)
        {
            try
            {
                // Đường dẫn tới tệp XML
                string dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
                string fileXML = Path.Combine(dataFolder, "Nhanvien.xml"); // Đường dẫn tới tệp XML

                // Kiểm tra xem tệp có tồn tại không
                if (!File.Exists(fileXML))
                {
                    // Nếu không tồn tại, tạo tệp XML mới
                    string sql = "SELECT * FROM NhanVien"; // Truy vấn SQL để lấy dữ liệu
                    string bang = "NhanVien"; // Tên bảng

                    // Tạo tệp XML từ cơ sở dữ liệu
                    taoXML.taoXML(sql, bang, fileXML);

                    // Thông báo cho người dùng
                    MessageBox.Show("Tệp XML đã được tạo thành công.");
                }

                // Tải dữ liệu từ tệp XML vào DataGridView
                LoadData(fileXML);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
        private void LoadData(string fileXML)
        {
            // Tải dữ liệu từ tệp XML vào DataGridView
            DataTable dt = taoXML.loadDataGridView(fileXML);
            dataGridView1.DataSource = dt;
        }
        private void btnTimKiemMa_Click(object sender, EventArgs e)
        {
            // Lấy mã nhân viên từ TextBox
            string maNV = txtSearchMa.Text.Trim(); // Giả sử bạn có một TextBox với tên txtSearchMa
            string fileXML = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Nhanvien.xml"); // Đường dẫn đến tệp XML của bạn
            string tenfileXSLT = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Nhanvien.xslt"); // Đường dẫn đến tệp XSLT của bạn

            // Kiểm tra xem mã nhân viên có hợp lệ không
            if (string.IsNullOrWhiteSpace(maNV))
            {
                MessageBox.Show("Vui lòng nhập mã nhân viên để tìm kiếm.");
                return;
            }

            // Tìm kiếm bằng XPath và hiển thị kết quả trong DataGridView
            string xmlXPath = $"/NewDataSet/NhanVien[Manhanvien[text()='{maNV}']]"; // XPath tìm kiếm
            taoXML.TimKiem(fileXML, xmlXPath, dataGridView1);

            // Tìm kiếm bằng XSLT và biến đổi XML thành HTML
            taoXML.TimKiemXSLT(maNV, fileXML, tenfileXSLT);
        }

        private void btnTimKiemTen_Click(object sender, EventArgs e)
        {
            // Lấy tên nhân viên từ TextBox
            string tenNV = txtSearchTen.Text.Trim(); // Giả sử bạn có một TextBox với tên txtSearchTen
            string fileXML = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Nhanvien.xml"); // Đường dẫn đến tệp XML của bạn
            string tenfileXSLT = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Nhanvien2.xslt"); // Đường dẫn đến tệp XSLT của bạn

            // Kiểm tra xem tên nhân viên có hợp lệ không
            if (string.IsNullOrWhiteSpace(tenNV))
            {
                MessageBox.Show("Vui lòng nhập tên nhân viên để tìm kiếm.");
                return;
            }


            // Tìm kiếm bằng XPath và hiển thị kết quả trong DataGridView
            string xmlXPath = $"/NewDataSet/NhanVien[contains(Tennhanvien, '{tenNV}')]"; // XPath tìm kiếm
            taoXML.TimKiem(fileXML, xmlXPath, dataGridView1);

            // Tìm kiếm bằng XSLT và biến đổi XML thành HTML
            taoXML.TimKiemXSLT(tenNV, fileXML, tenfileXSLT);
        }
    }
}
