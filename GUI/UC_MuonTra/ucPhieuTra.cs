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
    public partial class ucPhieuTra : UserControl
    {
        public ucPhieuTra()
        {
            InitializeComponent();
        }

        private void ucPhieuTra_Load(object sender, EventArgs e)
        {
            dgvPhieuTra.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.0F, FontStyle.Bold);
            dgvPhieuTra.AutoGenerateColumns = false;
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            if (dgvPhieuTra.CurrentRow != null
            && dgvPhieuTra.CurrentRow.Cells["colMaPhieuTra"].Value != null
            && !string.IsNullOrWhiteSpace(dgvPhieuTra.CurrentRow.Cells["colMaPhieuTra"].Value.ToString()))
            {
                string maPhieu = dgvPhieuTra.CurrentRow.Cells["colMaPhieuTra"].Value.ToString();
                FormTrangChu trangChu = (FormTrangChu)this.ParentForm;
                if (trangChu != null)
                {
                    ucChiTietPhieuTra ucChiTiet = new ucChiTietPhieuTra(maPhieu);
                    trangChu.HienThiUserControl(ucChiTiet);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một phiếu trả hợp lệ để xem chi tiết!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
