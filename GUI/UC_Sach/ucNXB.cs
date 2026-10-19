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
    public partial class ucNXB: UserControl
    {
        public ucNXB()
        {
            InitializeComponent();
        }

        private void ucNXB_Load(object sender, EventArgs e)
        {
            dgvNXB.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.0F, FontStyle.Bold);
            dgvNXB.AutoGenerateColumns = false;
        }
    }
}
