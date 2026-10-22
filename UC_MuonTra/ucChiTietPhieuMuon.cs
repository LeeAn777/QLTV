using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QLTV.GUI.UC_MuonTra
{
    public partial class ucChiTietPhieuMuon : UserControl
    {
        private readonly string connString =
            @"Data Source=DESKTOP-PDF3KTE\MSSQLSERVER03;Initial Catalog=QLThuVien;Integrated Security=True";
        private string maPhieuMuon = "";
        private string maSachGoc = "";
        private DateTime ngayMuonGoc;
        private bool dangTai = false;
        private bool coTheSua = false;

        // Constructor khong tham so cho WinForms Designer.
        public ucChiTietPhieuMuon() : this("") { }

        public ucChiTietPhieuMuon(string maPhieu)
        {
            InitializeComponent();
            maPhieuMuon = (maPhieu ?? "").Trim();

            // Designer da gan Load va Quay lai. Khong gan lai hai su kien do.
            guna2Button9.Click -= guna2Button9_Click;
            guna2Button9.Click += guna2Button9_Click; // Chon sach
            guna2Button6.Click -= guna2Button6_Click;
            guna2Button6.Click += guna2Button6_Click; // Luu thay doi
            dgvChiTietPhieuMuon.CellClick -= dgvChiTietPhieuMuon_CellClick;
            dgvChiTietPhieuMuon.CellClick += dgvChiTietPhieuMuon_CellClick;
        }

        private void ucChiTietPhieuMuon_Load(object sender, EventArgs e)
        {
            dgvChiTietPhieuMuon.AutoGenerateColumns = false;
            dgvChiTietPhieuMuon.AllowUserToAddRows = false;
            dgvChiTietPhieuMuon.ReadOnly = true;
            dgvChiTietPhieuMuon.MultiSelect = false;
            dgvChiTietPhieuMuon.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvChiTietPhieuMuon.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);

            colSTT.DataPropertyName = "STT";
            colMaSach.DataPropertyName = "MaSach";
            colTenSach.DataPropertyName = "TenSach";
            colSoLuong.DataPropertyName = "SoLuong";
            colHanTra.DataPropertyName = "HanTra";
            colXoa.Visible = false; // DB hien tai yeu cau phieumuon.masach NOT NULL.

            guna2TextBox1.ReadOnly = true; // Ma phieu
            guna2TextBox3.ReadOnly = true; // Ten doc gia
            guna2TextBox2.ReadOnly = true; // Ten nhan vien
            guna2TextBox10.ReadOnly = true; // Chon qua nut, tranh nhap sai ma
                                            // Cho phép thay đổi số lượng sách
            guna2NumericUpDown1.Enabled = true;
            guna2NumericUpDown1.Minimum = 1;
            guna2NumericUpDown1.Maximum = 100;
            guna2NumericUpDown1.Increment = 1;
            guna2NumericUpDown1.Value = 1;
            guna2NumericUpDown1.Value = 1;// SQL khong co cot so luong muon

            guna2Button9.Text = "Chọn sách";
            guna2Button6.Text = "Lưu thay đổi";
            guna2DateTimePicker1.Enabled = true;
            guna2DateTimePicker1.Format = DateTimePickerFormat.Custom;
            guna2DateTimePicker1.CustomFormat = "dd/MM/yyyy";

            if (string.IsNullOrWhiteSpace(maPhieuMuon))
            {
                MessageBox.Show("Hãy chọn phiếu mượn từ danh sách trước khi mở chi tiết.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                BatTatSua(false);
                return;
            }
            TaiDuLieu();
        }

        private void BatTatSua(bool choPhep)
        {
            coTheSua = choPhep;
            guna2DateTimePicker1.Enabled = choPhep;
            guna2Button9.Enabled = choPhep;
            guna2Button6.Enabled = choPhep;
        }

        private void TaiDuLieu()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(connString))
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT pm.maphieu AS MaPhieu, pm.masach AS MaSach,
                           s.tensach AS TenSach, dg.hoten AS TenDocGia,
                           nv.hoten AS TenNhanVien, pm.ngaymuon AS NgayMuon,
                           pm.ngaytra AS NgayTra, pm.trangthai AS TrangThai
                    FROM dbo.phieumuon AS pm
                    LEFT JOIN dbo.sach AS s ON s.masach = pm.masach
                    LEFT JOIN dbo.docgia AS dg ON dg.madocgia = pm.madocgia
                    LEFT JOIN dbo.nhanvien AS nv ON nv.manv = pm.manv
                    WHERE pm.maphieu = @MaPhieu", cn))
                {
                    cmd.Parameters.Add("@MaPhieu", SqlDbType.VarChar, 100).Value = maPhieuMuon;
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd)) da.Fill(dt);
                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("Không tìm thấy phiếu mượn " + maPhieuMuon + ".");
                        dgvChiTietPhieuMuon.DataSource = null;
                        BatTatSua(false);
                        return;
                    }

                    DataRow r = dt.Rows[0];
                    dangTai = true;
                    try
                    {
                        maSachGoc = Convert.ToString(r["MaSach"]);
                        ngayMuonGoc = Convert.ToDateTime(r["NgayMuon"]);
                        guna2TextBox1.Text = Convert.ToString(r["MaPhieu"]);
                        guna2TextBox3.Text = Convert.ToString(r["TenDocGia"]);
                        guna2TextBox2.Text = Convert.ToString(r["TenNhanVien"]);
                        guna2TextBox10.Text = maSachGoc;
                        guna2DateTimePicker1.Value = ngayMuonGoc;
                        guna2NumericUpDown1.Value = 1;

                        DataTable bang = new DataTable();
                        bang.Columns.Add("STT", typeof(int));
                        bang.Columns.Add("MaSach", typeof(string));
                        bang.Columns.Add("TenSach", typeof(string));
                        bang.Columns.Add("SoLuong", typeof(int));
                        bang.Columns.Add("HanTra", typeof(string));
                        bang.Rows.Add(1, maSachGoc, Convert.ToString(r["TenSach"]),
                            1, "Chưa lưu trong SQL");
                        dgvChiTietPhieuMuon.DataSource = bang;
                        dgvChiTietPhieuMuon.ClearSelection();
                        dgvChiTietPhieuMuon.CurrentCell = null;

                        bool daTra = r["NgayTra"] != DBNull.Value ||
                            string.Equals(Convert.ToString(r["TrangThai"]).Trim(),
                                "Đã trả", StringComparison.OrdinalIgnoreCase);
                        BatTatSua(!daTra);
                    }
                    finally { dangTai = false; }
                }
            }
            catch (Exception ex)
            {
                BatTatSua(false);
                MessageBox.Show("Không tải được chi tiết phiếu mượn: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvChiTietPhieuMuon_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dangTai || e.RowIndex < 0) return;
            DataRowView r = dgvChiTietPhieuMuon.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (r != null) guna2TextBox10.Text = Convert.ToString(r["MaSach"]);
        }

        private void guna2Button9_Click(object sender, EventArgs e)
        {
            if (!coTheSua) return;
            try
            {
                DataTable ds = new DataTable();
                using (SqlConnection cn = new SqlConnection(connString))
                using (SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT masach AS MaSach, tensach AS TenSach FROM dbo.sach ORDER BY tensach", cn))
                    da.Fill(ds);

                if (ds.Rows.Count == 0)
                {
                    MessageBox.Show("Danh mục sách chưa có dữ liệu.");
                    return;
                }

                using (Form f = new Form())
                {
                    f.Text = "Chọn sách thay cho sách hiện tại";
                    f.StartPosition = FormStartPosition.CenterParent;
                    f.FormBorderStyle = FormBorderStyle.FixedDialog;
                    f.ClientSize = new Size(520, 150);
                    f.MaximizeBox = false;
                    f.MinimizeBox = false;

                    ComboBox cbo = new ComboBox();
                    cbo.Location = new Point(20, 20);
                    cbo.Width = 480;
                    cbo.DropDownStyle = ComboBoxStyle.DropDownList;
                    cbo.DisplayMember = "HienThi";
                    cbo.ValueMember = "MaSach";
                    ds.Columns.Add("HienThi", typeof(string));
                    foreach (DataRow row in ds.Rows)
                        row["HienThi"] = Convert.ToString(row["MaSach"]) + " - " +
                            Convert.ToString(row["TenSach"]);
                    cbo.DataSource = ds;
                    cbo.SelectedValue = maSachGoc;

                    Button chon = new Button();
                    chon.Text = "Chọn";
                    chon.Location = new Point(287, 84);
                    chon.Width = 100;
                    chon.DialogResult = DialogResult.OK;
                    Button huy = new Button();
                    huy.Text = "Hủy";
                    huy.Location = new Point(398, 84);
                    huy.Width = 100;
                    huy.DialogResult = DialogResult.Cancel;
                    f.Controls.AddRange(new Control[] { cbo, chon, huy });
                    f.AcceptButton = chon;
                    f.CancelButton = huy;
                    if (f.ShowDialog(this) == DialogResult.OK && cbo.SelectedValue != null)
                        guna2TextBox10.Text = Convert.ToString(cbo.SelectedValue);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi chọn sách: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            if (!coTheSua) return;
            string sachMoi = guna2TextBox10.Text.Trim();
            DateTime ngayMoi = guna2DateTimePicker1.Value.Date;
            if (sachMoi.Length == 0 || sachMoi.Length > 100)
            {
                MessageBox.Show("Bạn cần chọn mã sách hợp lệ.");
                return;
            }
            if (ngayMoi > DateTime.Today)
            {
                MessageBox.Show("Ngày mượn không được lớn hơn ngày hiện tại.");
                return;
            }
            if (sachMoi == maSachGoc && ngayMoi == ngayMuonGoc.Date)
            {
                MessageBox.Show("Bạn chưa thay đổi sách hoặc ngày lập.");
                return;
            }
            if (MessageBox.Show("Lưu thay đổi cho phiếu " + maPhieuMuon + "?\n" +
                "Mã sách: " + maSachGoc + " → " + sachMoi + "\n" +
                "Ngày mượn: " + ngayMuonGoc.ToString("dd/MM/yyyy") + " → " +
                ngayMoi.ToString("dd/MM/yyyy"), "Xác nhận cập nhật",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                using (SqlConnection cn = new SqlConnection(connString))
                {
                    cn.Open();
                    using (SqlTransaction tx = cn.BeginTransaction())
                    {
                        try
                        {
                            // Kiem tra phieu va khoa dong, de tranh sua khi phieu da tra.
                            using (SqlCommand check = new SqlCommand(@"
                                SELECT masach, ngaymuon, ngaytra, trangthai
                                FROM dbo.phieumuon WITH (UPDLOCK, HOLDLOCK)
                                WHERE maphieu = @MaPhieu", cn, tx))
                            {
                                check.Parameters.Add("@MaPhieu", SqlDbType.VarChar, 100).Value = maPhieuMuon;
                                using (SqlDataReader rd = check.ExecuteReader())
                                {
                                    if (!rd.Read())
                                        throw new InvalidOperationException("Phiếu mượn không còn tồn tại.");
                                    if (rd["ngaytra"] != DBNull.Value ||
                                        string.Equals(Convert.ToString(rd["trangthai"]).Trim(),
                                            "Đã trả", StringComparison.OrdinalIgnoreCase))
                                        throw new InvalidOperationException("Phiếu đã trả, không được chỉnh sửa.");
                                    if (Convert.ToString(rd["masach"]) != maSachGoc ||
                                        Convert.ToDateTime(rd["ngaymuon"]).Date != ngayMuonGoc.Date)
                                        throw new InvalidOperationException("Phiếu đã thay đổi ở nơi khác. Hãy tải lại.");
                                }
                            }
                            using (SqlCommand checkSach = new SqlCommand(
                                "SELECT COUNT(*) FROM dbo.sach WHERE masach = @MaSach", cn, tx))
                            {
                                checkSach.Parameters.Add("@MaSach", SqlDbType.VarChar, 100).Value = sachMoi;
                                if (Convert.ToInt32(checkSach.ExecuteScalar()) == 0)
                                    throw new InvalidOperationException("Sách vừa chọn không tồn tại trong SQL.");
                            }
                            using (SqlCommand update = new SqlCommand(@"
                                UPDATE dbo.phieumuon SET masach = @MaSach, ngaymuon = @NgayMuon
                                WHERE maphieu = @MaPhieu AND ngaytra IS NULL", cn, tx))
                            {
                                update.Parameters.Add("@MaSach", SqlDbType.VarChar, 100).Value = sachMoi;
                                update.Parameters.Add("@NgayMuon", SqlDbType.Date).Value = ngayMoi;
                                update.Parameters.Add("@MaPhieu", SqlDbType.VarChar, 100).Value = maPhieuMuon;
                                if (update.ExecuteNonQuery() != 1)
                                    throw new InvalidOperationException("Không thể cập nhật phiếu mượn.");
                            }
                            tx.Commit();
                        }
                        catch { tx.Rollback(); throw; }
                    }
                }
                MessageBox.Show("Đã lưu thay đổi phiếu mượn thành công!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                TaiDuLieu();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không lưu được thay đổi: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                TaiDuLieu();
            }
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            FormTrangChu trangChu = this.FindForm() as FormTrangChu;
            if (trangChu != null)
                trangChu.HienThiUserControl(new ucPhieuMuon());
            else
                MessageBox.Show("Không tìm thấy FormTrangChu để quay lại.");
        }
    }
}
