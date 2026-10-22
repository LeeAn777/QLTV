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
    public partial class    ucHangDG : UserControl
    {
        SqlConnection conn = new SqlConnection(
           @"Data Source=.\SQLEXPRESS;Initial Catalog=QLThuVien;Integrated Security=True");
        public ucHangDG()
        {
            InitializeComponent();
            txtmahang.ReadOnly = false;
            dgvHangDG.AutoGenerateColumns = true;
            dgvHangDG.AllowUserToAddRows = false;

            hienthi();
        }
        private void hienthi()
        {
            try
            {
                string sql = @"SELECT MaHang, TenHang,
                                      SoSachToiDa, SoNgayMuonToiDa
                               FROM dbo.hangdocgia
                               ORDER BY MaHang";

                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvHangDG.DataSource = dt;
                }
                dgvHangDG.Columns["colMaHang"].HeaderText = "Mã hạng";
                dgvHangDG.Columns["colTenHang"].HeaderText = "Tên hạng";
                dgvHangDG.Columns["colSachMax"].HeaderText =
                    "Số sách được mượn tối đa";
                dgvHangDG.Columns["colNgayMax"].HeaderText =
                    "Số ngày được mượn";

                dgvHangDG.AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }
        private void lamMoi()
        {
            txtmahang.Clear();
            txttenhang.Clear();
            nudsach.Value = 1;
            nudngay.Value = 1;
            txttenhang.Focus();
            dgvHangDG.ClearSelection();
        }

        private void ucHangDG_Load(object sender, EventArgs e)
        {
            dgvHangDG.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.0F, FontStyle.Bold);
            dgvHangDG.AutoGenerateColumns = false;
        }

        private void btnthem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txttenhang.Text))
            {
                MessageBox.Show("Vui lòng nhập tên hạng!");
                txttenhang.Focus();
                return;
            }

            if (nudsach.Value < 1 || nudngay.Value < 1)
            {
                MessageBox.Show(
                    "Số sách và số ngày phải lớn hơn 0!");
                return;
            }

            try
            {
                string sql = @"INSERT INTO dbo.hangdocgia
                               (TenHang, SoSachToiDa, SoNgayMuonToiDa)
                               VALUES (@TenHang, @SoSach, @SoNgay)";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@TenHang", SqlDbType.NVarChar, 50)
                        .Value = txttenhang.Text.Trim();

                    cmd.Parameters.Add("@SoSach", SqlDbType.Int)
                        .Value = Convert.ToInt32(nudsach.Value);

                    cmd.Parameters.Add("@SoNgay", SqlDbType.Int)
                        .Value = Convert.ToInt32(nudngay.Value);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                MessageBox.Show("Thêm hạng độc giả thành công!");
                hienthi();
                lamMoi();
            }
            catch (SqlException ex)
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                if (ex.Number == 2627 || ex.Number == 2601)
                    MessageBox.Show("Tên hạng này đã tồn tại!");
                else
                    MessageBox.Show("Lỗi thêm dữ liệu: " + ex.Message);
            }
            catch (Exception ex)
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                MessageBox.Show("Lỗi: " + ex.Message);
            }
        
    }

        private void btnsua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtmahang.Text))
            {
                MessageBox.Show("Vui lòng chọn hạng cần sửa!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txttenhang.Text))
            {
                MessageBox.Show("Vui lòng nhập tên hạng!");
                return;
            }

            if (nudsach.Value < 1 || nudngay.Value < 1)
            {
                MessageBox.Show(
                    "Số sách và số ngày phải lớn hơn 0!");
                return;
            }

            try
            {
                string sql = @"UPDATE dbo.hangdocgia
                               SET TenHang = @TenHang,
                                   SoSachToiDa = @SoSach,
                                   SoNgayMuonToiDa = @SoNgay
                               WHERE MaHang = @MaHang";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@MaHang", SqlDbType.Int)
                        .Value = Convert.ToInt32(txtmahang.Text);

                    cmd.Parameters.Add("@TenHang", SqlDbType.NVarChar, 50)
                        .Value = txttenhang.Text.Trim();

                    cmd.Parameters.Add("@SoSach", SqlDbType.Int)
                        .Value = Convert.ToInt32(nudsach.Value);

                    cmd.Parameters.Add("@SoNgay", SqlDbType.Int)
                        .Value = Convert.ToInt32(nudngay.Value);

                    conn.Open();
                    int kq = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (kq > 0)
                        MessageBox.Show("Sửa hạng độc giả thành công!");
                    else
                        MessageBox.Show("Không tìm thấy hạng cần sửa.");
                }

                hienthi();
                lamMoi();
            }
            catch (SqlException ex)
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                if (ex.Number == 2627 || ex.Number == 2601)
                    MessageBox.Show("Tên hạng này đã tồn tại!");
                else
                    MessageBox.Show("Lỗi sửa dữ liệu: " + ex.Message);
            }
            catch (Exception ex)
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnxoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtmahang.Text))
            {
                MessageBox.Show("Vui lòng chọn hạng cần xóa!");
                return;
            }

            DialogResult xacNhan = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa hạng độc giả này?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (xacNhan != DialogResult.Yes)
                return;

            try
            {
                string sql = @"DELETE FROM dbo.hangdocgia
                               WHERE MaHang = @MaHang";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@MaHang", SqlDbType.Int)
                        .Value = Convert.ToInt32(txtmahang.Text);

                    conn.Open();
                    int kq = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (kq > 0)
                        MessageBox.Show("Xóa hạng độc giả thành công!");
                    else
                        MessageBox.Show("Không tìm thấy hạng cần xóa.");
                }

                hienthi();
                lamMoi();
            }
            catch (SqlException ex)
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                if (ex.Number == 547)
                    MessageBox.Show(
                        "Không thể xóa hạng này vì đang được độc giả sử dụng!");
                else
                    MessageBox.Show("Lỗi xóa dữ liệu: " + ex.Message);
            }
            catch (Exception ex)
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btntimkiem_Click(object sender, EventArgs e)
        {
            string tuKhoa = txtTimKiem.Text.Trim();

            try
            {
                string sql = @"
            SELECT MaHang, TenHang, SoSachToiDa, SoNgayMuonToiDa
            FROM dbo.hangdocgia
            WHERE CAST(MaHang AS NVARCHAR(20)) LIKE @TuKhoa
               OR TenHang LIKE @TuKhoa
            ORDER BY MaHang";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@TuKhoa", SqlDbType.NVarChar, 100)
                        .Value = "%" + tuKhoa + "%";

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgvHangDG.DataSource = dt;
                    }
                }

                if (dgvHangDG.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy hạng độc giả phù hợp!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message);
            }
        }

        private void btnlammoi_Click(object sender, EventArgs e)
        {
            txtTimKiem.Clear();

            hienthi();

            dgvHangDG.ClearSelection();

            lamMoi();

            txtTimKiem.Focus();
        }

        private void dgvHangDG_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = dgvHangDG.Rows[e.RowIndex];

            txtmahang.Text = row.Cells["colMaHang"].Value.ToString();
            txttenhang.Text = row.Cells["colTenHang"].Value.ToString();

            nudsach.Value = Convert.ToDecimal(
                row.Cells["colSachMax"].Value);

            nudngay.Value = Convert.ToDecimal(
                row.Cells["colNgayMax"].Value);
        }

        private void dgvHangDG_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            dgvHangDG.Rows[e.RowIndex].Cells["colSTT"].Value = (e.RowIndex + 1).ToString();
        }
    }
}
