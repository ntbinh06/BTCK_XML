using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace BTCK_XML
{
    public partial class QLKhachHang : Form
    {
        private string strCon = "Data Source=DESKTOP-PMTVGB7\\MSSQLTHAO;Initial Catalog=dbQUANLYCUAHANGGAUBONG;Integrated Security=True";
        private string fileXML = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "KhachHang.xml");
        private TaoXML taoXML = new TaoXML();

        public QLKhachHang()
        {
            InitializeComponent();
            this.Load += QLKhachHang_Load; // Gán sự kiện Load cho form
            dataGridView1.CellClick += dataGridView1_CellClick; // Gán sự kiện click trên DataGridView
        }

        // Phương thức xử lý sự kiện khi form được load
        private void QLKhachHang_Load(object sender, EventArgs e)
        {
            try
            {
                EnsureDataFolderExists(); // Tạo thư mục Data nếu chưa tồn tại

                // Kiểm tra nếu file XML chưa tồn tại thì tạo mới từ CSDL
                if (!File.Exists(fileXML))
                {
                    string sql = "SELECT * FROM KhachHang";
                    taoXML.taoXML(sql, "KhachHang", fileXML); // Gọi lớp tạo XML để tạo file
                }

                LoadData(); // Tải dữ liệu từ cơ sở dữ liệu lên DataGridView
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
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

        // Phương thức tải dữ liệu từ cơ sở dữ liệu lên DataGridView
        private void LoadData()
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

                    dataGridView1.DataSource = dt; // Hiển thị dữ liệu lên DataGridView
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
            }
        }

        // Phương thức xử lý sự kiện khi nhấn nút thêm
        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMaKhachHang.Text) ||
                    string.IsNullOrWhiteSpace(txtTenKhachHang.Text) ||
                    string.IsNullOrWhiteSpace(txtSDT.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                    return;
                }

                using (var connection = new SqlConnection(strCon))
                {
                    connection.Open();

                    string checkExists = "SELECT COUNT(*) FROM KhachHang WHERE Makhachhang = @Makhachhang";
                    using (var cmdCheck = new SqlCommand(checkExists, connection))
                    {
                        cmdCheck.Parameters.AddWithValue("@Makhachhang", txtMaKhachHang.Text.Trim());
                        int count = (int)cmdCheck.ExecuteScalar();
                        if (count > 0)
                        {
                            MessageBox.Show("Mã khách hàng đã tồn tại!");
                            return;
                        }
                    }

                    string sqlInsert = "INSERT INTO KhachHang (Makhachhang, Tenkhachhang, Gioitinh, Ngaysinh, Diachi, SDT, Email) " +
                                       "VALUES (@Makhachhang, @Tenkhachhang, @Gioitinh, @Ngaysinh, @Diachi, @SDT, @Email)";
                    using (var cmdInsert = new SqlCommand(sqlInsert, connection))
                    {
                        cmdInsert.Parameters.AddWithValue("@Makhachhang", txtMaKhachHang.Text.Trim());
                        cmdInsert.Parameters.AddWithValue("@Tenkhachhang", txtTenKhachHang.Text.Trim());
                        cmdInsert.Parameters.AddWithValue("@Gioitinh", txtGioiTinh.Text.Trim());
                        cmdInsert.Parameters.AddWithValue("@Ngaysinh", dateTimePicker1.Value);
                        cmdInsert.Parameters.AddWithValue("@Diachi", txtDiaChi.Text.Trim());
                        cmdInsert.Parameters.AddWithValue("@SDT", txtSDT.Text.Trim());
                        cmdInsert.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());

                        int rowsAffected = cmdInsert.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Thêm khách hàng thành công!");
                            LoadData();
                            taoXML.taoXML("SELECT * FROM KhachHang", "KhachHang", fileXML);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    txtMaKhachHang.Text = dataGridView1.Rows[e.RowIndex].Cells["Makhachhang"].Value.ToString();
                    txtTenKhachHang.Text = dataGridView1.Rows[e.RowIndex].Cells["Tenkhachhang"].Value.ToString();
                    txtGioiTinh.Text = dataGridView1.Rows[e.RowIndex].Cells["Gioitinh"].Value.ToString();
                    dateTimePicker1.Value = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells["Ngaysinh"].Value);
                    txtDiaChi.Text = dataGridView1.Rows[e.RowIndex].Cells["Diachi"].Value.ToString();
                    txtSDT.Text = dataGridView1.Rows[e.RowIndex].Cells["SDT"].Value.ToString();
                    txtEmail.Text = dataGridView1.Rows[e.RowIndex].Cells["Email"].Value.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi chọn dữ liệu: " + ex.Message);
                }
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(txtMaKhachHang.Text) ||
                        string.IsNullOrWhiteSpace(txtTenKhachHang.Text) ||
                        string.IsNullOrWhiteSpace(txtSDT.Text))
                    {
                        MessageBox.Show("Hãy nhập đầy đủ thông tin cần sửa!");
                        return;
                    }

                    using (var connection = new SqlConnection(strCon))
                    {
                        connection.Open();

                        string sqlUpdate = "UPDATE KhachHang SET Tenkhachhang = @Tenkhachhang, Gioitinh = @Gioitinh, " +
                                           "Ngaysinh = @Ngaysinh, Diachi = @Diachi, SDT = @SDT, Email = @Email " +
                                           "WHERE Makhachhang = @Makhachhang";

                        using (var cmdUpdate = new SqlCommand(sqlUpdate, connection))
                        {
                            cmdUpdate.Parameters.AddWithValue("@Makhachhang", txtMaKhachHang.Text.Trim());
                            cmdUpdate.Parameters.AddWithValue("@Tenkhachhang", txtTenKhachHang.Text.Trim());
                            cmdUpdate.Parameters.AddWithValue("@Gioitinh", txtGioiTinh.Text.Trim());
                            cmdUpdate.Parameters.AddWithValue("@Ngaysinh", dateTimePicker1.Value);
                            cmdUpdate.Parameters.AddWithValue("@Diachi", txtDiaChi.Text.Trim());
                            cmdUpdate.Parameters.AddWithValue("@SDT", txtSDT.Text.Trim());
                            cmdUpdate.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());

                            int rowsAffected = cmdUpdate.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Cập nhật thông tin khách hàng thành công!");
                                LoadData();
                                taoXML.taoXML("SELECT * FROM KhachHang", "KhachHang", fileXML);
                            }
                            else
                            {
                                MessageBox.Show("Không tìm thấy khách hàng để sửa!");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi sửa: " + ex.Message);
                }

            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMaKhachHang.Text))
                {
                    MessageBox.Show("Vui lòng chọn khách hàng để xóa!");
                    return;
                }

                var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa khách hàng này?",
                    "Xác nhận xóa", MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.No) return;

                using (var connection = new SqlConnection(strCon))
                {
                    connection.Open();

                    string sqlDelete = "DELETE FROM KhachHang WHERE Makhachhang = @Makhachhang";

                    using (var cmdDelete = new SqlCommand(sqlDelete, connection))
                    {
                        cmdDelete.Parameters.AddWithValue("@Makhachhang", txtMaKhachHang.Text.Trim());

                        int rowsAffected = cmdDelete.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Xóa khách hàng thành công!");
                            LoadData();
                            taoXML.taoXML("SELECT * FROM KhachHang", "KhachHang", fileXML);
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy khách hàng để xóa!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa: " + ex.Message);
            }
        }
    }
}
