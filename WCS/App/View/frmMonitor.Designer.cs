namespace App.View
{
    partial class frmMonitor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMonitor));
            this.bsMain = new System.Windows.Forms.BindingSource(this.components);
            this.pnlMain = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.picCrane = new System.Windows.Forms.PictureBox();
            this.picCar = new System.Windows.Forms.PictureBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtRow1 = new System.Windows.Forms.TextBox();
            this.btnBack1 = new System.Windows.Forms.Button();
            this.btnStop1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtState1 = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.txtErrorNo1 = new System.Windows.Forms.TextBox();
            this.txtHeight1 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtForkStatus1 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtErrorDesc1 = new System.Windows.Forms.TextBox();
            this.txtCraneAction1 = new System.Windows.Forms.TextBox();
            this.txtColumn1 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtTaskNo1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.bsMain)).BeginInit();
            this.pnlMain.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCrane)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCar)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.panel1);
            this.pnlMain.Controls.Add(this.groupBox4);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(1284, 750);
            this.pnlMain.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.picCrane);
            this.panel1.Controls.Add(this.picCar);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1083, 626);
            this.panel1.TabIndex = 9;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pictureBox1.Image = global::App.Properties.Resources._12;
            this.pictureBox1.Location = new System.Drawing.Point(3, 9);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1070, 505);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // picCrane
            // 
            this.picCrane.BackColor = System.Drawing.SystemColors.Control;
            this.picCrane.Image = global::App.Properties.Resources.Crane12;
            this.picCrane.Location = new System.Drawing.Point(62, 38);
            this.picCrane.Name = "picCrane";
            this.picCrane.Size = new System.Drawing.Size(47, 23);
            this.picCrane.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picCrane.TabIndex = 6;
            this.picCrane.TabStop = false;
            // 
            // picCar
            // 
            this.picCar.BackColor = System.Drawing.SystemColors.Control;
            this.picCar.Image = ((System.Drawing.Image)(resources.GetObject("picCar.Image")));
            this.picCar.Location = new System.Drawing.Point(982, 16);
            this.picCar.Name = "picCar";
            this.picCar.Size = new System.Drawing.Size(72, 61);
            this.picCar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picCar.TabIndex = 8;
            this.picCar.TabStop = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.txtRow1);
            this.groupBox4.Controls.Add(this.btnBack1);
            this.groupBox4.Controls.Add(this.btnStop1);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.txtState1);
            this.groupBox4.Controls.Add(this.label20);
            this.groupBox4.Controls.Add(this.txtErrorNo1);
            this.groupBox4.Controls.Add(this.txtHeight1);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.txtForkStatus1);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.txtErrorDesc1);
            this.groupBox4.Controls.Add(this.txtCraneAction1);
            this.groupBox4.Controls.Add(this.txtColumn1);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.txtTaskNo1);
            this.groupBox4.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox4.ForeColor = System.Drawing.Color.Red;
            this.groupBox4.Location = new System.Drawing.Point(1088, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(187, 509);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "堆垛机";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label8.Location = new System.Drawing.Point(2, 110);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(67, 20);
            this.label8.TabIndex = 20;
            this.label8.Text = "当  前  排";
            // 
            // txtRow1
            // 
            this.txtRow1.Location = new System.Drawing.Point(70, 108);
            this.txtRow1.Name = "txtRow1";
            this.txtRow1.ReadOnly = true;
            this.txtRow1.Size = new System.Drawing.Size(126, 26);
            this.txtRow1.TabIndex = 19;
            // 
            // btnBack1
            // 
            this.btnBack1.BackColor = System.Drawing.Color.Lime;
            this.btnBack1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnBack1.Location = new System.Drawing.Point(18, 315);
            this.btnBack1.Name = "btnBack1";
            this.btnBack1.Size = new System.Drawing.Size(75, 30);
            this.btnBack1.TabIndex = 16;
            this.btnBack1.Text = "召回";
            this.btnBack1.UseVisualStyleBackColor = false;
            this.btnBack1.Click += new System.EventHandler(this.btnBack1_Click);
            // 
            // btnStop1
            // 
            this.btnStop1.BackColor = System.Drawing.Color.Red;
            this.btnStop1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnStop1.Location = new System.Drawing.Point(113, 314);
            this.btnStop1.Name = "btnStop1";
            this.btnStop1.Size = new System.Drawing.Size(75, 30);
            this.btnStop1.TabIndex = 9;
            this.btnStop1.Text = "急停";
            this.btnStop1.UseVisualStyleBackColor = false;
            this.btnStop1.Click += new System.EventHandler(this.btnStop1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(2, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "任务编号";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(2, 231);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 20);
            this.label4.TabIndex = 7;
            this.label4.Text = "错误代码";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(2, 202);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "货叉位置";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(2, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "工作方式";
            // 
            // txtState1
            // 
            this.txtState1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtState1.Location = new System.Drawing.Point(70, 50);
            this.txtState1.Name = "txtState1";
            this.txtState1.ReadOnly = true;
            this.txtState1.Size = new System.Drawing.Size(112, 26);
            this.txtState1.TabIndex = 10;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label20.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label20.Location = new System.Drawing.Point(4, 170);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(67, 20);
            this.label20.TabIndex = 18;
            this.label20.Text = "当  前  层";
            // 
            // txtErrorNo1
            // 
            this.txtErrorNo1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtErrorNo1.Location = new System.Drawing.Point(70, 228);
            this.txtErrorNo1.Name = "txtErrorNo1";
            this.txtErrorNo1.ReadOnly = true;
            this.txtErrorNo1.Size = new System.Drawing.Size(112, 26);
            this.txtErrorNo1.TabIndex = 3;
            // 
            // txtHeight1
            // 
            this.txtHeight1.Location = new System.Drawing.Point(70, 167);
            this.txtHeight1.Name = "txtHeight1";
            this.txtHeight1.ReadOnly = true;
            this.txtHeight1.Size = new System.Drawing.Size(126, 26);
            this.txtHeight1.TabIndex = 17;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(2, 53);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 20);
            this.label5.TabIndex = 11;
            this.label5.Text = "工作状态";
            // 
            // txtForkStatus1
            // 
            this.txtForkStatus1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtForkStatus1.Location = new System.Drawing.Point(70, 199);
            this.txtForkStatus1.Name = "txtForkStatus1";
            this.txtForkStatus1.ReadOnly = true;
            this.txtForkStatus1.Size = new System.Drawing.Size(112, 26);
            this.txtForkStatus1.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label7.Location = new System.Drawing.Point(2, 139);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 20);
            this.label7.TabIndex = 15;
            this.label7.Text = "当  前  列";
            // 
            // txtErrorDesc1
            // 
            this.txtErrorDesc1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtErrorDesc1.Location = new System.Drawing.Point(70, 260);
            this.txtErrorDesc1.Name = "txtErrorDesc1";
            this.txtErrorDesc1.ReadOnly = true;
            this.txtErrorDesc1.Size = new System.Drawing.Size(112, 26);
            this.txtErrorDesc1.TabIndex = 12;
            // 
            // txtCraneAction1
            // 
            this.txtCraneAction1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCraneAction1.Location = new System.Drawing.Point(70, 78);
            this.txtCraneAction1.Name = "txtCraneAction1";
            this.txtCraneAction1.ReadOnly = true;
            this.txtCraneAction1.Size = new System.Drawing.Size(112, 26);
            this.txtCraneAction1.TabIndex = 1;
            // 
            // txtColumn1
            // 
            this.txtColumn1.Location = new System.Drawing.Point(70, 137);
            this.txtColumn1.Name = "txtColumn1";
            this.txtColumn1.ReadOnly = true;
            this.txtColumn1.Size = new System.Drawing.Size(126, 26);
            this.txtColumn1.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label6.Location = new System.Drawing.Point(2, 259);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 20);
            this.label6.TabIndex = 13;
            this.label6.Text = "错误描述";
            // 
            // txtTaskNo1
            // 
            this.txtTaskNo1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTaskNo1.Location = new System.Drawing.Point(70, 19);
            this.txtTaskNo1.Name = "txtTaskNo1";
            this.txtTaskNo1.ReadOnly = true;
            this.txtTaskNo1.Size = new System.Drawing.Size(112, 26);
            this.txtTaskNo1.TabIndex = 0;
            // 
            // frmMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1284, 750);
            this.ControlBox = false;
            this.Controls.Add(this.pnlMain);
            this.Name = "frmMonitor";
            this.Text = "监控";
            this.Load += new System.EventHandler(this.frmMonitor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bsMain)).EndInit();
            this.pnlMain.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCrane)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCar)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox txtErrorNo1;
        private System.Windows.Forms.TextBox txtForkStatus1;
        private System.Windows.Forms.TextBox txtCraneAction1;
        private System.Windows.Forms.TextBox txtTaskNo1;
        private System.Windows.Forms.Button btnStop1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtErrorDesc1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtState1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtColumn1;
        private System.Windows.Forms.PictureBox picCrane;
        private System.Windows.Forms.BindingSource bsMain;
        private System.Windows.Forms.Button btnBack1;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txtHeight1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.PictureBox picCar;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtRow1;
        private System.Windows.Forms.Panel panel1;
    }
}