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
    public partial class    ucTheDG : UserControl
    {
        public ucTheDG()
        {
            InitializeComponent();
        }

        private void ucTheDG_Load(object sender, EventArgs e)
        {
            dgvTheDG.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.0F, FontStyle.Bold);
            dgvTheDG.AutoGenerateColumns = false;
        }
    }
}
