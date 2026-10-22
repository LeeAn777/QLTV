using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace QLTV.GUI.UC_TacGia
{
    public partial class ucTacGia : UserControl
    {
        // Khai báo chuỗi kết nối cơ sở dữ liệu
        string connString = @"Data Source=DESKTOP-PDF3KTE\MSSQLSERVER03;Initial Catalog=QLThuVien;Integrated Security=True";

        public ucTacGia()
        {
            InitializeComponent();
        }

        private void ucTacGia_Load(object sender, EventArgs e)
        {
            dgvTacGia.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.0F, FontStyle.Bold);
            dgvTacGia.AutoGenerateColumns = false;

            // Ánh xạ cột DataGridView
            colSTT.DataPropertyName = "STT";
            colMaTacGia.DataPropertyName = "MaTacGia";
            colTenTacGia.DataPropertyName = "HoTen";
            colNgaySinh.DataPropertyName = "NgaySinh";
            colQuocTich.DataPropertyName = "QuocTich";

            LoadData();

            // Đăng ký sự kiện Click cho các nút và DataGridView (nếu trong Designer chưa có)
            dgvTacGia.CellClick += dgvTacGia_CellClick;
            guna2Button4.Click += guna2Button4_Click; // Thêm
            guna2Button2.Click += guna2Button2_Click; // Sửa
            guna2Button7.Click += guna2Button7_Click; // Xóa
            guna2Button1.Click += guna2Button1_Click; // Tìm kiếm
            guna2Button3.Click += guna2Button3_Click; // Làm mới
        }

        // 1. Tải danh sách tác giả
        private void LoadData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "SELECT ROW_NUMBER() OVER (ORDER BY MaTacGia) AS STT, MaTacGia, HoTen, NgaySinh, QuocTich, TieuSu FROM dbo.tacgia";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvTacGia.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 2. Sự kiện chọn dòng trên DataGridView
        private void dgvTacGia_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvTacGia.Rows[e.RowIndex];
                guna2TextBox1.Text = row.Cells["colMaTacGia"].Value?.ToString() ?? "";
                guna2TextBox3.Text = row.Cells["colTenTacGia"].Value?.ToString() ?? "";

                if (row.Cells["colNgaySinh"].Value != DBNull.Value && row.Cells["colNgaySinh"].Value != null)
                {
                    guna2DateTimePicker1.Value = Convert.ToDateTime(row.Cells["colNgaySinh"].Value);
                }
                else
                {
                    guna2DateTimePicker1.Value = DateTime.Now;
                }

                guna2TextBox5.Text = row.Cells["colQuocTich"].Value?.ToString() ?? "";

                DataTable dt = (DataTable)dgvTacGia.DataSource;
                if (dt != null && e.RowIndex < dt.Rows.Count)
                {
                    guna2TextBox6.Text = dt.Rows[e.RowIndex]["TieuSu"]?.ToString() ?? "";
                }
            }
        }

        // 3. Thêm tác giả
        private void guna2Button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(guna2TextBox3.Text))
            {
                MessageBox.Show("Vui lòng nhập tên tác giả!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                guna2TextBox3.Focus();
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = "INSERT INTO dbo.tacgia (HoTen, NgaySinh, QuocTich, TieuSu) VALUES (@HoTen, @NgaySinh, @QuocTich, @TieuSu)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@HoTen", guna2TextBox3.Text.Trim());
                        cmd.Parameters.AddWithValue("@NgaySinh", guna2DateTimePicker1.Value);
                        cmd.Parameters.AddWithValue("@QuocTich", string.IsNullOrWhiteSpace(guna2TextBox5.Text) ? (object)DBNull.Value : guna2TextBox5.Text.Trim());
                        cmd.Parameters.AddWithValue("@TieuSu", string.IsNullOrWhiteSpace(guna2TextBox6.Text) ? (object)DBNull.Value : guna2TextBox6.Text.Trim());

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Thêm tác giả thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm tác giả: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 4. Sửa tác giả
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(guna2TextBox1.Text))
            {
                MessageBox.Show("Vui lòng chọn tác giả cần sửa từ danh sách!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = "UPDATE dbo.tacgia SET HoTen = @HoTen, NgaySinh = @NgaySinh, QuocTich = @QuocTich, TieuSu = @TieuSu WHERE MaTacGia = @MaTacGia";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaTacGia", Convert.ToInt32(guna2TextBox1.Text.Trim()));
                        cmd.Parameters.AddWithValue("@HoTen", guna2TextBox3.Text.Trim());
                        cmd.Parameters.AddWithValue("@NgaySinh", guna2DateTimePicker1.Value);
                        cmd.Parameters.AddWithValue("@QuocTich", string.IsNullOrWhiteSpace(guna2TextBox5.Text) ? (object)DBNull.Value : guna2TextBox5.Text.Trim());
                        cmd.Parameters.AddWithValue("@TieuSu", string.IsNullOrWhiteSpace(guna2TextBox6.Text) ? (object)DBNull.Value : guna2TextBox6.Text.Trim());

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Cập nhật thông tin tác giả thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật tác giả: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 5. Xóa tác giả
        private void guna2Button7_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(guna2TextBox1.Text))
            {
                MessageBox.Show("Vui lòng chọn tác giả cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa tác giả này không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        conn.Open();
                        string query = "DELETE FROM dbo.tacgia WHERE MaTacGia = @MaTacGia";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@MaTacGia", Convert.ToInt32(guna2TextBox1.Text.Trim()));
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Xóa tác giả thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    ClearFields();
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 547)
                    {
                        MessageBox.Show("Không thể xóa tác giả này vì đang có sách liên kết trong hệ thống!", "Lỗi ràng buộc", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Lỗi SQL khi xóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // 6. Tìm kiếm tác giả
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text.Trim();
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"SELECT ROW_NUMBER() OVER (ORDER BY MaTacGia) AS STT, MaTacGia, HoTen, NgaySinh, QuocTich, TieuSu 
                                     FROM dbo.tacgia 
                                     WHERE CAST(MaTacGia AS VARCHAR) LIKE @Key OR HoTen LIKE @Key";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Key", "%" + keyword + "%");
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgvTacGia.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 7. Làm mới
        private void guna2Button3_Click(object sender, EventArgs e)
        {
            txtTimKiem.Clear();
            ClearFields();
            LoadData();
        }

        // Hàm làm sạch ô nhập
        private void ClearFields()
        {
            guna2TextBox1.Clear();
            guna2TextBox3.Clear();
            guna2DateTimePicker1.Value = DateTime.Now;
            guna2TextBox5.Clear();
            guna2TextBox6.Clear();
            dgvTacGia.ClearSelection();
        }
    }
}