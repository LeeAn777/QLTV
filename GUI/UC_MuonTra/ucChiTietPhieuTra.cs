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
    public partial class ucChiTietPhieuTra : UserControl
    {
        private string _maPhieuTra;
        public ucChiTietPhieuTra(string maPhieu)
        {
            InitializeComponent();
            this._maPhieuTra = maPhieu;
        }
        private void ucChiTietPhieuTra_Load(object sender, EventArgs e)
        {
            dgvChiTietPhieuTra.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.0F, FontStyle.Bold);
            dgvChiTietPhieuTra.AutoGenerateColumns = false;
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            FormTrangChu trangChu = (FormTrangChu)this.ParentForm;
            if (trangChu != null)
            {
                ucPhieuTra ucPT = new ucPhieuTra();
                trangChu.HienThiUserControl(ucPT);
            }
        }
    }
}
