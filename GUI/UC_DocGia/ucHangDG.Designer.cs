namespace QLTV.GUI.UC_DocGia
{
    partial class ucHangDG
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucHangDG));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnl2 = new Guna.UI2.WinForms.Guna2Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.nudsach = new Guna.UI2.WinForms.Guna2NumericUpDown();
            this.nudngay = new Guna.UI2.WinForms.Guna2NumericUpDown();
            this.btnxoa = new Guna.UI2.WinForms.Guna2Button();
            this.txttenhang = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtmahang = new Guna.UI2.WinForms.Guna2TextBox();
            this.guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2Panel6 = new Guna.UI2.WinForms.Guna2Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.btnthem = new Guna.UI2.WinForms.Guna2Button();
            this.btnsua = new Guna.UI2.WinForms.Guna2Button();
            this.label11 = new System.Windows.Forms.Label();
            this.pnl1 = new Guna.UI2.WinForms.Guna2Panel();
            this.dgvHangDG = new System.Windows.Forms.DataGridView();
            this.colSTT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMaHang = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTenhang = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSachMax = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNgayMax = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.guna2Panel4 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2Panel7 = new Guna.UI2.WinForms.Guna2Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.btnlammoi = new Guna.UI2.WinForms.Guna2Button();
            this.btntimkiem = new Guna.UI2.WinForms.Guna2Button();
            this.txtTimKiem = new Guna.UI2.WinForms.Guna2TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.pnl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudsach)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudngay)).BeginInit();
            this.guna2Panel1.SuspendLayout();
            this.pnl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHangDG)).BeginInit();
            this.guna2Panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnl2
            // 
            this.pnl2.BackColor = System.Drawing.Color.Transparent;
            this.pnl2.BorderColor = System.Drawing.Color.Black;
            this.pnl2.BorderRadius = 10;
            this.pnl2.Controls.Add(this.label1);
            this.pnl2.Controls.Add(this.nudsach);
            this.pnl2.Controls.Add(this.nudngay);
            this.pnl2.Controls.Add(this.btnxoa);
            this.pnl2.Controls.Add(this.txttenhang);
            this.pnl2.Controls.Add(this.txtmahang);
            this.pnl2.Controls.Add(this.guna2Panel1);
            this.pnl2.Controls.Add(this.label14);
            this.pnl2.Controls.Add(this.label12);
            this.pnl2.Controls.Add(this.btnthem);
            this.pnl2.Controls.Add(this.btnsua);
            this.pnl2.Controls.Add(this.label11);
            this.pnl2.FillColor = System.Drawing.Color.White;
            this.pnl2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnl2.Location = new System.Drawing.Point(1060, 55);
            this.pnl2.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.pnl2.Name = "pnl2";
            this.pnl2.Size = new System.Drawing.Size(780, 1058);
            this.pnl2.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(20, 434);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(271, 32);
            this.label1.TabIndex = 21;
            this.label1.Text = "Số ngày được mượn/lần";
            // 
            // nudsach
            // 
            this.nudsach.BackColor = System.Drawing.Color.Transparent;
            this.nudsach.BorderRadius = 5;
            this.nudsach.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.nudsach.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.nudsach.Location = new System.Drawing.Point(26, 353);
            this.nudsach.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.nudsach.Name = "nudsach";
            this.nudsach.Size = new System.Drawing.Size(412, 45);
            this.nudsach.TabIndex = 20;
            this.nudsach.UpDownButtonFillColor = System.Drawing.Color.Honeydew;
            // 
            // nudngay
            // 
            this.nudngay.BackColor = System.Drawing.Color.Transparent;
            this.nudngay.BorderRadius = 5;
            this.nudngay.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.nudngay.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.nudngay.Location = new System.Drawing.Point(26, 483);
            this.nudngay.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.nudngay.Name = "nudngay";
            this.nudngay.Size = new System.Drawing.Size(412, 45);
            this.nudngay.TabIndex = 19;
            this.nudngay.UpDownButtonFillColor = System.Drawing.Color.Honeydew;
            // 
            // btnxoa
            // 
            this.btnxoa.BackColor = System.Drawing.Color.Transparent;
            this.btnxoa.BorderRadius = 5;
            this.btnxoa.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnxoa.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnxoa.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnxoa.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnxoa.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnxoa.FillColor = System.Drawing.Color.Crimson;
            this.btnxoa.Font = new System.Drawing.Font("Segoe UI", 10.125F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnxoa.ForeColor = System.Drawing.Color.White;
            this.btnxoa.Image = ((System.Drawing.Image)(resources.GetObject("btnxoa.Image")));
            this.btnxoa.Location = new System.Drawing.Point(280, 788);
            this.btnxoa.Name = "btnxoa";
            this.btnxoa.ShadowDecoration.Depth = 10;
            this.btnxoa.ShadowDecoration.Enabled = true;
            this.btnxoa.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(0, 0, 5, 5);
            this.btnxoa.Size = new System.Drawing.Size(308, 59);
            this.btnxoa.TabIndex = 18;
            this.btnxoa.Text = "Xóa";
            this.btnxoa.Click += new System.EventHandler(this.btnxoa_Click);
            // 
            // txttenhang
            // 
            this.txttenhang.BorderRadius = 5;
            this.txttenhang.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txttenhang.DefaultText = "";
            this.txttenhang.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txttenhang.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txttenhang.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txttenhang.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txttenhang.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txttenhang.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txttenhang.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txttenhang.Location = new System.Drawing.Point(26, 231);
            this.txttenhang.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txttenhang.Name = "txttenhang";
            this.txttenhang.PlaceholderText = "Nhập tên hạng";
            this.txttenhang.SelectedText = "";
            this.txttenhang.Size = new System.Drawing.Size(736, 45);
            this.txttenhang.TabIndex = 14;
            // 
            // txtmahang
            // 
            this.txtmahang.BorderRadius = 5;
            this.txtmahang.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtmahang.DefaultText = "";
            this.txtmahang.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtmahang.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtmahang.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtmahang.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtmahang.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtmahang.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtmahang.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtmahang.Location = new System.Drawing.Point(26, 120);
            this.txtmahang.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtmahang.Name = "txtmahang";
            this.txtmahang.PlaceholderText = "Tự động sinh";
            this.txtmahang.ReadOnly = true;
            this.txtmahang.SelectedText = "";
            this.txtmahang.Size = new System.Drawing.Size(736, 45);
            this.txtmahang.TabIndex = 0;
            // 
            // guna2Panel1
            // 
            this.guna2Panel1.BackColor = System.Drawing.Color.Transparent;
            this.guna2Panel1.BorderRadius = 10;
            this.guna2Panel1.Controls.Add(this.guna2Panel6);
            this.guna2Panel1.Controls.Add(this.label10);
            this.guna2Panel1.CustomizableEdges.BottomLeft = false;
            this.guna2Panel1.CustomizableEdges.BottomRight = false;
            this.guna2Panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.guna2Panel1.FillColor = System.Drawing.Color.WhiteSmoke;
            this.guna2Panel1.Location = new System.Drawing.Point(0, 0);
            this.guna2Panel1.Name = "guna2Panel1";
            this.guna2Panel1.Size = new System.Drawing.Size(780, 59);
            this.guna2Panel1.TabIndex = 12;
            // 
            // guna2Panel6
            // 
            this.guna2Panel6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.guna2Panel6.FillColor = System.Drawing.Color.ForestGreen;
            this.guna2Panel6.Location = new System.Drawing.Point(0, 54);
            this.guna2Panel6.Name = "guna2Panel6";
            this.guna2Panel6.Size = new System.Drawing.Size(780, 5);
            this.guna2Panel6.TabIndex = 1;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 10.125F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label10.Location = new System.Drawing.Point(34, 12);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(313, 37);
            this.label10.TabIndex = 0;
            this.label10.Text = "Thông tin chi tiết Hạng";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(20, 316);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(204, 32);
            this.label14.TabIndex = 9;
            this.label14.Text = "Số sách tối đa/lần";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(20, 194);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(113, 32);
            this.label12.TabIndex = 7;
            this.label12.Text = "Tên hạng";
            // 
            // btnthem
            // 
            this.btnthem.BackColor = System.Drawing.Color.Transparent;
            this.btnthem.BorderRadius = 5;
            this.btnthem.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnthem.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnthem.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnthem.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnthem.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnthem.FillColor = System.Drawing.Color.RoyalBlue;
            this.btnthem.Font = new System.Drawing.Font("Segoe UI", 10.125F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnthem.ForeColor = System.Drawing.Color.White;
            this.btnthem.Image = ((System.Drawing.Image)(resources.GetObject("btnthem.Image")));
            this.btnthem.Location = new System.Drawing.Point(280, 677);
            this.btnthem.Name = "btnthem";
            this.btnthem.ShadowDecoration.Depth = 10;
            this.btnthem.ShadowDecoration.Enabled = true;
            this.btnthem.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(0, 0, 5, 5);
            this.btnthem.Size = new System.Drawing.Size(308, 59);
            this.btnthem.TabIndex = 4;
            this.btnthem.Text = "Thêm";
            this.btnthem.Click += new System.EventHandler(this.btnthem_Click);
            // 
            // btnsua
            // 
            this.btnsua.BackColor = System.Drawing.Color.Transparent;
            this.btnsua.BorderRadius = 5;
            this.btnsua.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnsua.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnsua.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnsua.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnsua.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnsua.FillColor = System.Drawing.Color.Orange;
            this.btnsua.Font = new System.Drawing.Font("Segoe UI", 10.125F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnsua.ForeColor = System.Drawing.Color.White;
            this.btnsua.Image = ((System.Drawing.Image)(resources.GetObject("btnsua.Image")));
            this.btnsua.Location = new System.Drawing.Point(280, 895);
            this.btnsua.Name = "btnsua";
            this.btnsua.ShadowDecoration.Depth = 10;
            this.btnsua.ShadowDecoration.Enabled = true;
            this.btnsua.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(0, 0, 5, 5);
            this.btnsua.Size = new System.Drawing.Size(308, 59);
            this.btnsua.TabIndex = 2;
            this.btnsua.Text = "Sửa";
            this.btnsua.Click += new System.EventHandler(this.btnsua_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(20, 81);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(116, 32);
            this.label11.TabIndex = 1;
            this.label11.Text = "Mã hạng ";
            // 
            // pnl1
            // 
            this.pnl1.BackColor = System.Drawing.Color.Transparent;
            this.pnl1.BorderColor = System.Drawing.Color.Black;
            this.pnl1.BorderRadius = 10;
            this.pnl1.Controls.Add(this.dgvHangDG);
            this.pnl1.Controls.Add(this.guna2Panel4);
            this.pnl1.Controls.Add(this.btnlammoi);
            this.pnl1.Controls.Add(this.btntimkiem);
            this.pnl1.Controls.Add(this.txtTimKiem);
            this.pnl1.FillColor = System.Drawing.Color.White;
            this.pnl1.Location = new System.Drawing.Point(12, 55);
            this.pnl1.Name = "pnl1";
            this.pnl1.Size = new System.Drawing.Size(1034, 1058);
            this.pnl1.TabIndex = 2;
            // 
            // dgvHangDG
            // 
            this.dgvHangDG.AllowUserToAddRows = false;
            this.dgvHangDG.AllowUserToResizeColumns = false;
            this.dgvHangDG.AllowUserToResizeRows = false;
            this.dgvHangDG.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.ForestGreen;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Transparent;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvHangDG.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvHangDG.ColumnHeadersHeight = 35;
            this.dgvHangDG.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvHangDG.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSTT,
            this.colMaHang,
            this.colTenhang,
            this.colSachMax,
            this.colNgayMax});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvHangDG.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvHangDG.EnableHeadersVisualStyles = false;
            this.dgvHangDG.Location = new System.Drawing.Point(20, 145);
            this.dgvHangDG.MultiSelect = false;
            this.dgvHangDG.Name = "dgvHangDG";
            this.dgvHangDG.ReadOnly = true;
            this.dgvHangDG.RowHeadersVisible = false;
            this.dgvHangDG.RowHeadersWidth = 82;
            this.dgvHangDG.RowTemplate.Height = 33;
            this.dgvHangDG.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHangDG.Size = new System.Drawing.Size(996, 811);
            this.dgvHangDG.TabIndex = 8;
            this.dgvHangDG.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvHangDG_CellClick);
            this.dgvHangDG.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dgvHangDG_RowPostPaint);
            // 
            // colSTT
            // 
            this.colSTT.FillWeight = 12.08459F;
            this.colSTT.HeaderText = "STT";
            this.colSTT.MinimumWidth = 10;
            this.colSTT.Name = "colSTT";
            this.colSTT.ReadOnly = true;
            this.colSTT.Width = 40;
            // 
            // colMaHang
            // 
            this.colMaHang.DataPropertyName = "MaHang";
            this.colMaHang.FillWeight = 78.36274F;
            this.colMaHang.HeaderText = "Mã hạng";
            this.colMaHang.MinimumWidth = 10;
            this.colMaHang.Name = "colMaHang";
            this.colMaHang.ReadOnly = true;
            this.colMaHang.Width = 80;
            // 
            // colTenhang
            // 
            this.colTenhang.DataPropertyName = "TenHang";
            this.colTenhang.FillWeight = 209.5527F;
            this.colTenhang.HeaderText = "Tên hạng";
            this.colTenhang.MinimumWidth = 10;
            this.colTenhang.Name = "colTenhang";
            this.colTenhang.ReadOnly = true;
            this.colTenhang.Width = 80;
            // 
            // colSachMax
            // 
            this.colSachMax.DataPropertyName = "SoSachToiDa";
            this.colSachMax.HeaderText = "Số sách được mượn tối đa/lần";
            this.colSachMax.MinimumWidth = 10;
            this.colSachMax.Name = "colSachMax";
            this.colSachMax.ReadOnly = true;
            this.colSachMax.Width = 200;
            // 
            // colNgayMax
            // 
            this.colNgayMax.DataPropertyName = "SoNgayMuonToiDa";
            this.colNgayMax.HeaderText = "Số ngày được mượn/lần";
            this.colNgayMax.MinimumWidth = 10;
            this.colNgayMax.Name = "colNgayMax";
            this.colNgayMax.ReadOnly = true;
            this.colNgayMax.Width = 200;
            // 
            // guna2Panel4
            // 
            this.guna2Panel4.BorderRadius = 10;
            this.guna2Panel4.Controls.Add(this.guna2Panel7);
            this.guna2Panel4.Controls.Add(this.label9);
            this.guna2Panel4.CustomizableEdges.BottomLeft = false;
            this.guna2Panel4.CustomizableEdges.BottomRight = false;
            this.guna2Panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.guna2Panel4.FillColor = System.Drawing.Color.WhiteSmoke;
            this.guna2Panel4.Location = new System.Drawing.Point(0, 0);
            this.guna2Panel4.Name = "guna2Panel4";
            this.guna2Panel4.Size = new System.Drawing.Size(1034, 59);
            this.guna2Panel4.TabIndex = 5;
            // 
            // guna2Panel7
            // 
            this.guna2Panel7.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.guna2Panel7.FillColor = System.Drawing.Color.ForestGreen;
            this.guna2Panel7.Location = new System.Drawing.Point(0, 54);
            this.guna2Panel7.Name = "guna2Panel7";
            this.guna2Panel7.Size = new System.Drawing.Size(1034, 5);
            this.guna2Panel7.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 10.125F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label9.Location = new System.Drawing.Point(21, 12);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(310, 37);
            this.label9.TabIndex = 0;
            this.label9.Text = "Danh sách và Tìm kiếm";
            // 
            // btnlammoi
            // 
            this.btnlammoi.BorderRadius = 5;
            this.btnlammoi.BorderThickness = 1;
            this.btnlammoi.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnlammoi.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnlammoi.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnlammoi.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnlammoi.FillColor = System.Drawing.Color.White;
            this.btnlammoi.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnlammoi.ForeColor = System.Drawing.Color.OliveDrab;
            this.btnlammoi.Location = new System.Drawing.Point(836, 69);
            this.btnlammoi.Name = "btnlammoi";
            this.btnlammoi.Size = new System.Drawing.Size(180, 50);
            this.btnlammoi.TabIndex = 3;
            this.btnlammoi.Text = "Làm mới";
            this.btnlammoi.Click += new System.EventHandler(this.btnlammoi_Click);
            // 
            // btntimkiem
            // 
            this.btntimkiem.BorderRadius = 5;
            this.btntimkiem.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btntimkiem.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btntimkiem.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btntimkiem.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btntimkiem.FillColor = System.Drawing.Color.OliveDrab;
            this.btntimkiem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btntimkiem.ForeColor = System.Drawing.Color.White;
            this.btntimkiem.Location = new System.Drawing.Point(650, 69);
            this.btntimkiem.Name = "btntimkiem";
            this.btntimkiem.Size = new System.Drawing.Size(180, 50);
            this.btntimkiem.TabIndex = 1;
            this.btntimkiem.Text = "Tìm kiếm";
            this.btntimkiem.Click += new System.EventHandler(this.btntimkiem_Click);
            // 
            // txtTimKiem
            // 
            this.txtTimKiem.BorderRadius = 5;
            this.txtTimKiem.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtTimKiem.DefaultText = "";
            this.txtTimKiem.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtTimKiem.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtTimKiem.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtTimKiem.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtTimKiem.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtTimKiem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtTimKiem.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtTimKiem.IconRight = ((System.Drawing.Image)(resources.GetObject("txtTimKiem.IconRight")));
            this.txtTimKiem.Location = new System.Drawing.Point(20, 69);
            this.txtTimKiem.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtTimKiem.Name = "txtTimKiem";
            this.txtTimKiem.PlaceholderForeColor = System.Drawing.Color.DarkGray;
            this.txtTimKiem.PlaceholderText = "Tìm kiếm mã hạng hoặc tên hạng";
            this.txtTimKiem.SelectedText = "";
            this.txtTimKiem.Size = new System.Drawing.Size(621, 50);
            this.txtTimKiem.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 10.125F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.ForestGreen;
            this.label6.Location = new System.Drawing.Point(15, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(291, 37);
            this.label6.TabIndex = 3;
            this.label6.Text = "Quản lý Hạng độc giả";
            // 
            // ucHangDG
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightCyan;
            this.Controls.Add(this.label6);
            this.Controls.Add(this.pnl2);
            this.Controls.Add(this.pnl1);
            this.Name = "ucHangDG";
            this.Padding = new System.Windows.Forms.Padding(12);
            this.Size = new System.Drawing.Size(1840, 1120);
            this.Load += new System.EventHandler(this.ucHangDG_Load);
            this.pnl2.ResumeLayout(false);
            this.pnl2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudsach)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudngay)).EndInit();
            this.guna2Panel1.ResumeLayout(false);
            this.guna2Panel1.PerformLayout();
            this.pnl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvHangDG)).EndInit();
            this.guna2Panel4.ResumeLayout(false);
            this.guna2Panel4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Guna.UI2.WinForms.Guna2Panel pnl2;
        private Guna.UI2.WinForms.Guna2Panel pnl1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private Guna.UI2.WinForms.Guna2TextBox txtTimKiem;
        private Guna.UI2.WinForms.Guna2Button btnsua;
        private Guna.UI2.WinForms.Guna2Button btntimkiem;
        private Guna.UI2.WinForms.Guna2Button btnthem;
        private Guna.UI2.WinForms.Guna2Button btnlammoi;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label12;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel4;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel6;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel7;
        private Guna.UI2.WinForms.Guna2TextBox txtmahang;
        private Guna.UI2.WinForms.Guna2TextBox txttenhang;
        private Guna.UI2.WinForms.Guna2Button btnxoa;
        private System.Windows.Forms.DataGridView dgvHangDG;
        private System.Windows.Forms.Label label1;
        private Guna.UI2.WinForms.Guna2NumericUpDown nudsach;
        private Guna.UI2.WinForms.Guna2NumericUpDown nudngay;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSTT;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMaHang;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTenhang;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSachMax;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNgayMax;
    }
}
