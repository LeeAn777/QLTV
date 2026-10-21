using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLTV.GUI.UC_DocGia
{
    public partial class ucTTDG : UserControl
    {
        private readonly string str =
            "Data Source=.;Initial Catalog=QLThuVien;Integrated Security=True;TrustServerCertificate=True";

        private readonly DataTable tb = new DataTable();

        // Mã độc giả đang chọn (dùng cho Sửa / Xóa)
        private string maDangChon = null;

        private bool dangLamMoi = false;

        // true nếu MaDocGia là cột IDENTITY (tự tăng) -> không nhập tay
        private bool maTuTang = false;

        // guna2DateTimePicker1 = Ngày sinh, guna2DateTimePicker2 = Ngày đăng ký
        private static readonly string[] dinhDangNgay =
            { "dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "yyyy-MM-dd", "M/d/yyyy" };

        public ucTTDG()
        {
            InitializeComponent();
        }

        // ================== LOAD ==================
        private void ucThongTinDocGia_Load(object sender, EventArgs e)
        {
            dgvDocGia.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9.0F, FontStyle.Bold);
            dgvDocGia.AutoGenerateColumns = false;
            dgvDocGia.AllowUserToAddRows = false;

            // Gắn cột bảng với dữ liệu (Designer chưa gắn)
            colMaDG.DataPropertyName = "MaDocGia";
            colTenDG.DataPropertyName = "HoTen";
            colNgaySinh.DataPropertyName = "NgaySinh";
            colGioiTinh.DataPropertyName = "GioiTinh";
            colDiaChi.DataPropertyName = "DiaChi";
            colSDT.DataPropertyName = "SoDienThoai";
            colEmail.DataPropertyName = "Email";
            colNgayDangKy.DataPropertyName = "NgayDangKy";
            colHangDG.DataPropertyName = "TenHang";
            colNgaySinh.DefaultCellStyle.Format = "dd/MM/yyyy";
            colNgayDangKy.DefaultCellStyle.Format = "dd/MM/yyyy";

            dgvDocGia.CellClick -= dgvDocGia_CellClick;
            dgvDocGia.CellClick += dgvDocGia_CellClick;
            dgvDocGia.DataBindingComplete -= dgvDocGia_DataBindingComplete;
            dgvDocGia.DataBindingComplete += dgvDocGia_DataBindingComplete;

            txtTimKiem.TextChanged -= txtTimKiem_TextChanged;
            txtTimKiem.TextChanged += txtTimKiem_TextChanged;

            cbbGioiTinh.Items.Clear();
            cbbGioiTinh.Items.AddRange(new object[] { "Nam", "Nữ" });
            cbbGioiTinh.SelectedIndex = -1;

            KiemTraMaTuTang();
            LoadHang();
            LoadData();
        }

        // Kiểm tra MaDocGia có phải cột tự tăng không
        private void KiemTraMaTuTang()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(str))
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT COLUMNPROPERTY(OBJECT_ID('dbo.DocGia'), 'MaDocGia', 'IsIdentity')", conn))
                {
                    conn.Open();
                    object v = cmd.ExecuteScalar();
                    maTuTang = v != null && v != DBNull.Value && Convert.ToInt32(v) == 1;
                }
            }
            catch (SqlException)
            {
                maTuTang = false;
            }

            txtMaDocGia.ReadOnly = maTuTang;
            if (maTuTang) txtMaDocGia.PlaceholderText = "Tự động tăng";
        }

        private void LoadHang()
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection conn = new SqlConnection(str))
                using (SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT MaHang, TenHang FROM dbo.HangDocGia ORDER BY MaHang", conn))
                {
                    da.Fill(dt);
                }

                cbbHangDocGia.DataSource = dt;
                cbbHangDocGia.DisplayMember = "TenHang";
                cbbHangDocGia.ValueMember = "MaHang";
                cbbHangDocGia.SelectedIndex = -1;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi tải hạng độc giả: " + ex.Message);
            }
        }

        private void LoadData(string keyword = null)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(str))
                using (SqlDataAdapter adapter = new SqlDataAdapter())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    string sql =
                        "SELECT d.MaDocGia, d.HoTen, d.NgaySinh, d.GioiTinh, d.DiaChi, " +
                        "d.SoDienThoai, d.Email, d.NgayDangKy, d.MaHang, h.TenHang " +
                        "FROM dbo.DocGia d LEFT JOIN dbo.HangDocGia h ON d.MaHang = h.MaHang ";

                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        sql += "WHERE CAST(d.MaDocGia AS NVARCHAR(20)) LIKE @kw " +
                               "OR d.HoTen LIKE @kw OR d.SoDienThoai LIKE @kw " +
                               "OR d.Email LIKE @kw OR h.TenHang LIKE @kw " +
                               "OR CONVERT(NVARCHAR(10), d.NgayDangKy, 103) LIKE @kw ";
                        cmd.Parameters.AddWithValue("@kw", "%" + keyword.Trim() + "%");
                    }

                    cmd.CommandText = sql + "ORDER BY d.MaDocGia";
                    adapter.SelectCommand = cmd;
                    tb.Clear();
                    adapter.Fill(tb);
                }

                dgvDocGia.DataSource = tb;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        private void dgvDocGia_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            for (int i = 0; i < dgvDocGia.Rows.Count; i++)
                dgvDocGia.Rows[i].Cells["colSTT"].Value = i + 1;
        }

        // ================== HÀM PHỤ ==================
        private void ClearInput()
        {
            maDangChon = null;
            txtMaDocGia.Clear();
            txtTenDocGia.Clear();
            txtSoDienThoai.Clear();
            txtEmail.Clear();
            txtDiaChi.Clear();
            cbbGioiTinh.SelectedIndex = -1;
            cbbHangDocGia.SelectedIndex = -1;
            guna2DateTimePicker1.Value = DateTime.Now;
            guna2DateTimePicker2.Value = DateTime.Now;
            txtTenDocGia.Focus();
        }

        private object NullIfEmpty(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? (object)DBNull.Value
                : value.Trim();
        }

        private bool Loi(string msg, Control focus)
        {
            MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            focus.Focus();
            return false;
        }

        private bool ValidateInput(bool kiemTraMa)
        {
            if (kiemTraMa && !maTuTang && string.IsNullOrWhiteSpace(txtMaDocGia.Text))
                return Loi("Vui lòng nhập mã độc giả!", txtMaDocGia);

            if (string.IsNullOrWhiteSpace(txtTenDocGia.Text))
                return Loi("Vui lòng nhập tên độc giả!", txtTenDocGia);

            if (cbbGioiTinh.SelectedIndex < 0)
                return Loi("Vui lòng chọn giới tính!", cbbGioiTinh);

            if (cbbHangDocGia.SelectedIndex < 0 || cbbHangDocGia.SelectedValue == null)
                return Loi("Vui lòng chọn hạng độc giả!", cbbHangDocGia);

            if (guna2DateTimePicker1.Value.Date > DateTime.Today)
                return Loi("Ngày sinh không được lớn hơn ngày hiện tại!", guna2DateTimePicker1);

            if (guna2DateTimePicker2.Value.Date < guna2DateTimePicker1.Value.Date)
                return Loi("Ngày đăng ký không được trước ngày sinh!", guna2DateTimePicker2);

            string sdt = txtSoDienThoai.Text.Trim();
            if (sdt != "" && !Regex.IsMatch(sdt, @"^\d{9,11}$"))
                return Loi("Số điện thoại phải gồm 9-11 chữ số!", txtSoDienThoai);

            string email = txtEmail.Text.Trim();
            if (email != "" && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return Loi("Email không hợp lệ!", txtEmail);

            return true;
        }

        // Gắn các tham số dùng chung cho Thêm / Sửa
        private void ThemThamSo(SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@Ten", txtTenDocGia.Text.Trim());
            cmd.Parameters.AddWithValue("@NS", guna2DateTimePicker1.Value.Date);
            cmd.Parameters.AddWithValue("@GT", cbbGioiTinh.Text);
            cmd.Parameters.AddWithValue("@DC", NullIfEmpty(txtDiaChi.Text));
            cmd.Parameters.AddWithValue("@SDT", NullIfEmpty(txtSoDienThoai.Text));
            cmd.Parameters.AddWithValue("@Email", NullIfEmpty(txtEmail.Text));
            cmd.Parameters.AddWithValue("@NDK", guna2DateTimePicker2.Value.Date);
            cmd.Parameters.AddWithValue("@Hang", cbbHangDocGia.SelectedValue);
        }

        // ================== CHỌN DÒNG ==================
        private void dgvDocGia_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataRowView row = dgvDocGia.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (row == null) return;

            maDangChon = row["MaDocGia"].ToString();
            txtMaDocGia.Text = maDangChon;
            txtTenDocGia.Text = row["HoTen"].ToString();
            txtDiaChi.Text = row["DiaChi"] == DBNull.Value ? "" : row["DiaChi"].ToString();
            txtSoDienThoai.Text = row["SoDienThoai"] == DBNull.Value ? "" : row["SoDienThoai"].ToString();
            txtEmail.Text = row["Email"] == DBNull.Value ? "" : row["Email"].ToString();

            if (row["NgaySinh"] != DBNull.Value)
                guna2DateTimePicker1.Value = Convert.ToDateTime(row["NgaySinh"]);
            if (row["NgayDangKy"] != DBNull.Value)
                guna2DateTimePicker2.Value = Convert.ToDateTime(row["NgayDangKy"]);

            cbbGioiTinh.SelectedIndex = row["GioiTinh"] == DBNull.Value
                ? -1
                : cbbGioiTinh.FindStringExact(row["GioiTinh"].ToString().Trim());

            if (row["MaHang"] == DBNull.Value) cbbHangDocGia.SelectedIndex = -1;
            else cbbHangDocGia.SelectedValue = row["MaHang"];
        }

        // ================== THÊM ==================
        private void guna2Button4_Click(object sender, EventArgs e)
        {
            if (!ValidateInput(true)) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(str))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = maTuTang
                        ? "INSERT INTO dbo.DocGia (HoTen, NgaySinh, GioiTinh, DiaChi, SoDienThoai, Email, NgayDangKy, MaHang) " +
                          "VALUES (@Ten, @NS, @GT, @DC, @SDT, @Email, @NDK, @Hang)"
                        : "INSERT INTO dbo.DocGia (MaDocGia, HoTen, NgaySinh, GioiTinh, DiaChi, SoDienThoai, Email, NgayDangKy, MaHang) " +
                          "VALUES (@Ma, @Ten, @NS, @GT, @DC, @SDT, @Email, @NDK, @Hang)";

                    if (!maTuTang) cmd.Parameters.AddWithValue("@Ma", txtMaDocGia.Text.Trim());
                    ThemThamSo(cmd);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Thêm độc giả thành công!");
                LamMoi();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                    MessageBox.Show("Độc giả đã tồn tại (trùng mã, SDT hoặc email)!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else if (ex.Number == 547)
                    MessageBox.Show("Hạng độc giả không tồn tại!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("Lỗi SQL: " + ex.Message);
            }
        }

        // ================== SỬA ==================
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(maDangChon))
            {
                MessageBox.Show("Vui lòng chọn độc giả cần sửa trên bảng!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtMaDocGia.Text.Trim() != maDangChon)
            {
                MessageBox.Show("Không được thay đổi mã độc giả khi sửa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaDocGia.Text = maDangChon;
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
                        "UPDATE dbo.DocGia SET HoTen = @Ten, NgaySinh = @NS, GioiTinh = @GT, " +
                        "DiaChi = @DC, SoDienThoai = @SDT, Email = @Email, NgayDangKy = @NDK, MaHang = @Hang " +
                        "WHERE MaDocGia = @Ma";
                    cmd.Parameters.AddWithValue("@Ma", maDangChon);
                    ThemThamSo(cmd);

                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        MessageBox.Show("Không tìm thấy độc giả để sửa!");
                        return;
                    }
                }

                MessageBox.Show("Sửa độc giả thành công!");
                LamMoi();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                    MessageBox.Show("SDT hoặc email đã được độc giả khác sử dụng!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else if (ex.Number == 547)
                    MessageBox.Show("Hạng độc giả không tồn tại!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("Lỗi SQL: " + ex.Message);
            }
        }

        // ================== XÓA ==================
        private void guna2Button7_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(maDangChon))
            {
                MessageBox.Show("Vui lòng chọn độc giả cần xóa trên bảng!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Bạn có chắc muốn xóa độc giả \"" + txtTenDocGia.Text + "\"?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(str))
                using (SqlCommand del = conn.CreateCommand())
                {
                    conn.Open();
                    del.CommandText = "DELETE FROM dbo.DocGia WHERE MaDocGia = @ma";
                    del.Parameters.AddWithValue("@ma", maDangChon);
                    del.ExecuteNonQuery();
                }

                MessageBox.Show("Xóa độc giả thành công!");
                LamMoi();
            }
            catch (SqlException ex)
            {
                // 547: độc giả đang được dùng ở bảng khác (thẻ độc giả, phiếu mượn...)
                if (ex.Number == 547)
                    MessageBox.Show("Không thể xóa độc giả này vì đang có dữ liệu liên quan (thẻ, phiếu mượn...)!",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("Lỗi SQL: " + ex.Message);
            }
        }

        // ================== TÌM KIẾM ==================
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            TimKiem();
        }

        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            TimKiem();
        }

        // Ô mã độc giả (Designer đang gắn sự kiện này) - không làm gì
        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void TimKiem()
        {
            if (dangLamMoi) return;

            maDangChon = null;
            LoadData(txtTimKiem.Text);
        }

        // ================== LÀM MỚI ==================
        private void guna2Button3_Click(object sender, EventArgs e)
        {
            LamMoi();
        }

        private void LamMoi()
        {
            dangLamMoi = true;
            txtTimKiem.Clear();
            dangLamMoi = false;

            ClearInput();
            LoadData();
        }

        // ================== XUẤT FILE ==================
        // Cột: MaDocGia | HoTen | NgaySinh | GioiTinh | DiaChi | SDT | Email | NgayDangKy | Hang
        private void guna2Button6_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Lưu File";
            sfd.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
            sfd.FileName = "DocGia.xlsx";

            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("DocGia");

                    // Bỏ cột STT để file xuất ra có thể nhập lại đúng thứ tự cột
                    List<DataGridViewColumn> cols = new List<DataGridViewColumn>();
                    foreach (DataGridViewColumn c in dgvDocGia.Columns)
                    {
                        if (c.Visible && c.Name != "colSTT") cols.Add(c);
                    }

                    for (int j = 0; j < cols.Count; j++)
                        ws.Cell(1, j + 1).Value = cols[j].HeaderText;

                    int dong = 2;
                    for (int i = 0; i < dgvDocGia.Rows.Count; i++)
                    {
                        if (dgvDocGia.Rows[i].IsNewRow) continue;

                        for (int j = 0; j < cols.Count; j++)
                        {
                            object v = dgvDocGia.Rows[i].Cells[cols[j].Index].Value;
                            ws.Cell(dong, j + 1).Value = v is DateTime
                                ? ((DateTime)v).ToString("dd/MM/yyyy")
                                : Convert.ToString(v);
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
        // Cột trong file: MaDocGia | HoTen | NgaySinh | GioiTinh | DiaChi | SDT | Email | NgayDangKy | Hang
        // - Nếu MaDocGia tự tăng: bỏ qua cột mã; trùng = cùng tên + ngày sinh + SDT
        // - Cột Hang ghi tên hạng (hoặc mã hạng) có trong bảng HangDocGia
        private bool ThemDocGia(SqlConnection conn, string ma, string ten, DateTime ns, string gt,
            string dc, string sdt, string email, DateTime ndk, object maHang)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = maTuTang
                    ? @"IF NOT EXISTS (SELECT 1 FROM dbo.DocGia
                                       WHERE HoTen = @ten AND NgaySinh = @ns
                                         AND ISNULL(SoDienThoai, '') = ISNULL(@sdt, ''))
                        BEGIN
                            INSERT INTO dbo.DocGia (HoTen, NgaySinh, GioiTinh, DiaChi, SoDienThoai, Email, NgayDangKy, MaHang)
                            VALUES (@ten, @ns, @gt, @dc, @sdt, @email, @ndk, @hang);
                            SELECT 1;
                        END
                        ELSE SELECT 0;"
                    : @"IF NOT EXISTS (SELECT 1 FROM dbo.DocGia WHERE MaDocGia = @ma)
                        BEGIN
                            INSERT INTO dbo.DocGia (MaDocGia, HoTen, NgaySinh, GioiTinh, DiaChi, SoDienThoai, Email, NgayDangKy, MaHang)
                            VALUES (@ma, @ten, @ns, @gt, @dc, @sdt, @email, @ndk, @hang);
                            SELECT 1;
                        END
                        ELSE SELECT 0;";

                if (!maTuTang) cmd.Parameters.Add("@ma", SqlDbType.NVarChar, 50).Value = ma;
                cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 200).Value = ten;
                cmd.Parameters.Add("@ns", SqlDbType.Date).Value = ns.Date;
                cmd.Parameters.Add("@gt", SqlDbType.NVarChar, 10).Value = gt;
                cmd.Parameters.Add("@dc", SqlDbType.NVarChar, 255).Value =
                    string.IsNullOrWhiteSpace(dc) ? (object)DBNull.Value : dc;
                cmd.Parameters.Add("@sdt", SqlDbType.NVarChar, 20).Value =
                    string.IsNullOrWhiteSpace(sdt) ? (object)DBNull.Value : sdt;
                cmd.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value =
                    string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email;
                cmd.Parameters.Add("@ndk", SqlDbType.Date).Value = ndk.Date;
                cmd.Parameters.AddWithValue("@hang", maHang);

                return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
            }
        }

        // Đọc ô Excel: ô ngày thì đổi sang dd/MM/yyyy
        private string DocO(IXLCell c)
        {
            if (c.DataType == XLDataType.DateTime)
                return c.GetDateTime().ToString("dd/MM/yyyy");
            return c.GetString().Trim();
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
                    string[] r = new string[9];
                    for (int k = 0; k < 9; k++)
                        r[k] = DocO(sheet.Cell(dong, k + 1));
                    rows.Add(r);
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
                string[] r = new string[9];
                for (int k = 0; k < 9; k++)
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

        private bool DocNgay(string s, out DateTime d)
        {
            return DateTime.TryParseExact(s.Trim(), dinhDangNgay,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out d);
        }

        // Chuẩn hóa giới tính: Nam / Nữ, khác -> null
        private string ChuanHoaGioiTinh(string s)
        {
            s = s.Trim().ToLower();
            if (s == "nam") return "Nam";
            if (s == "nữ" || s == "nu") return "Nữ";
            return null;
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn file Excel hoặc CSV";
                ofd.Filter = "Excel & CSV|*.xlsx;*.csv|Excel|*.xlsx|CSV|*.csv";
                ofd.InitialDirectory =
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (ofd.ShowDialog() != DialogResult.OK) return;

                int thanhCong = 0;
                int trung = 0;      // độc giả đã tồn tại
                int saiDuLieu = 0;  // thiếu / sai thông tin, hạng không tồn tại

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

                        // Tên hạng (hoặc mã hạng) -> mã hạng
                        Dictionary<string, object> dsHang =
                            new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                        using (SqlCommand cmdH = new SqlCommand("SELECT MaHang, TenHang FROM dbo.HangDocGia", conn))
                        using (SqlDataReader rd = cmdH.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                dsHang[Convert.ToString(rd["TenHang"]).Trim()] = rd["MaHang"];
                                dsHang[Convert.ToString(rd["MaHang"]).Trim()] = rd["MaHang"];
                            }
                        }

                        foreach (string[] r in rows)
                        {
                            // Bỏ qua dòng trống hoàn toàn
                            if (r.All(x => string.IsNullOrWhiteSpace(x)))
                                continue;

                            DateTime ns, ndk;
                            object maHang;
                            string gt = ChuanHoaGioiTinh(r[3]);

                            if ((!maTuTang && string.IsNullOrWhiteSpace(r[0])) ||
                                string.IsNullOrWhiteSpace(r[1]) ||
                                !DocNgay(r[2], out ns) ||
                                gt == null ||
                                !DocNgay(r[7], out ndk) ||
                                !dsHang.TryGetValue(r[8].Trim(), out maHang))
                            {
                                saiDuLieu++;
                                continue;
                            }

                            try
                            {
                                if (ThemDocGia(conn, r[0], r[1], ns, gt, r[4], r[5], r[6], ndk, maHang))
                                    thanhCong++;
                                else
                                    trung++;
                            }
                            catch (SqlException ex)
                            {
                                if (ex.Number == 2627 || ex.Number == 2601) trung++;
                                // 547: khóa ngoại, 8152: chuỗi quá dài, 245/241/242/8114: sai kiểu dữ liệu
                                else if (ex.Number == 547 || ex.Number == 8152 || ex.Number == 245 ||
                                         ex.Number == 241 || ex.Number == 242 || ex.Number == 8114) saiDuLieu++;
                                else throw;
                            }
                        }
                    }

                    LamMoi();

                    MessageBox.Show(
                        "Nhập dữ liệu hoàn tất!\n" +
                        "Thêm thành công: " + thanhCong + "\n" +
                        "Bỏ qua (đã tồn tại): " + trung + "\n" +
                        "Bỏ qua (thông tin sai): " + saiDuLieu);
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