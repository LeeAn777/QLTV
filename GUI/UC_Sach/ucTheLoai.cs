using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using System.IO;


namespace QLTV.GUI.UC_Sach
{
    public partial class ucTheLoai : UserControl
    {
        private readonly string str =@"Data Source=.\SQLEXPRESS;Initial Catalog=QLThuVien;Integrated Security=True";

        private readonly DataTable tb = new DataTable();

        // Mã thể loại của dòng đang được chọn (dùng cho Sửa / Xóa)
        private int? maDangChon = null;

        private bool dangLamMoi = false;

        public ucTheLoai()
        {
            InitializeComponent();
        }

        // ================== LOAD ==================
        private void ucTheLoai_Load(object sender, EventArgs e)
        {
            dgvTheLoai.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9.0F, FontStyle.Bold);

            // Đảm bảo sự kiện chọn dòng chỉ được gắn một lần
            dgvTheLoai.CellClick -= dgvTheLoai_CellClick;
            dgvTheLoai.CellClick += dgvTheLoai_CellClick;

            LoadData();
        }

        // keyword rỗng/null: hiển thị tất cả; có giá trị: tìm theo mã, tên hoặc mô tả
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
                            "SELECT MaTheLoai, TenTheLoai, MoTa FROM dbo.TheLoai " +
                            "ORDER BY MaTheLoai";
                    }
                    else
                    {
                        cmd.CommandText =
                            "SELECT MaTheLoai, TenTheLoai, MoTa FROM dbo.TheLoai " +
                            "WHERE CAST(MaTheLoai AS NVARCHAR(20)) LIKE @kw " +
                            "OR TenTheLoai LIKE @kw OR MoTa LIKE @kw " +
                            "ORDER BY MaTheLoai";
                        cmd.Parameters.AddWithValue("@kw", "%" + keyword.Trim() + "%");
                    }

                    adapter.SelectCommand = cmd;
                    tb.Clear();
                    adapter.Fill(tb);
                }

                dgvTheLoai.AutoGenerateColumns = true;
                dgvTheLoai.DataSource = tb;

                if (dgvTheLoai.Columns.Contains("MaTheLoai"))
                    dgvTheLoai.Columns["MaTheLoai"].HeaderText = "Mã thể loại";
                if (dgvTheLoai.Columns.Contains("TenTheLoai"))
                    dgvTheLoai.Columns["TenTheLoai"].HeaderText = "Tên thể loại";
                if (dgvTheLoai.Columns.Contains("MoTa"))
                    dgvTheLoai.Columns["MoTa"].HeaderText = "Mô tả";
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        // ================== HÀM PHỤ ==================
        private void ClearInput()
        {
            maDangChon = null;
            txtMaTheLoai.Clear();
            txtTenTheLoai.Clear();
            txtMoTa.Clear();
            txtMaTheLoai.Focus();
        }

        private object MoTaValue()
        {
            return string.IsNullOrWhiteSpace(txtMoTa.Text)
                ? (object)DBNull.Value
                : txtMoTa.Text.Trim();
        }

        private bool ValidateInput(bool kiemTraMa)
        {
            if (kiemTraMa)
            {
                int ma;
                if (!int.TryParse(txtMaTheLoai.Text.Trim(), out ma) || ma <= 0)
                {
                    MessageBox.Show("Mã thể loại phải là số nguyên dương!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMaTheLoai.Focus();
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(txtTenTheLoai.Text))
            {
                MessageBox.Show("Vui lòng nhập tên thể loại!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenTheLoai.Focus();
                return false;
            }
            return true;
        }

  
        // ================== LÀM MỚI ==================
        // Gắn vào sự kiện Click của nút Làm mới
        

        // ================== CHỌN DÒNG -> ĐỔ LÊN TEXTBOX ==================
        private void dgvTheLoai_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataRowView row = dgvTheLoai.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (row == null) return;

            maDangChon = Convert.ToInt32(row["MaTheLoai"]);
            txtMaTheLoai.Text = row["MaTheLoai"].ToString();
            txtTenTheLoai.Text = row["TenTheLoai"].ToString();
            txtMoTa.Text = row["MoTa"] == DBNull.Value ? "" : row["MoTa"].ToString();
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
                    cmd.CommandText =
                        "INSERT INTO dbo.TheLoai (MaTheLoai, TenTheLoai, MoTa) " +
                        "VALUES (@Ma, @Ten, @MoTa)";
                    cmd.Parameters.AddWithValue("@Ma", Convert.ToInt32(txtMaTheLoai.Text.Trim()));
                    cmd.Parameters.AddWithValue("@Ten", txtTenTheLoai.Text.Trim());
                    cmd.Parameters.AddWithValue("@MoTa", MoTaValue());
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Thêm thể loại thành công!");
                LamMoi();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    // Trùng khóa chính (mã) hay trùng cột UNIQUE (tên)?
                    if (ex.Message.Contains("PRIMARY KEY"))
                        MessageBox.Show("Mã thể loại đã tồn tại!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                        MessageBox.Show("Tên thể loại đã tồn tại!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Lỗi SQL: " + ex.Message);
                }
            }
        }

        // ================== SỬA ==================
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (maDangChon == null)
            {
                MessageBox.Show("Vui lòng chọn thể loại cần sửa trên bảng!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Mã là khóa chính, không cho đổi khi sửa
            if (txtMaTheLoai.Text.Trim() != maDangChon.Value.ToString())
            {
                MessageBox.Show("Không được thay đổi mã thể loại khi sửa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaTheLoai.Text = maDangChon.Value.ToString();
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
                        "UPDATE dbo.TheLoai SET TenTheLoai = @Ten, MoTa = @MoTa " +
                        "WHERE MaTheLoai = @Ma";
                    cmd.Parameters.AddWithValue("@Ma", maDangChon.Value);
                    cmd.Parameters.AddWithValue("@Ten", txtTenTheLoai.Text.Trim());
                    cmd.Parameters.AddWithValue("@MoTa", MoTaValue());

                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        MessageBox.Show("Không tìm thấy thể loại để sửa!");
                        return;
                    }
                }

                MessageBox.Show("Sửa thể loại thành công!");
                LamMoi();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                    MessageBox.Show("Tên thể loại đã tồn tại!", "Thông báo",
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
                MessageBox.Show("Vui lòng chọn thể loại cần xóa trên bảng!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Bạn có chắc muốn xóa thể loại \"" + txtTenTheLoai.Text + "\"?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            int ma = maDangChon.Value;

            try
            {
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();

                    // Kiểm tra còn sách thuộc thể loại này không
                    using (SqlCommand check = conn.CreateCommand())
                    {
                        check.CommandText =
                            "SELECT COUNT(*) FROM dbo.sach WHERE MaTheLoai = @ma";
                        check.Parameters.AddWithValue("@ma", ma);

                        if ((int)check.ExecuteScalar() > 0)
                        {
                            MessageBox.Show(
                                "Không thể xóa thể loại này vì đang có sách sử dụng!",
                                "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    using (SqlCommand del = conn.CreateCommand())
                    {
                        del.CommandText = "DELETE FROM dbo.TheLoai WHERE MaTheLoai = @ma";
                        del.Parameters.AddWithValue("@ma", ma);
                        del.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Xóa thể loại thành công!");
                LamMoi();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi SQL: " + ex.Message);
            }
        }
        // ================== TÌM KIẾM ==================
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            TimKiem();
        }

        private void guna2Button1_TextChanged(object sender, EventArgs e)
        {
            TimKiem();
        }
        private void TimKiem()
        {
            if (dangLamMoi) return;

            maDangChon = null;
            LoadData(txtTimKiem.Text);
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            LamMoi();
        }
        private void LamMoi()
        {
            dangLamMoi = true;   // tránh tìm lặp khi xóa chữ trong ô tìm kiếm
            txtTimKiem.Clear();
            dangLamMoi = false;

            ClearInput();
            LoadData();
        }
        //================Xuất File====================
        private void guna2Button6_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Lưu File";
            sfd.Filter = "Excel Workbook (.xlsx)|.xlsx";
            sfd.FileName = "TheloaiSach.xlsx";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("TheLoai");

                    // Xuất tiêu đề cột
                    for (int i = 0; i < dgvTheLoai.Columns.Count; i++)
                    {
                        if (dgvTheLoai.Columns[i].Visible)
                        {
                            ws.Cell(1, i + 1).Value = dgvTheLoai.Columns[i].HeaderText;
                        }
                    }

                    // Xuất dữ liệu
                    for (int i = 0; i < dgvTheLoai.Rows.Count; i++)
                    {
                        if (!dgvTheLoai.Rows[i].IsNewRow)
                        {
                            for (int j = 0; j < dgvTheLoai.Columns.Count; j++)
                            {
                                if (dgvTheLoai.Columns[j].Visible)
                                {
                                    ws.Cell(i + 2, j + 1).Value =
                                        Convert.ToString(
                                            dgvTheLoai.Rows[i].Cells[j].Value);
                                }
                            }
                        }
                    }

                    ws.Columns().AdjustToContents();
                    wb.SaveAs(sfd.FileName);
                }

                MessageBox.Show("Xuất Excel thành công!");
            }
        }
    
    private bool ThemTheLoai(SqlConnection conn, int ma, string ten, string moTa)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    IF NOT EXISTS (SELECT 1 FROM dbo.TheLoai
                                   WHERE MaTheLoai = @ma OR TenTheLoai = @ten)
                    BEGIN
                        INSERT INTO dbo.TheLoai (MaTheLoai, TenTheLoai, MoTa)
                        VALUES (@ma, @ten, @mota);
                        SELECT 1;
                    END
                    ELSE
                        SELECT 0;";

                cmd.Parameters.Add("@ma", SqlDbType.Int).Value = ma;
                cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 100).Value = ten;
                cmd.Parameters.Add("@mota", SqlDbType.NVarChar, 255).Value =
                    string.IsNullOrWhiteSpace(moTa) ? (object)DBNull.Value : moTa;

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
                        sheet.Cell(dong, 3).GetString().Trim()
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
                rows.Add(new string[]
                {
                    cot.Length > 0 ? cot[0] : "",
                    cot.Length > 1 ? cot[1] : "",
                    cot.Length > 2 ? cot[2] : ""
                });
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
                int trung = 0;   // mã hoặc tên đã tồn tại
                int saiDuLieu = 0; // thiếu tên hoặc mã không hợp lệ

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
                            if (string.IsNullOrWhiteSpace(r[0]) &&
                                string.IsNullOrWhiteSpace(r[1]) &&
                                string.IsNullOrWhiteSpace(r[2]))
                                continue;

                            int ma;
                            if (!int.TryParse(r[0], out ma) || ma <= 0 ||
                                string.IsNullOrWhiteSpace(r[1]))
                            {
                                saiDuLieu++;
                                continue;
                            }

                            if (ThemTheLoai(conn, ma, r[1], r[2]))
                                thanhCong++;
                            else
                                trung++;
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
