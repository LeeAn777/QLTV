using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLTV.GUI.UC_DocGia
{
    public partial class ucTTDG : UserControl
    {
        public ucTTDG()
        {
            InitializeComponent();
        }

        private void ucThongTinDocGia_Load(object sender, EventArgs e)
        {
            dgvDocGia.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.0F, FontStyle.Bold);
            dgvDocGia.AutoGenerateColumns = false;
        }
    }
}
