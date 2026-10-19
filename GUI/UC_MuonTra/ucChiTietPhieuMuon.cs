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
    public partial class ucChiTietPhieuMuon : UserControl
    {
        private string _maPhieuMuon;
        public ucChiTietPhieuMuon()
        {
            InitializeComponent();
        }
        public ucChiTietPhieuMuon(string maPhieu)
        {
            InitializeComponent();
            this._maPhieuMuon = maPhieu;
        }
        private void ucChiTietPhieuMuon_Load(object sender, EventArgs e)
        {
            dgvChiTietPhieuMuon.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.0F, FontStyle.Bold);
            dgvChiTietPhieuMuon.AutoGenerateColumns = false;
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            FormTrangChu trangChu = (FormTrangChu)this.ParentForm;
            if (trangChu != null)
            {
                ucPhieuMuon ucPM = new ucPhieuMuon();
                trangChu.HienThiUserControl(ucPM);
            }
        }
    }
}
