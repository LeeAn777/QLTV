using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLTV.GUI.UC_TChu
{
    public partial class ucTChu : UserControl
    {
        private readonly string connString = @"Data Source=.\SQLEXPRESS;Initial Catalog=QLThuVien;Integrated Security=True";
        public ucTChu()
        {
            InitializeComponent();
        }
        private void ucTChu_Load(object sender, EventArgs e)
        {
            LoadThongKeTongQuan();
        }
        private void LoadThongKeTongQuan()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    // 1. Đếm tổng số sách trong bảng sach (hoặc tính tổng số lượng tồn kho tùy bạn muốn)
                    // Ở đây dùng COUNT(*) để đếm tổng số đầu sách, hoặc SUM(SoLuong) nếu muốn tổng số lượng quyển sách
                    string sqlTongSach = "SELECT COUNT(*) FROM dbo.sach";
                    using (SqlCommand cmd = new SqlCommand(sqlTongSach, conn))
                    {
                        int tongSach = Convert.ToInt32(cmd.ExecuteScalar());

                        // Thay lblTongSach bằng tên Label hiển thị tổng số sách của bạn trên giao diện
                        if (lblTongSach != null)
                        {
                            lblTongSach.Text = tongSach.ToString("N0");
                        }
                    }

                    // 2. Đếm tổng số thể loại trong bảng theloai
                    string sqlTongTheLoai = "SELECT COUNT(*) FROM dbo.theloai";
                    using (SqlCommand cmd = new SqlCommand(sqlTongTheLoai, conn))
                    {
                        int tongTheLoai = Convert.ToInt32(cmd.ExecuteScalar());

                        // Thay lblTongTheLoai bằng tên Label hiển thị tổng số thể loại của bạn trên giao diện
                        if (lblTongTheLoai != null)
                        {
                            lblTongTheLoai.Text = tongTheLoai.ToString("N0");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải thống kê trang chủ: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
