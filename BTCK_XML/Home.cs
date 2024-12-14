using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace BTCK_XML
{
    public partial class Home : Form
    {
        string strCon = "Data Source=DESKTOP-PMTVGB7\\MSSQLTHAO;Initial Catalog=dbQUANLYCUAHANGGAUBONG;Integrated Security=True";
        private TaoXML taoXML = new TaoXML();
        public Home()
        {
            InitializeComponent();
        }

        private void Home_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
            LoadNhanVienData();
            LoadHoaDonData();
            LoadKH();
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

        private void btnTimKiemMaNV_Click(object sender, EventArgs e)
        {
            // Lấy mã nhân viên từ TextBox
            string maNV = txtSearchMaNV.Text.Trim(); // Giả sử bạn có một TextBox với tên txtSearchMa
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
            taoXML.TimKiem(fileXML, xmlXPath, dataGridViewNV);

            // Tìm kiếm bằng XSLT và biến đổi XML thành HTML
            taoXML.TimKiemXSLT(maNV, fileXML, tenfileXSLT);
        }

        private void btnTimKiemTenNV_Click(object sender, EventArgs e)
        {
            // Lấy tên nhân viên từ TextBox
            string tenNV = txtSearchTenNV.Text.Trim(); // Giả sử bạn có một TextBox với tên txtSearchTen
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
            taoXML.TimKiem(fileXML, xmlXPath, dataGridViewNV);

            // Tìm kiếm bằng XSLT và biến đổi XML thành HTML
            taoXML.TimKiemXSLT(tenNV, fileXML, tenfileXSLT);
        }

        private void LoadNhanVienData()
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
            dataGridViewNV.DataSource = dt;
        }
        private void LoadDataHD(string fileXML)
        {
            // Tải dữ liệu từ tệp XML vào DataGridView
            DataTable dt = taoXML.loadDataGridView(fileXML);
            dataGridViewHD.DataSource = dt;
        }

        private void LoadHoaDonData()
        {
            try
            {
                // Đường dẫn tới tệp XML
                string dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
                string fileXML = Path.Combine(dataFolder, "HoaDon.xml"); // Đường dẫn tới tệp XML

                // Kiểm tra xem tệp có tồn tại không
                if (!File.Exists(fileXML))
                {
                    // Nếu không tồn tại, tạo tệp XML mới
                    string sql = "SELECT HoaDon.Mahoadon, KhachHang.Tenkhachhang, GauBong.Tengaubong, "
                               + "HoaDon.Ngaydathang, HoaDon.Noigiaohang, CTHoaDon.Soluongmua, NhanVien.Tennhanvien "
                               + "FROM HoaDon "
                               + "INNER JOIN CTHoaDon ON HoaDon.Mahoadon = CTHoaDon.Mahoadon "
                               + "INNER JOIN NhanVien ON HoaDon.Manhanvien = NhanVien.Manhanvien "
                               + "INNER JOIN GauBong ON CTHoaDon.Magaubong = GauBong.Magaubong "
                               + "INNER JOIN KhachHang ON HoaDon.Makhachhang = KhachHang.Makhachhang"; // Truy vấn SQL để lấy dữ liệu
                    string bang = "HoaDon"; // Tên bảng

                    // Tạo tệp XML từ cơ sở dữ liệu
                    taoXML.taoXML(sql, bang, fileXML);

                    // Thông báo cho người dùng
                    MessageBox.Show("Tệp XML đã được tạo thành công.");
                }

                // Tải dữ liệu từ tệp XML vào DataGridView
                LoadDataHD(fileXML);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
        private void btnTimKiemMaHD_Click(object sender, EventArgs e)
        {

            string maHD = txtMaHD.Text.Trim(); // Giả sử bạn có một TextBox với tên txtSearchMa
            string fileXML = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "HoaDon.xml"); // Đường dẫn đến tệp XML của bạn
            string tenfileXSLT = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "HoaDon.xslt"); // Đường dẫn đến tệp XSLT của bạn

            // Kiểm tra xem mã hóa đơn có hợp lệ không
            if (string.IsNullOrWhiteSpace(maHD))
            {
                MessageBox.Show("Vui lòng nhập mã hóa đơn để tìm kiếm.");
                return;
            }


            string xmlXPath = $"/NewDataSet/HoaDon[Mahoadon[text()='{maHD}']]"; // XPath tìm kiếm
            taoXML.TimKiem(fileXML, xmlXPath, dataGridViewHD);

            // Tìm kiếm bằng XSLT và biến đổi XML thành HTML
            taoXML.TimKiemXSLT(maHD, fileXML, tenfileXSLT);
        }

        private void btnTimKiemTenK_Click(object sender, EventArgs e)
        {
            // Lấy tên khách hàng từ TextBox
            string tenKH = txtTenKHHD.Text.Trim(); // Giả sử bạn có một TextBox với tên txtSearchTen
            string fileXML = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "HoaDon.xml"); // Đường dẫn đến tệp XML của bạn
            string tenfileXSLT = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "HoaDon2.xslt"); // Đường dẫn đến tệp XSLT của bạn

            // Kiểm tra xem tên khách hàng có hợp lệ không
            if (string.IsNullOrWhiteSpace(tenKH))
            {
                MessageBox.Show("Vui lòng nhập tên khách hàng để tìm kiếm.");
                return;
            }


            // Tìm kiếm bằng XPath và hiển thị kết quả trong DataGridView
            string xmlXPath = $"/NewDataSet/HoaDon[contains(Tenkhachhang, '{tenKH}')]"; // XPath tìm kiếm
            taoXML.TimKiem(fileXML, xmlXPath, dataGridViewHD);

            // Tìm kiếm bằng XSLT và biến đổi XML thành HTML
            taoXML.TimKiemXSLT(tenKH, fileXML, tenfileXSLT);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            DSHoaDon DSHoaDon = new DSHoaDon();
            DSHoaDon.Show();
            this.Hide();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            QLNhanVien QLNhanVien = new QLNhanVien();
            QLNhanVien.Show();
            this.Hide();
        }

        private void btnTimKiemKHMa_Click(object sender, EventArgs e)
        {
            // Lấy mã khách hàng từ TextBox
            string maKH = txtSearchMa.Text.Trim(); // Giả sử bạn có một TextBox với tên txtSearchMaKH
            string fileXML = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "KhachHang.xml"); // Đường dẫn đến tệp XML của bạn
            string tenfileXSLT = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "KhachHang_MaKH.xslt"); // Đường dẫn đến tệp XSLT của bạn

            // Kiểm tra xem mã khách hàng có hợp lệ không
            if (string.IsNullOrWhiteSpace(maKH))
            {
                MessageBox.Show("Vui lòng nhập mã khách hàng để tìm kiếm.");
                return;
            }

            // Tìm kiếm bằng XPath và hiển thị kết quả trong DataGridView
            string xmlXPath = $"/NewDataSet/KhachHang[contains(Makhachhang, '{maKH}')]"; // XPath tìm kiếm
            taoXML.TimKiem(fileXML, xmlXPath, dataGridView2);

            // Tìm kiếm bằng XSLT và biến đổi XML thành HTML
            taoXML.TimKiemXSLT(maKH, fileXML, tenfileXSLT);
        }

        private void btnTimKiemKHTen_Click(object sender, EventArgs e)
        {
            // Lấy tên khách hàng từ TextBox
            string tenKH = txtSearchTen.Text.Trim(); // Giả sử bạn có một TextBox với tên txtSearchTenKH
            string fileXML = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "KhachHang.xml"); // Đường dẫn đến tệp XML của bạn
            string tenfileXSLT = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "KhachHang_TenKH.xslt"); // Đường dẫn đến tệp XSLT của bạn

            // Kiểm tra xem tên khách hàng có hợp lệ không
            if (string.IsNullOrWhiteSpace(tenKH))
            {
                MessageBox.Show("Vui lòng nhập tên khách hàng để tìm kiếm.");
                return;
            }

            // Tìm kiếm bằng XPath và hiển thị kết quả trong DataGridView
            string xmlXPath = $"/NewDataSet/KhachHang[contains(Tenkhachhang, '{tenKH}')]"; // XPath tìm kiếm
            taoXML.TimKiem(fileXML, xmlXPath, dataGridView2);

            // Tìm kiếm bằng XSLT và biến đổi XML thành HTML
            taoXML.TimKiemXSLT(tenKH, fileXML, tenfileXSLT);
        }

        private void LoadKH()
        {
            try
            {
                using (var connection = new SqlConnection(strCon))
                {
                    connection.Open();
                    string sql = "SELECT * FROM KhachHang"; // Truy vấn để lấy dữ liệu
                    SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView2.DataSource = dt; // Hiển thị dữ liệu lên DataGridView
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            QLTaiKhoan qLTaiKhoan = new QLTaiKhoan();
            qLTaiKhoan.Show();
            this.Hide();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            QLKhachHang qliKH = new QLKhachHang();
            qliKH.Show();
            this.Hide();
        }

        
    }
}