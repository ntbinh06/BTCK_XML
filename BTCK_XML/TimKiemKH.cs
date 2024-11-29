using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace BTCK_XML
{
    public partial class TimKiemKH : Form
    {
        private string strCon = "Data Source=DESKTOP-PMTVGB7\\MSSQLTHAO;Initial Catalog=dbQUANLYCUAHANGGAUBONG;Integrated Security=True";

        public TimKiemKH()
        {
            InitializeComponent();
        }

        private void TimKiemKH_Load(object sender, EventArgs e)
        {
            // Tải danh sách khách hàng khi form load
            LoadCustomers();
        }

        // Phương thức tải danh sách khách hàng theo từ khóa tìm kiếm
        private void LoadCustomers(string search = "", string searchType = "")
        {
            try
            {
                using (var connection = new SqlConnection(strCon))
                {
                    connection.Open();
                    string sql = "SELECT * FROM KhachHang";

                    // Thêm điều kiện tìm kiếm nếu có
                    if (!string.IsNullOrEmpty(search))
                    {
                        if (searchType == "MaKH")
                        {
                            // Tìm kiếm theo mã khách hàng (CHAR(5))
                            sql += " WHERE Makhachhang LIKE @search"; // Dùng LIKE vì Makhachhang là chuỗi
                        }
                        else if (searchType == "TenKH")
                        {
                            // Tìm kiếm theo tên khách hàng
                            sql += " WHERE Tenkhachhang LIKE @search";
                        }
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
                    adapter.SelectCommand.Parameters.AddWithValue("@search", "%" + search + "%");
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

        // Tìm kiếm theo mã khách hàng (button TimKiemMa)
        private void btnTimKiemMa_Click(object sender, EventArgs e)
        {
            string searchText = txtSearchMa.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                LoadCustomers(searchText, "MaKH"); // Tìm kiếm theo mã khách hàng
            }
            else
            {
                MessageBox.Show("Vui lòng nhập mã khách hàng để tìm kiếm.");
            }
        }

        // Tìm kiếm theo tên khách hàng (button TimKiemTen)
        private void btnTimKiemTen_Click(object sender, EventArgs e)
        {
            string searchText = txtSearchTen.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                LoadCustomers(searchText, "TenKH"); // Tìm kiếm theo tên khách hàng
            }
            else
            {
                MessageBox.Show("Vui lòng nhập tên khách hàng để tìm kiếm.");
            }
        }

        // Thoát khỏi form (button Thoat)
        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close(); // Đóng form tìm kiếm
        }
    }
}