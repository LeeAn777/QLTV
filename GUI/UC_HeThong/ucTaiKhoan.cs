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

namespace QLTV.GUI.UC_HeThong
{
    public partial class ucTaiKhoan : UserControl
    {
        private readonly string connString = @"Data Source=.\SQLEXPRESS;Initial Catalog=QLThuVien;Integrated Security=True";
        public ucTaiKhoan()
        {
            InitializeComponent();
        }

        #region SỰ KIỆN LOAD & HÀM BỔ TRỢ

        private void ucTaiKhoan_Load(object sender, EventArgs e)
        {
            txttaikhoan.ReadOnly = true; // MaTK tự tăng nên không cho sửa
            dgvTaiKhoan.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.0F, FontStyle.Bold);
            dgvTaiKhoan.AutoGenerateColumns = false;
            dgvTaiKhoan.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            LoadComboBoxNhanVien();
            LoadComboBoxQuyenVaTrangThai();
            LoadData();
        }
        private void LoadComboBoxNhanVien()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = "SELECT MaNV, Hoten FROM dbo.nhanvien";
                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        DataTable dtNV = new DataTable();
                        da.Fill(dtNV);

                        cboMaNV.DataSource = dtNV;
                        cboMaNV.DisplayMember = "HoTen";
                        cboMaNV.ValueMember = "MaNV";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách nhân viên: " + ex.Message, "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadComboBoxQuyenVaTrangThai()
        {
            cbquyenhan.Items.Clear();
            cbquyenhan.Items.Add("Admin");
            cbquyenhan.Items.Add("Thủ thư");
            cbquyenhan.DropDownStyle = ComboBoxStyle.DropDownList;

            cbtrangthai.Items.Clear();
            cbtrangthai.Items.Add("Hoạt động");
            cbtrangthai.Items.Add("Bị khóa");
            cbtrangthai.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        private void LoadData(string keyword = "")
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = @"SELECT tk.MaTK, tk.MaNV, nv.HoTen, tk.TenDangNhap, tk.MatKhau, tk.Quyen, tk.TrangThai 
                                   FROM dbo.taikhoannhanvien tk 
                                   LEFT JOIN dbo.nhanvien nv ON tk.MaNV = nv.MaNV";

                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        query += " WHERE tk.TenDangNhap LIKE @kw OR tk.Quyen LIKE @kw OR nv.HoTen LIKE @kw " +
                            " OR tk.TrangThai LIKE @kw" +
                            " OR CAST(tk.MaTK AS VARCHAR) LIKE @kw" +
                            " OR CAST(tk.MaNV AS VARCHAR) LIKE @kw";
                    }
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (!string.IsNullOrWhiteSpace(keyword))
                            cmd.Parameters.AddWithValue("@kw", "%" + keyword.Trim() + "%");

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            dgvTaiKhoan.DataSource = dt;
                        }
                        if (dgvTaiKhoan.Columns.Contains("matkhau"))
                        {
                            dgvTaiKhoan.Columns["matkhau"].Visible = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu tài khoản: " + ex.Message, "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void XoaTrangNhapLieu()
        {
            txttaikhoan.Clear();
            if (cboMaNV.Items.Count > 0) cboMaNV.SelectedIndex = 0;
            txttendangnhap.Clear();
            txtMatKhau.Clear();
            cbquyenhan.SelectedIndex = -1;
            cbtrangthai.SelectedIndex = -1;
            dgvTaiKhoan.ClearSelection();
        }
        private void dgvTaiKhoan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvTaiKhoan.Rows[e.RowIndex].Cells["matk"].Value != null)
            {
                DataGridViewRow row = dgvTaiKhoan.Rows[e.RowIndex];

                txttaikhoan.Text = row.Cells["matk"].Value?.ToString();
                if (row.Cells["manhanvien"].Value != DBNull.Value)
                {
                    cboMaNV.SelectedValue = Convert.ToInt32(row.Cells["manhanvien"].Value);
                }
                txttendangnhap.Text = row.Cells["tendangnhap"].Value?.ToString();
                txtMatKhau.UseSystemPasswordChar = true;
                txtMatKhau.PasswordChar = '●';
                txtMatKhau.Text = row.Cells["matkhau"].Value?.ToString();
                cbquyenhan.Text = row.Cells["quyen"].Value?.ToString();
                if (dgvTaiKhoan.Columns.Contains("trangthai"))
                {
                    cbtrangthai.Text = row.Cells["trangthai"].Value?.ToString();
                }
            }
        }
        private void dgvTaiKhoan_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            dgvTaiKhoan.Rows[e.RowIndex].Cells["stt"].Value = (e.RowIndex + 1).ToString();
        }
        #endregion

        #region BỘ NÚT THAO TÁC (THÊM, SỬA, XÓA, LÀM MỚI, TÌM KIẾM)
        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            LoadData(txtTimKiem.Text);
        }

        private void btxoa_Click(object sender, EventArgs e)
        {
            string maTK = txttaikhoan.Text.Trim();
            if (string.IsNullOrEmpty(maTK))
            {
                MessageBox.Show("Vui lòng chọn tài khoản cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa tài khoản này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        conn.Open();
                        string query = "DELETE FROM dbo.taikhoannhanvien WHERE MaTK = @MaTK";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@MaTK", Convert.ToInt32(maTK));
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Xóa tài khoản thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    XoaTrangNhapLieu();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa tài khoản: " + ex.Message, "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            txtTimKiem.Clear(); 
            LoadData();     
        }

        private void txtMatKhau_IconRightClick(object sender, EventArgs e)
        {
            if (txtMatKhau.PasswordChar == '\0' && !txtMatKhau.UseSystemPasswordChar)
            {
                txtMatKhau.UseSystemPasswordChar = true;
                txtMatKhau.PasswordChar = '●';
            }
            else
            {
                txtMatKhau.UseSystemPasswordChar = false;
                txtMatKhau.PasswordChar = '\0';
            }
        }

        private void btluu_Click(object sender, EventArgs e)
        {
            if (cboMaNV.SelectedValue == null || string.IsNullOrWhiteSpace(txttendangnhap.Text) ||
                string.IsNullOrWhiteSpace(txtMatKhau.Text) || cbquyenhan.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin tài khoản!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = @"INSERT INTO dbo.taikhoannhanvien (MaNV, TenDangNhap, MatKhau, Quyen, TrangThai) 
                                   VALUES (@MaNV, @TenDN, @MatKhau, @Quyen, @TrangThai)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaNV", Convert.ToInt32(cboMaNV.SelectedValue));
                        cmd.Parameters.AddWithValue("@TenDN", txttendangnhap.Text.Trim());
                        cmd.Parameters.AddWithValue("@MatKhau", txtMatKhau.Text.Trim());
                        cmd.Parameters.AddWithValue("@Quyen", cbquyenhan.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@TrangThai", cbtrangthai.SelectedItem != null ? cbtrangthai.SelectedItem.ToString() : "Hoạt động");

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Thêm tài khoản thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                XoaTrangNhapLieu();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm tài khoản (có thể tên đăng nhập hoặc nhân viên đã có tài khoản): " + ex.Message, "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btsua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txttaikhoan.Text))
            {
                MessageBox.Show("Vui lòng chọn tài khoản cần sửa từ danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = @"UPDATE dbo.taikhoannhanvien 
                                   SET MaNV = @MaNV, TenDangNhap = @TenDN, MatKhau = @MatKhau, Quyen = @Quyen, TrangThai = @TrangThai 
                                   WHERE MaTK = @MaTK";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaTK", Convert.ToInt32(txttaikhoan.Text));
                        cmd.Parameters.AddWithValue("@MaNV", Convert.ToInt32(cboMaNV.SelectedValue));
                        cmd.Parameters.AddWithValue("@TenDN", txttendangnhap.Text.Trim());
                        cmd.Parameters.AddWithValue("@MatKhau", txtMatKhau.Text.Trim());
                        cmd.Parameters.AddWithValue("@Quyen", cbquyenhan.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@TrangThai", cbtrangthai.SelectedItem.ToString());

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Cập nhật thông tin tài khoản thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                XoaTrangNhapLieu();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật tài khoản: " + ex.Message, "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btlammoi_Click(object sender, EventArgs e)
        {
            XoaTrangNhapLieu();
        }
        #endregion

        
    }
}
