using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QLTV.GUI.UC_MuonTra
{
    public partial class ucPhieuMuon : UserControl
    {
        private readonly string connString =
            @"Data Source=DESKTOP-PDF3KTE\MSSQLSERVER03;Initial Catalog=QLThuVien;Integrated Security=True";

        private string maPhieuDangChon = "";
        private bool dangTai = false;

        public ucPhieuMuon()
        {
            InitializeComponent();
            // Designer da dang ky nut xem chi tiet; go bo truoc khi gan.
            Load -= ucPhieuMuon_Load;
            Load += ucPhieuMuon_Load;
            guna2Button6.Click -= guna2Button6_Click;
            guna2Button6.Click += guna2Button6_Click;
            guna2Button4.Click += guna2Button4_Click; // Tao
            guna2Button2.Click += guna2Button2_Click; // Sua
            guna2Button7.Click += guna2Button7_Click; // Xoa
            guna2Button1.Click += guna2Button1_Click; // Tim
            guna2Button3.Click += guna2Button3_Click; // Lam moi
            guna2Button5.Click += guna2Button5_Click; // Huy
            dgvPhieuMuon.CellClick += dgvPhieuMuon_CellClick;
            guna2TextBox1.Leave += (s, e) => HienThiTen(true);
            guna2TextBox3.Leave += (s, e) => HienThiTen(false);
        }

        private void ucPhieuMuon_Load(object sender, EventArgs e)
        {
            dgvPhieuMuon.AutoGenerateColumns = false;
            dgvPhieuMuon.AllowUserToAddRows = false;
            dgvPhieuMuon.ReadOnly = true;
            dgvPhieuMuon.MultiSelect = false;
            dgvPhieuMuon.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPhieuMuon.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            colSTT.DataPropertyName = "STT";
            colMaPhieuMuon.DataPropertyName = "MaPhieuMuon";
            colMaDG.DataPropertyName = "MaDocGia";
            colTenDG.DataPropertyName = "TenDocGia";
            colMaNV.DataPropertyName = "MaNV";
            colTenNV.DataPropertyName = "TenNhanVien";
            colNgayMuon.DataPropertyName = "NgayMuon";
            colNgayMuon.DefaultCellStyle.Format = "dd/MM/yyyy";

            // SQL that su: maphieu VARCHAR(100), nguoi dung phai nhap ma.
            guna2TextBox4.ReadOnly = false;
            guna2TextBox5.ReadOnly = true;
            guna2TextBox2.ReadOnly = true;
            XoaNhap();
            LoadDanhSachPhieuMuon();
        }

        private void LoadDanhSachPhieuMuon(string keyword = "")
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT ROW_NUMBER() OVER(ORDER BY pm.maphieu DESC) AS STT,
                           pm.maphieu AS MaPhieuMuon,
                           pm.madocgia AS MaDocGia,
                           pm.manv AS MaNV,
                           pm.masach AS MaSach,
                           pm.ngaymuon AS NgayMuon,
                           pm.ngaytra AS NgayTra,
                           pm.trangthai AS TrangThai
                    FROM dbo.phieumuon pm
                    WHERE @Key = N''
                          OR pm.maphieu LIKE N'%' + @Key + N'%'
                          OR pm.madocgia LIKE N'%' + @Key + N'%'
                    ORDER BY pm.maphieu DESC", conn))
                {
                    cmd.Parameters.Add("@Key", SqlDbType.NVarChar, 100).Value = keyword.Trim();
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd)) da.Fill(dt);

                    // Ten lay tu bang danh muc, khong can JOIN khoa ngoai khac kieu.
                    dt.Columns.Add("TenDocGia", typeof(string));
                    dt.Columns.Add("TenNhanVien", typeof(string));
                    foreach (DataRow row in dt.Rows)
                    {
                        row["TenDocGia"] = LayTen("docgia", Convert.ToString(row["MaDocGia"]));
                        row["TenNhanVien"] = LayTen("nhanvien", Convert.ToString(row["MaNV"]));
                    }
                    dangTai = true;
                    dgvPhieuMuon.DataSource = dt;
                    dgvPhieuMuon.ClearSelection();
                    dgvPhieuMuon.CurrentCell = null;
                    dangTai = false;
                }
            }
            catch (Exception ex)
            {
                dangTai = false;
                MessageBox.Show("Loi tai danh sach phieu muon: " + ex.Message,
                    "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Lay ten neu bang danh muc co cot ho ten va cot ma tuong ung.
        private string LayTen(string bang, string ma)
        {
            if (string.IsNullOrWhiteSpace(ma)) return "";
            string tenBang, tenCot;
            if (bang == "docgia") { tenBang = "dbo.docgia"; tenCot = "MaDocGia"; }
            else if (bang == "nhanvien") { tenBang = "dbo.nhanvien"; tenCot = "MaNV"; }
            else return "";

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT TOP 1 HoTen FROM " + tenBang +
                    " WHERE CONVERT(NVARCHAR(100), [" + tenCot + "]) = @Ma", conn))
                {
                    cmd.Parameters.Add("@Ma", SqlDbType.NVarChar, 100).Value = ma;
                    conn.Open();
                    object value = cmd.ExecuteScalar();
                    return value == null || value == DBNull.Value ? "" : Convert.ToString(value);
                }
            }
            catch (SqlException)
            {
                // Danh muc tren may co the dung ten cot khac.
                return "";
            }
        }

        private void HienThiTen(bool docGia)
        {
            if (dangTai) return;
            if (docGia)
                guna2TextBox5.Text = LayTen("docgia", guna2TextBox1.Text.Trim());
            else
                guna2TextBox2.Text = LayTen("nhanvien", guna2TextBox3.Text.Trim());
        }

        private void XoaNhap()
        {
            maPhieuDangChon = "";
            guna2TextBox4.ReadOnly = false;
            guna2TextBox4.Clear();
            guna2TextBox1.Clear();
            guna2TextBox3.Clear();
            guna2TextBox5.Clear();
            guna2TextBox2.Clear();
            guna2DateTimePicker1.Value = DateTime.Today;
            dgvPhieuMuon.ClearSelection();
            if (dgvPhieuMuon.Rows.Count > 0) dgvPhieuMuon.CurrentCell = null;
        }

        private bool HopLe()
        {
            if (string.IsNullOrWhiteSpace(guna2TextBox4.Text) ||
                string.IsNullOrWhiteSpace(guna2TextBox1.Text) ||
                string.IsNullOrWhiteSpace(guna2TextBox3.Text))
            {
                MessageBox.Show("Hay nhap ma phieu, ma doc gia va ma nhan vien!");
                return false;
            }
            if (guna2TextBox4.Text.Trim().Length > 100 ||
                guna2TextBox1.Text.Trim().Length > 100 ||
                guna2TextBox3.Text.Trim().Length > 100)
            {
                MessageBox.Show("Ma khong duoc vuot qua 100 ky tu!");
                return false;
            }
            HienThiTen(true);
            HienThiTen(false);
            // Van cho phep ma khong tim thay ten neu bang danh muc dung cot khac.
            return true;
        }

        // Bang phieumuon THUC TE co masach NOT NULL, nen phai chon sach.
        private string ChonMaSach()
        {
            DataTable ds = new DataTable();
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlDataAdapter da = new SqlDataAdapter(
                "SELECT masach AS MaSach, TenSach FROM dbo.sach ORDER BY TenSach", conn))
                da.Fill(ds);
            if (ds.Rows.Count == 0)
            {
                MessageBox.Show("Chua co sach trong danh muc!");
                return null;
            }
            using (Form popup = new Form())
            {
                popup.Text = "Chon sach cho phieu muon";
                popup.StartPosition = FormStartPosition.CenterParent;
                popup.FormBorderStyle = FormBorderStyle.FixedDialog;
                popup.ClientSize = new Size(460, 155);
                popup.MaximizeBox = false;
                popup.MinimizeBox = false;
                Label label = new Label { Text = "Sach can muon:", Location = new Point(18, 18), AutoSize = true };
                ComboBox combo = new ComboBox
                {
                    Location = new Point(18, 42),
                    Width = 424,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    DataSource = ds,
                    DisplayMember = "TenSach",
                    ValueMember = "MaSach"
                };
                Button ok = new Button
                { Text = "Dong y", Location = new Point(238, 95), Width = 95, DialogResult = DialogResult.OK };
                Button cancel = new Button
                { Text = "Huy", Location = new Point(347, 95), Width = 95, DialogResult = DialogResult.Cancel };
                popup.Controls.AddRange(new Control[] { label, combo, ok, cancel });
                popup.AcceptButton = ok;
                popup.CancelButton = cancel;
                return popup.ShowDialog(this) == DialogResult.OK
                    ? Convert.ToString(combo.SelectedValue) : null;
            }
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            if (!HopLe()) return;
            try
            {
                string maSach = ChonMaSach();
                if (string.IsNullOrWhiteSpace(maSach)) return;

                using (SqlConnection conn = new SqlConnection(connString))
                using (SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO dbo.phieumuon
                        (maphieu, madocgia, masach, manv, ngaymuon, ngaytra, trangthai)
                    VALUES
                        (@MaPhieu, @MaDG, @MaSach, @MaNV, @NgayMuon, NULL, @TrangThai)", conn))
                {
                    cmd.Parameters.Add("@MaPhieu", SqlDbType.VarChar, 100).Value = guna2TextBox4.Text.Trim();
                    cmd.Parameters.Add("@MaDG", SqlDbType.VarChar, 100).Value = guna2TextBox1.Text.Trim();
                    cmd.Parameters.Add("@MaSach", SqlDbType.VarChar, 100).Value = maSach;
                    cmd.Parameters.Add("@MaNV", SqlDbType.VarChar, 100).Value = guna2TextBox3.Text.Trim();
                    cmd.Parameters.Add("@NgayMuon", SqlDbType.Date).Value = guna2DateTimePicker1.Value.Date;
                    cmd.Parameters.Add("@TrangThai", SqlDbType.NVarChar, 100).Value = "Đang mượn";
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Tao phieu muon thanh cong!");
                LoadDanhSachPhieuMuon();
                XoaNhap();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Khong tao duoc phieu: " + ex.Message +
                    "\nKiem tra ma phieu trung, ma sach va rang buoc trang thai trong SQL.");
            }
            catch (Exception ex) { MessageBox.Show("Loi: " + ex.Message); }
        }

        private void dgvPhieuMuon_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dangTai || e.RowIndex < 0) return;
            DataRowView row = dgvPhieuMuon.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (row == null) return;

            maPhieuDangChon = Convert.ToString(row["MaPhieuMuon"]);
            guna2TextBox4.Text = maPhieuDangChon;
            guna2TextBox4.ReadOnly = true; // Khong doi khoa chinh khi sua
            guna2TextBox1.Text = Convert.ToString(row["MaDocGia"]);
            guna2TextBox3.Text = Convert.ToString(row["MaNV"]);
            guna2TextBox5.Text = Convert.ToString(row["TenDocGia"]);
            guna2TextBox2.Text = Convert.ToString(row["TenNhanVien"]);
            if (row["NgayMuon"] != DBNull.Value)
                guna2DateTimePicker1.Value = Convert.ToDateTime(row["NgayMuon"]);
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(maPhieuDangChon))
            { MessageBox.Show("Hay chon phieu can sua!"); return; }
            if (!HopLe()) return;
            if (MessageBox.Show("Luu thong tin phieu muon?", "Xac nhan",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                using (SqlCommand cmd = new SqlCommand(@"
                    UPDATE dbo.phieumuon
                    SET madocgia = @MaDG, manv = @MaNV, ngaymuon = @NgayMuon
                    WHERE maphieu = @MaPhieu", conn))
                {
                    cmd.Parameters.Add("@MaPhieu", SqlDbType.VarChar, 100).Value = maPhieuDangChon;
                    cmd.Parameters.Add("@MaDG", SqlDbType.VarChar, 100).Value = guna2TextBox1.Text.Trim();
                    cmd.Parameters.Add("@MaNV", SqlDbType.VarChar, 100).Value = guna2TextBox3.Text.Trim();
                    cmd.Parameters.Add("@NgayMuon", SqlDbType.Date).Value = guna2DateTimePicker1.Value.Date;
                    conn.Open();
                    int n = cmd.ExecuteNonQuery();
                    MessageBox.Show(n > 0 ? "Cap nhat thanh cong!" : "Khong tim thay phieu!");
                }
                LoadDanhSachPhieuMuon();
                XoaNhap();
            }
            catch (Exception ex) { MessageBox.Show("Loi sua phieu: " + ex.Message); }
        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(maPhieuDangChon))
            { MessageBox.Show("Hay chon phieu can xoa!"); return; }
            if (MessageBox.Show("Xoa phieu " + maPhieuDangChon +
                "?\nPhieu co chi tiet/lich su lien quan se bi chan boi khoa ngoai.",
                "Xac nhan", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                using (SqlCommand cmd = new SqlCommand(
                    "DELETE FROM dbo.phieumuon WHERE maphieu = @MaPhieu", conn))
                {
                    cmd.Parameters.Add("@MaPhieu", SqlDbType.VarChar, 100).Value = maPhieuDangChon;
                    conn.Open();
                    int n = cmd.ExecuteNonQuery();
                    MessageBox.Show(n > 0 ? "Xoa phieu thanh cong!" : "Khong tim thay phieu!");
                }
                LoadDanhSachPhieuMuon();
                XoaNhap();
            }
            catch (SqlException ex) when (ex.Number == 547)
            { MessageBox.Show("Khong the xoa vi phieu dang co du lieu lien quan!"); }
            catch (Exception ex) { MessageBox.Show("Loi xoa phieu: " + ex.Message); }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            LoadDanhSachPhieuMuon(txtTimKiem.Text);
        }
        private void guna2Button3_Click(object sender, EventArgs e)
        {
            txtTimKiem.Clear();
            XoaNhap();
            LoadDanhSachPhieuMuon();
        }
        private void guna2Button5_Click(object sender, EventArgs e)
        {
            XoaNhap();
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(maPhieuDangChon))
            {
                MessageBox.Show(
                    "Vui lòng chọn một phiếu mượn để xem chi tiết!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            FormTrangChu trangChu = this.FindForm() as FormTrangChu;

            if (trangChu != null)
            {
                ucChiTietPhieuMuon chiTiet =
                    new ucChiTietPhieuMuon(maPhieuDangChon);

                trangChu.HienThiUserControl(chiTiet);
            }
        }

    }
}
