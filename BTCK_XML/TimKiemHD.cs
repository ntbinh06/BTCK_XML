using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BTCK_XML
{
    public partial class TimKiemHD : Form
    {
        private string strCon = "Data Source=DESKTOP-PMTVGB7\\MSSQLTHAO;Initial Catalog=dbQUANLYCUAHANGGAUBONG;Integrated Security=True";
        private TaoXML taoXML = new TaoXML();
        private string fileXML = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "HoaDon.xml");
        public TimKiemHD()
        {
            InitializeComponent();
        }

        private void btnTimKiemMa_Click(object sender, EventArgs e)
        {
           
            string maHD = txtSearchMa.Text.Trim(); // Giả sử bạn có một TextBox với tên txtSearchMa
            string fileXML = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "HoaDon.xml"); // Đường dẫn đến tệp XML của bạn
            string tenfileXSLT = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "HoaDon.xslt"); // Đường dẫn đến tệp XSLT của bạn

            // Kiểm tra xem mã hóa đơn có hợp lệ không
            if (string.IsNullOrWhiteSpace(maHD))
            {
                MessageBox.Show("Vui lòng nhập mã hóa đơn để tìm kiếm.");
                return;
            }

          
            string xmlXPath = $"/NewDataSet/HoaDon[Mahoadon[text()='{maHD}']]"; // XPath tìm kiếm
            taoXML.TimKiem(fileXML, xmlXPath, dataGridView1);

            // Tìm kiếm bằng XSLT và biến đổi XML thành HTML
            taoXML.TimKiemXSLT(maHD, fileXML, tenfileXSLT);
        }

        private void TimKiemHD_Load(object sender, EventArgs e)
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
            dataGridView1.DataSource = dt;
        }

        private void btnTimKiemTen_Click(object sender, EventArgs e)
        {
            // Lấy tên khách hàng từ TextBox
            string tenKH = txtSearchTen.Text.Trim(); // Giả sử bạn có một TextBox với tên txtSearchTen
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
            taoXML.TimKiem(fileXML, xmlXPath, dataGridView1);

            // Tìm kiếm bằng XSLT và biến đổi XML thành HTML
            taoXML.TimKiemXSLT(tenKH, fileXML, tenfileXSLT);
        }
    }

}
