using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QLTV.GUI.UC_MuonTra
{
    public partial class ucChiTietPhieuMuon : UserControl
    {
        private readonly string connString = @"Data Source=.\SQLEXPRESS;Initial Catalog=QLThuVien;Integrated Security=True";
        private string maPhieu;
        private string tenDocGia;
        private string tenNV;
        private DateTime ngayMuon;
        private DataTable dtChiTietTam;
        #region HÀM KHỞI TẠO
        public ucChiTietPhieuMuon()
        {
            InitializeComponent();
        }

        public ucChiTietPhieuMuon(string _maPhieu, string _tenDG, string _tenNV, DateTime _ngayMuon)
        {
            InitializeComponent();
            maPhieu = _maPhieu;
            tenDocGia = _tenDG;
            tenNV = _tenNV;
            ngayMuon = _ngayMuon;
        }
        #endregion

        #region SỰ KIỆN LOAD FORM
        private void ucChiTietPhieuMuon_Load(object sender, EventArgs e)
        {
            cboSach.DropDownStyle = ComboBoxStyle.DropDown;
            txtMaPhieuMuon.Text = maPhieu;
            txtTenDocGia.Text = tenDocGia;
            txtTenNhanVien.Text = tenNV;
            dtpNgayLap.Value = ngayMuon;

            txtMaPhieuMuon.ReadOnly = true;
            txtTenDocGia.ReadOnly = true;
            txtTenNhanVien.ReadOnly = true;
            dtpNgayLap.Enabled = false; // Không cho sửa ngày lập bên này

            // 2. Load danh sách Sách vào ComboBox (kèm tự động tìm kiếm)
            LoadComboBoxSach();

            // 3. Khởi tạo cấu trúc bảng lưới và tải sách đã mượn (nếu đang sửa phiếu cũ)
            KhoiTaoBangTam();
            LoadSachCuDaMuon();

        }
        private void LoadComboBoxSach()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "SELECT MaSach, TenSach FROM dbo.sach";
                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        DataTable dtSach = new DataTable();
                        da.Fill(dtSach);

                        cboSach.DataSource = dtSach;
                        cboSach.DisplayMember = "TenSach";
                        cboSach.ValueMember = "MaSach";

                        // Bật tính năng gõ chữ -> Tự động tìm kiếm sách
                        cboSach.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                        cboSach.AutoCompleteSource = AutoCompleteSource.ListItems;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải danh sách sách: " + ex.Message); }
        }
        private void KhoiTaoBangTam()
        {
            dtChiTietTam = new DataTable();
            dtChiTietTam.Columns.Add("MaSach", typeof(int));
            dtChiTietTam.Columns.Add("TenSach", typeof(string));
            dtChiTietTam.Columns.Add("SoLuong", typeof(int));
            dtChiTietTam.Columns.Add("HanTra", typeof(DateTime));

            dgvChiTietPhieuMuon.AutoGenerateColumns = false;
            dgvChiTietPhieuMuon.AllowUserToAddRows = false;
            dgvChiTietPhieuMuon.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Map dữ liệu vào các cột của DataGridView
            if (dgvChiTietPhieuMuon.Columns.Contains("colMaSach")) dgvChiTietPhieuMuon.Columns["colMaSach"].DataPropertyName = "MaSach";
            if (dgvChiTietPhieuMuon.Columns.Contains("colTenSach")) dgvChiTietPhieuMuon.Columns["colTenSach"].DataPropertyName = "TenSach";
            if (dgvChiTietPhieuMuon.Columns.Contains("colSoLuong")) dgvChiTietPhieuMuon.Columns["colSoLuong"].DataPropertyName = "SoLuong";
            if (dgvChiTietPhieuMuon.Columns.Contains("colHanTra"))
            {
                dgvChiTietPhieuMuon.Columns["colHanTra"].DataPropertyName = "HanTra";
                dgvChiTietPhieuMuon.Columns["colHanTra"].DefaultCellStyle.Format = "dd/MM/yyyy";
            }

            dgvChiTietPhieuMuon.DataSource = dtChiTietTam;
        }
        private void LoadSachCuDaMuon()
        {
            if (string.IsNullOrEmpty(maPhieu)) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                        SELECT ct.MaSach, s.TenSach, ct.SoLuongMuon AS SoLuong, ct.HanTra
                        FROM dbo.chitietphieumuon ct
                        JOIN dbo.sach s ON ct.MaSach = s.MaSach
                        WHERE ct.MaPhieuMuon = @MaPhieu";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaPhieu", int.Parse(maPhieu));
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) da.Fill(dtChiTietTam);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải chi tiết mượn cũ: " + ex.Message); }
        }
        #endregion

        #region THÊM & XÓA SÁCH TRÊN LƯỚI
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (cboSach.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn một cuốn sách!", "Thông báo");
                return;
            }

            int maSach = Convert.ToInt32(cboSach.SelectedValue);
            string tenSach = cboSach.Text;
            int soLuong = Convert.ToInt32(nudSoLuong.Value);

            if (soLuong <= 0)
            {
                MessageBox.Show("Số lượng mượn phải lớn hơn 0!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra xem sách đã có trong lưới chưa -> Nếu có thì cộng dồn số lượng
            bool daTonTai = false;
            foreach (DataRow row in dtChiTietTam.Rows)
            {
                if (Convert.ToInt32(row["MaSach"]) == maSach)
                {
                    row["SoLuong"] = Convert.ToInt32(row["SoLuong"]) + soLuong;
                    daTonTai = true;
                    break;
                }
            }

            // Nếu chưa có thì thêm dòng mới vào bảng tạm
            if (!daTonTai)
            {
                DateTime hanTra = dtpHanTra.Value.Date;
                dtChiTietTam.Rows.Add(maSach, tenSach, soLuong, hanTra);
            }
        }
        private void dgvChiTietPhieuMuon_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra nếu người dùng click vào Cột có nút "Xóa"
            if (e.RowIndex >= 0 && dgvChiTietPhieuMuon.Columns[e.ColumnIndex].Name == "colXoa")
            {
                if (MessageBox.Show("Bạn có muốn bỏ cuốn sách này khỏi danh sách mượn?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    dtChiTietTam.Rows.RemoveAt(e.RowIndex);
                }
            }
        }
        #endregion

        #region LƯU DỮ LIỆU & QUAY LẠI

        private void btnLuuChiTiet_Click(object sender, EventArgs e)
        {
            if (dtChiTietTam.Rows.Count == 0)
            {
                MessageBox.Show("Vui lòng thêm ít nhất 1 cuốn sách vào danh sách trước khi lưu!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    // Bật Transaction để đảm bảo lưu dữ liệu an toàn 100%
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. Xóa toàn bộ dữ liệu chi tiết cũ của phiếu này (reset)
                            string sqlDel = "DELETE FROM dbo.chitietphieumuon WHERE MaPhieuMuon = @MaPhieu";
                            using (SqlCommand cmdDel = new SqlCommand(sqlDel, conn, trans))
                            {
                                cmdDel.Parameters.AddWithValue("@MaPhieu", int.Parse(maPhieu));
                                cmdDel.ExecuteNonQuery();
                            }

                            // 2. Chèn lại danh sách sách từ DataGridView vào CSDL
                            string sqlIns = @"
                                INSERT INTO dbo.chitietphieumuon (MaPhieuMuon, MaSach, SoLuongMuon, HanTra) 
                                VALUES (@MaPhieu, @MaSach, @SoLuong, @HanTra)";

                            using (SqlCommand cmdIns = new SqlCommand(sqlIns, conn, trans))
                            {
                                cmdIns.Parameters.Add("@MaPhieu", SqlDbType.Int).Value = int.Parse(maPhieu);
                                cmdIns.Parameters.Add("@MaSach", SqlDbType.Int);
                                cmdIns.Parameters.Add("@SoLuong", SqlDbType.Int);
                                cmdIns.Parameters.Add("@HanTra", SqlDbType.Date);

                                foreach (DataRow row in dtChiTietTam.Rows)
                                {
                                    cmdIns.Parameters["@MaSach"].Value = Convert.ToInt32(row["MaSach"]);
                                    cmdIns.Parameters["@SoLuong"].Value = Convert.ToInt32(row["SoLuong"]);
                                    cmdIns.Parameters["@HanTra"].Value = Convert.ToDateTime(row["HanTra"]);
                                    cmdIns.ExecuteNonQuery();
                                }
                            }

                            // Xác nhận lưu thành công
                            trans.Commit();
                            MessageBox.Show("Lưu chi tiết phiếu mượn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            // Nếu có lỗi giữa chừng -> Quay lại trạng thái ban đầu, không lưu gì cả
                            trans.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi CSDL khi lưu chi tiết: " + ex.Message); }
        }
        private void guna2Button5_Click(object sender, EventArgs e)
        {
            // Trở về UserControl quản lý phiếu mượn
            FormTrangChu f = this.FindForm() as FormTrangChu;
            if (f != null)
            {
                f.HienThiUserControl(new ucPhieuMuon());
            }
        }
        #endregion

        private void dgvChiTietPhieuMuon_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            dgvChiTietPhieuMuon.Rows[e.RowIndex].Cells["colSTT"].Value = (e.RowIndex + 1).ToString();
        }
    }
}
