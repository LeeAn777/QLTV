using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLTV.GUI.UC_Sach
{
    public partial class ucTTSach : UserControl
    {
        private readonly string str =
            "Data Source=.;Initial Catalog=QLThuVien;Integrated Security=True;TrustServerCertificate=True";

        private readonly DataTable tb = new DataTable();

        // Mã sách của dòng đang được chọn (dùng cho Sửa / Xóa)
        private int? maDangChon = null;

        // Chặn tìm kiếm tự động khi đang nạp ComboBox / làm mới
        private bool dangLamMoi = false;

        public ucTTSach()
        {
            InitializeComponent();
        }

        // ================== LOAD ==================
        private void ucTTSach_Load(object sender, EventArgs e)
        {
            dgvSach.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9.0F, FontStyle.Bold);

            // Các cột đã được tạo sẵn trong Designer -> không tự sinh cột
            dgvSach.AutoGenerateColumns = false;

            // Mặc định NumericUpDown chỉ tối đa 100 -> nới rộng
            guna2NumericUpDown1.Minimum = 0;
            guna2NumericUpDown1.Maximum = 1000000;

            // Đảm bảo các sự kiện chỉ được gắn một lần
            dgvSach.CellClick -= dgvSach_CellClick;
            dgvSach.CellClick += dgvSach_CellClick;

            dgvSach.DataBindingComplete -= dgvSach_DataBindingComplete;
            dgvSach.DataBindingComplete += dgvSach_DataBindingComplete;

            // Tìm kiếm tự động khi gõ chữ
            txtTimKiem.TextChanged -= txtTimKiem_TextChanged;
            txtTimKiem.TextChanged += txtTimKiem_TextChanged;

            // Tìm kiếm tự động khi đổi thể loại ở ComboBox
            guna2ComboBox1.SelectedIndexChanged -= guna2ComboBox1_SelectedIndexChanged;
            guna2ComboBox1.SelectedIndexChanged += guna2ComboBox1_SelectedIndexChanged;

            dangLamMoi = true;
            LoadComboBoxes();
            dangLamMoi = false;

            LoadData();
        }

        // ================== NẠP COMBOBOX ==================
        private void LoadComboBoxes()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(str))
                {
                    // --- Thể loại (form nhập liệu) ---
                    DataTable dtTL = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(
                        "SELECT MaTheLoai, TenTheLoai FROM dbo.TheLoai ORDER BY TenTheLoai", conn))
                    {
                        da.Fill(dtTL);
                    }

                    cbbTheLoai.DataSource = dtTL;
                    cbbTheLoai.DisplayMember = "TenTheLoai";
                    cbbTheLoai.ValueMember = "MaTheLoai";
                    cbbTheLoai.SelectedIndex = -1;

                    // --- Thể loại (ô lọc tìm kiếm) có thêm dòng "Tất cả" ---
                    DataTable dtLoc = dtTL.Copy();
                    DataRow tatCa = dtLoc.NewRow();
                    tatCa["MaTheLoai"] = 0;
                    tatCa["TenTheLoai"] = "-- Tất cả thể loại --";
                    dtLoc.Rows.InsertAt(tatCa, 0);

                    guna2ComboBox1.DataSource = dtLoc;
                    guna2ComboBox1.DisplayMember = "TenTheLoai";
                    guna2ComboBox1.ValueMember = "MaTheLoai";
                    guna2ComboBox1.SelectedIndex = 0;

                    // --- Nhà xuất bản ---
                    DataTable dtNXB = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(
                        "SELECT MaNXB, TenNXB FROM dbo.NhaXuatBan ORDER BY TenNXB", conn))
                    {
                        da.Fill(dtNXB);
                    }

                    cbbNXB.DataSource = dtNXB;
                    cbbNXB.DisplayMember = "TenNXB";
                    cbbNXB.ValueMember = "MaNXB";
                    cbbNXB.SelectedIndex = -1;
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi tải danh sách thể loại / NXB: " + ex.Message);
            }
        }

        // keyword rỗng: không lọc theo chữ; maTheLoai = 0: không lọc theo thể loại
        // Hai điều kiện kết hợp với nhau bằng AND
        private void LoadData(string keyword = null, int maTheLoai = 0)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(str))
                using (SqlDataAdapter adapter = new SqlDataAdapter())
                {
                    SqlCommand cmd = conn.CreateCommand();

                    StringBuilder sql = new StringBuilder();
                    sql.Append("SELECT s.MaSach, s.TenSach, s.MaTheLoai, s.MaNXB, s.NamXuatBan, s.SoLuong ");
                    sql.Append("FROM dbo.sach s ");
                    sql.Append("LEFT JOIN dbo.NhaXuatBan n ON s.MaNXB = n.MaNXB ");
                    sql.Append("WHERE 1 = 1 ");

                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        sql.Append("AND (CAST(s.MaSach AS NVARCHAR(20)) LIKE @kw ");
                        sql.Append("OR s.TenSach LIKE @kw ");
                        sql.Append("OR CAST(s.MaNXB AS NVARCHAR(20)) LIKE @kw ");
                        sql.Append("OR n.TenNXB LIKE @kw) ");
                        cmd.Parameters.AddWithValue("@kw", "%" + keyword.Trim() + "%");
                    }

                    if (maTheLoai > 0)
                    {
                        sql.Append("AND s.MaTheLoai = @tl ");
                        cmd.Parameters.AddWithValue("@tl", maTheLoai);
                    }

                    sql.Append("ORDER BY s.MaSach");
                    cmd.CommandText = sql.ToString();

                    adapter.SelectCommand = cmd;
                    tb.Clear();
                    adapter.Fill(tb);
                }

                dgvSach.DataSource = tb;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        // Đánh số thứ tự sau mỗi lần nạp dữ liệu
        private void dgvSach_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            for (int i = 0; i < dgvSach.Rows.Count; i++)
                dgvSach.Rows[i].Cells["colSTT"].Value = i + 1;
        }

        // ================== HÀM PHỤ ==================
        private void ClearInput()
        {
            maDangChon = null;
            txtMaSach.Clear();
            txtTenSach.Clear();
            cbbTheLoai.SelectedIndex = -1;
            cbbNXB.SelectedIndex = -1;
            dtpNamXuatBan.Value = DateTime.Now;
            guna2NumericUpDown1.Value = 0;
            txtMaSach.Focus();
        }

        // Lấy năm từ giá trị trong DB (hỗ trợ cả kiểu int lẫn DateTime)
        private int LayNam(object v)
        {
            int nam = DateTime.Now.Year;
            if (v == null || v == DBNull.Value) return nam;

            if (v is DateTime) nam = ((DateTime)v).Year;
            else if (!int.TryParse(v.ToString(), out nam)) nam = DateTime.Now.Year;

            if (nam < 1753) nam = 1753;
            if (nam > 9998) nam = 9998;
            return nam;
        }

        private int LayMaTheLoaiLoc()
        {
            object v = guna2ComboBox1.SelectedValue;
            if (v == null || v is DataRowView) return 0;

            int ma;
            return int.TryParse(v.ToString(), out ma) ? ma : 0;
        }

        private bool ValidateInput(bool kiemTraMa)
        {
            if (kiemTraMa)
            {
                int ma;
                if (!int.TryParse(txtMaSach.Text.Trim(), out ma) || ma <= 0)
                {
                    MessageBox.Show("Mã sách phải là số nguyên dương!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMaSach.Focus();
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(txtTenSach.Text))
            {
                MessageBox.Show("Vui lòng nhập tên sách!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenSach.Focus();
                return false;
            }

            if (cbbTheLoai.SelectedIndex < 0 || cbbTheLoai.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn thể loại sách!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbbTheLoai.Focus();
                return false;
            }

            if (cbbNXB.SelectedIndex < 0 || cbbNXB.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn nhà xuất bản!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbbNXB.Focus();
                return false;
            }

            return true;
        }

        // ================== CHỌN DÒNG -> ĐỔ LÊN FORM ==================
        private void dgvSach_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataRowView row = dgvSach.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (row == null) return;

            maDangChon = Convert.ToInt32(row["MaSach"]);
            txtMaSach.Text = row["MaSach"].ToString();
            txtTenSach.Text = row["TenSach"].ToString();

            cbbTheLoai.SelectedValue = Convert.ToInt32(row["MaTheLoai"]);
            cbbNXB.SelectedValue = Convert.ToInt32(row["MaNXB"]);

            dtpNamXuatBan.Value = new DateTime(LayNam(row["NamXuatBan"]), 1, 1);
            guna2NumericUpDown1.Value = row["SoLuong"] == DBNull.Value
                ? 0
                : Convert.ToDecimal(row["SoLuong"]);
        }

        // ================== THÊM ==================
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (!ValidateInput(true)) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(str))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText =
                        "INSERT INTO dbo.sach (MaSach, TenSach, MaTheLoai, MaNXB, NamXuatBan, SoLuong) " +
                        "VALUES (@Ma, @Ten, @TL, @NXB, @Nam, @SL)";
                    cmd.Parameters.AddWithValue("@Ma", Convert.ToInt32(txtMaSach.Text.Trim()));
                    cmd.Parameters.AddWithValue("@Ten", txtTenSach.Text.Trim());
                    cmd.Parameters.AddWithValue("@TL", Convert.ToInt32(cbbTheLoai.SelectedValue));
                    cmd.Parameters.AddWithValue("@NXB", Convert.ToInt32(cbbNXB.SelectedValue));
                    cmd.Parameters.AddWithValue("@Nam", dtpNamXuatBan.Value.Year);
                    cmd.Parameters.AddWithValue("@SL", Convert.ToInt32(guna2NumericUpDown1.Value));
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Thêm sách thành công!");
                LamMoi();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                    MessageBox.Show("Mã sách đã tồn tại!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else if (ex.Number == 547)
                    MessageBox.Show("Thể loại hoặc nhà xuất bản không tồn tại!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("Lỗi SQL: " + ex.Message);
            }
        }

        // ================== SỬA ==================
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (maDangChon == null)
            {
                MessageBox.Show("Vui lòng chọn sách cần sửa trên bảng!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Mã là khóa chính, không cho đổi khi sửa
            if (txtMaSach.Text.Trim() != maDangChon.Value.ToString())
            {
                MessageBox.Show("Không được thay đổi mã sách khi sửa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaSach.Text = maDangChon.Value.ToString();
                return;
            }

            if (!ValidateInput(false)) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(str))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText =
                        "UPDATE dbo.sach SET TenSach = @Ten, MaTheLoai = @TL, MaNXB = @NXB, " +
                        "NamXuatBan = @Nam, SoLuong = @SL WHERE MaSach = @Ma";
                    cmd.Parameters.AddWithValue("@Ma", maDangChon.Value);
                    cmd.Parameters.AddWithValue("@Ten", txtTenSach.Text.Trim());
                    cmd.Parameters.AddWithValue("@TL", Convert.ToInt32(cbbTheLoai.SelectedValue));
                    cmd.Parameters.AddWithValue("@NXB", Convert.ToInt32(cbbNXB.SelectedValue));
                    cmd.Parameters.AddWithValue("@Nam", dtpNamXuatBan.Value.Year);
                    cmd.Parameters.AddWithValue("@SL", Convert.ToInt32(guna2NumericUpDown1.Value));

                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        MessageBox.Show("Không tìm thấy sách để sửa!");
                        return;
                    }
                }

                MessageBox.Show("Sửa sách thành công!");
                LamMoi();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547)
                    MessageBox.Show("Thể loại hoặc nhà xuất bản không tồn tại!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("Lỗi SQL: " + ex.Message);
            }
        }

        // ================== XÓA ==================
        private void guna2Button7_Click(object sender, EventArgs e)
        {
            if (maDangChon == null)
            {
                MessageBox.Show("Vui lòng chọn sách cần xóa trên bảng!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Bạn có chắc muốn xóa sách \"" + txtTenSach.Text + "\"?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(str))
                using (SqlCommand del = conn.CreateCommand())
                {
                    conn.Open();
                    del.CommandText = "DELETE FROM dbo.sach WHERE MaSach = @ma";
                    del.Parameters.AddWithValue("@ma", maDangChon.Value);
                    del.ExecuteNonQuery();
                }

                MessageBox.Show("Xóa sách thành công!");
                LamMoi();
            }
            catch (SqlException ex)
            {
                // 547: vi phạm khóa ngoại (sách đang được dùng ở bảng khác, vd: phiếu mượn)
                if (ex.Number == 547)
                    MessageBox.Show("Không thể xóa sách này vì đang được sử dụng ở dữ liệu khác!",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("Lỗi SQL: " + ex.Message);
            }
        }

        // ================== TÌM KIẾM ==================
        // Nút "Tìm kiếm"
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            TimKiem();
        }

        // Gõ chữ vào ô tìm kiếm
        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            TimKiem();
        }

        // Chọn thể loại ở ComboBox tìm kiếm
        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TimKiem();
        }

        // Kết hợp cả 2 điều kiện: chữ trong ô tìm kiếm + thể loại đang chọn
        private void TimKiem()
        {
            if (dangLamMoi) return;

            maDangChon = null;
            LoadData(txtTimKiem.Text, LayMaTheLoaiLoc());
        }

        // ================== LÀM MỚI ==================
        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LamMoi();
        }

        private void LamMoi()
        {
            dangLamMoi = true;   // tránh tìm lặp khi xóa chữ / đặt lại ComboBox
            txtTimKiem.Clear();
            LoadComboBoxes();    // nạp lại để cập nhật thể loại / NXB mới thêm
            dangLamMoi = false;

            ClearInput();
            LoadData();
        }

        // ================== XUẤT FILE ==================
        private void btnXuatFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Lưu File";
            sfd.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
            sfd.FileName = "ThongTinSach.xlsx";

            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Sach");

                    // Bỏ cột STT để file xuất ra có thể nhập lại đúng thứ tự cột
                    List<DataGridViewColumn> cols = new List<DataGridViewColumn>();
                    foreach (DataGridViewColumn c in dgvSach.Columns)
                    {
                        if (c.Visible && c.Name != "colSTT") cols.Add(c);
                    }

                    // Tiêu đề cột
                    for (int j = 0; j < cols.Count; j++)
                        ws.Cell(1, j + 1).Value = cols[j].HeaderText;

                    // Dữ liệu
                    int dong = 2;
                    for (int i = 0; i < dgvSach.Rows.Count; i++)
                    {
                        if (dgvSach.Rows[i].IsNewRow) continue;

                        for (int j = 0; j < cols.Count; j++)
                        {
                            ws.Cell(dong, j + 1).Value =
                                Convert.ToString(dgvSach.Rows[i].Cells[cols[j].Index].Value);
                        }
                        dong++;
                    }

                    ws.Columns().AdjustToContents();
                    wb.SaveAs(sfd.FileName);
                }

                MessageBox.Show("Xuất Excel thành công!");
            }
            catch (IOException)
            {
                MessageBox.Show("Không ghi được file. Hãy đóng file nếu đang mở trong Excel rồi thử lại!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xuất file: " + ex.Message);
            }
        }

        // ================== NHẬP FILE ==================
        // Thứ tự cột trong file: MaSach | TenSach | MaTheLoai | MaNXB | NamXuatBan | SoLuong
        private bool ThemSach(SqlConnection conn, int ma, string ten, int maTL, int maNXB, int nam, int soLuong)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    IF NOT EXISTS (SELECT 1 FROM dbo.sach WHERE MaSach = @ma)
                    BEGIN
                        INSERT INTO dbo.sach (MaSach, TenSach, MaTheLoai, MaNXB, NamXuatBan, SoLuong)
                        VALUES (@ma, @ten, @tl, @nxb, @nam, @sl);
                        SELECT 1;
                    END
                    ELSE
                        SELECT 0;";

                cmd.Parameters.Add("@ma", SqlDbType.Int).Value = ma;
                cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 200).Value = ten;
                cmd.Parameters.Add("@tl", SqlDbType.Int).Value = maTL;
                cmd.Parameters.Add("@nxb", SqlDbType.Int).Value = maNXB;
                cmd.Parameters.Add("@nam", SqlDbType.Int).Value = nam;
                cmd.Parameters.Add("@sl", SqlDbType.Int).Value = soLuong;

                return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
            }
        }

        private List<string[]> DocExcel(string path)
        {
            List<string[]> rows = new List<string[]>();

            using (XLWorkbook wb = new XLWorkbook(path))
            {
                IXLWorksheet sheet = wb.Worksheet(1);
                IXLRow last = sheet.LastRowUsed();
                if (last == null) return rows;

                int dongCuoi = last.RowNumber();
                for (int dong = 2; dong <= dongCuoi; dong++)
                {
                    rows.Add(new string[]
                    {
                        sheet.Cell(dong, 1).GetString().Trim(),
                        sheet.Cell(dong, 2).GetString().Trim(),
                        sheet.Cell(dong, 3).GetString().Trim(),
                        sheet.Cell(dong, 4).GetString().Trim(),
                        sheet.Cell(dong, 5).GetString().Trim(),
                        sheet.Cell(dong, 6).GetString().Trim()
                    });
                }
            }
            return rows;
        }

        private List<string[]> DocCsv(string path)
        {
            List<string[]> rows = new List<string[]>();
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            if (lines.Length == 0) return rows;

            char sep = (lines[0].Contains(";") && !lines[0].Contains(",")) ? ';' : ',';

            for (int i = 1; i < lines.Length; i++) // bỏ dòng tiêu đề
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                string[] cot = TachDongCsv(lines[i], sep);
                string[] r = new string[6];
                for (int k = 0; k < 6; k++)
                    r[k] = cot.Length > k ? cot[k] : "";
                rows.Add(r);
            }
            return rows;
        }

        // Tách 1 dòng CSV, hỗ trợ giá trị đặt trong dấu ngoặc kép
        private string[] TachDongCsv(string line, char sep)
        {
            List<string> list = new List<string>();
            StringBuilder sb = new StringBuilder();
            bool trongNgoac = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (trongNgoac)
                {
                    if (c == '"')
                    {
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            sb.Append('"');
                            i++;
                        }
                        else trongNgoac = false;
                    }
                    else sb.Append(c);
                }
                else
                {
                    if (c == '"') trongNgoac = true;
                    else if (c == sep)
                    {
                        list.Add(sb.ToString().Trim());
                        sb.Clear();
                    }
                    else sb.Append(c);
                }
            }
            list.Add(sb.ToString().Trim());
            return list.ToArray();
        }

        private void btnNhapFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn file Excel hoặc CSV";
                ofd.Filter = "Excel & CSV|*.xlsx;*.csv|Excel|*.xlsx|CSV|*.csv";
                ofd.InitialDirectory =
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (ofd.ShowDialog() != DialogResult.OK) return;

                int thanhCong = 0;
                int trung = 0;       // mã sách đã tồn tại
                int saiDuLieu = 0;   // thiếu dữ liệu, sai kiểu số, thể loại/NXB không tồn tại

                try
                {
                    string duoiFile = Path.GetExtension(ofd.FileName).ToLower();

                    List<string[]> rows;
                    if (duoiFile == ".xlsx")
                        rows = DocExcel(ofd.FileName);
                    else if (duoiFile == ".csv")
                        rows = DocCsv(ofd.FileName);
                    else
                    {
                        MessageBox.Show("Chỉ hỗ trợ file .xlsx hoặc .csv!");
                        return;
                    }

                    using (SqlConnection conn = new SqlConnection(str))
                    {
                        conn.Open();

                        foreach (string[] r in rows)
                        {
                            // Bỏ qua dòng trống hoàn toàn
                            if (r.All(x => string.IsNullOrWhiteSpace(x)))
                                continue;

                            int ma, maTL, maNXB, nam, sl;
                            if (!int.TryParse(r[0], out ma) || ma <= 0 ||
                                string.IsNullOrWhiteSpace(r[1]) ||
                                !int.TryParse(r[2], out maTL) ||
                                !int.TryParse(r[3], out maNXB) ||
                                !int.TryParse(r[4], out nam) || nam < 1 ||
                                !int.TryParse(r[5], out sl) || sl < 0)
                            {
                                saiDuLieu++;
                                continue;
                            }

                            try
                            {
                                if (ThemSach(conn, ma, r[1], maTL, maNXB, nam, sl))
                                    thanhCong++;
                                else
                                    trung++;
                            }
                            catch (SqlException ex)
                            {
                                // 547: mã thể loại hoặc mã NXB không có trong CSDL
                                if (ex.Number == 547) saiDuLieu++;
                                else throw;
                            }
                        }
                    }

                    LamMoi();

                    MessageBox.Show(
                        "Nhập dữ liệu hoàn tất!\n" +
                        "Thêm thành công: " + thanhCong + "\n" +
                        "Bỏ qua (đã tồn tại): " + trung + "\n" +
                        "Bỏ qua (dữ liệu sai): " + saiDuLieu);
                }
                catch (IOException)
                {
                    MessageBox.Show("Không đọc được file. Hãy đóng file nếu đang mở trong Excel rồi thử lại!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi nhập file: " + ex.Message);
                }
            }
        }
    }
}