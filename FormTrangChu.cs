using Guna.UI2.WinForms;
using QLTV.GUI.UC_DocGia;
using QLTV.GUI.UC_Sach;
using QLTV.GUI.UC_TChu;
using QLTV.GUI.UC_Nhanvien;
using QLTV.GUI.UC_TacGia;
using QLTV.GUI.UC_MuonTra;
using QLTV.GUI.UC_XuHuong;
using QLTV.GUI.UC_HeThong;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Collections.Specialized.BitVector32;

namespace QLTV
{

    public partial class FormTrangChu : Form
    {  
        public bool IsLogout { get; private set; } = false;
        public FormTrangChu()
        {
            InitializeComponent();
        }

        private void FormTrangChu_Load(object sender, EventArgs e)
        {
            lblUser.Text = "Xin chào, " + Session.HoTen;
            lblChucVu.Text = Session.ChucVu;
            if (Session.ChucVu == "Nhân viên" || Session.ChucVu == "NhanVien") 
            {
                btnQLTKNV.Visible = false; // Ẩn nút Quản lý tài khoản
            }
            else
            {
                btnQLTKNV.Visible = true;  // Hiện đầy đủ nếu là Admin
            }
            this.Location = new Point(40, 20);
            this.Size = new Size(1200, 720);

            HienThiUserControl(new ucTChu());
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
        }
      
        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            IsLogout = true;
            this.Close();
        }
        private void Thoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void FormTrangChu_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsLogout)
            {
                return;
            }
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát khỏi ứng dụng không?","Xác nhận thoát",MessageBoxButtons.YesNo,MessageBoxIcon.Question
            );

            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                Application.ExitThread();
            }
        }
        public void HienThiUserControl(UserControl uc)
        {
            pnlMainContent.SuspendLayout();
            pnlMainContent.Controls.Clear();

            uc.Dock = DockStyle.None;

            int offset = 15;

            uc.Location = new Point(offset, offset);

            uc.Size = new Size(pnlMainContent.Width - (offset * 2), pnlMainContent.Height - (offset * 2));


            uc.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            pnlMainContent.Controls.Add(uc);
            uc.BringToFront();

            pnlMainContent.ResumeLayout(true);
        }
        private void btnQLSach_Click(object sender, EventArgs e)
        {
            ucTTSach uc = new ucTTSach();
            HienThiUserControl(uc);
        }
        private void btnTheLoaiSach_Click(object sender, EventArgs e)
        {
            ucTheLoai uc = new ucTheLoai();
            HienThiUserControl(uc);
        }

        private void btnNXB_Click(object sender, EventArgs e)
        {
            ucNXB uc = new ucNXB();
            HienThiUserControl(uc);
        }

        private void btnThongTinDG_Click(object sender, EventArgs e)
        {
            ucTTDG uc = new ucTTDG();
            HienThiUserControl(uc);
        }

        private void btnHangDG_Click(object sender, EventArgs e)
        {
            ucHangDG uc = new ucHangDG();
            HienThiUserControl(uc);
        }

        private void btnTheDG_Click(object sender, EventArgs e)
        {
            ucTheDG uc = new ucTheDG();
            HienThiUserControl(uc);
        }

        private void btnTTNV_Click(object sender, EventArgs e)
        {
            ucNhanvien uc = new ucNhanvien();
            HienThiUserControl(uc);
        }
        private void btnTTTG_Click(object sender, EventArgs e)
        {
            ucTacGia uc = new ucTacGia();
            HienThiUserControl(uc);
        }
        private void btnSachTG_Click(object sender, EventArgs e)
        {
            ucSachTacGia uc = new ucSachTacGia();
            HienThiUserControl(uc);
        }

        private void btnPhieuMuonSach_Click(object sender, EventArgs e)
        {
            ucPhieuMuon uc = new ucPhieuMuon();
            HienThiUserControl(uc);
        }

        private void btnPhieuTraSach_Click(object sender, EventArgs e)
        {
            ucPhieuTra uc = new ucPhieuTra();
            HienThiUserControl(uc);
        }
        private void btnXuHuong_Click(object sender, EventArgs e)
        {
            ucXuHuong uc = new ucXuHuong();
            HienThiUserControl(uc);
        }
        private void btnQLTKNV_Click(object sender, EventArgs e)
        {
            ucTaiKhoan uc = new ucTaiKhoan();
            HienThiUserControl(uc);
        }

        private void btnTrangChu_Click(object sender, EventArgs e)
        {
            HienThiUserControl(new ucTChu());
        }

        
    }
}