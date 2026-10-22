using QLTV.GUI.UC_TienPhat;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace QLTV.GUI.UC_MuonTra
{
    public partial class ucChiTietPhieuTra : UserControl
    {
        private readonly string connString =@"Data Source=.\SQLEXPRESS;Initial Catalog=QLThuVien;Integrated Security=True";
        private string maPhieuTra, maPhieuMuon, tenDocGia, tenNV;
        private DateTime ngayTra;
        private DataTable dtChiTietTam;
        #region HÀM KHỞI TẠO
        public ucChiTietPhieuTra()
        {
            InitializeComponent();
        }
        public ucChiTietPhieuTra(string _maPT, string _maPM, string _tenDG, string _tenNV, DateTime _ngayTra)
        {
            InitializeComponent();
            maPhieuTra = _maPT;
            maPhieuMuon = _maPM;
            tenDocGia = _tenDG;
            tenNV = _tenNV;
            ngayTra = _ngayTra;
        }
        #endregion

        #region SỰ KIỆN LOAD FORM

        private void ucChiTietPhieuTra_Load(object sender, EventArgs e)
        {
            txtMaPhieuTra.Text = maPhieuTra;
            txtMaPhieuMuon.Text = maPhieuMuon;
            txtTenDocGia.Text = tenDocGia;
            txtTenNV.Text = tenNV;
            DateTime ngayTraHienThi = ngayTra == DateTime.MinValue
                ? DateTime.Today
                : ngayTra;
            if (ngayTraHienThi < dtpNgayTra.MinDate ||
               ngayTraHienThi > dtpNgayTra.MaxDate)
            {
                ngayTraHienThi = DateTime.Today;
            }
            dtpNgayTra.Value = ngayTraHienThi;
            dtpHanTra.Enabled = false;
            dtpNgayTra.Enabled = false;
            nudSoNgayTre.Minimum = 0;
            nudSoNgayTre.Maximum = 36500;

            nudTienPhatTre.Minimum = 0;
            nudTienPhatTre.Maximum = 1000000000;

            nudTienPhatTinhTrang.Minimum = 0;
            nudTienPhatTinhTrang.Maximum = 1000000000;

            txtMaPhieuTra.ReadOnly = true;
            txtMaPhieuMuon.ReadOnly = true;
            txtTenDocGia.ReadOnly = true;
            txtTenNV.ReadOnly = true;
            dtpNgayTra.Enabled = false;
            cboTinhTrang.Items.Clear();
            cboTinhTrang.Items.Add("Tốt");
            cboTinhTrang.Items.Add("Hư hỏng");
            cboTinhTrang.Items.Add("Mất");
            cboTinhTrang.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTinhTrang.SelectedIndex = 0;

            // Gắn sự kiện để tự động tính lại tiền phạt
            cboTinhTrang.SelectedIndexChanged += (s, ev) => TinhTienPhat();
            nudSoLuong.ValueChanged += (s, ev) => TinhTienPhat();
            cboSach.SelectedIndexChanged += (s, ev) => TinhTienPhat();
            dtpNgayTra.ValueChanged += (s, ev) => TinhTienPhat();

            LoadNgayTraTuSQL();

            LoadComboBoxSach(maPhieuMuon);
            KhoiTaoBangTam();
            LoadSachDaTraCu();

            TinhTienPhat();
            TinhTongTienPhatPhieu();
        }   
        private void LoadComboBoxSach(string maPM)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    // Truy vấn cực xịn: Lấy riêng những sách có trong ChiTietPhieuMuon
                    string query = @"
                        SELECT ct.MaSach, s.TenSach, ct.HanTra 
                        FROM dbo.chitietphieumuon ct
                        JOIN dbo.sach s ON ct.MaSach = s.MaSach
                        WHERE ct.MaPhieuMuon = @MaPM";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaPM", int.Parse(maPM));
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dtSach = new DataTable();
                            da.Fill(dtSach);
                            cboSach.DataSource = dtSach;
                            cboSach.DisplayMember = "TenSach";
                            cboSach.ValueMember = "MaSach";
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải sách mượn: " + ex.Message); }
        }
        private void KhoiTaoBangTam()
        {
            dtChiTietTam = new DataTable();
            dtChiTietTam.Columns.Add("MaSach", typeof(int));
            dtChiTietTam.Columns.Add("TenSach", typeof(string));
            dtChiTietTam.Columns.Add("SoLuongTra", typeof(int));
            dtChiTietTam.Columns.Add("TinhTrangSach", typeof(string));
            dtChiTietTam.Columns.Add("TienPhat", typeof(decimal));

            dgvChiTietPhieuTra.AutoGenerateColumns = false;
            dgvChiTietPhieuTra.AllowUserToAddRows = false;
            dgvChiTietPhieuTra.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            if (dgvChiTietPhieuTra.Columns.Contains("colMaSach")) dgvChiTietPhieuTra.Columns["colMaSach"].DataPropertyName = "MaSach";
            if (dgvChiTietPhieuTra.Columns.Contains("colTenSach")) dgvChiTietPhieuTra.Columns["colTenSach"].DataPropertyName = "TenSach";
            if (dgvChiTietPhieuTra.Columns.Contains("colSoLuong")) dgvChiTietPhieuTra.Columns["colSoLuong"].DataPropertyName = "SoLuongTra";
            if (dgvChiTietPhieuTra.Columns.Contains("colTinhTrang")) dgvChiTietPhieuTra.Columns["colTinhTrang"].DataPropertyName = "TinhTrangSach";
            if (dgvChiTietPhieuTra.Columns.Contains("colTienPhat"))
            {
                dgvChiTietPhieuTra.Columns["colTienPhat"].DataPropertyName = "TienPhat";
                dgvChiTietPhieuTra.Columns["colTienPhat"].DefaultCellStyle.Format = "N0";
            }

            dgvChiTietPhieuTra.DataSource = dtChiTietTam;
        }
        private void LoadSachDaTraCu()
        {
            if (string.IsNullOrEmpty(maPhieuTra)) return;
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                        SELECT ct.MaSach, s.TenSach, ct.SoLuongTra, ct.TinhTrangSach, ct.TienPhat
                        FROM dbo.chitietphieutra ct
                        JOIN dbo.sach s ON ct.MaSach = s.MaSach
                        WHERE ct.MaPhieuTra = @MaPT";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaPT", int.Parse(maPhieuTra));
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) da.Fill(dtChiTietTam);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải chi tiết cũ: " + ex.Message); }
        }
        private void LoadNgayTraTuSQL()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"SELECT NgayTra
                       FROM dbo.phieutra
                       WHERE MaPhieuTra = @MaPT";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaPT", int.Parse(maPhieuTra));

                    conn.Open();
                    dtpNgayTra.Value = Convert.ToDateTime(cmd.ExecuteScalar());
                }
            }
        }
        #endregion

        #region BỘ HÀM TÍNH TIỀN PHẠT
        private void TinhTienPhat()
        {
            DataRowView sachDangChon = cboSach.SelectedItem as DataRowView;

            if (sachDangChon == null)
                return;

            try
            {
                DateTime hanTra =
                    Convert.ToDateTime(sachDangChon["HanTra"]).Date;

                DateTime ngayTraThucTe = dtpNgayTra.Value.Date;
                int soLuong = Convert.ToInt32(nudSoLuong.Value);

                // Hiển thị hạn trả
                if (hanTra >= dtpHanTra.MinDate &&
                    hanTra <= dtpHanTra.MaxDate)
                {
                    dtpHanTra.Value = hanTra;
                }

                // 1. Tính số ngày trễ
                int soNgayTre = Math.Max(
                    0, (ngayTraThucTe - hanTra).Days);

                // 2. Tiền phạt trễ hạn = số ngày trễ * 5.000
                decimal tienPhatTreHan = soNgayTre * 5000m;

                // 3. Mức phạt theo tình trạng của một cuốn sách
                decimal mucPhatTinhTrang = 0;

                if (cboTinhTrang.SelectedItem != null)
                {
                    switch (cboTinhTrang.SelectedItem.ToString())
                    {
                        case "Tốt":
                            mucPhatTinhTrang = 0;
                            break;

                        case "Hư hỏng":
                            mucPhatTinhTrang = 100000;
                            break;

                        case "Mất":
                            mucPhatTinhTrang = 200000;
                            break;
                    }
                }

                // 4. Tiền phạt tình trạng = số lượng * mức phạt
                decimal tienPhatTinhTrang =
                    soLuong * mucPhatTinhTrang;

                // 5. Hiển thị các khoản phạt
                DatGiaTriNumeric(nudSoNgayTre, soNgayTre);
                DatGiaTriNumeric(nudTienPhatTre, tienPhatTreHan);
                DatGiaTriNumeric(nudTienPhatTinhTrang, tienPhatTinhTrang);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tính tiền phạt: " + ex.Message);
            }
        }
        private void DatGiaTriNumeric(Guna.UI2.WinForms.Guna2NumericUpDown control,decimal value)
        {
            if (control == null) return;

            if (value < control.Minimum)
                value = control.Minimum;

            if (value > control.Maximum)
                value = control.Maximum;

            control.Value = value;
        }
        private void TinhTongTienPhatPhieu()
        {
            decimal tienPhatTongCong = 0;
            if (dtChiTietTam != null && dtChiTietTam.Rows.Count > 0)
            {
                foreach (DataRow row in dtChiTietTam.Rows)
                {
                    if (row["TienPhat"] != DBNull.Value)
                    {
                        tienPhatTongCong += Convert.ToDecimal(row["TienPhat"]);
                    }
                }
            }

            if (lblTongTienPhat != null)
            {
                lblTongTienPhat.Text = tienPhatTongCong.ToString("N0") + " VNĐ";
            }
        }
        #endregion

        #region THÊM, XÓA, LƯU, QUAY LẠI
        private void guna2Button9_Click(object sender, EventArgs e)
        {
            if (cboSach.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn sách cần trả!", "Thông báo");
                return;
            }

            int maSach = Convert.ToInt32(cboSach.SelectedValue);
            string tenSach = cboSach.Text;
            int soLuong = Convert.ToInt32(nudSoLuong.Value);
            string tinhTrang = cboTinhTrang.SelectedItem.ToString();


            if (soLuong <= 0)
            {
                MessageBox.Show("Số lượng trả phải lớn hơn 0!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            TinhTienPhat();

            decimal tienPhatTinhTrang = nudTienPhatTinhTrang.Value;
            decimal tienPhatTreHan = nudTienPhatTre.Value;
            decimal tienPhatTongCong = tienPhatTinhTrang + tienPhatTreHan;

            // Xóa dòng cũ nếu thêm lại cuốn sách đó để tránh trùng khóa chính (MaPhieuTra, MaSach)
            for (int i = dtChiTietTam.Rows.Count - 1; i >= 0; i--)
            {
                if (Convert.ToInt32(dtChiTietTam.Rows[i]["MaSach"]) == maSach)
                    dtChiTietTam.Rows.RemoveAt(i);
            }
            dtChiTietTam.Rows.Add(maSach, tenSach, soLuong, tinhTrang, tienPhatTongCong);

            // Cập nhật tổng tiền phạt của toàn bộ các dòng.
            TinhTongTienPhatPhieu();
        }

        private void dgvChiTietPhieuTra_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvChiTietPhieuTra.Columns[e.ColumnIndex].Name == "colXoa")
            {
                dtChiTietTam.Rows.RemoveAt(e.RowIndex);
                dgvChiTietPhieuTra.Refresh();
                TinhTongTienPhatPhieu();
            }
        }


        private void guna2Button6_Click_1(object sender, EventArgs e)
        {
            if (dtChiTietTam.Rows.Count == 0)
            {
                MessageBox.Show("Vui lòng thêm ít nhất 1 cuốn sách vào danh sách trả!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. Reset dữ liệu cũ
                            using (SqlCommand cmdDel = new SqlCommand("DELETE FROM dbo.chitietphieutra WHERE MaPhieuTra = @MaPT", conn, trans))
                            {
                                cmdDel.Parameters.AddWithValue("@MaPT", int.Parse(maPhieuTra));
                                cmdDel.ExecuteNonQuery();
                            }

                            // 2. Chèn danh sách sách trả mới vào CSDL
                            string sqlIns = @"INSERT INTO dbo.chitietphieutra (MaPhieuTra, MaSach, SoLuongTra, TinhTrangSach, TienPhat) 
                                              VALUES (@MaPT, @MaSach, @SoLuong, @TinhTrang, @TienPhat)";
                            using (SqlCommand cmdIns = new SqlCommand(sqlIns, conn, trans))
                            {
                                cmdIns.Parameters.Add("@MaPT", SqlDbType.Int).Value = int.Parse(maPhieuTra);
                                cmdIns.Parameters.Add("@MaSach", SqlDbType.Int);
                                cmdIns.Parameters.Add("@SoLuong", SqlDbType.Int);
                                cmdIns.Parameters.Add("@TinhTrang", SqlDbType.NVarChar, 50);
                                cmdIns.Parameters.Add("@TienPhat", SqlDbType.Decimal);

                                foreach (DataRow row in dtChiTietTam.Rows)
                                {
                                    cmdIns.Parameters["@MaSach"].Value = Convert.ToInt32(row["MaSach"]);
                                    cmdIns.Parameters["@SoLuong"].Value = Convert.ToInt32(row["SoLuongTra"]);
                                    cmdIns.Parameters["@TinhTrang"].Value = row["TinhTrangSach"].ToString();
                                    cmdIns.Parameters["@TienPhat"].Value = Convert.ToDecimal(row["TienPhat"]);
                                    cmdIns.ExecuteNonQuery();
                                }
                            }
                            trans.Commit();
                            MessageBox.Show("Lưu chi tiết phiếu trả thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception)
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi CSDL khi lưu chi tiết trả: " + ex.Message); }
        }
        private void guna2Button5_Click(object sender, EventArgs e)
        {
            FormTrangChu f = this.FindForm() as FormTrangChu;
            if (f != null) f.HienThiUserControl(new ucPhieuTra());
        }
        #endregion
    }
}
