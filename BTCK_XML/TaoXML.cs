using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace BTCK_XML
{
    internal class TaoXML
    {
        string strCon = "Data Source=DESKTOP-PMTVGB7\\MSSQLTHAO;Initial Catalog=dbQUANLYCUAHANGGAUBONG;Integrated Security=True";
        public void taoXML(string sql, string bang, string _FileXML)
        {
            using (SqlConnection con = new SqlConnection(strCon))
            {
                con.Open();
                using (SqlDataAdapter ad = new SqlDataAdapter(sql, con))
                {
                    DataTable dt = new DataTable(bang);
                    ad.Fill(dt);
                    dt.WriteXml(_FileXML, XmlWriteMode.WriteSchema);
                }
            }
        }
        public DataTable loadDataGridView(string filePath)
        {
            DataTable dt = new DataTable();

            if (!File.Exists(filePath))
            {
                MessageBox.Show("File không tồn tại");
                return dt;
            }

            try
            {
             
                dt.ReadXml(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đọc file XML: {ex.Message}");
            }

            return dt;
        }
        public void Them(string FileXML, string xml)
        { 
            try { 
                XmlTextReader textread = new XmlTextReader(FileXML);
                XmlDocument doc = new XmlDocument();
                doc.Load(textread); textread.Close();
                XmlNode currNode; 
                XmlDocumentFragment docFrag = doc.CreateDocumentFragment();
                docFrag.InnerXml = xml;
                currNode = doc.DocumentElement; 
                currNode.InsertAfter(docFrag, currNode.LastChild); 
                doc.Save(FileXML); 
            } catch {
                MessageBox.Show("lỗi");
            } 
        }
        public void xoa(string FileXML, string xml)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(FileXML);

                XmlNode nodeCu = doc.SelectSingleNode(xml);
                doc.DocumentElement.RemoveChild(nodeCu);
                doc.Save(FileXML);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
            }
        }
        public void sua(string FileXML, string sql, string xml, string bang)
        {
            XmlTextReader reader = new XmlTextReader(FileXML);
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);
            reader.Close();

            XmlNode oldValue;
            XmlElement root = doc.DocumentElement;

            // Tìm nút cần sửa
            oldValue = root.SelectSingleNode(sql);

            // Kiểm tra xem oldValue có null không
            if (oldValue == null)
            {
                throw new InvalidOperationException("Không tìm thấy nút để thay thế. Kiểm tra lại truy vấn XPath: " + sql);
            }

            // Tạo phần tử mới từ xml
            XmlElement newValue = doc.CreateElement(bang);
            newValue.InnerXml = xml;

            // Thay thế oldValue bằng newValue
            root.ReplaceChild(newValue, oldValue);

            // Lưu lại tệp XML
            doc.Save(FileXML);
        }
        public void TimKiem(string _FileXML, string xml, DataGridView dgv)
        {
            XmlDocument xDoc = new XmlDocument();

            // Xây dựng đường dẫn tệp XML một cách an toàn
            string fullPath = Path.Combine(Application.StartupPath, _FileXML);

            // Kiểm tra xem tệp có tồn tại không
            if (!File.Exists(fullPath))
            {
                MessageBox.Show("Tệp XML không tồn tại: " + fullPath);
                return;
            }

            xDoc.Load(fullPath);
            string xPath = xml;
            XmlNode node = xDoc.SelectSingleNode(xPath);

            if (node == null)
            {
                MessageBox.Show("Không tìm thấy dữ liệu với XPath: " + xPath);
                return;
            }

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            using (XmlNodeReader nr = new XmlNodeReader(node))
            {
                ds.ReadXml(nr);
            }

            if (ds.Tables.Count > 0)
            {
                dgv.DataSource = ds.Tables[0];
            }
            else
            {
                MessageBox.Show("Không có dữ liệu để hiển thị.");
            }
        }
        public string LayGiaTri(string duongDan, string truongA, string giaTriA, string truongB)
        {
            string giatriB = "";
            DataTable dt = new DataTable();
            dt = loadDataGridView(duongDan);
            int soDongNhanVien = dt.Rows.Count;

            for (int i = 0; i < soDongNhanVien; i++)
            {
                if (dt.Rows[i][truongA].ToString().Trim().Equals(giaTriA))
                {
                    giatriB = dt.Rows[i][truongB].ToString();
                    return giatriB;
                }
            }

            return giatriB;
        }

        public bool KiemTra(string _FileXML, string truongKiemTra, string giaTriKiemTra)
        {
            DataTable dt = new DataTable();
            dt = loadDataGridView(_FileXML);
            dt.DefaultView.RowFilter = truongKiemTra + " ='" + giaTriKiemTra + "'";

            return dt.DefaultView.Count > 0;
        }

        public string txtMa(string tienTo, string _FileXML, string tenCot)
        {
            string txtMa = "";
            DataTable dt = new DataTable();
            dt = loadDataGridView(_FileXML);
            int dem = dt.Rows.Count;

            if (dem == 0)
            {
                txtMa = tienTo + "001";

            }
            else
            {
                int duoi = int.Parse(dt.Rows[dem - 1][tenCot].ToString().Substring(2, 3)) + 1;
                string cuoi = "00" + duoi;
                txtMa = tienTo + "" + cuoi.Substring(cuoi.Length - 3, 3);
            }

            return txtMa;
        }

        public bool KTMa(string _FileXML, string cotMa, string ma)
        {
            bool kt = true;
            DataTable dt = new DataTable();
            dt = loadDataGridView(_FileXML);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][cotMa].ToString().Trim().Equals(ma))
                {
                    kt = false;
                    break; 
                }
            }

            return kt;
        }
        public DataTable ExecuteQuery(string sql)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(strCon))
            {
                con.Open();
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(com))
                    {
                        adapter.Fill(dt); 
                    }
                }
            }
            return dt; 
        }
        public void exCuteNonQuery(string sql)
        {
            using (SqlConnection con = new SqlConnection(strCon))
            {
                con.Open();
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    com.ExecuteNonQuery();
                }
            }
        }

        public void Them_Database(string tenBang, string _FileXML)
        {
            string duongDan = _FileXML;
            DataTable table = loadDataGridView(duongDan);
            int dong = table.Rows.Count - 1;
            string sql = "insert into " + tenBang + " values(";

            for (int j = 0; j < table.Columns.Count - 1; j++)
            {
                sql += "N'" + table.Rows[dong][j].ToString().Trim() + "',";
            }

            sql += "N'" + table.Rows[dong][table.Columns.Count - 1].ToString().Trim() + "')";
            exCuteNonQuery(sql);
        }
        public void Them_Database_NhanVien(string tenBang, string _FileXML)
        {
            string duongDan = _FileXML;
            DataTable table = loadDataGridView(duongDan);
            int dong = table.Rows.Count - 1; 

            string sql = "INSERT INTO " + tenBang + " VALUES (";

            for (int j = 0; j < table.Columns.Count -1; j++)
            {

                if (j == 3)
                {

                    sql += "'" + Convert.ToDateTime(table.Rows[dong][j]).ToString("yyyy-MM-dd") + "', ";
                }
                else
                {
                    sql += "N'" + table.Rows[dong][j].ToString().Trim() + "', ";
                }
            }
            sql += "N'" + table.Rows[dong][table.Columns.Count - 1].ToString().Trim() + "'"; 
            sql += ")";
            exCuteNonQuery(sql);
        }
        public void Sua_Database(string tableName, string _FileXML, string columnName, string value)
        {
            string filePath = _FileXML;
            DataTable table = loadDataGridView(filePath);
            int rowIndex = -1;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (table.Rows[i][columnName].ToString().Trim() == value)
                {
                    rowIndex = i;
                    break;
                }
            }

            if (rowIndex > -1)
            {
                string sql = $"UPDATE {tableName} SET ";

                for (int j = 0; j < table.Columns.Count; j++)
                {

                     columnName = table.Columns[j].ColumnName;
                    string columnValue = table.Rows[rowIndex][j].ToString().Trim();

                    if (table.Columns[j].DataType == typeof(DateTime))
                    {
                        DateTime dateValue = Convert.ToDateTime(columnValue);
                        columnValue = dateValue.ToString("yyyy-MM-dd");
                        sql += $"{columnName} = '{columnValue}', ";
                    }
                    else if (table.Columns[j].DataType == typeof(int))
                    {
                        if (int.TryParse(columnValue, out int intValue))
                        {
                            sql += $"{columnName} = {intValue}, ";
                        }
                        else
                        {
                            MessageBox.Show($"Invalid value for column '{columnName}'. Please check the data.");
                            return;
                        }
                    }
                    else
                    {
                        sql += $"{columnName} = N'{columnValue}', ";
                    }
                }

                sql = sql.TrimEnd(',', ' ');

                sql += $" WHERE {columnName} = '{value}'";

                exCuteNonQuery(sql);
            }
        }

        public void Xoa_Database(string _FileXML, string tenCot, string giaTri, string tenBang)
        {
            string duongDan = _FileXML;
            DataTable table = loadDataGridView(duongDan);
            int dong = -1;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (table.Rows[i][tenCot].ToString().Trim() == giaTri)
                {
                    dong = i;
                    break; 
                }
            }

            if (dong > -1)
            {
                string sql = "delete from " + tenBang + " where " + tenCot + " = '" + giaTri + "'";
                exCuteNonQuery(sql);
            }
        }

        public void CapNhapTungBang(string tenBang, string _FileXML)
        {
            string duongDan = _FileXML;
            DataTable table = loadDataGridView(duongDan);

            for (int i = 0; i < table.Rows.Count; i++)
            {
                string sql = "insert into " + tenBang + " values(";

                for (int j = 0; j < table.Columns.Count - 1; j++)
                {
                    sql += "N'" + table.Rows[i][j].ToString().Trim() + "',";
                }

                sql += "N'" + table.Rows[i][table.Columns.Count - 1].ToString().Trim() + "')";
                exCuteNonQuery(sql);
            }
        }

        public void TimKiemXSLT(string data, string tenFileXML, string tenfileXSLT)
        {
            XslCompiledTransform xslt = new XslCompiledTransform();

            xslt.Load(tenfileXSLT); 

            XsltArgumentList argList = new XsltArgumentList();
            argList.AddParam("Data", "", data);

            using (XmlWriter writer = XmlWriter.Create(tenFileXML.Replace(".xml", ".html"))) 
            {
                xslt.Transform(new XPathDocument(tenFileXML), argList, writer);
            }

            System.Diagnostics.Process.Start(tenFileXML.Replace(".xml", ".html"));
        }
    }
}
