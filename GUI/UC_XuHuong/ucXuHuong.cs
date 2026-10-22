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
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;

namespace QLTV.GUI.UC_XuHuong
{
    public partial class ucXuHuong : UserControl
    {
        private readonly string connString = @"Data Source=.\SQLEXPRESS;Initial Catalog=QLThuVien;Integrated Security=True";
        public ucXuHuong()
        {
            InitializeComponent();
        }

        private void ucXuHuong_Load(object sender, EventArgs e)
        {
            dtpTuNgay.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpDenNgay.Value = DateTime.Now;

            // Load dữ liệu lần đầu
            LoadThongKe();
        }
        #region HÀM TẢI DỮ LIỆU THỐNG KÊ

        private void LoadThongKe()
        {
            DateTime tuNgay = dtpTuNgay.Value.Date;
            DateTime denNgay = dtpDenNgay.Value.Date.AddDays(1).AddSeconds(-1); // Cuối ngày

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    // 1. THẺ TỔNG QUAN: Lượt mượn
                    string sqlLuotMuon = "SELECT COUNT(*) FROM dbo.phieumuon WHERE NgayMuon BETWEEN @TuNgay AND @DenNgay";
                    using (SqlCommand cmd = new SqlCommand(sqlLuotMuon, conn))
                    {
                        cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                        cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                        int luotMuon = Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
                        label5.Text = luotMuon.ToString("N0"); // Lượt mượn
                    }

                    // 2. THẺ TỔNG QUAN: Độc giả mới đăng ký
                    string sqlDocGiaMoi = "SELECT COUNT(*) FROM dbo.docgia WHERE NgayDangKy BETWEEN @TuNgay AND @DenNgay";
                    using (SqlCommand cmd = new SqlCommand(sqlDocGiaMoi, conn))
                    {
                        cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                        cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                        int docGiaMoi = Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
                        label7.Text = docGiaMoi.ToString("N0"); // Độc giả mới
                    }

                    // 3. THẺ TỔNG QUAN: Tổng Tiền phạt
                    string sqlTienPhat = "SELECT ISNULL(SUM(TienPhat), 0) FROM dbo.chitietphieutra ct JOIN dbo.phieutra pt ON ct.MaPhieuTra = pt.MaPhieuTra WHERE pt.NgayTra BETWEEN @TuNgay AND @DenNgay";
                    using (SqlCommand cmd = new SqlCommand(sqlTienPhat, conn))
                    {
                        cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                        cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                        decimal tienPhat = Convert.ToDecimal(cmd.ExecuteScalar() ?? 0);
                        label12.Text = tienPhat.ToString("N0") + " VNĐ"; // Tiền phạt
                    }

                    // 4. THẺ TỔNG QUAN: Sách quá hạn
                    string sqlSachQuaHan = @"SELECT ISNULL(SUM(ctpm.SoLuongMuon - ISNULL(ctpt.SoLuongTra, 0)), 0)
                                            FROM dbo.chitietphieumuon ctpm
                                            JOIN dbo.phieumuon pm ON ctpm.MaPhieuMuon = pm.MaPhieuMuon
                                            LEFT JOIN (
                                                SELECT pt.MaPhieuMuon, ct.MaSach, SUM(ct.SoLuongTra) AS SoLuongTra
                                                FROM dbo.chitietphieutra ct
                                                JOIN dbo.phieutra pt ON ct.MaPhieuTra = pt.MaPhieuTra
                                                GROUP BY pt.MaPhieuMuon, ct.MaSach
                                            ) ctpt ON ctpm.MaPhieuMuon = ctpt.MaPhieuMuon AND ctpm.MaSach = ctpt.MaSach
                                            WHERE ctpm.HanTra < GETDATE() AND (ctpm.SoLuongMuon - ISNULL(ctpt.SoLuongTra, 0)) > 0";
                    using (SqlCommand cmd = new SqlCommand(sqlSachQuaHan, conn))
                    {
                        int quaHan = Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
                        label8.Text = quaHan.ToString("N0"); // Sách quá hạn
                    }

                    // 5. NAP BẢNG & BIỂU ĐỒ TỪ VIEW Top 10 Sách Hot
                    string sqlTopSach = "SELECT TenSach, TongLuotMuon FROM dbo.vw_XuHuong_TopSach";
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlTopSach, conn))
                    {
                        DataTable dtTopSach = new DataTable();
                        da.Fill(dtTopSach);
                        dgvTopSach.AutoGenerateColumns = false;
                        // Nạp vào DataGridView
                        dgvTopSach.DataSource = dtTopSach;

                        // Nạp vào Biểu đồ Chart Control (nếu có control Chart)
                        VeBieuDoTopSach(dtTopSach);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nạp thống kê: " + ex.Message, "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Vẽ Biểu đồ từ DataTable
        private void VeBieuDoTopSach(DataTable dt)
        {
            if (chartTopSach == null) return; // Nếu trên form chưa kéo Chart thì bỏ qua

            chartTopSach.Series.Clear();
            Series series = new Series("Lượt mượn");
            series.ChartType = SeriesChartType.Column; // Biểu đồ dạng cột

            foreach (DataRow row in dt.Rows)
            {
                string tenSach = row["TenSach"].ToString();
                int luotMuon = Convert.ToInt32(row["TongLuotMuon"]);
                series.Points.AddXY(tenSach, luotMuon);
            }

            chartTopSach.Series.Add(series);
            chartTopSach.Titles.Clear();
            chartTopSach.Titles.Add("Top Sách Mượn Nhiều Nhất");
        }

        

        #endregion

        #region BỘ NÚT LỌC, LÀM MỚI VÀ XUẤT EXCEL
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            LoadThongKe();
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            dtpTuNgay.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpDenNgay.Value = DateTime.Now;
            LoadThongKe();
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            if (dgvTopSach.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất Excel!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Excel Files (*.csv)|*.csv",
                FileName = "BaoCao_XuHuong_SachHot_" + DateTime.Now.ToString("ddMMyyyy") + ".csv"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();

                    // Ghi tiêu đề cột
                    for (int i = 0; i < dgvTopSach.Columns.Count; i++)
                    {
                        sb.Append(dgvTopSach.Columns[i].HeaderText + (i == dgvTopSach.Columns.Count - 1 ? "" : ","));
                    }
                    sb.AppendLine();

                    // Ghi dòng dữ liệu
                    foreach (DataGridViewRow row in dgvTopSach.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            for (int i = 0; i < dgvTopSach.Columns.Count; i++)
                            {
                                string val = row.Cells[i].Value?.ToString().Replace(",", " ") ?? "";
                                sb.Append(val + (i == dgvTopSach.Columns.Count - 1 ? "" : ","));
                            }
                            sb.AppendLine();
                        }
                    }

                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("Xuất file báo cáo thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xuất file: " + ex.Message, "Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        private void dgvTopSach_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex >= 0)
            {
                e.Value = (e.RowIndex + 1).ToString();
                e.FormattingApplied = true;
            }
        }
    }
}
