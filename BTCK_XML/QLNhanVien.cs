using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;


namespace BTCK_XML
{
    public partial class QLNhanVien : Form
    {
        private string strCon = "Data Source=DESKTOP-PMTVGB7\\MSSQLTHAO;Initial Catalog=dbQUANLYCUAHANGGAUBONG;Integrated Security=True";
        private TaoXML taoXML = new TaoXML();
        private string fileXML = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Nhanvien.xml");

        public QLNhanVien()
        {
            InitializeComponent();
            this.Load += new EventHandler(QLNhanVien_Load);
            dataGridView1.CellClick += dataGridView1_CellClick;
        }

        private void QLNhanVien_Load(object sender, EventArgs e)
        {
            try
            {
                // Đường dẫn tới tệp XML
                EnsureDataFolderExists();

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
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
        // Tạo thư mục Data nếu chưa tồn tại
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

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Đảm bảo rằng hàng đã chọn là hợp lệ
            {
                // Lấy mã nhân viên từ cột "Manhanvien"
                string maNV = dataGridView1.Rows[e.RowIndex].Cells["Manhanvien"].Value.ToString();

                // Sử dụng phương thức LayGiaTri để lấy các thông tin khác
                string tenNV = taoXML.LayGiaTri(fileXML, "Manhanvien", maNV, "Tennhanvien");
                string gioiTinh = taoXML.LayGiaTri(fileXML, "Manhanvien", maNV, "Gioitinh");
                string ngaysinh = taoXML.LayGiaTri(fileXML, "Manhanvien", maNV, "Ngaysinh");
                string diaChi = taoXML.LayGiaTri(fileXML, "Manhanvien", maNV, "Diachi");
                string sdt = taoXML.LayGiaTri(fileXML, "Manhanvien", maNV, "SDT");

                // Điền dữ liệu vào các TextBox
                txtMaNV.Text = maNV;
                txtTenNV.Text = tenNV;
                txtGioitinh.Text = gioiTinh;
                dtpNgaysinh.Value = Convert.ToDateTime(ngaysinh); // Chuyển đổi định dạng ngày
                txtDiachi.Text = diaChi;
                txtSdt.Text = sdt;
            }
        }
        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                // Tạo một phần tử XML mới cho nhân viên
                XElement newNhanVien = new XElement("NhanVien",
                    new XElement("Manhanvien", txtMaNV.Text.Trim()),
                    new XElement("Tennhanvien", txtTenNV.Text.Trim()),
                    new XElement("Gioitinh", txtGioitinh.Text.Trim()),
                    new XElement("Ngaysinh", dtpNgaysinh.Value.ToString("yyyy-MM-dd")),
                    new XElement("Diachi", txtDiachi.Text.Trim()),
                    new XElement("SDT", txtSdt.Text.Trim())
                );

                // Kiểm tra xem tệp XML có tồn tại không
                if (System.IO.File.Exists(fileXML))
                {
                    // Thêm phần tử mới vào tệp XML
                    taoXML.Them(fileXML, newNhanVien.ToString());
                }
                else
                {
                    // Nếu tệp không tồn tại, tạo tệp XML mới với phần tử nhân viên đầu tiên
                    XDocument doc = new XDocument(new XElement("NewDataSet", newNhanVien));
                    doc.Save(fileXML);
                }

                // Thêm vào cơ sở dữ liệu
                taoXML.Them_Database_NhanVien("NhanVien", fileXML);


                MessageBox.Show("Thêm thành công!");

                // Xóa dữ liệu trong các textbox sau khi thêm
                txtMaNV.Text = "";
                txtTenNV.Text = "";
                txtGioitinh.Text = "";
                txtDiachi.Text = "";
                txtSdt.Text = "";
                LoadData();
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
                if (string.IsNullOrWhiteSpace(txtMaNV.Text) ||
                    string.IsNullOrWhiteSpace(txtTenNV.Text) ||
                    string.IsNullOrWhiteSpace(txtGioitinh.Text) ||
                    string.IsNullOrWhiteSpace(txtDiachi.Text) ||
                    string.IsNullOrWhiteSpace(txtSdt.Text))
                {
                    MessageBox.Show("Hãy nhập đầy đủ thông tin cần sửa!");
                    return;
                }

                using (var connection = new SqlConnection(strCon))
                {
                    connection.Open();

                    // SQL query to update employee information
                    string sqlUpdate = "UPDATE NhanVien SET Tennhanvien = @Tennhanvien, Gioitinh = @Gioitinh, " +
                                       "Ngaysinh = @Ngaysinh, Diachi = @Diachi, SDT = @SDT " +
                                       "WHERE Manhanvien = @Manhanvien";

                    using (var cmdUpdate = new SqlCommand(sqlUpdate, connection))
                    {
                        // Add parameters to avoid SQL injection
                        cmdUpdate.Parameters.AddWithValue("@Manhanvien", txtMaNV.Text.Trim());
                        cmdUpdate.Parameters.AddWithValue("@Tennhanvien", txtTenNV.Text.Trim());
                        cmdUpdate.Parameters.AddWithValue("@Gioitinh", txtGioitinh.Text.Trim());
                        cmdUpdate.Parameters.AddWithValue("@Ngaysinh", dtpNgaysinh.Value);
                        cmdUpdate.Parameters.AddWithValue("@Diachi", txtDiachi.Text.Trim());
                        cmdUpdate.Parameters.AddWithValue("@SDT", txtSdt.Text.Trim());

                        // Execute the update command
                        int rowsAffected = cmdUpdate.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Cập nhật thông tin nhân viên thành công!");

                            // Load updated data into the form or grid
                            LoadData();

                            // Optionally, update the XML file if needed
                            taoXML.taoXML("SELECT * FROM NhanVien", "NhanVien", fileXML);
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy nhân viên với mã: " + txtMaNV.Text);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi sửa: " + ex.Message);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                string tenBang = "NhanVien";
                string tenCot = "Manhanvien";
                string giaTri = txtMaNV.Text.Trim();

                // Kiểm tra xem tệp XML có tồn tại không
                if (!System.IO.File.Exists(fileXML))
                {
                    MessageBox.Show("Tệp XML không tồn tại.");
                    return;
                }

                // Xóa bản ghi khỏi cơ sở dữ liệu
                taoXML.Xoa_Database(fileXML, tenCot, giaTri, tenBang);

                // Tạo XPath để tìm nút cần xóa trong XML
                string xpath = $"/NewDataSet/NhanVien[Manhanvien[text()='{giaTri}']]";

                // Gọi phương thức để xóa khỏi XML
                taoXML.xoa(fileXML, xpath);

                // Tải lại dữ liệu sau khi xóa
                LoadData();

                MessageBox.Show("Xóa thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Home Home = new Home();
            Home.Show();
            this.Hide();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void QLNhanVien_FormClosing(object sender, FormClosingEventArgs e)
        {
            Home home = new Home();
            home.Show();
        }

      
    }


}

