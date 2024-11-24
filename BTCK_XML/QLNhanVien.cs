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
        private TaoXML taoXML = new TaoXML();
        private string fileXML = Path.Combine("F:\\124\\XML\\BaiTapLonCK", "nhanvien.xml");

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
        private void btn_Them_Click(object sender, EventArgs e)
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
        private void btn_Sua_Click(object sender, EventArgs e)
        {
            try
            {
                string tenBang = "NhanVien"; // Tên bảng trong cơ sở dữ liệu
                string tenCot = "Manhanvien"; // Tên cột để tìm kiếm (ví dụ: mã nhân viên)
                string giaTri = txtMaNV.Text.Trim(); // Giá trị tìm kiếm từ textbox

                // Kiểm tra xem tệp XML có tồn tại không
                if (!System.IO.File.Exists(fileXML))
                {
                    MessageBox.Show("Tệp XML không tồn tại.");
                    return;
                }

                // Cập nhật cơ sở dữ liệu
                taoXML.Sua_Database(tenBang, fileXML, tenCot, giaTri);

                // Tải tệp XML và tìm nút cần cập nhật
                XmlDocument doc = new XmlDocument();
                doc.Load(fileXML);
                string xpath = $"/NewDataSet/NhanVien[Manhanvien[text()='{giaTri}']]"; // XPath để xác định nút cần cập nhật
                XmlNode nodeToUpdate = doc.SelectSingleNode(xpath);

                // Kiểm tra xem nút có tồn tại không
                if (nodeToUpdate != null)
                {
                    // Cập nhật các giá trị của nút hiện có
                    nodeToUpdate["Tennhanvien"].InnerText = txtTenNV.Text.Trim();
                    nodeToUpdate["Gioitinh"].InnerText = txtGioitinh.Text.Trim();
                    nodeToUpdate["Ngaysinh"].InnerText = dtpNgaysinh.Value.ToString("yyyy-MM-dd");
                    nodeToUpdate["Diachi"].InnerText = txtDiachi.Text.Trim();
                    nodeToUpdate["SDT"].InnerText = txtSdt.Text.Trim();

                    // Lưu lại tệp XML
                    doc.Save(fileXML);

                    // Tải lại dữ liệu sau khi cập nhật
                    LoadData();
                    txtMaNV.Text = "";
                    txtTenNV.Text = "";
                    txtGioitinh.Text = "";
                    txtDiachi.Text = "";
                    txtSdt.Text = "";
                    MessageBox.Show("Cập nhật thành công!");
                }
                else
                {
                    MessageBox.Show("Không tìm thấy nhân viên với mã: " + giaTri);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
            }
        }

        private void btn_Xoa_Click(object sender, EventArgs e)
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

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }


}

