using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
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
        private readonly string connString = @"Data Source=.\SQLEXPRESS;Initial Catalog=QLThuVien;Integrated Security=True";
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
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    // TRUY VẤN: Lấy thông tin tài khoản kết hợp tên nhân viên từ bảng nhanvien
                    string sql = @"SELECT tk.MaNV, nv.HoTen, tk.Quyen, tk.TrangThai 
                                   FROM dbo.taikhoannhanvien tk 
                                   INNER JOIN dbo.nhanvien nv ON tk.MaNV = nv.MaNV 
                                   WHERE tk.TenDangNhap = @TaiKhoan AND tk.MatKhau = @MatKhau";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@TaiKhoan", taiKhoan);
                        cmd.Parameters.AddWithValue("@MatKhau", matKhau);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string trangThai = reader["TrangThai"]?.ToString();
                                if (trangThai == "Bị khóa")
                                {
                                    HienThiLoi("Tài khoản của bạn hiện đang bị khóa, không thể đăng nhập!");
                                    return;
                                }

                                // Đăng nhập thành công -> Lưu vào Session
                                Session.MaNV = reader["MaNV"]?.ToString();
                                Session.HoTen = reader["HoTen"]?.ToString();
                                Session.ChucVu = reader["Quyen"]?.ToString();

                                lblThongBaoLoi.Visible = false;
                                this.DialogResult = DialogResult.OK;
                                this.Close(); // Đóng Form Đăng nhập để chuyển sang Trang chủ
                            }
                            else
                            {
                                HienThiLoi("Tài khoản hoặc mật khẩu không chính xác!");
                                txtMatKhau.Clear();
                                txtMatKhau.Focus();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu: " + ex.Message, "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
