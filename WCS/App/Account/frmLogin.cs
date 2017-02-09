using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace App.Account
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }
        private void frmLogin_Load(object sender, EventArgs e)
        {
            this.Show();
            if (txtUserName.Text != "")
                txtPassWord.Focus();
            else
                txtUserName.Focus();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.txtUserName.Text.Trim().Length != 0)
            {
                BLL.UserBll userBll = new BLL.UserBll();

                DataTable dtUserList = userBll.GetUserInfo(txtUserName.Text.Trim());
                if (dtUserList != null && dtUserList.Rows.Count > 0)
                {
                    if (dtUserList.Rows[0]["UserPassword"].ToString().Trim() == txtPassWord.Text.Trim())
                    {
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        MessageBox.Show("对不起,您输入的密码有误!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("对不起,您输入的用户名不存在!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("请输入用户名!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }


        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }        
    }
}
