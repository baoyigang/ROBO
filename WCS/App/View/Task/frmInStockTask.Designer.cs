namespace App.View.Task
{
    partial class frmInStockTask
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
            this.btnClose = new System.Windows.Forms.Button();
            this.btnRequest = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTaskNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.txtCellCode = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cbHeight = new System.Windows.Forms.ComboBox();
            this.cbColumn = new System.Windows.Forms.ComboBox();
            this.cbRow = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbDepth = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtBarcode = new System.Windows.Forms.TextBox();
            this.cmbAreaCode = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cbStationNo = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnClose.Location = new System.Drawing.Point(398, 187);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 38);
            this.btnClose.TabIndex = 50;
            this.btnClose.Text = "关  闭";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnRequest
            // 
            this.btnRequest.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnRequest.Location = new System.Drawing.Point(223, 187);
            this.btnRequest.Name = "btnRequest";
            this.btnRequest.Size = new System.Drawing.Size(101, 38);
            this.btnRequest.TabIndex = 49;
            this.btnRequest.Text = "入库执行";
            this.btnRequest.UseVisualStyleBackColor = true;
            this.btnRequest.Click += new System.EventHandler(this.btnRequest_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(3, 123);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 21);
            this.label2.TabIndex = 41;
            this.label2.Text = "货位代码";
            // 
            // txtTaskNo
            // 
            this.txtTaskNo.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtTaskNo.Location = new System.Drawing.Point(422, 124);
            this.txtTaskNo.Name = "txtTaskNo";
            this.txtTaskNo.ReadOnly = true;
            this.txtTaskNo.Size = new System.Drawing.Size(259, 29);
            this.txtTaskNo.TabIndex = 40;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(347, 128);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 21);
            this.label1.TabIndex = 39;
            this.label1.Text = "任  务 号";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radioButton1.Location = new System.Drawing.Point(17, 19);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(92, 25);
            this.radioButton1.TabIndex = 55;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "自动获取";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radioButton2.Location = new System.Drawing.Point(116, 19);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(92, 25);
            this.radioButton2.TabIndex = 56;
            this.radioButton2.Text = "手动分配";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // txtCellCode
            // 
            this.txtCellCode.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtCellCode.Location = new System.Drawing.Point(83, 124);
            this.txtCellCode.MaxLength = 10;
            this.txtCellCode.Name = "txtCellCode";
            this.txtCellCode.ReadOnly = true;
            this.txtCellCode.Size = new System.Drawing.Size(196, 29);
            this.txtCellCode.TabIndex = 42;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(438, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(26, 21);
            this.label6.TabIndex = 62;
            this.label6.Text = "层";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(350, 21);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(26, 21);
            this.label7.TabIndex = 61;
            this.label7.Text = "列";
            // 
            // cbHeight
            // 
            this.cbHeight.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbHeight.Enabled = false;
            this.cbHeight.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbHeight.FormattingEnabled = true;
            this.cbHeight.Location = new System.Drawing.Point(470, 17);
            this.cbHeight.Name = "cbHeight";
            this.cbHeight.Size = new System.Drawing.Size(40, 29);
            this.cbHeight.TabIndex = 60;
            // 
            // cbColumn
            // 
            this.cbColumn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbColumn.Enabled = false;
            this.cbColumn.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbColumn.FormattingEnabled = true;
            this.cbColumn.Location = new System.Drawing.Point(386, 17);
            this.cbColumn.Name = "cbColumn";
            this.cbColumn.Size = new System.Drawing.Size(48, 29);
            this.cbColumn.TabIndex = 59;
            this.cbColumn.SelectedIndexChanged += new System.EventHandler(this.cbColumn_SelectedIndexChanged);
            // 
            // cbRow
            // 
            this.cbRow.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRow.Enabled = false;
            this.cbRow.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbRow.FormattingEnabled = true;
            this.cbRow.Items.AddRange(new object[] {
            "001",
            "002",
            "003",
            "004",
            "005",
            "006",
            "007",
            "008",
            "009",
            "010",
            "011",
            "012"});
            this.cbRow.Location = new System.Drawing.Point(262, 17);
            this.cbRow.Name = "cbRow";
            this.cbRow.Size = new System.Drawing.Size(85, 29);
            this.cbRow.TabIndex = 58;
            this.cbRow.SelectedIndexChanged += new System.EventHandler(this.cbRow_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(214, 21);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(42, 21);
            this.label8.TabIndex = 57;
            this.label8.Text = "货架";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.Location = new System.Drawing.Point(288, 15);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(81, 21);
            this.label12.TabIndex = 71;
            this.label12.Text = "盘/箱条码";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbDepth);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.cbRow);
            this.groupBox1.Controls.Add(this.cbColumn);
            this.groupBox1.Controls.Add(this.cbHeight);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Location = new System.Drawing.Point(83, 59);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(598, 56);
            this.groupBox1.TabIndex = 72;
            this.groupBox1.TabStop = false;
            // 
            // cmbDepth
            // 
            this.cmbDepth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDepth.Enabled = false;
            this.cmbDepth.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmbDepth.FormattingEnabled = true;
            this.cmbDepth.Location = new System.Drawing.Point(552, 17);
            this.cmbDepth.Name = "cmbDepth";
            this.cmbDepth.Size = new System.Drawing.Size(40, 29);
            this.cmbDepth.TabIndex = 63;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(520, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 21);
            this.label3.TabIndex = 64;
            this.label3.Text = "深";
            // 
            // txtBarcode
            // 
            this.txtBarcode.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtBarcode.Location = new System.Drawing.Point(375, 11);
            this.txtBarcode.Name = "txtBarcode";
            this.txtBarcode.Size = new System.Drawing.Size(138, 29);
            this.txtBarcode.TabIndex = 73;
            this.txtBarcode.TextChanged += new System.EventHandler(this.txtBarcode_TextChanged);
            this.txtBarcode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBarcode_KeyDown);
            // 
            // cmbAreaCode
            // 
            this.cmbAreaCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAreaCode.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmbAreaCode.FormattingEnabled = true;
            this.cmbAreaCode.Location = new System.Drawing.Point(83, 11);
            this.cmbAreaCode.Name = "cmbAreaCode";
            this.cmbAreaCode.Size = new System.Drawing.Size(196, 29);
            this.cmbAreaCode.TabIndex = 75;
            this.cmbAreaCode.SelectedIndexChanged += new System.EventHandler(this.cmbAreaCode_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label13.Location = new System.Drawing.Point(3, 15);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(74, 21);
            this.label13.TabIndex = 74;
            this.label13.Text = "库区编号";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(3, 72);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(74, 21);
            this.label9.TabIndex = 78;
            this.label9.Text = "货位分配";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(519, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 21);
            this.label4.TabIndex = 79;
            this.label4.Text = "入库站台";
            // 
            // cbStationNo
            // 
            this.cbStationNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStationNo.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbStationNo.FormattingEnabled = true;
            this.cbStationNo.Location = new System.Drawing.Point(608, 11);
            this.cbStationNo.Name = "cbStationNo";
            this.cbStationNo.Size = new System.Drawing.Size(68, 29);
            this.cbStationNo.TabIndex = 65;
            // 
            // frmInStockTask
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(733, 258);
            this.Controls.Add(this.cbStationNo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.cmbAreaCode);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.txtBarcode);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnRequest);
            this.Controls.Add(this.txtCellCode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtTaskNo);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmInStockTask";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "扫码入库";
            this.Activated += new System.EventHandler(this.frmInStockTask_Activated);
            this.Load += new System.EventHandler(this.frmInStockTask_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnRequest;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTaskNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.TextBox txtCellCode;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cbHeight;
        private System.Windows.Forms.ComboBox cbColumn;
        private System.Windows.Forms.ComboBox cbRow;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtBarcode;
        private System.Windows.Forms.ComboBox cmbAreaCode;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cmbDepth;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbStationNo;
    }
}