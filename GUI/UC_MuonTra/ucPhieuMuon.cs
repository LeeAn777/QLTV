using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLTV.GUI.UC_MuonTra
{
    public partial class ucPhieuMuon : UserControl
    {
        public ucPhieuMuon()
        {
            InitializeComponent();
        }

        private void ucPhieuMuon_Load(object sender, EventArgs e)
        {
            dgvPhieuMuon.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.0F, FontStyle.Bold);
            dgvPhieuMuon.AutoGenerateColumns = false;
        }
        private void guna2Button6_Click(object sender, EventArgs e)
        {
            if (dgvPhieuMuon.CurrentRow != null
            && dgvPhieuMuon.CurrentRow.Cells["colMaPhieuMuon"].Value != null
            && !string.IsNullOrWhiteSpace(dgvPhieuMuon.CurrentRow.Cells["colMaPhieuMuon"].Value.ToString()))
            {
                string maPhieu = dgvPhieuMuon.CurrentRow.Cells["colMaPhieuMuon"].Value.ToString();
                FormTrangChu trangChu = (FormTrangChu)this.ParentForm;
                if (trangChu != null)
                {
                    ucChiTietPhieuMuon ucChiTiet = new ucChiTietPhieuMuon(maPhieu);
                    trangChu.HienThiUserControl(ucChiTiet);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một phiếu mượn hợp lệ để xem chi tiết!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
