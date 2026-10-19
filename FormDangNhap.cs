using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLTV
{
    public partial class FormDangNhap : Form
    {
        public FormDangNhap()
        {
            InitializeComponent();
        }
        private void FormDangNhap_Load(object sender, EventArgs e)
        {
            this.AcceptButton = btnDangNhap;
            txtTaiKhoan.Focus();
        }
        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string taiKhoan = txtTaiKhoan.Text.Trim();
            string matKhau = txtMatKhau.Text.Trim();

            if (string.IsNullOrEmpty(taiKhoan) && string.IsNullOrEmpty(matKhau))
            {
                HienThiLoi("Vui lòng nhập tài khoản và mật khẩu!");
                txtTaiKhoan.Focus(); 
                return;
            }
            if (string.IsNullOrEmpty(taiKhoan))
            {
                HienThiLoi("Vui lòng nhập tài khoản!");
                txtTaiKhoan.Focus();
                return;
            }
            if (string.IsNullOrEmpty(matKhau))
            {
                HienThiLoi("Vui lòng nhập mật khẩu!");
                txtMatKhau.Focus(); 
                return;
            }
            if (taiKhoan == "admin" && matKhau == "1")
            {
                Session.MaNV = "ADMIN";
                Session.HoTen = "Nguyễn Văn A";
                Session.ChucVu = "Quản Trị Viên";
                lblThongBaoLoi.Visible = false;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else if (taiKhoan == "nv01" && matKhau == "2")
            {
                Session.MaNV = "NV01";
                Session.HoTen = "Trần Thị B";
                Session.ChucVu = "Nhân viên";
                lblThongBaoLoi.Visible = false;
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                HienThiLoi("Tài khoản hoặc mật khẩu không chính xác!");
                txtMatKhau.Clear();
                txtMatKhau.Focus();
            }
        }
        private void HienThiLoi(string message)
        {
            lblThongBaoLoi.Text = message;
            lblThongBaoLoi.Visible = true;
        }

        private void txtMatKhau_IconRightClick(object sender, EventArgs e)
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

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát khỏi ứng dụng không?","Xác nhận thoát",MessageBoxButtons.YesNo,MessageBoxIcon.Question
            );
            if (result == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.Cancel;
                Application.Exit();
            }
        }
    }
}
