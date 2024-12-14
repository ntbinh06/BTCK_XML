namespace BTCK_XML.Resources
{
    partial class TimKiemNV
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnThoat = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnTimKiemTen = new System.Windows.Forms.Button();
            this.txtSearchTen = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnTimKiemMa = new System.Windows.Forms.Button();
            this.txtSearchMa = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnThoat
            // 
            this.btnThoat.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThoat.Image = global::BTCK_XML.Properties.Resources.icons8_exit_25;
            this.btnThoat.Location = new System.Drawing.Point(755, 660);
            this.btnThoat.Name = "btnThoat";
            this.btnThoat.Size = new System.Drawing.Size(123, 53);
            this.btnThoat.TabIndex = 11;
            this.btnThoat.Text = "Thoát";
            this.btnThoat.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnThoat.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(85, 425);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(793, 220);
            this.dataGridView1.TabIndex = 10;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.groupBox2.Controls.Add(this.btnTimKiemTen);
            this.groupBox2.Controls.Add(this.txtSearchTen);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.Crimson;
            this.groupBox2.Location = new System.Drawing.Point(89, 270);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(790, 120);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Tìm kiếm theo tên nhân viên";
            // 
            // btnTimKiemTen
            // 
            this.btnTimKiemTen.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTimKiemTen.Image = global::BTCK_XML.Properties.Resources.icons8_search_30;
            this.btnTimKiemTen.Location = new System.Drawing.Point(606, 34);
            this.btnTimKiemTen.Name = "btnTimKiemTen";
            this.btnTimKiemTen.Size = new System.Drawing.Size(149, 59);
            this.btnTimKiemTen.TabIndex = 3;
            this.btnTimKiemTen.Text = "Tìm kiếm";
            this.btnTimKiemTen.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnTimKiemTen.UseVisualStyleBackColor = true;
            this.btnTimKiemTen.Click += new System.EventHandler(this.btnTimKiemTen_Click);
            // 
            // txtSearchTen
            // 
            this.txtSearchTen.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearchTen.Location = new System.Drawing.Point(293, 48);
            this.txtSearchTen.Name = "txtSearchTen";
            this.txtSearchTen.Size = new System.Drawing.Size(254, 30);
            this.txtSearchTen.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Crimson;
            this.label3.Location = new System.Drawing.Point(74, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(192, 25);
            this.label3.TabIndex = 1;
            this.label3.Text = "Nhập tên nhân viên :";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.groupBox1.Controls.Add(this.btnTimKiemMa);
            this.groupBox1.Controls.Add(this.txtSearchMa);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.Crimson;
            this.groupBox1.Location = new System.Drawing.Point(89, 126);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(790, 120);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tìm kiếm theo mã nhân viên";
            // 
            // btnTimKiemMa
            // 
            this.btnTimKiemMa.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTimKiemMa.Image = global::BTCK_XML.Properties.Resources.icons8_search_30;
            this.btnTimKiemMa.Location = new System.Drawing.Point(606, 34);
            this.btnTimKiemMa.Name = "btnTimKiemMa";
            this.btnTimKiemMa.Size = new System.Drawing.Size(149, 59);
            this.btnTimKiemMa.TabIndex = 3;
            this.btnTimKiemMa.Text = "Tìm kiếm";
            this.btnTimKiemMa.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnTimKiemMa.UseVisualStyleBackColor = true;
            this.btnTimKiemMa.Click += new System.EventHandler(this.btnTimKiemMa_Click);
            // 
            // txtSearchMa
            // 
            this.txtSearchMa.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearchMa.Location = new System.Drawing.Point(293, 48);
            this.txtSearchMa.Name = "txtSearchMa";
            this.txtSearchMa.Size = new System.Drawing.Size(254, 30);
            this.txtSearchMa.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Crimson;
            this.label2.Location = new System.Drawing.Point(74, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(195, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "Nhập mã Nhân viên :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Crimson;
            this.label1.Location = new System.Drawing.Point(273, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(366, 38);
            this.label1.TabIndex = 7;
            this.label1.Text = "TÌM KIẾM NHÂN VIÊN";
            // 
            // TimKiemNV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(945, 736);
            this.Controls.Add(this.btnThoat);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Name = "TimKiemNV";
            this.Text = "TimKiemNV";
            this.Load += new System.EventHandler(this.TimKiemNV_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnThoat;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnTimKiemTen;
        private System.Windows.Forms.TextBox txtSearchTen;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnTimKiemMa;
        private System.Windows.Forms.TextBox txtSearchMa;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}