using System;
using System.IO;
using System.Windows.Forms;

namespace BTCK_XML.Resources
{
    public partial class TimKiemNV : Form
    {
        public TimKiemNV()
        {
            InitializeComponent();
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

            // Khởi tạo đối tượng TaoXML
            TaoXML taoXML = new TaoXML(); // Giả sử bạn đã tạo lớp TaoXML

            // Tìm kiếm bằng XPath và hiển thị kết quả trong DataGridView
            string xmlXPath = $"/NewDataSet/NhanVien[Manhanvien[text()='{maNV}']]"; // XPath tìm kiếm
            taoXML.TimKiem(fileXML, xmlXPath, dataGridView1);

            // Tìm kiếm bằng XSLT và biến đổi XML thành HTML
            taoXML.TimKiemXSLT(maNV, fileXML, tenfileXSLT);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}