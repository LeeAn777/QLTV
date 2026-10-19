using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLTV
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            while (true)
            {
                using (FormDangNhap frmLogin = new FormDangNhap())
                {
                    if (frmLogin.ShowDialog() != DialogResult.OK)
                    {
                        break;
                    }
                }
                using (FormTrangChu frmMain = new FormTrangChu())
                {
                    Application.Run(frmMain);
                }
            }
        }
        
    }
}
