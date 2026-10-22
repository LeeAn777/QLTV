
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QLTV.GUI.UC_TacGia
{
    public partial class ucSachTacGia : UserControl
    {
        // Kết nối SQL Server
        private readonly string connString =
            @"Data Source=DESKTOP-PDF3KTE\MSSQLSERVER03;Initial Catalog=QLThuVien;Integrated Security=True";

        private bool dangTaiDuLieu = false;
        private string maSachDangChon = "";

        public ucSachTacGia()
        {
            InitializeComponent();

            // Gắn sự kiện, tránh gắn trùng
            this.Load -= ucSachTacGia_Load;
            this.Load += ucSachTacGia_Load;

            guna2ComboBox2.SelectedIndexChanged -=
                guna2ComboBox2_SelectedIndexChanged;
            guna2ComboBox2.SelectedIndexChanged +=
                guna2ComboBox2_SelectedIndexChanged;

            guna2Button4.Click -= guna2Button4_Click;
            guna2Button4.Click += guna2Button4_Click;

            guna2Button7.Click += guna2Button7_Click;

            dgvSachTacGia.CellClick += dgvSachTacGia_CellClick;
        }

        // 1. KHỞI TẠO GIAO DIỆN
        private void ucSachTacGia_Load(object sender, EventArgs e)
        {
            dgvSachTacGia.AutoGenerateColumns = false;

            // Ánh xạ các cột với SQL
            colSTT.DataPropertyName = "STT";
            colMaSach.DataPropertyName = "MaSach";
            colTenSach.DataPropertyName = "TenSach";
            colTheLoai.DataPropertyName = "TenTheLoai";
            colNamXuatBan.DataPropertyName = "NamXuatBan";

            dgvSachTacGia.ReadOnly = true;
            dgvSachTacGia.AllowUserToAddRows = false;
            dgvSachTacGia.MultiSelect = false;
            dgvSachTacGia.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvSachTacGia.RowHeadersVisible = false;

            // Giữ màu giao diện hiện tại
            dgvSachTacGia.DefaultCellStyle.SelectionBackColor =
                Color.MediumSpringGreen;
            dgvSachTacGia.DefaultCellStyle.SelectionForeColor =
                Color.Black;

            colSTT.DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            colNamXuatBan.DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            // Tải dữ liệu
            dangTaiDuLieu = true;

            LoadComboBoxTacGia();
            LoadComboBoxSach();

            dangTaiDuLieu = false;

            guna2ComboBox2.SelectedIndex = -1;
            guna2ComboBox1.SelectedIndex = -1;

            dgvSachTacGia.DataSource = null;
            maSachDangChon = "";

            label1.Text = "Chưa chọn tác giả";
        }

        // 2. TẢI DANH SÁCH TÁC GIẢ
        private void LoadComboBoxTacGia()
        {
            try
            {
                using (SqlConnection conn =
                    new SqlConnection(connString))
                {
                    string sql = @"
                        SELECT MaTacGia, HoTen
                        FROM dbo.tacgia
                        ORDER BY HoTen";

                    SqlDataAdapter da =
                        new SqlDataAdapter(sql, conn);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    guna2ComboBox2.DataSource = dt;
                    guna2ComboBox2.DisplayMember = "HoTen";
                    guna2ComboBox2.ValueMember = "MaTacGia";
                    guna2ComboBox2.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tải tác giả: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // 3. TẢI DANH SÁCH SÁCH
        private void LoadComboBoxSach()
        {
            try
            {
                using (SqlConnection conn =
                    new SqlConnection(connString))
                {
                    string sql = @"
                        SELECT MaSach, TenSach
                        FROM dbo.sach
                        ORDER BY TenSach";

                    SqlDataAdapter da =
                        new SqlDataAdapter(sql, conn);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    guna2ComboBox1.DataSource = dt;
                    guna2ComboBox1.DisplayMember = "TenSach";
                    guna2ComboBox1.ValueMember = "MaSach";
                    guna2ComboBox1.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tải sách: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // 4. LẤY MÃ TÁC GIẢ ĐANG CHỌN
        private bool LayMaTacGia(out int maTacGia)
        {
            maTacGia = 0;

            if (guna2ComboBox2.SelectedIndex < 0 ||
                guna2ComboBox2.SelectedValue == null ||
                guna2ComboBox2.SelectedValue == DBNull.Value)
            {
                return false;
            }

            return int.TryParse(
                guna2ComboBox2.SelectedValue.ToString(),
                out maTacGia) && maTacGia > 0;
        }

        // 5. KHI CHỌN TÁC GIẢ
        private void guna2ComboBox2_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            if (dangTaiDuLieu)
                return;

            int maTacGia;

            if (LayMaTacGia(out maTacGia))
            {
                maSachDangChon = "";
                guna2ComboBox1.SelectedIndex = -1;

                label1.Text = guna2ComboBox2.Text;

                LoadSachCuaTacGia(maTacGia);
            }
            else
            {
                dgvSachTacGia.DataSource = null;
                label1.Text = "Chưa chọn tác giả";
                maSachDangChon = "";
            }
        }

        // 6. XÁC ĐỊNH CỘT NĂM XUẤT BẢN
        private string LayCotNamXuatBan(SqlConnection conn)
        {
            // Kiểm tra tên cột thực tế trong bảng sách
            string sql = @"
                SELECT COLUMN_NAME
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = 'dbo'
                  AND TABLE_NAME = 'sach'";

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string tenCot = reader.GetString(0);

                    // Những tên cột năm xuất bản thường gặp
                    if (string.Equals(tenCot, "NamXuatBan",
                            StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(tenCot, "NamXB",
                            StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(tenCot, "NamPhatHanh",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        return tenCot;
                    }
                }
            }

            return null;
        }

        // 7. HIỂN THỊ SÁCH CỦA TÁC GIẢ
        private void LoadSachCuaTacGia(int maTacGia)
        {
            try
            {
                using (SqlConnection conn =
                    new SqlConnection(connString))
                {
                    conn.Open();

                    string cotNam = LayCotNamXuatBan(conn);

                    // Nếu chưa có cột năm thì để trống
                    string bieuThucNam =
                        "CAST(NULL AS INT) AS NamXuatBan";

                    if (!string.IsNullOrEmpty(cotNam))
                    {
                        string tenCotAnToan =
                            "[" + cotNam.Replace("]", "]]") + "]";

                        bieuThucNam =
                            "s." + tenCotAnToan +
                            " AS NamXuatBan";
                    }

                    string sql = @"
                        SELECT
                            ROW_NUMBER() OVER
                                (ORDER BY s.MaSach) AS STT,
                            s.MaSach,
                            s.TenSach,
                            t.TenTheLoai,
                            " + bieuThucNam + @"
                        FROM dbo.sach s
                        INNER JOIN dbo.sach_tacgia st
                            ON s.MaSach = st.MaSach
                        LEFT JOIN dbo.theloai t
                            ON s.MaTheLoai = t.MaTheLoai
                        WHERE st.MaTacGia = @MaTacGia
                        ORDER BY s.MaSach";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@MaTacGia",
                            SqlDbType.Int).Value = maTacGia;

                        SqlDataAdapter da =
                            new SqlDataAdapter(cmd);

                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dgvSachTacGia.DataSource = dt;
                        dgvSachTacGia.ClearSelection();

                        maSachDangChon = "";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tải danh sách sách: " +
                    ex.Message,
                    "Lỗi", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // 8. CHỨC NĂNG THÊM SÁCH
        private void guna2Button4_Click(
            object sender, EventArgs e)
        {
            int maTacGia;

            if (!LayMaTacGia(out maTacGia))
            {
                MessageBox.Show(
                    "Vui lòng chọn tác giả!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (guna2ComboBox1.SelectedIndex < 0 ||
                guna2ComboBox1.SelectedValue == null ||
                guna2ComboBox1.SelectedValue == DBNull.Value)
            {
                MessageBox.Show(
                    "Vui lòng chọn sách cần thêm!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Mã sách trong SQL là VARCHAR(100)
            string maSach =
                guna2ComboBox1.SelectedValue.ToString();

            if (string.IsNullOrWhiteSpace(maSach))
            {
                MessageBox.Show("Mã sách không hợp lệ!");
                return;
            }

            try
            {
                using (SqlConnection conn =
                    new SqlConnection(connString))
                {
                    conn.Open();

                    string sql = @"
                        IF NOT EXISTS
                        (
                            SELECT 1
                            FROM dbo.sach_tacgia
                            WHERE MaSach = @MaSach
                              AND MaTacGia = @MaTacGia
                        )
                        BEGIN
                            INSERT INTO dbo.sach_tacgia
                                (MaSach, MaTacGia)
                            VALUES
                                (@MaSach, @MaTacGia)
                        END";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@MaSach",
                            SqlDbType.VarChar, 100).Value =
                            maSach;

                        cmd.Parameters.Add("@MaTacGia",
                            SqlDbType.Int).Value =
                            maTacGia;

                        int ketQua = cmd.ExecuteNonQuery();

                        if (ketQua > 0)
                        {
                            MessageBox.Show(
                                "Thêm sách cho tác giả thành công!",
                                "Thành công",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            LoadSachCuaTacGia(maTacGia);

                            // Giữ sách vừa chọn trên ComboBox
                            guna2ComboBox1.SelectedValue = maSach;
                        }
                        else
                        {
                            MessageBox.Show(
                                "Sách này đã thuộc danh sách của tác giả!",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi khi thêm sách: " + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // 9. CHỌN DÒNG TRÊN DATAGRIDVIEW
        private void dgvSachTacGia_CellClick(
            object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row =
                dgvSachTacGia.Rows[e.RowIndex];

            object giaTri =
                row.Cells["colMaSach"].Value;

            if (giaTri == null ||
                giaTri == DBNull.Value)
                return;

            maSachDangChon = giaTri.ToString();

            // Tự động chọn sách trong ComboBox
            guna2ComboBox1.SelectedValue =
                maSachDangChon;
        }

        // 10. CHỨC NĂNG XÓA SÁCH KHỎI TÁC GIẢ
        private void guna2Button7_Click(
            object sender, EventArgs e)
        {
            int maTacGia;

            if (!LayMaTacGia(out maTacGia))
            {
                MessageBox.Show(
                    "Vui lòng chọn tác giả!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(maSachDangChon))
            {
                MessageBox.Show(
                    "Vui lòng chọn một dòng sách trong bảng để xóa!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            DialogResult xacNhan = MessageBox.Show(
                "Bạn có chắc muốn xóa sách này khỏi tác giả?\n" +
                "Mã sách: " + maSachDangChon +
                "\nSách gốc trong thư viện sẽ được giữ nguyên.",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (xacNhan != DialogResult.Yes)
                return;

            try
            {
                using (SqlConnection conn =
                    new SqlConnection(connString))
                {
                    conn.Open();

                    string sql = @"
                        DELETE FROM dbo.sach_tacgia
                        WHERE MaSach = @MaSach
                          AND MaTacGia = @MaTacGia";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@MaSach",
                            SqlDbType.VarChar, 100).Value =
                            maSachDangChon;

                        cmd.Parameters.Add("@MaTacGia",
                            SqlDbType.Int).Value =
                            maTacGia;

                        int ketQua = cmd.ExecuteNonQuery();

                        if (ketQua > 0)
                        {
                            MessageBox.Show(
                                "Xóa sách khỏi tác giả thành công!",
                                "Thành công",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            maSachDangChon = "";
                            guna2ComboBox1.SelectedIndex = -1;

                            LoadSachCuaTacGia(maTacGia);
                        }
                        else
                        {
                            MessageBox.Show(
                                "Không tìm thấy liên kết sách cần xóa!",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi khi xóa sách: " + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // 11. SỰ KIỆN CÓ SẴN TRONG DESIGNER
        private void dgvSachTacGia_CellContentClick(
            object sender, DataGridViewCellEventArgs e)
        {
            // Để trống vì đã sử dụng CellClick
        }
    }
}
