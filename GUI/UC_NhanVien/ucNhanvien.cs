using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ex_cel = Microsoft.Office.Interop.Excel;

namespace QLTV.GUI.UC_Nhanvien
{
    public partial class ucNhanvien : UserControl
    {
        public ucNhanvien()
        {
            InitializeComponent();
        }
        SqlConnection con = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=QLThuVien;Integrated Security=True");

        private void opencon(
            )
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
        }
        private void closecon()
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
        private void load_data()
        {
            opencon();
            string sql = "select * from Nhanvien";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            cmd.Dispose();
            dgvNhanVien.DataSource = dt;
            closecon();

        }
        private Boolean checknull(string dl)
        {
            if (dl == "" || dl == null)
            {
                return true;

            }
            return false;
        }

        private string checktrung(string sodienthoai, string email)
        {
            string sql = @"SELECT sodienthoai, email
                   FROM dbo.nhanvien
                   WHERE sodienthoai = @sdt OR email = @email";

            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@sdt", sodienthoai);
                cmd.Parameters.AddWithValue("@email", email);

                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                foreach (DataRow row in dt.Rows)
                {
                    if (row["sodienthoai"].ToString() == sodienthoai)
                        return "Số điện thoại đã tồn tại";

                    if (row["email"].ToString().Equals(email, StringComparison.OrdinalIgnoreCase))
                        return "Email đã tồn tại";
                }
            }

