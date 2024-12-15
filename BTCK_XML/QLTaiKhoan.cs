using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace BTCK_XML
{
    public partial class QLTaiKhoan : Form
    {
        private string strCon = "Data Source=LAPTOP-HF76ABDE\\BINH;Initial Catalog=dbQUANLYCUAHANGGAUBONG;Integrated Security=True";
        private string fileXML = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "TaiKhoan.xml");
        private TaoXML taoXML = new TaoXML();

        public QLTaiKhoan()
        {
            InitializeComponent();
            this.Load += QLTaiKhoan_Load;
            dataGridView1.CellClick += dataGridView1_CellClick;
        }

        private void QLTaiKhoan_Load(object sender, EventArgs e)
        {
            try
            {
                EnsureDataFolderExists(); // Tạo thư mục Data nếu chưa tồn tại

                // Load dữ liệu vào ComboBox (cbxQuyen)
                LoadQuyen();

                if (!File.Exists(fileXML))
                {
                    string sql = "SELECT * FROM TaiKhoan";
                    taoXML.taoXML(sql, "TaiKhoan", fileXML); // Tạo file XML từ CSDL
                }

                LoadData(); // Tải dữ liệu từ CSDL lên DataGridView
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
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
            try
            {
                using (var connection = new SqlConnection(strCon))
                {
                    connection.Open();
                    string sql = "SELECT * FROM TaiKhoan"; // Truy vấn lấy dữ liệu tài khoản
                    SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt; // Hiển thị dữ liệu lên DataGridView
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
            }
        }

        private void LoadQuyen()
        {
            try
            {
                using (var connection = new SqlConnection(strCon))
                {
                    connection.Open();

                    // Truy vấn lấy các quyền từ bảng TaiKhoan
                    string sql = "SELECT DISTINCT quyen FROM TaiKhoan"; // Lấy các giá trị quyền duy nhất
                    SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Gán dữ liệu vào ComboBox
                    cbxQuyen.DataSource = dt;
                    cbxQuyen.DisplayMember = "quyen"; // Hiển thị tên quyền
                    cbxQuyen.ValueMember = "quyen";   // Lưu giá trị quyền
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu quyền: " + ex.Message);
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTaiKhoan.Text) || string.IsNullOrWhiteSpace(txtMatKhau.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                    return;
                }

                using (var connection = new SqlConnection(strCon))
                {
                    connection.Open();

                    // Kiểm tra tài khoản đã tồn tại
                    string checkExists = "SELECT COUNT(*) FROM TaiKhoan WHERE taikhoan = @taikhoan";
                    using (var cmdCheck = new SqlCommand(checkExists, connection))
                    {
                        cmdCheck.Parameters.AddWithValue("@taikhoan", txtTaiKhoan.Text.Trim());
                        int count = (int)cmdCheck.ExecuteScalar();
                        if (count > 0)
                        {
                            MessageBox.Show("Tài khoản đã tồn tại!");
                            return;
                        }
                    }

                    // Thêm tài khoản mới
                    string sqlInsert = "INSERT INTO TaiKhoan (taikhoan, Matkhau, quyen) VALUES (@taikhoan, @Matkhau, @quyen)";
                    using (var cmdInsert = new SqlCommand(sqlInsert, connection))
                    {
                        cmdInsert.Parameters.AddWithValue("@taikhoan", txtTaiKhoan.Text.Trim());
                        cmdInsert.Parameters.AddWithValue("@Matkhau", txtMatKhau.Text.Trim());
                        cmdInsert.Parameters.AddWithValue("@quyen", cbxQuyen.SelectedValue);

                        int rowsAffected = cmdInsert.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Thêm tài khoản thành công!");
                            LoadData();
                            taoXML.taoXML("SELECT * FROM TaiKhoan", "TaiKhoan", fileXML); // Cập nhật XML
                        }
                    }
                }
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
                if (string.IsNullOrWhiteSpace(txtTaiKhoan.Text) || string.IsNullOrWhiteSpace(txtMatKhau.Text))
                {
                    MessageBox.Show("Hãy nhập đầy đủ thông tin cần sửa!");
                    return;
                }

                using (var connection = new SqlConnection(strCon))
                {
                    connection.Open();

                    string sqlUpdate = "UPDATE TaiKhoan SET Matkhau = @Matkhau, quyen = @quyen WHERE taikhoan = @taikhoan";
                    using (var cmdUpdate = new SqlCommand(sqlUpdate, connection))
                    {
                        cmdUpdate.Parameters.AddWithValue("@taikhoan", txtTaiKhoan.Text.Trim());
                        cmdUpdate.Parameters.AddWithValue("@Matkhau", txtMatKhau.Text.Trim());
                        cmdUpdate.Parameters.AddWithValue("@quyen", cbxQuyen.SelectedValue);

                        int rowsAffected = cmdUpdate.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Cập nhật tài khoản thành công!");
                            LoadData();
                            taoXML.taoXML("SELECT * FROM TaiKhoan", "TaiKhoan", fileXML); // Cập nhật XML
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy tài khoản để sửa!");
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
                if (string.IsNullOrWhiteSpace(txtTaiKhoan.Text))
                {
                    MessageBox.Show("Vui lòng chọn tài khoản để xóa!");
                    return;
                }

                var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa tài khoản này?", "Xác nhận xóa", MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.No) return;

                using (var connection = new SqlConnection(strCon))
                {
                    connection.Open();

                    string sqlDelete = "DELETE FROM TaiKhoan WHERE taikhoan = @taikhoan";
                    using (var cmdDelete = new SqlCommand(sqlDelete, connection))
                    {
                        cmdDelete.Parameters.AddWithValue("@taikhoan", txtTaiKhoan.Text.Trim());

                        int rowsAffected = cmdDelete.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Xóa tài khoản thành công!");
                            LoadData();
                            taoXML.taoXML("SELECT * FROM TaiKhoan", "TaiKhoan", fileXML); // Cập nhật XML
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy tài khoản để xóa!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa: " + ex.Message);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
            Home Home = new Home();
            Home.Show();

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                    txtTaiKhoan.Text = row.Cells["taikhoan"].Value.ToString();
                    txtMatKhau.Text = row.Cells["Matkhau"].Value.ToString();
                    cbxQuyen.SelectedValue = row.Cells["quyen"].Value;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi chọn tài khoản: " + ex.Message);
            }
        }

        private void QLTaiKhoan_FormClosing(object sender, FormClosingEventArgs e)
        {
            Home trangchu = new Home();
            trangchu.Show();
        }
    }
}
