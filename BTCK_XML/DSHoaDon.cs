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
        string strCon = "Data Source=DESKTOP-NLSH69G\\OANH;Initial Catalog=dbQUANLYCUAHANGGAUBONG;Integrated Security=True";
        private TaoXML taoXML = new TaoXML();
        private string fileXML = Path.Combine("F:\\124\\XML\\BaiTapLonCK", "hoadon.xml");

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

                // Kiểm tra xem tệp có tồn tại không
                if (!File.Exists(fileXML))
                {
                    // Nếu không tồn tại, tạo tệp XML mới
                    string sql = "SELECT HoaDon.Mahoadon, HoaDon.Tenkhachhang, GauBong.Tengaubong, "
           + "HoaDon.Ngaydathang, HoaDon.Noigiaohang, CTHoaDon.Soluongmua, NhanVien.Tennhanvien "
           + "FROM HoaDon "
           + "INNER JOIN CTHoaDon ON HoaDon.Mahoadon = CTHoaDon.Mahoadon "
           + "INNER JOIN NhanVien ON HoaDon.Manhanvien = NhanVien.Manhanvien "
           + "INNER JOIN GauBong ON CTHoaDon.Magaubong = GauBong.Magaubong";// Truy vấn SQL để lấy dữ liệu
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu vào ComboBox: " + ex.Message);
            }
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Đảm bảo rằng hàng đã chọn là hợp lệ
            {

                string maHD = dataGridView1.Rows[e.RowIndex].Cells["Mahoadon"].Value.ToString();

                // Sử dụng phương thức LayGiaTri để lấy các thông tin khác
                string tenKH = taoXML.LayGiaTri(fileXML, "Mahoadon", maHD, "Tenkhachhang");
                string tenGB = taoXML.LayGiaTri(fileXML, "Mahoadon", maHD, "Tengaubong");
                string ngaydat = taoXML.LayGiaTri(fileXML, "Mahoadon", maHD, "Ngaydathang");
                string Noigiaohang = taoXML.LayGiaTri(fileXML, "Mahoadon", maHD, "Noigiaohang");
                string Soluongmua = taoXML.LayGiaTri(fileXML, "Mahoadon", maHD, "Soluongmua");
                string Tennhanvien = taoXML.LayGiaTri(fileXML, "Mahoadon", maHD, "Tennhanvien");

                // Điền dữ liệu vào các TextBox
                txtMahoadon.Text = maHD;
                txtTenkhachhang.Text = tenKH;
                int indexgb = cbbSanpham.FindStringExact(tenGB);
                cbbSanpham.SelectedIndex = indexgb;
                ngaydat = taoXML.LayGiaTri(fileXML, "Mahoadon", maHD, "Ngaydathang");
                if (DateTime.TryParseExact(ngaydat, new[] { "yyyy-MM-dd", "dd/MM/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    dtpNgaydat.Value = parsedDate;
                }
                else
                {
                    // Xử lý lỗi định dạng ngày tháng
                }
                txtNoigiaohang.Text = Noigiaohang;
                txtSoluong.Text = Soluongmua;
                int index = cbbNhanvien.FindStringExact(Tennhanvien);
                cbbNhanvien.SelectedIndex = index;
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
                    new XElement("Tenkhachhang", txtTenkhachhang.Text.Trim()),
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
                            string sqlInsertHoaDon = "INSERT INTO HoaDon (Mahoadon, Tenkhachhang, Manhanvien, Ngaydathang, Noigiaohang) " +
                                                     "VALUES (@Mahoadon, @Tenkhachhang, @Manhanvien, @Ngaydathang, @Noigiaohang)";
                            SqlCommand cmdInsertHoaDon = new SqlCommand(sqlInsertHoaDon, connection);
                            cmdInsertHoaDon.Transaction = transaction;
                            cmdInsertHoaDon.Parameters.AddWithValue("@Mahoadon", txtMahoadon.Text.Trim());
                            cmdInsertHoaDon.Parameters.AddWithValue("@Tenkhachhang", txtTenkhachhang.Text.Trim());
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
                            txtTenkhachhang.Text = "";
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
                txtTenkhachhang.Text = "";
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

                // Cập nhật cơ sở dữ liệu
                taoXML.Sua_Database(tenBang2, fileXML, tenCot, giaTri);
                taoXML.Sua_Database(tenBang1, fileXML, tenCot, giaTri);

                // Tải tệp XML và tìm nút cần cập nhật
                XmlDocument doc = new XmlDocument();
                doc.Load(fileXML);
                string xpath = $"/NewDataSet/{tenBang1}[{tenCot}[text()='{giaTri}']]";
                XmlNode nodeToUpdate = doc.SelectSingleNode(xpath);

                // Kiểm tra xem nút có tồn tại không
                if (nodeToUpdate != null)
                {

                    // Cập nhật các giá trị của nút hiện có
                    nodeToUpdate["Tenkhachhang"].InnerText = txtTenkhachhang.Text.Trim();
                    nodeToUpdate["Tengaubong"].InnerText = cbbSanpham.SelectedValue?.ToString().Trim(); // Sửa ở đây
                    nodeToUpdate["Ngaydathang"].InnerText = dtpNgaydat.Value.ToString("yyyy-MM-dd");
                    nodeToUpdate["Noigiaohang"].InnerText = txtNoigiaohang.Text.Trim();
                    nodeToUpdate["Soluongmua"].InnerText = txtSoluong.Text.Trim();
                    nodeToUpdate["Tennhanvien"].InnerText = cbbNhanvien.SelectedValue?.ToString().Trim(); // Sửa ở đây

                    // Lưu lại tệp XML
                    doc.Save(fileXML);

                    // Tải lại dữ liệu sau khi cập nhật
                    LoadData();
                    txtMahoadon.Text = "";
                    txtTenkhachhang.Text = "";
                    cbbSanpham.Text = "";
                    dtpNgaydat.Text = "";
                    txtNoigiaohang.Text = "";
                    txtSoluong.Text = "";
                    cbbNhanvien.Text = "";
                    MessageBox.Show("Cập nhật thành công!");
                }
                else
                {
                    MessageBox.Show("Không tìm thấy hóa đơn với mã: " + giaTri);
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
    }
}