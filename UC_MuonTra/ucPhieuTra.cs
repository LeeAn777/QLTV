using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QLTV.GUI.UC_MuonTra
{
    public partial class ucPhieuTra : UserControl
    {
        private readonly string connString = @"Data Source=DESKTOP-PDF3KTE\MSSQLSERVER03;Initial Catalog=QLThuVien;Integrated Security=True";
        private string maPhieuDangChon = "";
        private bool dangTai = false;

        public ucPhieuTra()
        {
            InitializeComponent();
            Load -= ucPhieuTra_Load;
            Load += ucPhieuTra_Load;
            // Nút xem chi tiết đã được gắn trong Designer.
            guna2Button6.Click -= guna2Button6_Click;
            guna2Button6.Click += guna2Button6_Click;
            guna2Button4.Click += guna2Button4_Click; // Ghi nhận trả
            guna2Button2.Click += guna2Button2_Click; // Sửa ngày trả
            guna2Button7.Click += guna2Button7_Click; // Hủy ghi nhận trả
            guna2Button1.Click += guna2Button1_Click; // Tìm kiếm
            guna2Button3.Click += guna2Button3_Click; // Làm mới
            guna2Button5.Click += guna2Button5_Click; // Hủy nhập
            dgvPhieuTra.CellClick += dgvPhieuTra_CellClick;
            guna2TextBox1.Leave += (s, e) => HienThiThongTinMuon();
        }

        private void ucPhieuTra_Load(object sender, EventArgs e)
        {
            dgvPhieuTra.AutoGenerateColumns = false;
            dgvPhieuTra.AllowUserToAddRows = false;
            dgvPhieuTra.ReadOnly = true;
            dgvPhieuTra.MultiSelect = false;
            dgvPhieuTra.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPhieuTra.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            colSTT.DataPropertyName = "STT";
            colMaPhieuTra.DataPropertyName = "MaPhieuTra";
            colMaPhieuMuon.DataPropertyName = "MaPhieuMuon";
            colMaNV.DataPropertyName = "MaNV";
            colTenNV.DataPropertyName = "TenNhanVien";
            colNgayTra.DataPropertyName = "NgayTra";
            colNgayTra.DefaultCellStyle.Format = "dd/MM/yyyy";
            // Không có bảng phieutra nên dùng mã phiếu mượn làm mã tham chiếu trả.
            guna2TextBox3.ReadOnly = true;
            guna2TextBox5.ReadOnly = true;
            guna2TextBox2.ReadOnly = true;
            guna2DateTimePicker1.Value = DateTime.Today;
            XoaNhap();
            TaiDanhSach();
        }

        private string LayTen(string bang, string cot, string ma)
        {
            if (string.IsNullOrWhiteSpace(ma)) return "";
            try
            {
                using (SqlConnection cn = new SqlConnection(connString))
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT TOP 1 hoten FROM dbo." + bang + " WHERE " + cot + " = @Ma", cn))
                {
                    cmd.Parameters.Add("@Ma", SqlDbType.VarChar, 100).Value = ma;
                    cn.Open();
                    object result = cmd.ExecuteScalar();
                    return result == null || result == DBNull.Value ? "" : Convert.ToString(result);
                }
            }
            catch (SqlException) { return ""; }
        }

        private void TaiDanhSach(string tim = "")
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(connString))
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT ROW_NUMBER() OVER (ORDER BY pm.maphieu) AS STT,
                           CASE
    WHEN pm.maphieu LIKE 'PM%'
    THEN 'PT' + SUBSTRING(pm.maphieu, 3, LEN(pm.maphieu))
    ELSE 'PT-' + pm.maphieu
END AS MaPhieuTra,
                           pm.maphieu AS MaPhieuMuon,
                           pm.manv AS MaNV,
                           pm.madocgia AS MaDocGia,
                           pm.ngaymuon AS NgayMuon,
                           pm.ngaytra AS NgayTra,
                           pm.trangthai AS TrangThai
                    FROM dbo.phieumuon pm
                    WHERE pm.ngaytra IS NOT NULL
                      AND (@Tim = N'' OR pm.maphieu LIKE N'%' + @Tim + N'%'
                           OR pm.madocgia LIKE N'%' + @Tim + N'%')
                    ORDER BY pm.maphieu", cn))
                {
                    cmd.Parameters.Add("@Tim", SqlDbType.NVarChar, 100).Value = tim.Trim();
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd)) da.Fill(dt);
                    dt.Columns.Add("TenNhanVien", typeof(string));
                    foreach (DataRow row in dt.Rows)
                        row["TenNhanVien"] = LayTen("nhanvien", "manv", Convert.ToString(row["MaNV"]));
                    dangTai = true;
                    dgvPhieuTra.DataSource = dt;
                    dgvPhieuTra.ClearSelection();
                    dgvPhieuTra.CurrentCell = null;
                    dangTai = false;
                }
            }
            catch (Exception ex)
            {
                dangTai = false;
                MessageBox.Show("Lỗi tải phiếu trả: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HienThiThongTinMuon()
        {
            if (dangTai) return;
            string ma = guna2TextBox1.Text.Trim();
            guna2TextBox3.Text =
    ma.StartsWith("PM", StringComparison.OrdinalIgnoreCase)
    ? "PT" + ma.Substring(2)
    : "PT-" + ma; // Mã phiếu trả tham chiếu chính mã phiếu mượn
            guna2TextBox5.Clear();
            guna2TextBox2.Clear();
            if (ma.Length == 0) return;
            try
            {
                using (SqlConnection cn = new SqlConnection(connString))
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT madocgia, manv, ngaymuon, ngaytra
                    FROM dbo.phieumuon WHERE maphieu = @Ma", cn))
                {
                    cmd.Parameters.Add("@Ma", SqlDbType.VarChar, 100).Value = ma;
                    cn.Open();
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        if (!rd.Read())
                        {
                            MessageBox.Show("Không tìm thấy mã phiếu mượn: " + ma);
                            return;
                        }
                        string maDG = Convert.ToString(rd["madocgia"]);
                        string maNV = Convert.ToString(rd["manv"]);
                        rd.Close();
                        guna2TextBox5.Text = LayTen("docgia", "madocgia", maDG);
                        guna2TextBox2.Text = LayTen("nhanvien", "manv", maNV);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tìm phiếu mượn: " + ex.Message); }
        }

        private void XoaNhap()
        {
            maPhieuDangChon = "";
            guna2TextBox1.ReadOnly = false;
            guna2TextBox1.Clear();
            guna2TextBox3.Clear();
            guna2TextBox5.Clear();
            guna2TextBox2.Clear();
            guna2DateTimePicker1.Value = DateTime.Today;
            dgvPhieuTra.ClearSelection();
            if (dgvPhieuTra.Rows.Count > 0) dgvPhieuTra.CurrentCell = null;
        }

        // Ghi nhận trả trực tiếp trên phieumuon. Không INSERT bảng mới.
        private void guna2Button4_Click(object sender, EventArgs e)
        {
            string ma = guna2TextBox1.Text.Trim();
            if (ma.Length == 0) { MessageBox.Show("Vui lòng nhập mã phiếu mượn!"); return; }
            if (ma.Length > 100) { MessageBox.Show("Mã phiếu tối đa 100 ký tự!"); return; }
            try
            {
                using (SqlConnection cn = new SqlConnection(connString))
                {
                    cn.Open();
                    using (SqlTransaction tx = cn.BeginTransaction())
                    {
                        try
                        {
                            DateTime ngayMuon;
                            bool daTra;
                            using (SqlCommand check = new SqlCommand(@"
                                SELECT ngaymuon, ngaytra FROM dbo.phieumuon WITH (UPDLOCK, HOLDLOCK)
                                WHERE maphieu = @Ma", cn, tx))
                            {
                                check.Parameters.Add("@Ma", SqlDbType.VarChar, 100).Value = ma;
                                using (SqlDataReader rd = check.ExecuteReader())
                                {
                                    if (!rd.Read())
                                    { MessageBox.Show("Mã phiếu mượn không tồn tại!"); return; }
                                    ngayMuon = Convert.ToDateTime(rd["ngaymuon"]);
                                    daTra = rd["ngaytra"] != DBNull.Value;
                                }
                            }
                            if (daTra) { MessageBox.Show("Phiếu này đã được ghi nhận trả!"); return; }
                            if (guna2DateTimePicker1.Value.Date < ngayMuon.Date)
                            { MessageBox.Show("Ngày trả không thể trước ngày mượn!"); return; }
                            using (SqlCommand cmd = new SqlCommand(@"
                                UPDATE dbo.phieumuon SET ngaytra = @NgayTra, trangthai = @TrangThai
                                WHERE maphieu = @Ma AND ngaytra IS NULL", cn, tx))
                            {
                                cmd.Parameters.Add("@NgayTra", SqlDbType.Date).Value = guna2DateTimePicker1.Value.Date;
                                cmd.Parameters.Add("@TrangThai", SqlDbType.NVarChar, 100).Value = "Đã trả";
                                cmd.Parameters.Add("@Ma", SqlDbType.VarChar, 100).Value = ma;
                                if (cmd.ExecuteNonQuery() != 1) throw new Exception("Không thể cập nhật phiếu mượn.");
                            }
                            tx.Commit();
                        }
                        catch { tx.Rollback(); throw; }
                    }
                }
                MessageBox.Show("Đã ghi nhận trả sách thành công!");
                TaiDanhSach();
                XoaNhap();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi ghi nhận trả: " + ex.Message); }
        }

        private void dgvPhieuTra_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dangTai || e.RowIndex < 0) return;
            DataRowView row = dgvPhieuTra.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (row == null) return;
            maPhieuDangChon = Convert.ToString(row["MaPhieuMuon"]);
            guna2TextBox1.Text = maPhieuDangChon;
            guna2TextBox1.ReadOnly = true;
            guna2TextBox3.Text = Convert.ToString(row["MaPhieuTra"]);
            guna2TextBox5.Text = LayTen("docgia", "madocgia", Convert.ToString(row["MaDocGia"]));
            guna2TextBox2.Text = Convert.ToString(row["TenNhanVien"]);
            if (row["NgayTra"] != DBNull.Value)
                guna2DateTimePicker1.Value = Convert.ToDateTime(row["NgayTra"]);
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (maPhieuDangChon.Length == 0) { MessageBox.Show("Chọn phiếu trả cần sửa trong bảng!"); return; }
            if (MessageBox.Show("Lưu ngày trả đã sửa?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                using (SqlConnection cn = new SqlConnection(connString))
                using (SqlCommand cmd = new SqlCommand(@"
                    UPDATE dbo.phieumuon SET ngaytra = @NgayTra
                    WHERE maphieu = @Ma AND ngaytra IS NOT NULL
                      AND ngaymuon <= @NgayTra", cn))
                {
                    cmd.Parameters.Add("@NgayTra", SqlDbType.Date).Value = guna2DateTimePicker1.Value.Date;
                    cmd.Parameters.Add("@Ma", SqlDbType.VarChar, 100).Value = maPhieuDangChon;
                    cn.Open();
                    int n = cmd.ExecuteNonQuery();
                    MessageBox.Show(n == 1 ? "Sửa ngày trả thành công!" : "Không sửa được: phiếu không tồn tại hoặc ngày trả trước ngày mượn.");
                }
                TaiDanhSach(); XoaNhap();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi sửa: " + ex.Message); }
        }

        // "Xóa phiếu" trên UI = hủy ghi nhận trả, KHÔNG xóa lịch sử mượn.
        private void guna2Button7_Click(object sender, EventArgs e)
        {
            if (maPhieuDangChon.Length == 0) { MessageBox.Show("Chọn phiếu trả cần hủy trong bảng!"); return; }
            if (MessageBox.Show("Hủy ghi nhận trả của phiếu " + maPhieuDangChon +
                "?\nNgày trả sẽ được xóa và trạng thái đổi về 'Đang mượn'.\nPhiếu mượn gốc được giữ nguyên.",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            try
            {
                using (SqlConnection cn = new SqlConnection(connString))
                using (SqlCommand cmd = new SqlCommand(@"
                    UPDATE dbo.phieumuon
                    SET ngaytra = NULL, trangthai = @TrangThai
                    WHERE maphieu = @Ma AND ngaytra IS NOT NULL", cn))
                {
                    cmd.Parameters.Add("@TrangThai", SqlDbType.NVarChar, 100).Value = "Đang mượn";
                    cmd.Parameters.Add("@Ma", SqlDbType.VarChar, 100).Value = maPhieuDangChon;
                    cn.Open();
                    int n = cmd.ExecuteNonQuery();
                    MessageBox.Show(n == 1 ? "Đã hủy ghi nhận trả!" : "Phiếu chưa trả hoặc không tồn tại.");
                }
                TaiDanhSach(); XoaNhap();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi hủy trả: " + ex.Message); }
        }

        private void guna2Button1_Click(object sender, EventArgs e) { TaiDanhSach(txtTimKiem.Text); }
        private void guna2Button3_Click(object sender, EventArgs e)
        { txtTimKiem.Clear(); XoaNhap(); TaiDanhSach(); }
        private void guna2Button5_Click(object sender, EventArgs e) { XoaNhap(); }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            if (maPhieuDangChon.Length == 0)
            { MessageBox.Show("Hãy chọn một phiếu trả trong bảng!"); return; }
            MessageBox.Show("Phiếu mượn: " + maPhieuDangChon +
                "\nThông tin trả đang lưu tại bảng phieumuon." +
                "\nMàn hình chi tiết phiếu trả riêng chưa được kết nối.");
        }
    }
}
