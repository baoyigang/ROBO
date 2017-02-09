namespace App.View.Dispatcher
{
    partial class frmScan
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btndelDetail = new System.Windows.Forms.Button();
            this.btnAddDetail = new System.Windows.Forms.Button();
            this.btnGetBack = new System.Windows.Forms.Button();
            this.dgView = new System.Windows.Forms.DataGridView();
            this.bsMain = new System.Windows.Forms.BindingSource(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtCellCode = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtBillID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTaskNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPalletCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.colCheck = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colRowID = new DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn();
            this.colProductCode = new DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn();
            this.colProductName = new DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn();
            this.colModelNo = new DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn();
            this.colQty = new DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn();
            this.colRealQty = new DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn();
            this.colInStockTime = new DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsMain)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btndelDetail);
            this.panel1.Controls.Add(this.btnAddDetail);
            this.panel1.Controls.Add(this.btnGetBack);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.panel1.Location = new System.Drawing.Point(0, 371);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(814, 44);
            this.panel1.TabIndex = 1;
            // 
            // btndelDetail
            // 
            this.btndelDetail.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btndelDetail.Location = new System.Drawing.Point(98, 6);
            this.btndelDetail.Name = "btndelDetail";
            this.btndelDetail.Size = new System.Drawing.Size(81, 30);
            this.btndelDetail.TabIndex = 86;
            this.btndelDetail.Text = "删除明细";
            this.btndelDetail.Click += new System.EventHandler(this.btndelDetail_Click);
            // 
            // btnAddDetail
            // 
            this.btnAddDetail.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnAddDetail.Location = new System.Drawing.Point(3, 6);
            this.btnAddDetail.Name = "btnAddDetail";
            this.btnAddDetail.Size = new System.Drawing.Size(80, 30);
            this.btnAddDetail.TabIndex = 85;
            this.btnAddDetail.Text = "新增明细";
            this.btnAddDetail.Click += new System.EventHandler(this.btnAddDetail_Click);
            // 
            // btnGetBack
            // 
            this.btnGetBack.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnGetBack.Location = new System.Drawing.Point(712, 6);
            this.btnGetBack.Name = "btnGetBack";
            this.btnGetBack.Size = new System.Drawing.Size(61, 30);
            this.btnGetBack.TabIndex = 83;
            this.btnGetBack.Text = "确认";
            this.btnGetBack.Click += new System.EventHandler(this.btnGetBack_Click);
            // 
            // dgView
            // 
            this.dgView.AllowUserToAddRows = false;
            this.dgView.AllowUserToDeleteRows = false;
            this.dgView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.dgView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgView.AutoGenerateColumns = false;
            this.dgView.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgView.ColumnHeadersHeight = 30;
            this.dgView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCheck,
            this.colRowID,
            this.colProductCode,
            this.colProductName,
            this.colModelNo,
            this.colQty,
            this.colRealQty,
            this.colInStockTime});
            this.dgView.DataSource = this.bsMain;
            this.dgView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgView.Location = new System.Drawing.Point(3, 3);
            this.dgView.Name = "dgView";
            this.dgView.RowHeadersVisible = false;
            this.dgView.RowTemplate.Height = 30;
            this.dgView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgView.Size = new System.Drawing.Size(808, 323);
            this.dgView.TabIndex = 101;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.txtCellCode);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.txtBillID);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.txtTaskNo);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.txtPalletCode);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(814, 42);
            this.panel2.TabIndex = 102;
            // 
            // txtCellCode
            // 
            this.txtCellCode.Location = new System.Drawing.Point(649, 8);
            this.txtCellCode.Name = "txtCellCode";
            this.txtCellCode.ReadOnly = true;
            this.txtCellCode.Size = new System.Drawing.Size(93, 26);
            this.txtCellCode.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(608, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 20);
            this.label4.TabIndex = 6;
            this.label4.Text = "货位";
            // 
            // txtBillID
            // 
            this.txtBillID.Location = new System.Drawing.Point(464, 8);
            this.txtBillID.Name = "txtBillID";
            this.txtBillID.ReadOnly = true;
            this.txtBillID.Size = new System.Drawing.Size(139, 26);
            this.txtBillID.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(397, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "盘点单据";
            // 
            // txtTaskNo
            // 
            this.txtTaskNo.Location = new System.Drawing.Point(244, 8);
            this.txtTaskNo.Name = "txtTaskNo";
            this.txtTaskNo.ReadOnly = true;
            this.txtTaskNo.Size = new System.Drawing.Size(143, 26);
            this.txtTaskNo.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(187, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "任务号";
            // 
            // txtPalletCode
            // 
            this.txtPalletCode.Location = new System.Drawing.Point(74, 8);
            this.txtPalletCode.Name = "txtPalletCode";
            this.txtPalletCode.Size = new System.Drawing.Size(107, 26);
            this.txtPalletCode.TabIndex = 1;
            this.txtPalletCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPalletCode_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "托盘编号";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.dgView);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.panel3.Location = new System.Drawing.Point(0, 42);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(3);
            this.panel3.Size = new System.Drawing.Size(814, 329);
            this.panel3.TabIndex = 103;
            // 
            // colCheck
            // 
            this.colCheck.DataPropertyName = "chk";
            this.colCheck.Frozen = true;
            this.colCheck.HeaderText = "选取";
            this.colCheck.Name = "colCheck";
            this.colCheck.Width = 50;
            // 
            // colRowID
            // 
            this.colRowID.DataPropertyName = "RowID";
            this.colRowID.FilteringEnabled = false;
            this.colRowID.Frozen = true;
            this.colRowID.HeaderText = "序号";
            this.colRowID.Name = "colRowID";
            this.colRowID.ReadOnly = true;
            this.colRowID.Width = 60;
            // 
            // colProductCode
            // 
            this.colProductCode.DataPropertyName = "ProductCode";
            this.colProductCode.FilteringEnabled = false;
            this.colProductCode.Frozen = true;
            this.colProductCode.HeaderText = "产品编码";
            this.colProductCode.Name = "colProductCode";
            this.colProductCode.ReadOnly = true;
            // 
            // colProductName
            // 
            this.colProductName.DataPropertyName = "ProductName";
            this.colProductName.FilteringEnabled = false;
            this.colProductName.HeaderText = "产品名称";
            this.colProductName.Name = "colProductName";
            this.colProductName.ReadOnly = true;
            this.colProductName.Width = 150;
            // 
            // colModelNo
            // 
            this.colModelNo.DataPropertyName = "ModelNo";
            this.colModelNo.FilteringEnabled = false;
            this.colModelNo.HeaderText = "规格";
            this.colModelNo.Name = "colModelNo";
            this.colModelNo.ReadOnly = true;
            this.colModelNo.Width = 120;
            // 
            // colQty
            // 
            this.colQty.DataPropertyName = "Quantity";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colQty.DefaultCellStyle = dataGridViewCellStyle3;
            this.colQty.FilteringEnabled = false;
            this.colQty.HeaderText = "现有数量";
            this.colQty.Name = "colQty";
            this.colQty.ReadOnly = true;
            this.colQty.Width = 80;
            // 
            // colRealQty
            // 
            this.colRealQty.DataPropertyName = "RealQty";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colRealQty.DefaultCellStyle = dataGridViewCellStyle4;
            this.colRealQty.FilteringEnabled = false;
            this.colRealQty.HeaderText = "实际数量";
            this.colRealQty.Name = "colRealQty";
            this.colRealQty.Width = 80;
            // 
            // colInStockTime
            // 
            this.colInStockTime.DataPropertyName = "InDate";
            this.colInStockTime.FilteringEnabled = false;
            this.colInStockTime.HeaderText = "入库时间";
            this.colInStockTime.Name = "colInStockTime";
            this.colInStockTime.ReadOnly = true;
            this.colInStockTime.Width = 150;
            // 
            // frmScan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(814, 415);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmScan";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "盘点";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmScan_FormClosing);
            this.Load += new System.EventHandler(this.frmScan_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsMain)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnGetBack;
        private System.Windows.Forms.Button btndelDetail;
        private System.Windows.Forms.Button btnAddDetail;
        private System.Windows.Forms.DataGridView dgView;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txtBillID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTaskNo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPalletCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCellCode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.BindingSource bsMain;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colCheck;
        private DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn colRowID;
        private DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn colProductCode;
        private DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn colProductName;
        private DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn colModelNo;
        private DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn colQty;
        private DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn colRealQty;
        private DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn colInStockTime;
    }
}