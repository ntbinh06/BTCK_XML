using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace BTCK_XML
{
    public partial class DangNhap : Form
    {
        // Chuỗi kết nối tới cơ sở dữ liệu
        string strCon = "Data Source=DESKTOP-PMTVGB7\\MSSQLTHAO;Initial Catalog=dbQUANLYCUAHANGGAUBONG;Integrated Security=True";

        public DangNhap()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string tenDangNhap = txtName.Text.Trim();
            string matKhau = txtPass.Text.Trim();

            // Kiểm tra người dùng đã nhập đầy đủ thông tin chưa
            if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrWhiteSpace(matKhau))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin đăng nhập!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Kết nối tới cơ sở dữ liệu
                using (SqlConnection connection = new SqlConnection(strCon))
                {
                    connection.Open();

                    // Lấy thông tin tài khoản và quyền
                    string sql = "SELECT Matkhau, Quyen FROM TaiKhoan WHERE taikhoan = @TenDangNhap";
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string matKhauDb = reader["Matkhau"].ToString();
                                int quyen = Convert.ToInt32(reader["Quyen"]);

                                if (matKhauDb == matKhau)
                                {
                                    MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    // Kiểm tra quyền của tài khoản
                                    if (quyen == 1)
                                    {
                                        // Quyền hạn chế - Chỉ xem form DSHoaDon
                                        DSHoaDon formDSHoaDon = new DSHoaDon();
                                        formDSHoaDon.Show();
                                    }
                                    else if (quyen == 2)
                                    {
                                        // Quyền đầy đủ - Xem tất cả form
                                        Home formHome = new Home();
                                        formHome.Show();
                                    }

                                    this.Hide(); // Ẩn form đăng nhập
                                }
                                else
                                {
                                    MessageBox.Show("Mật khẩu không đúng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Tài khoản không tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
