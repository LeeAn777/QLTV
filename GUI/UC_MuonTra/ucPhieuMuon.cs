using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QLTV.GUI.UC_MuonTra
{
    public partial class ucPhieuMuon : UserControl
    {
        private readonly string connString = @"Data Source=.\SQLEXPRESS;Initial Catalog=QLThuVien;Integrated Security=True";

        private string maPhieuDangChon = "";
        public ucPhieuMuon()
        {
            InitializeComponent();
            cboTenDocGia.SelectedValueChanged += cboTenDG_SelectedValueChanged;
            cboTenNV.SelectedValueChanged += cboTenNV_SelectedValueChanged;
        }

        private void ucPhieuMuon_Load(object sender, EventArgs e)
        {
            dgvPhieuMuon.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvPhieuMuon.AutoGenerateColumns = false;
            dgvPhieuMuon.AllowUserToAddRows = false;
            dgvPhieuMuon.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPhieuMuon.MultiSelect = false;
            dgvPhieuMuon.ReadOnly = true;


            // Tải dữ liệu vào 2 ComboBox
            LoadComboBoxes();

            // Tải danh sách phiếu lên DataGridView
            LoadDanhSachPhieuMuon();
        }
        private void LoadComboBoxes()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    // Nạp Độc Giả
                    using (SqlDataAdapter da = new SqlDataAdapter("SELECT MaDocGia, HoTen FROM dbo.docgia", conn))
                    {
                        DataTable dtDG = new DataTable();
                        da.Fill(dtDG);
                        cboTenDocGia.DataSource = dtDG;
                        cboTenDocGia.DisplayMember = "HoTen";
                        cboTenDocGia.ValueMember = "MaDocGia";
                        cboTenDocGia.SelectedIndex = -1; // Mặc định không chọn ai
                    }
                    // Nạp Nhân Viên
                    using (SqlDataAdapter da = new SqlDataAdapter("SELECT MaNV, HoTen FROM dbo.nhanvien", conn))
                    {
                        DataTable dtNV = new DataTable();
                        da.Fill(dtNV);
                        cboTenNV.DataSource = dtNV;
                        cboTenNV.DisplayMember = "HoTen";
                        cboTenNV.ValueMember = "MaNV";
                        cboTenNV.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải danh mục: " + ex.Message); }
        }
        private void cboTenDG_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cboTenDocGia.SelectedValue != null && cboTenDocGia.SelectedValue is int)
            {
                txtMaDocGia.Text = cboTenDocGia.SelectedValue.ToString();
            }
            else { txtMaDocGia.Clear(); }
        }

        private void cboTenNV_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cboTenNV.SelectedValue != null && cboTenNV.SelectedValue is int)
            {
                txtMaNV.Text = cboTenNV.SelectedValue.ToString();
            }
            else { txtMaNV.Clear(); }
        }
        private void LoadDanhSachPhieuMuon(string keyword = "")
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                        SELECT 
                            ROW_NUMBER() OVER(ORDER BY pm.MaPhieuMuon DESC) AS STT,
                            pm.MaPhieuMuon,
                            pm.MaDocGia,
                            dg.HoTen AS TenDocGia,
                            pm.MaNV,
                            nv.HoTen AS TenNhanVien,
                            pm.NgayMuon
                        FROM dbo.phieumuon pm
                        LEFT JOIN dbo.docgia dg ON pm.MaDocGia = dg.MaDocGia
                        LEFT JOIN dbo.nhanvien nv ON pm.MaNV = nv.MaNV
                        WHERE @Kw = N'' 
                           OR CAST(pm.MaPhieuMuon AS NVARCHAR) LIKE N'%' + @Kw + N'%'
                           OR dg.HoTen LIKE N'%' + @Kw + N'%'
                        ORDER BY pm.MaPhieuMuon DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Kw", keyword.Trim());
                        DataTable dt = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) da.Fill(dt);

                        if (dgvPhieuMuon.Columns.Count >= 6)
                        {
                            dgvPhieuMuon.Columns[1].DataPropertyName = "MaPhieuMuon";
                            dgvPhieuMuon.Columns[2].DataPropertyName = "MaDocGia";
                            dgvPhieuMuon.Columns[3].DataPropertyName = "TenDocGia";
                            dgvPhieuMuon.Columns[4].DataPropertyName = "MaNV";
                            dgvPhieuMuon.Columns[5].DataPropertyName = "TenNhanVien";
                            if (dgvPhieuMuon.Columns.Count > 6)
                            {
                                dgvPhieuMuon.Columns[6].DataPropertyName = "NgayMuon";
                                dgvPhieuMuon.Columns[6].DefaultCellStyle.Format = "dd/MM/yyyy";
                            }
                        }
                        dgvPhieuMuon.DataSource = dt;
                        dgvPhieuMuon.ClearSelection();
                        maPhieuDangChon = "";
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải danh sách: " + ex.Message); }
        }
        // Click lên lưới -> Nạp ngược lại ComboBox (TextBox tự nhảy mã theo)
        private void dgvPhieuMuon_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvPhieuMuon.Rows[e.RowIndex];
                maPhieuDangChon = row.Cells[1].Value?.ToString();

                txtMaPhieu.Text = maPhieuDangChon;

                // Gán mã vào Value của ComboBox -> ComboBox sẽ tự hiện Tên và kích hoạt nhảy Mã ra TextBox
                if (row.Cells[2].Value != DBNull.Value)
                    cboTenDocGia.SelectedValue = row.Cells[2].Value;

                if (row.Cells[4].Value != DBNull.Value)
                    cboTenNV.SelectedValue = row.Cells[4].Value;

                if (row.Cells[6].Value != DBNull.Value)
                    dtpNgayMuon.Value = Convert.ToDateTime(row.Cells[6].Value);
            }
        } 
        private void XoaNhap()
        {
            maPhieuDangChon = "";
            txtMaPhieu.Clear();
            cboTenDocGia.SelectedIndex = -1;
            cboTenNV.SelectedIndex = -1;
            txtMaDocGia.Clear();
            txtMaNV.Clear();
            dtpNgayMuon.Value = DateTime.Today;
            dgvPhieuMuon.ClearSelection();
        }
        // ================= TẠO PHIẾU MỚI =================
        private void guna2Button4_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaDocGia.Text) || string.IsNullOrWhiteSpace(txtMaNV.Text))
            {
                MessageBox.Show("Vui lòng chọn Độc giả và Nhân viên!", "Thông báo");
                return;
            }

            try
            {
                int newMaPhieu = 0;
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                        INSERT INTO dbo.phieumuon (MaDocGia, MaNV, NgayMuon) 
                        VALUES (@MaDG, @MaNV, @NgayMuon);
                        SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaDG", int.Parse(txtMaDocGia.Text));
                        cmd.Parameters.AddWithValue("@MaNV", int.Parse(txtMaNV.Text));
                        cmd.Parameters.AddWithValue("@NgayMuon", dtpNgayMuon.Value.Date);

                        conn.Open();
                        newMaPhieu = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
                MessageBox.Show($"Tạo phiếu mượn thành công! Mã phiếu mới: {newMaPhieu}", "Thông báo");
                LoadDanhSachPhieuMuon();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tạo phiếu: " + ex.Message); }
        }
        // ================= XEM CHI TIẾT =================
        private void btnxemctpm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(maPhieuDangChon))
            {
                MessageBox.Show("Vui lòng click chọn 1 phiếu mượn trên bảng trước!");
                return;
            }

            FormTrangChu f = this.FindForm() as FormTrangChu;
            if (f != null)
            {
                ucChiTietPhieuMuon uc = new ucChiTietPhieuMuon(
                    maPhieuDangChon,
                    cboTenDocGia.Text, // Gửi tên ĐG sang
                    cboTenNV.Text,     // Gửi tên NV sang
                    dtpNgayMuon.Value
                );
                f.HienThiUserControl(uc);
            }
        }
        private void guna2Button2_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(maPhieuDangChon))
            {
                MessageBox.Show("Vui lòng chọn một phiếu từ danh sách để sửa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn lưu các thay đổi cho phiếu này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        string query = "UPDATE dbo.phieumuon SET MaDocGia = @MaDG, MaNV = @MaNV, NgayMuon = @NgayMuon WHERE MaPhieuMuon = @MaPhieu";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@MaPhieu", int.Parse(maPhieuDangChon));
                            cmd.Parameters.AddWithValue("@MaDG", int.Parse(txtMaDocGia.Text));
                            cmd.Parameters.AddWithValue("@MaNV", int.Parse(txtMaNV.Text));
                            cmd.Parameters.AddWithValue("@NgayMuon", dtpNgayMuon.Value.Date);

                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show("Cập nhật phiếu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDanhSachPhieuMuon(txtTimKiem.Text); // Giữ nguyên kết quả tìm kiếm nếu có
                }
                catch (Exception ex) { MessageBox.Show("Lỗi sửa phiếu: " + ex.Message); }
            }
        }

        private void guna2Button7_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(maPhieuDangChon))
            {
                MessageBox.Show("Vui lòng chọn một phiếu từ danh sách để xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa Phiếu Mượn số {maPhieuDangChon} không?\nChú ý: Hành động này không thể hoàn tác!", "Cảnh báo Xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        string query = "DELETE FROM dbo.phieumuon WHERE MaPhieuMuon = @MaPhieu";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@MaPhieu", int.Parse(maPhieuDangChon));
                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show("Đã xóa phiếu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDanhSachPhieuMuon();
                }
                catch (SqlException ex) when (ex.Number == 547)
                {
                    MessageBox.Show("Không thể xóa phiếu này vì đang chứa sách mượn bên trong!\nVui lòng vào 'Xem/Nhập chi tiết' để xóa sách trước.", "Lỗi khóa ngoại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex) { MessageBox.Show("Lỗi xóa phiếu: " + ex.Message); }
            }
        }

        private void guna2Button5_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn hủy thao tác và làm mới phần nhập liệu?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                XoaNhap();
            }
        }

        private void guna2Button1_Click_1(object sender, EventArgs e)
        {
            LoadDanhSachPhieuMuon(txtTimKiem.Text);
        }

        private void guna2Button3_Click_1(object sender, EventArgs e)
        {
            txtTimKiem.Clear();
            LoadDanhSachPhieuMuon();
        }

        private void dgvPhieuMuon_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            dgvPhieuMuon.Rows[e.RowIndex].Cells["colSTT"].Value = (e.RowIndex + 1).ToString();
        }
    }
}
