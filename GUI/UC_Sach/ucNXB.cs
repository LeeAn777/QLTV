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
    public partial class ucNXB : UserControl
    {
        private readonly string str =
            @"Data Source=.\SQLEXPRESS;Initial Catalog=QLThuVien;Integrated Security=True";

        private readonly DataTable tb = new DataTable();

        // Mã NXB đang chọn (dùng cho Sửa / Xóa)
        private string maDangChon = null;

        private bool dangLamMoi = false;

        public ucNXB()
        {
            InitializeComponent();
        }

        // ================== LOAD ==================
        private void ucNXB_Load(object sender, EventArgs e)
        {
            dgvNXB.ColumnHeadersDefaultCellStyle.Font =
    new Font("Segoe UI", 9.0F, FontStyle.Bold);

            // Các cột đã tạo sẵn trong Designer (có DataPropertyName) -> không tự sinh thêm cột
            dgvNXB.AutoGenerateColumns = false;

            dgvNXB.CellClick -= dgvNXB_CellClick;
            dgvNXB.CellClick += dgvNXB_CellClick;

            // Tự đánh số thứ tự sau mỗi lần nạp dữ liệu
            dgvNXB.DataBindingComplete -= dgvNXB_DataBindingComplete;
            dgvNXB.DataBindingComplete += dgvNXB_DataBindingComplete;

            LoadData();
        }

        private void LoadData(string keyword = null)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(str))
                using (SqlDataAdapter adapter = new SqlDataAdapter())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    if (string.IsNullOrWhiteSpace(keyword))
                    {
                        cmd.CommandText =
                            "SELECT MaNXB, TenNXB, SoDienThoai, Email, DiaChi FROM dbo.NhaXuatBan " +
                            "ORDER BY MaNXB";
                    }
                    else
                    {
                        cmd.CommandText =
                            "SELECT MaNXB, TenNXB, SoDienThoai, Email, DiaChi FROM dbo.NhaXuatBan " +
                            "WHERE MaNXB LIKE @kw " +
                            "OR TenNXB LIKE @kw OR SoDienThoai LIKE @kw OR Email LIKE @kw OR DiaChi LIKE @kw " +
                            "ORDER BY MaNXB";
                        cmd.Parameters.AddWithValue("@kw", "%" + keyword.Trim() + "%");
                    }

                    adapter.SelectCommand = cmd;
                    tb.Clear();
                    adapter.Fill(tb);
                }

                dgvNXB.DataSource = tb;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        private void dgvNXB_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            for (int i = 0; i < dgvNXB.Rows.Count; i++)
                dgvNXB.Rows[i].Cells["colSTT"].Value = i + 1;
        }

        // ================== HÀM PHỤ ==================
        private void ClearInput()
        {
            maDangChon = null;
            txtMaNXB.Clear();
            txtTenNXB.Clear();
            txtSDT.Clear();
            txtEmail.Clear();
            txtDiaChi.Clear();
            txtMaNXB.Focus();
        }

        private object NullIfEmpty(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? (object)DBNull.Value
                : value.Trim();
        }

        private bool ValidateInput(bool kiemTraMa)
        {
            if (kiemTraMa)
            {
                if (string.IsNullOrWhiteSpace(txtMaNXB.Text))
                {
                    MessageBox.Show("Vui lòng nhập mã NXB!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMaNXB.Focus();
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(txtTenNXB.Text))
            {
                MessageBox.Show("Vui lòng nhập tên nhà xuất bản!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenNXB.Focus();
                return false;
            }
            return true;
        }

        // ================== CHỌN DÒNG ==================
        private void dgvNXB_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataRowView row = dgvNXB.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (row == null) return;

            maDangChon = row["MaNXB"].ToString();
            txtMaNXB.Text = row["MaNXB"].ToString();
            txtTenNXB.Text = row["TenNXB"].ToString();
            txtSDT.Text = row["SoDienThoai"] == DBNull.Value ? "" : row["SoDienThoai"].ToString();
            txtEmail.Text = row["Email"] == DBNull.Value ? "" : row["Email"].ToString();
            txtDiaChi.Text = row["DiaChi"] == DBNull.Value ? "" : row["DiaChi"].ToString();
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
                        "INSERT INTO dbo.NhaXuatBan (MaNXB, TenNXB, SoDienThoai, Email, DiaChi) " +
                        "VALUES (@Ma, @Ten, @SDT, @Email, @DiaChi)";
                    cmd.Parameters.AddWithValue("@Ma", txtMaNXB.Text.Trim());
                    cmd.Parameters.AddWithValue("@Ten", txtTenNXB.Text.Trim());
                    cmd.Parameters.AddWithValue("@SDT", NullIfEmpty(txtSDT.Text));
                    cmd.Parameters.AddWithValue("@Email", NullIfEmpty(txtEmail.Text));
                    cmd.Parameters.AddWithValue("@DiaChi", NullIfEmpty(txtDiaChi.Text));
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Thêm nhà xuất bản thành công!");
                LamMoi();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    if (ex.Message.Contains("PRIMARY KEY") || ex.Message.Contains("MaNXB"))
                        MessageBox.Show("Mã NXB đã tồn tại!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                        MessageBox.Show("Tên nhà xuất bản đã tồn tại!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Lỗi SQL: " + ex.Message);
                }
            }
        }

        // ================== SỬA ==================
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(maDangChon))
            {
                MessageBox.Show("Vui lòng chọn nhà xuất bản cần sửa trên bảng!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtMaNXB.Text.Trim() != maDangChon)
            {
                MessageBox.Show("Không được thay đổi mã NXB khi sửa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaNXB.Text = maDangChon;
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
                        "UPDATE dbo.NhaXuatBan SET TenNXB = @Ten, SoDienThoai = @SDT, Email = @Email, DiaChi = @DiaChi " +
                        "WHERE MaNXB = @Ma";
                    cmd.Parameters.AddWithValue("@Ma", maDangChon);
                    cmd.Parameters.AddWithValue("@Ten", txtTenNXB.Text.Trim());
                    cmd.Parameters.AddWithValue("@SDT", NullIfEmpty(txtSDT.Text));
                    cmd.Parameters.AddWithValue("@Email", NullIfEmpty(txtEmail.Text));
                    cmd.Parameters.AddWithValue("@DiaChi", NullIfEmpty(txtDiaChi.Text));

                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        MessageBox.Show("Không tìm thấy nhà xuất bản để sửa!");
                        return;
                    }
                }

                MessageBox.Show("Sửa nhà xuất bản thành công!");
                LamMoi();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                    MessageBox.Show("Tên nhà xuất bản đã tồn tại!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("Lỗi SQL: " + ex.Message);
            }
        }

        // ================== XÓA ==================
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(maDangChon))
            {
                MessageBox.Show("Vui lòng chọn nhà xuất bản cần xóa trên bảng!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Bạn có chắc muốn xóa nhà xuất bản \"" + txtTenNXB.Text + "\"?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            string ma = maDangChon;

            try
            {
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();

                    // Kiểm tra còn sách thuộc NXB này không
                    using (SqlCommand check = conn.CreateCommand())
                    {
                        check.CommandText =
                            "SELECT COUNT(*) FROM dbo.sach WHERE MaNXB = @ma";
                        check.Parameters.AddWithValue("@ma", ma);

                        if ((int)check.ExecuteScalar() > 0)
                        {
                            MessageBox.Show(
                                "Không thể xóa nhà xuất bản này vì đang có sách sử dụng!",
                                "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    using (SqlCommand del = conn.CreateCommand())
                    {
                        del.CommandText = "DELETE FROM dbo.NhaXuatBan WHERE MaNXB = @ma";
                        del.Parameters.AddWithValue("@ma", ma);
                        del.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Xóa nhà xuất bản thành công!");
                LamMoi();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi SQL: " + ex.Message);
            }
        }

        // ================== TÌM KIẾM ==================
        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            TimKiem();
        }

        private void TimKiem()
        {
            if (dangLamMoi) return;

            maDangChon = null;
            LoadData(txtTimKiem.Text);
        }

        // ================== LÀM MỚI ==================
        private void btnLamMoi_Click(object sender, EventArgs e)
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
        private void btnXuatFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Lưu File";
            sfd.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
            sfd.FileName = "NhaXuatBan.xlsx";

            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("NhaXuatBan");

                    // Bỏ cột STT để file xuất ra có thể nhập lại đúng thứ tự cột:
                    // MaNXB | TenNXB | SDT | Email | DiaChi
                    List<DataGridViewColumn> cols = new List<DataGridViewColumn>();
                    foreach (DataGridViewColumn c in dgvNXB.Columns)
                    {
                        if (c.Visible && c.Name != "colSTT") cols.Add(c);
                    }

                    for (int j = 0; j < cols.Count; j++)
                        ws.Cell(1, j + 1).Value = cols[j].HeaderText;

                    int dong = 2;
                    for (int i = 0; i < dgvNXB.Rows.Count; i++)
                    {
                        if (dgvNXB.Rows[i].IsNewRow) continue;

                        for (int j = 0; j < cols.Count; j++)
                        {
                            ws.Cell(dong, j + 1).Value =
                                Convert.ToString(dgvNXB.Rows[i].Cells[cols[j].Index].Value);
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
        // Thứ tự cột trong file: MaNXB | TenNXB | SDT | Email | DiaChi
        private bool ThemNXB(SqlConnection conn, string ma, string ten, string sdt, string email, string diaChi)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    IF NOT EXISTS (SELECT 1 FROM dbo.NhaXuatBan
                                   WHERE MaNXB = @ma OR TenNXB = @ten)
                    BEGIN
                        INSERT INTO dbo.NhaXuatBan (MaNXB, TenNXB, SoDienThoai, Email, DiaChi)
                        VALUES (@ma, @ten, @sdt, @email, @diachi);
                        SELECT 1;
                    END
                    ELSE
                        SELECT 0;";

                cmd.Parameters.Add("@ma", SqlDbType.NVarChar, 50).Value = ma;
                cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 200).Value = ten;
                cmd.Parameters.Add("@sdt", SqlDbType.NVarChar, 20).Value =
                    string.IsNullOrWhiteSpace(sdt) ? (object)DBNull.Value : sdt;
                cmd.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value =
                    string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email;
                cmd.Parameters.Add("@diachi", SqlDbType.NVarChar, 255).Value =
                    string.IsNullOrWhiteSpace(diaChi) ? (object)DBNull.Value : diaChi;

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
                        sheet.Cell(dong, 5).GetString().Trim()
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

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                string[] cot = TachDongCsv(lines[i], sep);
                rows.Add(new string[]
                {
                    cot.Length > 0 ? cot[0] : "",
                    cot.Length > 1 ? cot[1] : "",
                    cot.Length > 2 ? cot[2] : "",
                    cot.Length > 3 ? cot[3] : "",
                    cot.Length > 4 ? cot[4] : ""
                });
            }
            return rows;
        }

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
                int trung = 0;      // mã hoặc tên NXB đã tồn tại
                int saiDuLieu = 0;  // thiếu mã hoặc thiếu tên

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

                            // Thiếu mã hoặc tên -> thông tin sai
                            if (string.IsNullOrWhiteSpace(r[0]) ||
                                string.IsNullOrWhiteSpace(r[1]))
                            {
                                saiDuLieu++;
                                continue;
                            }

                            try
                            {
                                if (ThemNXB(conn, r[0], r[1], r[2], r[3], r[4]))
                                    thanhCong++;
                                else
                                    trung++;
                            }
                            catch (SqlException ex)
                            {
                                // 8152: chuỗi quá dài so với cột (mã/tên/SDT/email/địa chỉ) -> thông tin sai
                                if (ex.Number == 8152) saiDuLieu++;
                                else if (ex.Number == 2627 || ex.Number == 2601) trung++;
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