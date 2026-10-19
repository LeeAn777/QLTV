using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLTV.GUI.UC_TienPhat
{
    public partial class ucTienPhat : UserControl
    {
        public ucTienPhat()
        {
            InitializeComponent();
        }

        private void ucTienPhat_Load(object sender, EventArgs e)
        {
            dgvTienPhat.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.0F, FontStyle.Bold);
            dgvTienPhat.AutoGenerateColumns = false;
        }
    }
}
