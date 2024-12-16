using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace BTCK_XML
{
    public partial class QuanLiGauBong : Form
    {
        string strCon = "Data Source=DESKTOP-PMTVGB7\\MSSQLTHAO;Initial Catalog=dbQUANLYCUAHANGGAUBONG;Integrated Security=True";
        private TaoXML taoXML = new TaoXML();
        private string fileXML = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "gaubong.xml");
        private string danhMucXML = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "danhmuc.xml");

        public QuanLiGauBong()
        {
            InitializeComponent();
            this.Load += QuanLiGauBong_Load;
            dataGridView1.CellClick += dataGridView1_CellClick;
        }

        private void QuanLiGauBong_Load(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(fileXML))
                {
                    string sql = "SELECT * FROM GauBong";
                    taoXML.taoXML(sql, "GauBong", fileXML);
                }

                if (!File.Exists(danhMucXML))
                {
                    string sqlDanhMuc = "SELECT * FROM DanhMuc";
                    taoXML.taoXML(sqlDanhMuc, "DanhMuc", danhMucXML);
                }

                LoadData();
                LoadDanhMuc();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void LoadData()
        {
            try
            {
                using (var connection = new SqlConnection(strCon))
                {
                    connection.Open();
                    string sql = "SELECT * FROM GauBong";  // Truy vấn để lấy dữ liệu mới nhất
                    SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Cập nhật lại DataGridView
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
            }
        }

        private void LoadDanhMuc()
        {
            DataTable dt = taoXML.loadDataGridView(danhMucXML);
            cbDanhMuc.DataSource = dt;
            cbDanhMuc.DisplayMember = "Tendanhmuc";
            cbDanhMuc.ValueMember = "Madanhmuc";
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra nếu người dùng click vào một dòng hợp lệ (không phải header)
            if (e.RowIndex >= 0)
            {
                // Lấy các giá trị từ dòng đã click (thông qua e.RowIndex)
                string maGauBong = dataGridView1.Rows[e.RowIndex].Cells["Magaubong"].Value.ToString();
                string tenGauBong = dataGridView1.Rows[e.RowIndex].Cells["Tengaubong"].Value.ToString();
                string maDanhMuc = dataGridView1.Rows[e.RowIndex].Cells["Madanhmuc"].Value.ToString();
                string soLuong = dataGridView1.Rows[e.RowIndex].Cells["Soluong"].Value.ToString();
                string gia = dataGridView1.Rows[e.RowIndex].Cells["Gia"].Value.ToString();
                string moTa = dataGridView1.Rows[e.RowIndex].Cells["Mota"].Value.ToString();

                // Hiển thị các giá trị này vào các TextBox tương ứng
                txtMaGauBong.Text = maGauBong;
                txtTenGauBong.Text = tenGauBong;
                cbDanhMuc.SelectedValue = maDanhMuc; // Nếu cột "Madanhmuc" là khóa chính, sử dụng SelectedValue để thay đổi danh mục
                txtSoLuong.Text = soLuong;
                txtGia.Text = gia;
                txtMoTa.Text = moTa;
            }
        }




        private void btn_Them_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra đầu vào
                if (string.IsNullOrWhiteSpace(txtMaGauBong.Text) ||
                    string.IsNullOrWhiteSpace(txtTenGauBong.Text) ||
                    cbDanhMuc.SelectedValue == null ||
                    !int.TryParse(txtSoLuong.Text, out int soluong) ||
                    !decimal.TryParse(txtGia.Text, out decimal gia))
                {
                    MessageBox.Show("Hãy nhập đầy đủ và đúng định dạng thông tin!");
                    return;
                }

                // Kết nối cơ sở dữ liệu và kiểm tra mã gấu bông
                using (var connection = new SqlConnection(strCon))
                {
                    connection.Open();

                    // Kiểm tra nếu mã gấu bông đã tồn tại
                    string checkExists = "SELECT COUNT(*) FROM GauBong WHERE Magaubong = @Magaubong";
                    using (var cmdCheck = new SqlCommand(checkExists, connection))
                    {
                        cmdCheck.Parameters.AddWithValue("@Magaubong", txtMaGauBong.Text.Trim());
                        int count = (int)cmdCheck.ExecuteScalar();
                        if (count > 0)
                        {
                            MessageBox.Show("Mã gấu bông đã tồn tại! Vui lòng nhập mã khác.");
                            return;
                        }
                    }

                    // Thêm dữ liệu vào bảng GauBong
                    string sqlInsert = "INSERT INTO GauBong (Magaubong, Tengaubong, Madanhmuc, Soluong, Gia, Mota) " +
                                       "VALUES (@Magaubong, @Tengaubong, @Madanhmuc, @Soluong, @Gia, @Mota)";
                    using (var cmdInsert = new SqlCommand(sqlInsert, connection))
                    {
                        cmdInsert.Parameters.AddWithValue("@Magaubong", txtMaGauBong.Text.Trim());
                        cmdInsert.Parameters.AddWithValue("@Tengaubong", txtTenGauBong.Text.Trim());
                        cmdInsert.Parameters.AddWithValue("@Madanhmuc", cbDanhMuc.SelectedValue.ToString());
                        cmdInsert.Parameters.AddWithValue("@Soluong", soluong);
                        cmdInsert.Parameters.AddWithValue("@Gia", gia);
                        cmdInsert.Parameters.AddWithValue("@Mota", txtMoTa.Text.Trim());

                        int rowsAffected = cmdInsert.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Thêm thành công!");

                            // Cập nhật lại file XML sau khi thêm
                            string sql = "SELECT * FROM GauBong";
                            taoXML.taoXML(sql, "GauBong", fileXML);
                        }
                        else
                        {
                            MessageBox.Show("Không thể thêm dữ liệu.");
                        }
                    }
                }

                // Làm mới dữ liệu trên DataGridView
                ResetFields();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
            }
        }




        private void btn_sua_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra đầu vào
                if (string.IsNullOrWhiteSpace(txtMaGauBong.Text) ||
                    string.IsNullOrWhiteSpace(txtTenGauBong.Text) ||
                    cbDanhMuc.SelectedValue == null ||
                    !int.TryParse(txtSoLuong.Text, out int soluong) ||
                    !decimal.TryParse(txtGia.Text, out decimal gia))
                {
                    MessageBox.Show("Hãy nhập đầy đủ và đúng định dạng thông tin!");
                    return;
                }

                string tenBang = "GauBong"; // Tên bảng trong cơ sở dữ liệu
                string tenCot = "Magaubong"; // Tên cột để tìm kiếm (ví dụ: mã Gấu Bông)
                string giaTri = txtMaGauBong.Text.Trim(); // Giá trị tìm kiếm từ textbox

                // Cập nhật cơ sở dữ liệu
                using (var connection = new SqlConnection(strCon))
                {
                    connection.Open();

                    string sqlUpdate = "UPDATE GauBong SET Tengaubong = @Tengaubong, Madanhmuc = @Madanhmuc, Soluong = @Soluong, Gia = @Gia, Mota = @Mota WHERE Magaubong = @Magaubong";

                    using (var cmdUpdate = new SqlCommand(sqlUpdate, connection))
                    {
                        // Thêm tham số vào câu lệnh SQL
                        cmdUpdate.Parameters.AddWithValue("@Magaubong", txtMaGauBong.Text.Trim()); // Mã Gấu Bông
                        cmdUpdate.Parameters.AddWithValue("@Tengaubong", txtTenGauBong.Text.Trim()); // Tên Gấu Bông
                        cmdUpdate.Parameters.AddWithValue("@Madanhmuc", cbDanhMuc.SelectedValue.ToString()); // Danh Mục
                        cmdUpdate.Parameters.AddWithValue("@Soluong", soluong); // Số Lượng
                        cmdUpdate.Parameters.AddWithValue("@Gia", gia); // Giá
                        cmdUpdate.Parameters.AddWithValue("@Mota", txtMoTa.Text.Trim()); // Mô Tả

                        int rowsAffected = cmdUpdate.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Cập nhật thành công!");

                            // Cập nhật lại file XML sau khi sửa
                            string sql = "SELECT * FROM GauBong";
                            taoXML.taoXML(sql, "GauBong", fileXML);  // Cập nhật lại XML

                            // Tải lại dữ liệu sau khi sửa
                            LoadData();
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy Gấu Bông với mã: " + giaTri);
                        }
                    }
                }

                // Làm mới các textbox sau khi cập nhật
                ResetFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
            }
        }



        // Hàm làm mới các textbox
        private void ResetFields()
        {
            txtMaGauBong.Text = "";
            txtTenGauBong.Text = "";
            txtSoLuong.Text = "";
            txtGia.Text = "";
            txtMoTa.Text = "";
            cbDanhMuc.SelectedIndex = -1;
        }


        private void btn_xoa_Click(object sender, EventArgs e)
        {
            try
            {
                // Tên bảng và cột trong cơ sở dữ liệu
                string tenBang = "GauBong";
                string tenCot = "Magaubong";
                string giaTri = txtMaGauBong.Text.Trim();  // Lấy mã Gấu Bông từ textbox

                // Kiểm tra xem tệp XML có tồn tại không
                if (!System.IO.File.Exists(fileXML))
                {
                    MessageBox.Show("Tệp XML không tồn tại.");
                    return;
                }

                // Xóa bản ghi khỏi cơ sở dữ liệu
                taoXML.Xoa_Database(fileXML, tenCot, giaTri, tenBang);

                // Tạo XPath để tìm nút cần xóa trong XML
                string xpath = $"/NewDataSet/GauBong[Magaubong[text()='{giaTri}']]";

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



        

        private void btn_thoat_Click(object sender, EventArgs e)
        {
            this.Hide();
            Home home = new Home();
            home.Show();
            
        }
    }
}
