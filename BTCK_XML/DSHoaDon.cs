using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
namespace BTCK_XML
{
    public partial class DSHoaDon : Form
    {
        string strCon = "Data Source=LAPTOP-HF76ABDE\\BINH;Initial Catalog=dbQUANLYCUAHANGGAUBONG;Integrated Security=True";
        private string fileXML = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "HoaDon.xml");
        private TaoXML taoXML = new TaoXML();

        public DSHoaDon()
        {
            InitializeComponent();
            this.Load += new EventHandler(DSHoaDon_Load);
            dataGridView1.CellClick += dataGridView1_CellClick;
        }

        private void DSHoaDon_Load(object sender, EventArgs e)
        {
            try
            {
                EnsureDataFolderExists();
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
           + "INNER JOIN KhachHang ON HoaDon.Makhachhang = KhachHang.Makhachhang";
                    // Truy vấn SQL để lấy dữ liệu
                    string bang = "HoaDon"; // Tên bảng

                    // Tạo tệp XML từ cơ sở dữ liệu
                    taoXML.taoXML(sql, bang, fileXML);

                    // Thông báo cho người dùng
                    MessageBox.Show("Tệp XML đã được tạo thành công.");
                }
                LoadComboBoxData();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
        private void EnsureDataFolderExists()
        {
            string dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }
        }
        private void LoadData()
        {
            // Tải dữ liệu từ tệp XML vào DataGridView
            DataTable dt = taoXML.loadDataGridView(fileXML);
            dataGridView1.DataSource = dt;
        }
        private void LoadComboBoxData()
        {
            try
            {
                // Truy vấn để lấy danh sách nhân viên
                string sql = "SELECT Manhanvien, Tennhanvien FROM NhanVien";

                // Sử dụng phương thức ExecuteQuery để tải dữ liệu từ cơ sở dữ liệu
                DataTable dtNhanVien = taoXML.ExecuteQuery(sql);

                // Gán dữ liệu vào ComboBox
                cbbNhanvien.DataSource = dtNhanVien;
                cbbNhanvien.DisplayMember = "Tennhanvien"; // Hiển thị tên nhân viên
                cbbNhanvien.ValueMember = "Manhanvien"; // Giá trị khóa

                // Truy vấn để lấy danh sách gaubong
                string sqlgb = "SELECT Magaubong,Tengaubong FROM Gaubong";

                // Sử dụng phương thức ExecuteQuery để tải dữ liệu từ cơ sở dữ liệu
                DataTable dtGaubong = taoXML.ExecuteQuery(sqlgb);

                // Gán dữ liệu vào ComboBox
                cbbSanpham.DataSource = dtGaubong;
                cbbSanpham.DisplayMember = "Tengaubong"; // Hiển thị tên nhân viên
                cbbSanpham.ValueMember = "Magaubong"; // Giá trị khóa
                // Truy vấn để lấy danh sách gaubong
                string sqlkh = "SELECT Makhachhang,Tenkhachhang FROM KhachHang";

                // Sử dụng phương thức ExecuteQuery để tải dữ liệu từ cơ sở dữ liệu
                DataTable dtkhachhang = taoXML.ExecuteQuery(sqlkh);

                // Gán dữ liệu vào ComboBox
                ccbKhachhang.DataSource = dtkhachhang;
                ccbKhachhang.DisplayMember = "Tenkhachhang";
                ccbKhachhang.ValueMember = "Makhachhang"; // Giá trị khóa
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu vào ComboBox: " + ex.Message);
            }
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Ensure valid row
            {
                string maHD = dataGridView1.Rows[e.RowIndex].Cells["Mahoadon"].Value.ToString();

                // Safely retrieve values from XML
                string tenKH = taoXML.LayGiaTri(fileXML, "Mahoadon", maHD, "Tenkhachhang");
                string tenGB = taoXML.LayGiaTri(fileXML, "Mahoadon", maHD, "Tengaubong");
                string ngaydat = taoXML.LayGiaTri(fileXML, "Mahoadon", maHD, "Ngaydathang");
                string Noigiaohang = taoXML.LayGiaTri(fileXML, "Mahoadon", maHD, "Noigiaohang");
                string Soluongmua = taoXML.LayGiaTri(fileXML, "Mahoadon", maHD, "Soluongmua");
                string Tennhanvien = taoXML.LayGiaTri(fileXML, "Mahoadon", maHD, "Tennhanvien");

                // Fill controls with fetched data
                txtMahoadon.Text = maHD;

                // Ensure ComboBox selections are safe
                ccbKhachhang.SelectedValue = ccbKhachhang.Items.Cast<DataRowView>()
                    .FirstOrDefault(item => item["Tenkhachhang"].ToString() == tenKH)?["Makhachhang"];

                cbbSanpham.SelectedValue = cbbSanpham.Items.Cast<DataRowView>()
                    .FirstOrDefault(item => item["Tengaubong"].ToString() == tenGB)?["Magaubong"];

                // Parse and set the date
                if (DateTime.TryParseExact(ngaydat, new[] { "yyyy-MM-dd", "dd/MM/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    dtpNgaydat.Value = parsedDate;
                }

                txtNoigiaohang.Text = Noigiaohang;
                txtSoluong.Text = Soluongmua;

                // Set the selected index for employees
                cbbNhanvien.SelectedValue = cbbNhanvien.Items.Cast<DataRowView>()
                    .FirstOrDefault(item => item["Tennhanvien"].ToString() == Tennhanvien)?["Manhanvien"];
            }
        }


        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                int soluong;
                if (!int.TryParse(txtSoluong.Text.Trim(), out soluong))
                {
                    MessageBox.Show("Số lượng mua không hợp lệ!");
                    return;
                }

                // Tạo phần tử XML
                XElement newHoaDon = new XElement("HoaDon",
                    new XElement("Mahoadon", txtMahoadon.Text.Trim()),
                    new XElement("Tenkhachhang", ccbKhachhang.Text.Trim()), 
                    new XElement("Tengaubong", cbbSanpham.Text.Trim()),
                    new XElement("Ngaydathang", dtpNgaydat.Value.ToString("yyyy-MM-dd")),
                    new XElement("Noigiaohang", txtNoigiaohang.Text.Trim()),
                    new XElement("Soluongmua", soluong),
                    new XElement("Tennhanvien", cbbNhanvien.Text.Trim())
                    );
                // Kiểm tra xem tệp XML có tồn tại không
                if (System.IO.File.Exists(fileXML))
                {
                    // Thêm phần tử mới vào tệp XML
                    taoXML.Them(fileXML, newHoaDon.ToString());
                }
                else
                {
                    // Nếu tệp không tồn tại, tạo tệp XML mới với phần tử nhân viên đầu tiên
                    XDocument doc = new XDocument(new XElement("NewDataSet", newHoaDon));
                    doc.Save(fileXML);
                }

                // Bắt đầu một giao dịch
                using (var connection = new SqlConnection(strCon))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Thêm dữ liệu vào bảng HoaDon
                            string sqlInsertHoaDon = "INSERT INTO HoaDon (Mahoadon, Makhachhang, Manhanvien, Ngaydathang, Noigiaohang) " +
                                                     "VALUES (@Mahoadon, @Makhachhang, @Manhanvien, @Ngaydathang, @Noigiaohang)";
                            SqlCommand cmdInsertHoaDon = new SqlCommand(sqlInsertHoaDon, connection);
                            cmdInsertHoaDon.Transaction = transaction;
                            cmdInsertHoaDon.Parameters.AddWithValue("@Mahoadon", txtMahoadon.Text.Trim());
                            cmdInsertHoaDon.Parameters.AddWithValue("@Makhachhang", ccbKhachhang.SelectedValue);
                            cmdInsertHoaDon.Parameters.AddWithValue("@Manhanvien", cbbNhanvien.SelectedValue);
                            cmdInsertHoaDon.Parameters.AddWithValue("@Ngaydathang", dtpNgaydat.Value);
                            cmdInsertHoaDon.Parameters.AddWithValue("@Noigiaohang", txtNoigiaohang.Text.Trim());
                            cmdInsertHoaDon.ExecuteNonQuery();

                            // Thêm dữ liệu vào bảng CTHoaDon
                            string sqlInsertCTHoaDon = "INSERT INTO CTHoaDon (Mahoadon, Magaubong, Soluongmua) " +
                                                       "VALUES (@Mahoadon, @Magaubong, @Soluongmua)";
                            SqlCommand cmdInsertCTHoaDon = new SqlCommand(sqlInsertCTHoaDon, connection);
                            cmdInsertCTHoaDon.Transaction = transaction;
                            cmdInsertCTHoaDon.Parameters.AddWithValue("@Mahoadon", txtMahoadon.Text.Trim());
                            cmdInsertCTHoaDon.Parameters.AddWithValue("@Magaubong", cbbSanpham.SelectedValue);
                            cmdInsertCTHoaDon.Parameters.AddWithValue("@Soluongmua", int.Parse(txtSoluong.Text.Trim()));
                            cmdInsertCTHoaDon.ExecuteNonQuery();

                            // Xác nhận giao dịch
                            transaction.Commit();

                            MessageBox.Show("Thêm thành công!");

                            // Xóa dữ liệu trong các textbox sau khi thêm
                            txtMahoadon.Text = "";
                            ccbKhachhang.Text = "";
                            cbbSanpham.Text = "";
                            dtpNgaydat.Text = "";
                            txtNoigiaohang.Text = "";
                            txtSoluong.Text = "";
                            cbbNhanvien.Text = "";

                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            // Bỏ qua giao dịch nếu có lỗi xảy ra
                            transaction.Rollback();
                            MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
            }
        }



        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                string tenBang1 = "HoaDon";
                string tenBang2 = "CTHoaDon";
                string tenCot = "Mahoadon";
                string giaTri = txtMahoadon.Text.Trim();

