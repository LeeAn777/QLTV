using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLTV.GUI.UC_HeThong
{
    public partial class ucTaiKhoan : UserControl
    {
        public ucTaiKhoan()
        {
            InitializeComponent();
        }

        private void ucTaiKhoan_Load(object sender, EventArgs e)
        {
            dgvTaiKhoan.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.0F, FontStyle.Bold);
            dgvTaiKhoan.AutoGenerateColumns = false;
        }
        private void txtMatKhau_TextChanged(object sender, System.EventArgs e)
        {
            if (txtMatKhau.PasswordChar == '\0' && !txtMatKhau.UseSystemPasswordChar)
            {
                txtMatKhau.UseSystemPasswordChar = true;
                txtMatKhau.PasswordChar = '●';
            }
            else
            {
                txtMatKhau.UseSystemPasswordChar = false;
                txtMatKhau.PasswordChar = '\0';
            }
        }

    }
}
