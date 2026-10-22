using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QLTV.GUI.UC_MuonTra
{
    public partial class ucPhieuTra : UserControl
    {
        private readonly string connString = @"Data Source=.\SQLEXPRESS;Initial Catalog=QLThuVien;Integrated Security=True";
        private string maPhieuTraDangChon = "";
        private string maPhieuMuonDangChon = "";

        public ucPhieuTra()
        {
            InitializeComponent();
            cboMaPhieuMuon.SelectedValueChanged += cboMaPhieuMuon_SelectedValueChanged;
        }

        private void ucPhieuTra_Load(object sender, EventArgs e)
        {
            dgvPhieuTra.AutoGenerateColumns = false;
            dgvPhieuTra.AllowUserToAddRows = false;
            dgvPhieuTra.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPhieuTra.MultiSelect = false;
            dgvPhieuTra.ReadOnly = true;

            txtMaPhieuTra.ReadOnly = true;
            txtTenDocGia.ReadOnly = true;

            LoadComboBoxes();
            LoadDanhSachPhieuTra();
        }
        #region NẠP DỮ LIỆU & XỬ LÝ COMBOBOX
        private void LoadComboBoxes()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    // Nạp Mã Phiếu Mượn
                    using (SqlDataAdapter da = new SqlDataAdapter("SELECT MaPhieuMuon FROM dbo.phieumuon ORDER BY MaPhieuMuon DESC", conn))
                    {
                        DataTable dtPM = new DataTable();
                        da.Fill(dtPM);
                        cboMaPhieuMuon.DataSource = dtPM;
                        cboMaPhieuMuon.DisplayMember = "MaPhieuMuon";
                        cboMaPhieuMuon.ValueMember = "MaPhieuMuon";
                        cboMaPhieuMuon.SelectedIndex = -1;
                    }
                    // Nạp Nhân Viên (Người nhận trả sách)
                    using (SqlDataAdapter da = new SqlDataAdapter("SELECT MaNV, HoTen FROM dbo.nhanvien", conn))
                    {
                        DataTable dtNV = new DataTable();
                        da.Fill(dtNV);
                        cboNhanVien.DataSource = dtNV;
                        cboNhanVien.DisplayMember = "HoTen";
                        cboNhanVien.ValueMember = "MaNV";
                        cboNhanVien.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải danh mục: " + ex.Message); }
        }
        private void guna2Button6_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(maPhieuTraDangChon))
            {
                MessageBox.Show("Chọn phiếu trả để xem chi tiết!");
                return;
            }

            FormTrangChu f = this.FindForm() as FormTrangChu;
            if (f != null)
            {
                ucChiTietPhieuTra uc = new ucChiTietPhieuTra(
                    maPhieuTraDangChon,
                    maPhieuMuonDangChon,
                    txtTenDocGia.Text,
                    cboNhanVien.Text,
                    dtpNgayTra.Value
                );
                f.HienThiUserControl(uc);
            }
        }

        private void cboMaPhieuMuon_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cboMaPhieuMuon.SelectedValue != null && cboMaPhieuMuon.SelectedValue is int maPM)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        string query = @"
                            SELECT dg.HoTen 
                            FROM dbo.phieumuon pm 
                            JOIN dbo.docgia dg ON pm.MaDocGia = dg.MaDocGia 
                            WHERE pm.MaPhieuMuon = @MaPM";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@MaPM", maPM);
                            conn.Open();
                            object result = cmd.ExecuteScalar();
                            txtTenDocGia.Text = result != null ? result.ToString() : "";
                        }
                    }
                }
                catch { txtTenDocGia.Clear(); }
            }
            else { txtTenDocGia.Clear(); }
        }
        #endregion

        #region TẢI LƯỚI & CLICK LƯỚI
        private void LoadDanhSachPhieuTra(string keyword = "")
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                        SELECT 
                            ROW_NUMBER() OVER(ORDER BY pt.MaPhieuTra DESC) AS STT,
                            pt.MaPhieuTra,
                            pt.MaPhieuMuon,
                            pt.MaNV,
                            nv.HoTen AS TenNhanVien,
                            pt.NgayTra
                        FROM dbo.phieutra pt
                        LEFT JOIN dbo.nhanvien nv ON pt.MaNV = nv.MaNV
                        WHERE @Kw = N'' 
                           OR CAST(pt.MaPhieuTra AS NVARCHAR) LIKE N'%' + @Kw + N'%'
                           OR CAST(pt.MaPhieuMuon AS NVARCHAR) LIKE N'%' + @Kw + N'%'
                           OR nv.HoTen LIKE N'%' + @Kw + N'%'
                        ORDER BY pt.MaPhieuTra DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Kw", keyword.Trim());
                        DataTable dt = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) da.Fill(dt);

                        if (dgvPhieuTra.Columns.Count >= 6)
                        {
                            dgvPhieuTra.Columns[1].DataPropertyName = "MaPhieuTra";
                            dgvPhieuTra.Columns[2].DataPropertyName = "MaPhieuMuon";
                            dgvPhieuTra.Columns[3].DataPropertyName = "MaNV";
                            dgvPhieuTra.Columns[4].DataPropertyName = "TenNhanVien";
                            if (dgvPhieuTra.Columns.Count > 5)
                            {
                                dgvPhieuTra.Columns[5].DataPropertyName = "NgayTra";
                                dgvPhieuTra.Columns[5].DefaultCellStyle.Format = "dd/MM/yyyy";
                            }
                        }
                        dgvPhieuTra.DataSource = dt;
                        XoaNhap();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải danh sách: " + ex.Message); }
        }

        private void dgvPhieuTra_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvPhieuTra.Rows[e.RowIndex];
                maPhieuTraDangChon = row.Cells[1].Value?.ToString();
                maPhieuMuonDangChon = row.Cells[2].Value?.ToString();

                txtMaPhieuTra.Text = maPhieuTraDangChon;
                if (row.Cells[2].Value != DBNull.Value) cboMaPhieuMuon.SelectedValue = row.Cells[2].Value;
                if (row.Cells[3].Value != DBNull.Value) cboNhanVien.SelectedValue = row.Cells[3].Value;
                if (row.Cells[5].Value != DBNull.Value) dtpNgayTra.Value = Convert.ToDateTime(row.Cells[5].Value);
            }
        }

        private void dgvPhieuTra_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            dgvPhieuTra.Rows[e.RowIndex].Cells["colSTT"].Value = (e.RowIndex + 1).ToString();
        }
        private void XoaNhap()
        {
            maPhieuTraDangChon = "";
            maPhieuMuonDangChon = "";
            txtMaPhieuTra.Clear();
            cboMaPhieuMuon.SelectedIndex = -1;
            cboNhanVien.SelectedIndex = -1;
            txtTenDocGia.Clear();
            dtpNgayTra.Value = DateTime.Today;
            dgvPhieuTra.ClearSelection();
        }
        #endregion

        #region CRUD (TẠO, SỬA, XÓA, HỦY)
        private void guna2Button4_Click_1(object sender, EventArgs e)
        {
            if (cboMaPhieuMuon.SelectedValue == null || cboNhanVien.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn Mã phiếu mượn và Nhân viên tiếp nhận!", "Thông báo");
                return;
            }

            try
            {
                int newMaPhieuTra = 0;
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                        INSERT INTO dbo.phieutra (MaPhieuMuon, MaNV, NgayTra) 
                        VALUES (@MaPM, @MaNV, @NgayTra);
                        SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaPM", cboMaPhieuMuon.SelectedValue);
                        cmd.Parameters.AddWithValue("@MaNV", cboNhanVien.SelectedValue);
                        cmd.Parameters.AddWithValue("@NgayTra", dtpNgayTra.Value.Date);

                        conn.Open();
                        newMaPhieuTra = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
                MessageBox.Show($"Tạo phiếu trả thành công! Mã phiếu mới: {newMaPhieuTra}", "Thông báo");
                LoadDanhSachPhieuTra();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tạo phiếu: " + ex.Message); }
        }

        private void guna2Button2_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(maPhieuTraDangChon)) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "UPDATE dbo.phieutra SET MaPhieuMuon = @MaPM, MaNV = @MaNV, NgayTra = @NgayTra WHERE MaPhieuTra = @MaPT";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaPT", int.Parse(maPhieuTraDangChon));
                        cmd.Parameters.AddWithValue("@MaPM", cboMaPhieuMuon.SelectedValue);
                        cmd.Parameters.AddWithValue("@MaNV", cboNhanVien.SelectedValue);
                        cmd.Parameters.AddWithValue("@NgayTra", dtpNgayTra.Value.Date);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Cập nhật thành công!");
                LoadDanhSachPhieuTra();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi sửa phiếu: " + ex.Message); }
        }

        private void guna2Button7_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(maPhieuTraDangChon)) return;
            if (MessageBox.Show("Xóa phiếu trả này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM dbo.phieutra WHERE MaPhieuTra = @MaPT", conn))
                        {
                            cmd.Parameters.AddWithValue("@MaPT", int.Parse(maPhieuTraDangChon));
                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                    LoadDanhSachPhieuTra();
                }
                catch (SqlException ex) when (ex.Number == 547)
                { MessageBox.Show("Vui lòng xóa các sách chi tiết bên trong phiếu trước khi xóa phiếu này!"); }
            }
        }

        private void guna2Button5_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show("Hủy thao tác và làm mới?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes) XoaNhap();
        }
        #endregion  

        #region TÌM KIẾM & XEM CHI TIẾT
        private void guna2Button1_Click_1(object sender, EventArgs e)
        {
            LoadDanhSachPhieuTra(txtTimKiem.Text);
        }

        private void guna2Button3_Click_1(object sender, EventArgs e)
        {
            txtTimKiem.Clear(); LoadDanhSachPhieuTra();
        }
        #endregion
    }
}
