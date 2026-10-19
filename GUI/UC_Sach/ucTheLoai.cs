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
    public partial class ucTheLoai : UserControl
    {
        public ucTheLoai()
        {
            InitializeComponent();
        }

        private void ucTheLoai_Load(object sender, EventArgs e)
        {
            dgvTheLoai.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.0F, FontStyle.Bold);
            dgvTheLoai.AutoGenerateColumns = false;
        }
    }
}