                // Kiểm tra xem tệp XML có tồn tại không
                if (!System.IO.File.Exists(fileXML))
                {
                    MessageBox.Show("Tệp XML không tồn tại.");
                    return;
                }

                // Xóa bản ghi khỏi cơ sở dữ liệu
                taoXML.Xoa_Database(fileXML, tenCot, giaTri, tenBang2);
                taoXML.Xoa_Database(fileXML, tenCot, giaTri, tenBang1);
                // Tạo XPath để tìm nút cần xóa trong XML
                string xpath = $"/NewDataSet/HoaDon[Mahoadon[text()='{giaTri}']]";

                // Gọi phương thức để xóa khỏi XML
                taoXML.xoa(fileXML, xpath);
                txtMahoadon.Text = "";
                ccbKhachhang.Text = "";
                cbbSanpham.Text = "";
                dtpNgaydat.Text = "";
                txtNoigiaohang.Text = "";
                txtSoluong.Text = "";
                cbbNhanvien.Text = "";
                // Tải lại dữ liệu sau khi xóa
                LoadData();

                MessageBox.Show("Xóa thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
            }
        }



        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if required fields are filled
                if (string.IsNullOrWhiteSpace(txtMahoadon.Text) ||
                    string.IsNullOrWhiteSpace(txtNoigiaohang.Text) ||
                    string.IsNullOrWhiteSpace(txtSoluong.Text) ||
                    ccbKhachhang.SelectedValue == null || // Ensure customer is selected
                    cbbNhanvien.SelectedValue == null || // Ensure employee is selected
                    cbbSanpham.SelectedValue == null) // Ensure product is selected
                {
                    MessageBox.Show("Hãy nhập đầy đủ thông tin cần sửa và chọn khách hàng, nhân viên, sản phẩm!");
                    return;
                }

                using (var connection = new SqlConnection(strCon))
                {
                    connection.Open();

                    // Update query for HoaDon table
                    string sqlUpdateHoaDon = "UPDATE HoaDon SET Makhachhang = @Makhachhang, " +
                                             "Manhanvien = @Manhanvien, Ngaydathang = @Ngaydathang, Noigiaohang = @Noigiaohang " +
                                             "WHERE Mahoadon = @Mahoadon";

                    using (var cmdUpdateHoaDon = new SqlCommand(sqlUpdateHoaDon, connection))
                    {
                        // Parameters for HoaDon update
                        cmdUpdateHoaDon.Parameters.AddWithValue("@Mahoadon", txtMahoadon.Text.Trim());
                        cmdUpdateHoaDon.Parameters.AddWithValue("@Makhachhang", ccbKhachhang.SelectedValue.ToString().Trim()); // Customer ID
                        cmdUpdateHoaDon.Parameters.AddWithValue("@Manhanvien", cbbNhanvien.SelectedValue.ToString().Trim()); // Employee ID
                        cmdUpdateHoaDon.Parameters.AddWithValue("@Ngaydathang", dtpNgaydat.Value); // Date
                        cmdUpdateHoaDon.Parameters.AddWithValue("@Noigiaohang", txtNoigiaohang.Text.Trim()); // Delivery address

                        // Execute the HoaDon update query
                        int rowsAffectedHoaDon = cmdUpdateHoaDon.ExecuteNonQuery();

                        if (rowsAffectedHoaDon <= 0)
                        {
                            MessageBox.Show("Không tìm thấy hóa đơn với mã: " + txtMahoadon.Text);
                            return;
                        }
                        else
                        {
                            LoadData();

                            // Optionally, update the XML file if needed
                            taoXML.taoXML("SELECT HoaDon.Mahoadon, KhachHang.Tenkhachhang, GauBong.Tengaubong, HoaDon.Ngaydathang, HoaDon.Noigiaohang, CTHoaDon.Soluongmua, NhanVien.Tennhanvien \r\nFROM HoaDon INNER JOIN CTHoaDon ON HoaDon.Mahoadon = CTHoaDon.Mahoadon \r\n          INNER JOIN NhanVien ON HoaDon.Manhanvien = NhanVien.Manhanvien\r\n           INNER JOIN GauBong ON CTHoaDon.Magaubong = GauBong.Magaubong\r\n\t\t   INNER JOIN KhachHang ON HoaDon.Makhachhang = KhachHang.Makhachhang", "HoaDon", fileXML);
                        }
                    }

                    // Update query for CTHoaDon table
                    string sqlUpdateCTHoaDon = "UPDATE CTHoaDon SET Soluongmua = @Soluongmua " +
                                               "WHERE Mahoadon = @Mahoadon AND Magaubong = @Magaubong";

                    using (var cmdUpdateCTHoaDon = new SqlCommand(sqlUpdateCTHoaDon, connection))
                    {
                        // Parameters for CTHoaDon update
                        cmdUpdateCTHoaDon.Parameters.AddWithValue("@Mahoadon", txtMahoadon.Text.Trim());
                        cmdUpdateCTHoaDon.Parameters.AddWithValue("@Magaubong", cbbSanpham.SelectedValue?.ToString().Trim()); // Product ID
                        cmdUpdateCTHoaDon.Parameters.AddWithValue("@Soluongmua", txtSoluong.Text.Trim()); // Quantity

                        // Execute the CTHoaDon update query
                        int rowsAffectedCTHoaDon = cmdUpdateCTHoaDon.ExecuteNonQuery();

                        if (rowsAffectedCTHoaDon <= 0)
                        {
                            MessageBox.Show("Không tìm thấy chi tiết hóa đơn với mã: " + txtMahoadon.Text);
                            return;
                        }
                        else
                        {
                            LoadData();

                            // Optionally, update the XML file if needed
                            taoXML.taoXML("SELECT HoaDon.Mahoadon, KhachHang.Tenkhachhang, GauBong.Tengaubong, HoaDon.Ngaydathang, HoaDon.Noigiaohang, CTHoaDon.Soluongmua, NhanVien.Tennhanvien \r\nFROM HoaDon INNER JOIN CTHoaDon ON HoaDon.Mahoadon = CTHoaDon.Mahoadon \r\n          INNER JOIN NhanVien ON HoaDon.Manhanvien = NhanVien.Manhanvien\r\n           INNER JOIN GauBong ON CTHoaDon.Magaubong = GauBong.Magaubong\r\n\t\t   INNER JOIN KhachHang ON HoaDon.Makhachhang = KhachHang.Makhachhang", "HoaDon", fileXML);
                        }
                    }

                    // If both updates are successful, notify the user
                    MessageBox.Show("Cập nhật thông tin hóa đơn và chi tiết thành công!");

                    // Load updated data into the form or grid
                    LoadData();

                    // Clear the input fields
                    txtMahoadon.Text = "";
                    ccbKhachhang.SelectedIndex = -1;
                    cbbSanpham.SelectedIndex = -1;
                    dtpNgaydat.Value = DateTime.Now;
                    txtNoigiaohang.Text = "";
                    txtSoluong.Text = "";
                    cbbNhanvien.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
            }
        }



        private void DSHoaDon_Load_1(object sender, EventArgs e)
        {

        }



        private void btnThoat_Click(object sender, EventArgs e)
        {
            Home Home = new Home();
            Home.Show();
            this.Hide();
        }

        private void DSHoaDon_FormClosing(object sender, FormClosingEventArgs e)
        {
            Home trangchu = new Home();
            trangchu.Show();
        }
    }
}