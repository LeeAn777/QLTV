using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLTV.GUI.UC_Sach
{
    public partial class ucTTSach : UserControl
    {
        public ucTTSach()
        {
            InitializeComponent();
        }

        private void ucTTSach_Load(object sender, EventArgs e)
        {
            dgvSach.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.0F, FontStyle.Bold);
            dgvSach.AutoGenerateColumns = false;
        }
    }
}