            return null;
        }

        private Boolean checkemail(string email)
        {
            if (email.Contains("@") && email.Contains("."))
            {
                return true;
            }
            return false;
        }

        private void ucNhanvien_Load(object sender, EventArgs e)
        {
            dgvNhanVien.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.0F, FontStyle.Bold);
            dgvNhanVien.AutoGenerateColumns = false;

            cbtrangthai.Items.Clear();
            cbtrangthai.Items.Add("Đang làm");
            cbtrangthai.Items.Add("Nghỉ việc");
            cbtrangthai.DropDownStyle = ComboBoxStyle.DropDownList; 

            load_data();
        }

        private void btthem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMaNhanVien.Text.Trim()))
            {
                MessageBox.Show("Bạn đang chọn một nhân viên cũ. Vui lòng bấm 'Làm mới' trước khi thêm nhân viên mới!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string tennv = txttennhanvien.Text.Trim();
            string chucvu = txtchucvu.Text.Trim();
            string sdt = txtsodienthoai.Text.Trim();
            string email = txtemail.Text.Trim();
            string diachi = txtdiachi.Text.Trim();
            string trangthai = cbtrangthai.Text.Trim();

            if (checknull(tennv) || checknull(chucvu) || checknull(sdt) ||
                checknull(email) || checknull(diachi) || checknull(trangthai))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; 
            }

            if (!checkemail(email))
            {
                MessageBox.Show("Email không hợp lệ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string trung = checktrung(sdt, email);
            if (trung != null)
            {
                MessageBox.Show(trung, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                opencon();
                string sql = "INSERT INTO dbo.nhanvien (HoTen, ChucVu, SoDienThoai, Email, DiaChi, TrangThai) " +
                             "VALUES (N'" + tennv + "', N'" + chucvu + "', '" + sdt + "', '" + email + "', N'" + diachi + "', N'" + trangthai + "')";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                MessageBox.Show("Thêm nhân viên thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                load_data();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                closecon();
            }
        }



        private void btsua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaNhanVien.Text.Trim()))
            {
                MessageBox.Show("Vui lòng click chọn một nhân viên dưới bảng để sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string manv = txtMaNhanVien.Text.Trim();
            string tennv = txttennhanvien.Text.Trim();
            string chucvu = txtchucvu.Text.Trim();
            string sdt = txtsodienthoai.Text.Trim();
            string email = txtemail.Text.Trim();
            string diachi = txtdiachi.Text.Trim();
            string trangthai = cbtrangthai.Text.Trim();

            try
            {
                opencon();
                string sql = @"UPDATE dbo.nhanvien 
                       SET HoTen = @ten, ChucVu = @chucvu, SoDienThoai = @sdt, 
                           Email = @email, DiaChi = @diachi, TrangThai = @trangthai 
                       WHERE MaNV = @manv";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@manv", manv);
                    cmd.Parameters.AddWithValue("@ten", tennv);
                    cmd.Parameters.AddWithValue("@chucvu", chucvu);
                    cmd.Parameters.AddWithValue("@sdt", sdt);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@diachi", diachi);
                    cmd.Parameters.AddWithValue("@trangthai", trangthai);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Sửa thông tin nhân viên thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                load_data();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi sửa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                closecon();
            }
        }

        private void dgvNhanVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvNhanVien.Rows[e.RowIndex];

                txtMaNhanVien.Text = row.Cells["manhanvien"].Value.ToString();
                txttennhanvien.Text = row.Cells["hoten"].Value.ToString();
                txtchucvu.Text = row.Cells["chucvu"].Value.ToString();
                txtsodienthoai.Text = row.Cells["sodienthoai"].Value.ToString();
                txtemail.Text = row.Cells["email"].Value.ToString();
                txtdiachi.Text = row.Cells["diachi"].Value.ToString();
                cbtrangthai.SelectedItem = row.Cells["trangthai"].Value.ToString();
            }
        }

        private void btxoa_Click(object sender, EventArgs e)
        {
            string manv = txtMaNhanVien.Text.Trim();
            if (manv == "")
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần xóa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;

            }
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa nhân viên này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if
               (result == DialogResult.Yes)
            {
                string cmd = "DELETE FROM dbo.nhanvien WHERE MaNV = '" + manv + "'";
                opencon();
                SqlCommand sqlCommand = new SqlCommand(cmd, con);
                sqlCommand.ExecuteNonQuery();
                sqlCommand.Dispose();
                MessageBox.Show("Xóa nhân viên thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                load_data();
                closecon();
            }
        }

        private void btlammoi_Click(object sender, EventArgs e)
        {
            txtMaNhanVien.Clear();
            txttennhanvien.Clear();
            txtsodienthoai.Clear();
            txtemail.Clear();
            txtchucvu.Clear();
            txtdiachi.Clear();
            if (cbtrangthai.Items.Count > 0)
            {
                cbtrangthai.SelectedIndex = 0;
            }
        }

        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            string timkiem = txtTimKiem.Text.Trim();
            string query = "SELECT * FROM nhanvien WHERE HoTen LIKE N'%" + timkiem + "%'or ChucVu LIKE N'%" + timkiem + "%'";
            opencon();
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvNhanVien.DataSource = dt;
            closecon();

        }

        private void dgvNhanVien_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            dgvNhanVien.Rows[e.RowIndex].Cells["stt"].Value = (e.RowIndex + 1).ToString();
        }

        private void btxuatfile_Click(object sender, EventArgs e)
        {
            string sheetname = "DanhSachNhanVien";

            // Khởi tạo các đối tượng Excel
            ex_cel.Application oExcel = new ex_cel.Application();
            oExcel.Visible = true;
            oExcel.DisplayAlerts = false;
            oExcel.Application.SheetsInNewWorkbook = 1;

            ex_cel.Workbooks oBooks = oExcel.Workbooks;
            ex_cel.Workbook oBook = (ex_cel.Workbook)(oExcel.Workbooks.Add(Type.Missing));
            ex_cel.Sheets oSheets = oBook.Worksheets;
            ex_cel.Worksheet oSheet = (ex_cel.Worksheet)oSheets.get_Item(1);
            oSheet.Name = sheetname;

            // --- TẠO TIÊU ĐỀ BẢNG (Từ A1:H1 vì có 8 cột) ---
            ex_cel.Range head = oSheet.get_Range("A1", "H1");
            head.MergeCells = true;
            head.Value2 = "DANH SÁCH NHÂN VIÊN";
            head.Font.Bold = true;
            head.Font.Name = "Tahoma";
            head.Font.Size = 16;
            head.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;

            // --- CẤU HÌNH CÁC CỘT TIÊU ĐỀ (ROW 3) ---
            oSheet.get_Range("A3", "A3").Value2 = "STT";
            oSheet.get_Range("A3", "A3").ColumnWidth = 8;

            oSheet.get_Range("B3", "B3").Value2 = "MÃ NV";
            oSheet.get_Range("B3", "B3").ColumnWidth = 10;

            oSheet.get_Range("C3", "C3").Value2 = "HỌ VÀ TÊN";
            oSheet.get_Range("C3", "C3").ColumnWidth = 25;

            oSheet.get_Range("D3", "D3").Value2 = "CHỨC VỤ";
            oSheet.get_Range("D3", "D3").ColumnWidth = 15;

            oSheet.get_Range("E3", "E3").Value2 = "SỐ ĐIỆN THOẠI";
            // Ép kiểu Text cho SĐT để không bị mất số 0 ở đầu
            oSheet.get_Range("E3", "E3").EntireColumn.NumberFormat = "@";
            oSheet.get_Range("E3", "E3").ColumnWidth = 15;

            oSheet.get_Range("F3", "F3").Value2 = "EMAIL";
            oSheet.get_Range("F3", "F3").ColumnWidth = 25;

            oSheet.get_Range("G3", "G3").Value2 = "ĐỊA CHỈ";
            oSheet.get_Range("G3", "G3").ColumnWidth = 30;

            oSheet.get_Range("H3", "H3").Value2 = "TRẠNG THÁI";
            oSheet.get_Range("H3", "H3").ColumnWidth = 15;

            // Định dạng Header Row (A3 đến H3)
            ex_cel.Range rowHead = oSheet.get_Range("A3", "H3");
            rowHead.Font.Bold = true;
            rowHead.Borders.LineStyle = ex_cel.Constants.xlSolid;
            rowHead.Interior.ColorIndex = 15; // Màu xám nhẹ
            rowHead.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;

            // --- CHUYỂN DỮ LIỆU TỪ DATAGRIDVIEW VÀO MẢNG ---
            int rowCount = dgvNhanVien.Rows.Count;
            if (dgvNhanVien.AllowUserToAddRows) rowCount--; // Trừ đi dòng trống cuối cùng nếu có

            // Mảng 8 cột (STT, MaNV, HoTen, ChucVu, SoDienThoai, Email, DiaChi, TrangThai)
            object[,] arr = new object[rowCount, 8];

            for (int r = 0; r < rowCount; r++)
            {
                DataGridViewRow dr = dgvNhanVien.Rows[r];

                arr[r, 0] = r + 1; // STT
                arr[r, 1] = dr.Cells["MaNV"].Value?.ToString();
                arr[r, 2] = dr.Cells["HoTen"].Value?.ToString();
                arr[r, 3] = dr.Cells["ChucVu"].Value?.ToString();
                arr[r, 4] = dr.Cells["SoDienThoai"].Value?.ToString();
                arr[r, 5] = dr.Cells["Email"].Value?.ToString();
                arr[r, 6] = dr.Cells["DiaChi"].Value?.ToString();
                arr[r, 7] = dr.Cells["TrangThai"].Value?.ToString();
            }

            // --- ĐỔ DỮ LIỆU VÀO EXCEL ---
            int rowStart = 4;
            int columnStart = 1;
            int rowEnd = rowStart + rowCount - 1;
            int columnEnd = 8; // Đổi thành 8 cột

            ex_cel.Range c1 = (ex_cel.Range)oSheet.Cells[rowStart, columnStart];
            ex_cel.Range c2 = (ex_cel.Range)oSheet.Cells[rowEnd, columnEnd];
            ex_cel.Range range = oSheet.get_Range(c1, c2);

            // Đổ mảng dữ liệu vào Excel
            range.Value2 = arr;

            // Kẻ viền cho toàn bộ vùng dữ liệu
            range.Borders.LineStyle = ex_cel.Constants.xlSolid;

            // 1. Căn giữa toàn bộ vùng dữ liệu
            range.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = ex_cel.XlVAlign.xlVAlignCenter;

            // 2. Căn trái cho các cột chữ dài (Họ tên, Email, Địa chỉ) để dễ đọc
            ex_cel.Range colHoTen = oSheet.get_Range("C" + rowStart, "C" + rowEnd);
            colHoTen.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignLeft;

            ex_cel.Range colEmail = oSheet.get_Range("F" + rowStart, "F" + rowEnd);
            colEmail.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignLeft;

            ex_cel.Range colDiaChi = oSheet.get_Range("G" + rowStart, "G" + rowEnd);
            colDiaChi.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignLeft;

            MessageBox.Show("Xuất file Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnhapfile_Click(object sender, EventArgs e)
        {
            // Mở hộp thoại chọn file Excel
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            openFileDialog.Title = "Chọn file Excel danh sách nhân viên";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ReadExcelNhanVien(openFileDialog.FileName);

                // Load lại DataGridView sau khi nhập xong để hiển thị dữ liệu mới
                load_data();
            }
        }
        private void ReadExcelNhanVien(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                MessageBox.Show("Chưa chọn file Excel!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ex_cel.Application excelApp = null;
            ex_cel.Workbooks oBooks = null;
            ex_cel.Workbook oBook = null;

            try
            {
                excelApp = new ex_cel.Application();
                excelApp.Visible = false; // Chạy ẩn Excel ngầm

                oBooks = excelApp.Workbooks;
                oBook = oBooks.Open(filename);

                // Mở kết nối CSDL bằng hàm opencon() của bạn
                opencon();

                int thanhCong = 0;
                int loi = 0;

                foreach (ex_cel.Worksheet wsheet in oBook.Worksheets)
                {
                    int i = 4; // Bắt đầu đọc từ dòng 4 (vì dòng 1-3 là tiêu đề)
                    while (true)
                    {
                        // LẤY DỮ LIỆU TỪ CÁC CỘT TRONG EXCEL (Bỏ qua cột A - STT và cột B - Mã NV)
                        string hoten = wsheet.Cells[i, 3].Value?.ToString()?.Trim();     // Cột C: Họ tên

                        // Nếu Họ tên trống thì coi như đọc hết dữ liệu trong Sheet
                        if (string.IsNullOrEmpty(hoten))
                        {
                            break;
                        }

                        string chucvu = wsheet.Cells[i, 4].Value?.ToString()?.Trim() ?? "";     // Cột D: Chức vụ
                        string sdt = wsheet.Cells[i, 5].Value?.ToString()?.Trim() ?? "";        // Cột E: SĐT
                        string email = wsheet.Cells[i, 6].Value?.ToString()?.Trim() ?? "";      // Cột F: Email
                        string diachi = wsheet.Cells[i, 7].Value?.ToString()?.Trim() ?? "";     // Cột G: Địa chỉ
                        string trangthai = wsheet.Cells[i, 8].Value?.ToString()?.Trim() ?? "Đang làm"; // Cột H: Trạng thái

                        // Thực hiện thêm vào CSDL bảng nhanvien
                        try
                        {
                            // Lệnh INSERT KHÔNG có MaNV để SQL tự động sinh ID mới
                            string sql = @"INSERT INTO dbo.nhanvien (HoTen, ChucVu, SoDienThoai, Email, DiaChi, TrangThai) 
                                   VALUES (@ten, @chucvu, @sdt, @email, @diachi, @trangthai)";

                            using (SqlCommand cmd = new SqlCommand(sql, con))
                            {
                                cmd.Parameters.AddWithValue("@ten", hoten);
                                cmd.Parameters.AddWithValue("@chucvu", chucvu);
                                cmd.Parameters.AddWithValue("@sdt", sdt);
                                cmd.Parameters.AddWithValue("@email", email);
                                cmd.Parameters.AddWithValue("@diachi", diachi);
                                cmd.Parameters.AddWithValue("@trangthai", trangthai);

                                cmd.ExecuteNonQuery();
                                thanhCong++; // Chèn thành công
                            }
                        }
                        catch (Exception)
                        {
                            // Nếu lỗi (Ví dụ trùng Email - do cột Email có UNIQUE) thì đếm vào lỗi
                            loi++;
                        }

                        i++; // Xuống dòng tiếp theo trong Excel
                    }
                }

                MessageBox.Show($"Nhập dữ liệu hoàn tất!\n- Thành công: {thanhCong} dòng\n- Lỗi/Trùng lặp: {loi} dòng",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi đọc file Excel: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 1. Đóng kết nối SQL
                closecon();

                // 2. Dọn dẹp và giải phóng bộ nhớ Excel triệt để
                if (oBook != null)
                {
                    oBook.Close(false);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oBook);
                }
                if (oBooks != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oBooks);
                }
                if (excelApp != null)
                {
                    excelApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                }
            }
        }
    }
}
