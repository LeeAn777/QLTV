using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLTV.GUI.UC_TacGia
{
    public partial class ucSachTacGia : UserControl
    {
        public ucSachTacGia()
        {
            InitializeComponent();
        }

        private void ucSachTacGia_Load(object sender, EventArgs e)
        {
            dgvSachTacGia.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.0F, FontStyle.Bold);
            dgvSachTacGia.AutoGenerateColumns = false;
        }
    }
}
