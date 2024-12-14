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
    public partial class Home : Form
    {
        private string strCon = "Data Source=LAPTOP-HF76ABDE\\BINH;Initial Catalog=dbQUANLYCUAHANGGAUBONG;Integrated Security=True";
        private TaoXML taoXML = new TaoXML();
        private string fileXML = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Nhanvien.xml");
        public Home()
        {
            InitializeComponent();
        }

        private void Home_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void bearCute_Click(object sender, EventArgs e)
        {
            QuanLiGauBong quanLiGauBongForm = new QuanLiGauBong();
            quanLiGauBongForm.Show();
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
        private void tabPage2_Load(object sender, EventArgs e)
        {
            try
            {
                // Đường dẫn tới tệp XML
                string dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
                string fileXML = Path.Combine(dataFolder, "Nhanvien.xml"); // Đường dẫn tới tệp XML

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
        private void btnTimKiemMa_Click(object sender, EventArgs e)
        {
            // Lấy mã nhân viên từ TextBox
            string maNV = txtSearchMa.Text.Trim(); // Giả sử bạn có một TextBox với tên txtSearchMa
            string fileXML = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Nhanvien.xml"); // Đường dẫn đến tệp XML của bạn
            string tenfileXSLT = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Nhanvien.xslt"); // Đường dẫn đến tệp XSLT của bạn

            // Kiểm tra xem mã nhân viên có hợp lệ không
            if (string.IsNullOrWhiteSpace(maNV))
            {
                MessageBox.Show("Vui lòng nhập mã nhân viên để tìm kiếm.");
                return;
            }

            // Tìm kiếm bằng XPath và hiển thị kết quả trong DataGridView
            string xmlXPath = $"/NewDataSet/NhanVien[Manhanvien[text()='{maNV}']]"; // XPath tìm kiếm
            taoXML.TimKiem(fileXML, xmlXPath, dataGridView1);

            // Tìm kiếm bằng XSLT và biến đổi XML thành HTML
            taoXML.TimKiemXSLT(maNV, fileXML, tenfileXSLT);
        }

        private void btnTimKiemTen_Click(object sender, EventArgs e)
        {
            // Lấy tên nhân viên từ TextBox
            string tenNV = txtSearchTen.Text.Trim(); // Giả sử bạn có một TextBox với tên txtSearchTen
            string fileXML = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Nhanvien.xml"); // Đường dẫn đến tệp XML của bạn
            string tenfileXSLT = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Nhanvien2.xslt"); // Đường dẫn đến tệp XSLT của bạn

            // Kiểm tra xem tên nhân viên có hợp lệ không
            if (string.IsNullOrWhiteSpace(tenNV))
            {
                MessageBox.Show("Vui lòng nhập tên nhân viên để tìm kiếm.");
                return;
            }


            // Tìm kiếm bằng XPath và hiển thị kết quả trong DataGridView
            string xmlXPath = $"/NewDataSet/NhanVien[contains(Tennhanvien, '{tenNV}')]"; // XPath tìm kiếm
            taoXML.TimKiem(fileXML, xmlXPath, dataGridView1);

            // Tìm kiếm bằng XSLT và biến đổi XML thành HTML
            taoXML.TimKiemXSLT(tenNV, fileXML, tenfileXSLT);
        }
    }
}
