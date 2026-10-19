using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLTV.GUI.UC_Nhanvien
{
    public partial class ucNhanvien : UserControl
    {
        public ucNhanvien()
        {
            InitializeComponent();
        }

        private void ucNhanvien_Load(object sender, EventArgs e)
        {
            dgvNhanVien.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.0F, FontStyle.Bold);
            dgvNhanVien.AutoGenerateColumns = false;
        }
    }
}
