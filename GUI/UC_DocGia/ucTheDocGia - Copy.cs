using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace QLTV.GUI.UC_DocGia
{
    public partial class    ucTheDG : UserControl
    {
        private string connectionString =
            @"Data Source=.\SQLEXPRESS;Initial Catalog=QLThuVien;Integrated Security=True";
        public ucTheDG()
        {
            InitializeComponent();
        }

        private void ucTheDG_Load(object sender, EventArgs e)
        {
            dgvthedg.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.0F, FontStyle.Bold);
            dgvthedg.AutoGenerateColumns = false;

            dgvthedg.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvthedg.MultiSelect = false;
            dgvthedg.ReadOnly = true;
            dgvthedg.AllowUserToAddRows = false;

            cboxtrangthai.Items.Clear();
            cboxtrangthai.Items.Add("Hoạt động");
            cboxtrangthai.Items.Add("Hết hạn");
            cboxtrangthai.Items.Add("Bị khóa");


            cboxtrangthai.DropDownStyle = ComboBoxStyle.DropDownList;
            hienthi();

        }
        private void hienthi()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = @"
                        SELECT 
                            MaThe,
                            MaDocGia,
                            NgayCap,
                            NgayHetHan,
                            TrangThai
                        FROM dbo.thedocgia
                        ORDER BY MaThe";

                    SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvthedg.DataSource = dt;

                    // Đặt tiêu đề các cột
                    if (dgvthedg.Columns["MaThe"] != null)
                        dgvthedg.Columns["MaThe"].HeaderText = "Mã thẻ";

                    if (dgvthedg.Columns["MaDocGia"] != null)
                        dgvthedg.Columns["MaDocGia"].HeaderText = "Mã độc giả";

                    if (dgvthedg.Columns["NgayCap"] != null)
                    {
                        dgvthedg.Columns["NgayCap"].HeaderText = "Ngày cấp";
                        dgvthedg.Columns["NgayCap"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    }

                    if (dgvthedg.Columns["NgayHetHan"] != null)
                    {
                        dgvthedg.Columns["NgayHetHan"].HeaderText = "Ngày hết hạn";
                        dgvthedg.Columns["NgayHetHan"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    }

                    if (dgvthedg.Columns["TrangThai"] != null)
                        dgvthedg.Columns["TrangThai"].HeaderText = "Trạng thái";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị dữ liệu: " + ex.Message);
            }
        }
        private bool kiemTraDuLieu()
        {
            if (string.IsNullOrWhiteSpace(txtmadocgia.Text))
            {
                MessageBox.Show("Vui lòng nhập mã độc giả!");
                txtmadocgia.Focus();
                return false;
            }

            int maDocGia;
            if (!int.TryParse(txtmadocgia.Text.Trim(), out maDocGia))
            {
                MessageBox.Show("Mã độc giả phải là số nguyên!");
                txtmadocgia.Focus();
                return false;
            }

            if (cboxtrangthai.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn trạng thái thẻ!");
                cboxtrangthai.Focus();
                return false;
            }

            if (start.Value.Date >= end.Value.Date)
            {
                MessageBox.Show("Ngày hết hạn phải sau ngày cấp!");
                return false;
            }

            return true;
        }

        private void btnthem_Click(object sender, EventArgs e)
        {
            if (!kiemTraDuLieu())
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = @"
                        INSERT INTO dbo.thedocgia
                            (MaDocGia, NgayCap, NgayHetHan, TrangThai)
                        VALUES
                            (@MaDocGia, @NgayCap, @NgayHetHan, @TrangThai)";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@MaDocGia", SqlDbType.Int)
                            .Value = int.Parse(txtmadocgia.Text.Trim());

                        cmd.Parameters.Add("@NgayCap", SqlDbType.Date)
                            .Value = start.Value.Date;

                        cmd.Parameters.Add("@NgayHetHan", SqlDbType.Date)
                            .Value = end.Value.Date;

                        cmd.Parameters.Add("@TrangThai", SqlDbType.NVarChar, 20)
                            .Value = cboxtrangthai.Text;

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Thêm thẻ độc giả thành công!");
                hienthi();
                lamMoi();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547)
                    MessageBox.Show("Mã độc giả không tồn tại hoặc dữ liệu vi phạm ràng buộc khóa ngoại/ngày.");
                else
                    MessageBox.Show("Lỗi SQL: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm thẻ: " + ex.Message);
            }
        }

        private void btnsua_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtmathe.Text))
            {
                MessageBox.Show("Vui lòng chọn thẻ cần sửa!");
                return;
            }

            if (!kiemTraDuLieu())
                return;

            int maThe;
            if (!int.TryParse(txtmathe.Text.Trim(), out maThe))
            {
                MessageBox.Show("Mã thẻ không hợp lệ!");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = @"
                        UPDATE dbo.thedocgia
                        SET MaDocGia = @MaDocGia,
                            NgayCap = @NgayCap,
                            NgayHetHan = @NgayHetHan,
                            TrangThai = @TrangThai
                        WHERE MaThe = @MaThe";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@MaThe", SqlDbType.Int).Value = maThe;
                        cmd.Parameters.Add("@MaDocGia", SqlDbType.Int)
                            .Value = int.Parse(txtmadocgia.Text.Trim());
                        cmd.Parameters.Add("@NgayCap", SqlDbType.Date)
                            .Value = start.Value.Date;
                        cmd.Parameters.Add("@NgayHetHan", SqlDbType.Date)
                            .Value = end.Value.Date;
                        cmd.Parameters.Add("@TrangThai", SqlDbType.NVarChar, 20)
                            .Value = cboxtrangthai.Text;

                        conn.Open();

                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            MessageBox.Show("Không tìm thấy thẻ cần sửa!");
                            return;
                        }
                    }
                }

                MessageBox.Show("Sửa thẻ độc giả thành công!");
                hienthi();
                lamMoi();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547)
                    MessageBox.Show("Mã độc giả không tồn tại hoặc dữ liệu vi phạm ràng buộc.");
                else
                    MessageBox.Show("Lỗi SQL: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi sửa thẻ: " + ex.Message);
            }
        }

        private void btnxoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtmathe.Text))
            {
                MessageBox.Show("Vui lòng chọn thẻ cần xóa!");
                return;
            }

            int maThe;
            if (!int.TryParse(txtmathe.Text.Trim(), out maThe))
            {
                MessageBox.Show("Mã thẻ không hợp lệ!");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Bạn có chắc muốn xóa thẻ độc giả này?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = "DELETE FROM dbo.thedocgia WHERE MaThe = @MaThe";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@MaThe", SqlDbType.Int).Value = maThe;

                        conn.Open();

                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            MessageBox.Show("Không tìm thấy thẻ cần xóa!");
                            return;
                        }
                    }
                }

                MessageBox.Show("Xóa thẻ độc giả thành công!");
                hienthi();
                lamMoi();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547)
                    MessageBox.Show("Không thể xóa thẻ vì đang được bảng khác tham chiếu.");
                else
                    MessageBox.Show("Lỗi SQL: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xóa thẻ: " + ex.Message);
            }
        }

        private void btntimkiem_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = @"
                        SELECT MaThe, MaDocGia, NgayCap, NgayHetHan, TrangThai
                        FROM dbo.thedocgia
                        WHERE CAST(MaThe AS NVARCHAR(20)) LIKE @TuKhoa
                           OR CAST(MaDocGia AS NVARCHAR(20)) LIKE @TuKhoa
                           OR TrangThai LIKE @TuKhoa
                        ORDER BY MaThe";

                    using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                    {
                        da.SelectCommand.Parameters.Add("@TuKhoa", SqlDbType.NVarChar, 100)
                            .Value = "%" + txttimkiem.Text.Trim() + "%";

                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgvthedg.DataSource = dt;
                    }
                }

                if (dgvthedg.Rows.Count == 0)
                    MessageBox.Show("Không tìm thấy thẻ phù hợp.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message);
            }
        }

        private void btnlammoi_Click(object sender, EventArgs e)
        {
            lamMoi();
            hienthi();
        }

        private void lamMoi()
        {
            txtmathe.Clear();
            txtmadocgia.Clear();
            txttimkiem.Clear();

            if (cboxtrangthai.Items.Count > 0)
                cboxtrangthai.SelectedIndex = 0;

            start.Value = DateTime.Today;
            end.Value = DateTime.Today.AddYears(1);

            dgvthedg.ClearSelection();
        }

        private void btnlammoi_Click_1(object sender, EventArgs e)
        {
            txttimkiem.Clear();

            hienthi();

            dgvthedg.ClearSelection();

            txttimkiem.Focus();
        }

        private void dgvthedg_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = dgvthedg.Rows[e.RowIndex];

            txtmathe.Text = row.Cells["MaThe"].Value.ToString();
            txtmadocgia.Text = row.Cells["MaDocGia"].Value.ToString();

            start.Value = Convert.ToDateTime(row.Cells["NgayCap"].Value);

            end.Value = Convert.ToDateTime(row.Cells["NgayHetHan"].Value);
            cboxtrangthai.SelectedValue = row.Cells["TrangThai"].Value?.ToString();
             
        }

        private void dgvthedg_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            dgvthedg.Rows[e.RowIndex].Cells["colSTT"].Value = (e.RowIndex + 1).ToString();
        }
    }
}

