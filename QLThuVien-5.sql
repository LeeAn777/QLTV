-- =========================================================
-- DATABASE QUAN LY THU VIEN - SQL SERVER
-- Dung cho C# WinForms / ADO.NET
-- 15 form, 16 bang theo mo ta he thong
-- =========================================================

IF DB_ID(N'QLThuVien') IS NULL
    CREATE DATABASE QLThuVien;
GO

USE QLThuVien;
GO

-- Neu chay lai script tren database da co bang, cac lenh CREATE
-- TABLE se bao loi. Nen dung database moi hoac xoa database cu truoc.

-- 1. SACH
CREATE TABLE dbo.sach
(
    MaSach INT IDENTITY(1,1) PRIMARY KEY,
    TenSach NVARCHAR(200) NOT NULL,
    MaTheLoai INT NOT NULL,
    MaNXB INT NOT NULL,
    NamXuatBan INT NULL CHECK (NamXuatBan IS NULL OR NamXuatBan BETWEEN 1 AND 2100),
    SoLuong INT NOT NULL CONSTRAINT DF_sach_SoLuong DEFAULT 0
        CHECK (SoLuong >= 0),

    CONSTRAINT FK_sach_theloai
        FOREIGN KEY (MaTheLoai) REFERENCES dbo.theloai(MaTheLoai),
    CONSTRAINT FK_sach_nhaxuatban
        FOREIGN KEY (MaNXB) REFERENCES dbo.nhaxuatban(MaNXB)
);
GO

-- 2. THE LOAI
CREATE TABLE dbo.theloai
(
    MaTheLoai INT IDENTITY(1,1) PRIMARY KEY,
    TenTheLoai NVARCHAR(100) NOT NULL UNIQUE,
    MoTa NVARCHAR(255) NULL
);
GO

-- 3. NHA XUAT BAN
CREATE TABLE dbo.nhaxuatban
(
    MaNXB INT IDENTITY(1,1) PRIMARY KEY,
    TenNXB NVARCHAR(150) NOT NULL,
    DiaChi NVARCHAR(255) NULL,
    SoDienThoai VARCHAR(15) NULL,
    Email VARCHAR(100) NULL UNIQUE
);
GO

-- 4. DOC GIA
CREATE TABLE dbo.docgia
(
    MaDocGia INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    NgaySinh DATE NULL,
    GioiTinh NVARCHAR(10) NULL
        CHECK (GioiTinh IS NULL OR GioiTinh IN (N'Nam', N'Nữ', N'Khác')),
    DiaChi NVARCHAR(255) NULL,
    SoDienThoai VARCHAR(15) NULL,
    Email VARCHAR(100) NULL UNIQUE,
    NgayDangKy DATE NOT NULL
        CONSTRAINT DF_docgia_NgayDangKy DEFAULT (CONVERT(DATE, GETDATE())),
    MaHang INT NOT NULL,

    CONSTRAINT FK_docgia_hangdocgia
        FOREIGN KEY (MaHang) REFERENCES dbo.hangdocgia(MaHang)
);
GO

-- 5. HANG DOC GIA
CREATE TABLE dbo.hangdocgia
(
    MaHang INT IDENTITY(1,1) PRIMARY KEY,
    TenHang NVARCHAR(50) NOT NULL UNIQUE,
    SoSachToiDa INT NOT NULL CHECK (SoSachToiDa > 0),
    SoNgayMuonToiDa INT NOT NULL CHECK (SoNgayMuonToiDa > 0)
);
GO
-- 6. THE DOC GIA
CREATE TABLE dbo.thedocgia
(
    MaThe INT IDENTITY(1,1) PRIMARY KEY,
    MaDocGia INT NOT NULL,
    NgayCap DATE NOT NULL
        CONSTRAINT DF_thedocgia_NgayCap DEFAULT (CONVERT(DATE, GETDATE())),
    NgayHetHan DATE NOT NULL,
    TrangThai NVARCHAR(20) NOT NULL
        CONSTRAINT DF_thedocgia_TrangThai DEFAULT N'Hoạt động'
        CHECK (TrangThai IN (N'Hoạt động', N'Hết hạn', N'Bị khóa')),

    CONSTRAINT FK_thedocgia_docgia
        FOREIGN KEY (MaDocGia) REFERENCES dbo.docgia(MaDocGia),
    CONSTRAINT CK_thedocgia_Ngay
        CHECK (NgayHetHan > NgayCap)
);
GO
-- 6. NHAN VIEN
CREATE TABLE dbo.nhanvien
(
    MaNV INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    ChucVu NVARCHAR(50) NULL,
    SoDienThoai VARCHAR(15) NULL,
    Email VARCHAR(100) NULL UNIQUE,
    DiaChi NVARCHAR(255) NULL,
    TrangThai NVARCHAR(20) NOT NULL
        CONSTRAINT DF_nhanvien_TrangThai DEFAULT N'Đang làm'
        CHECK (TrangThai IN (N'Đang làm', N'Nghỉ việc'))
);
GO

--7. TAC GIA
CREATE TABLE dbo.tacgia
(
    MaTacGia INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    NgaySinh DATE NULL,
    QuocTich NVARCHAR(50) NULL,
    TieuSu NVARCHAR(MAX) NULL
);
GO
-- 8. LIEN KET SACH - TAC GIA (quan he nhieu-nhieu)
CREATE TABLE dbo.sach_tacgia
(
    MaSach INT NOT NULL,
    MaTacGia INT NOT NULL,

    CONSTRAINT PK_sach_tacgia PRIMARY KEY (MaSach, MaTacGia),
    CONSTRAINT FK_sach_tacgia_sach
        FOREIGN KEY (MaSach) REFERENCES dbo.sach(MaSach),
    CONSTRAINT FK_sach_tacgia_tacgia
        FOREIGN KEY (MaTacGia) REFERENCES dbo.tacgia(MaTacGia)
);
GO

-- 10. PHIEU MUON
CREATE TABLE dbo.phieumuon
(
    MaPhieuMuon INT IDENTITY(1,1) PRIMARY KEY,
    MaDocGia INT NOT NULL,
    MaNV INT NOT NULL,
    NgayMuon DATE NOT NULL
        CONSTRAINT DF_phieumuon_NgayMuon DEFAULT (CONVERT(DATE, GETDATE())),

    CONSTRAINT FK_phieumuon_docgia
        FOREIGN KEY (MaDocGia) REFERENCES dbo.docgia(MaDocGia),
    CONSTRAINT FK_phieumuon_nhanvien
        FOREIGN KEY (MaNV) REFERENCES dbo.nhanvien(MaNV)
);
GO

-- 11. CHI TIET PHIEU MUON
CREATE TABLE dbo.chitietphieumuon
(
    MaPhieuMuon INT NOT NULL,
    MaSach INT NOT NULL,
    SoLuongMuon INT NOT NULL CHECK (SoLuongMuon > 0),
    HanTra DATE NOT NULL,

    CONSTRAINT PK_chitietphieumuon PRIMARY KEY (MaPhieuMuon, MaSach),
    CONSTRAINT FK_ctpm_phieumuon
        FOREIGN KEY (MaPhieuMuon) REFERENCES dbo.phieumuon(MaPhieuMuon),
    CONSTRAINT FK_ctpm_sach
        FOREIGN KEY (MaSach) REFERENCES dbo.sach(MaSach)
);
GO

-- 12. PHIEU TRA
CREATE TABLE dbo.phieutra
(
    MaPhieuTra INT IDENTITY(1,1) PRIMARY KEY,
    MaPhieuMuon INT NOT NULL,
    MaNV INT NOT NULL,
    NgayTra DATE NOT NULL
        CONSTRAINT DF_phieutra_NgayTra DEFAULT (CONVERT(DATE, GETDATE())),

    CONSTRAINT FK_phieutra_phieumuon
        FOREIGN KEY (MaPhieuMuon) REFERENCES dbo.phieumuon(MaPhieuMuon),
    CONSTRAINT FK_phieutra_nhanvien
        FOREIGN KEY (MaNV) REFERENCES dbo.nhanvien(MaNV)
);
GO

-- 13. CHI TIET PHIEU TRA
CREATE TABLE dbo.chitietphieutra
(
    MaPhieuTra INT NOT NULL,
    MaSach INT NOT NULL,
    SoLuongTra INT NOT NULL CHECK (SoLuongTra > 0),
    TinhTrangSach NVARCHAR(50) NOT NULL
        CONSTRAINT DF_ctpt_TinhTrang DEFAULT N'Tốt'
        CHECK (TinhTrangSach IN (N'Tốt', N'Hư hỏng', N'Mất')),
    TienPhat DECIMAL(18,0) NOT NULL
        CONSTRAINT DF_ctpt_TienPhat DEFAULT 0
        CHECK (TienPhat >= 0),

    CONSTRAINT PK_chitietphieutra PRIMARY KEY (MaPhieuTra, MaSach),
    CONSTRAINT FK_ctpt_phieutra
        FOREIGN KEY (MaPhieuTra) REFERENCES dbo.phieutra(MaPhieuTra),
    CONSTRAINT FK_ctpt_sach
        FOREIGN KEY (MaSach) REFERENCES dbo.sach(MaSach)
);
GO

CREATE TABLE dbo.phieuphat
(
    MaPhieuPhat INT IDENTITY(1,1) PRIMARY KEY,
    MaDocGia INT NOT NULL,
    MaNV INT NOT NULL, -- Nhân viên lập phiếu thu
    SoTienPhat DECIMAL(18,0) NOT NULL CHECK (SoTienPhat > 0),
    LyDo NVARCHAR(255) NULL,
    NgayTao DATE NOT NULL 
        CONSTRAINT DF_phieuphat_NgayTao DEFAULT (CONVERT(DATE, GETDATE())),
    TrangThai NVARCHAR(50) NOT NULL 
        CONSTRAINT DF_phieuphat_TrangThai DEFAULT N'Chưa thanh toán'
        CHECK (TrangThai IN (N'Chưa thanh toán', N'Đã thanh toán')),

    CONSTRAINT FK_phieuphat_docgia
        FOREIGN KEY (MaDocGia) REFERENCES dbo.docgia(MaDocGia),
    CONSTRAINT FK_phieuphat_nhanvien
        FOREIGN KEY (MaNV) REFERENCES dbo.nhanvien(MaNV)
);
GO

CREATE VIEW dbo.vw_XuHuong_TopSach AS
SELECT TOP 10 
    s.MaSach, 
    s.TenSach, 
    tl.TenTheLoai,
    COUNT(ct.MaSach) AS TongLuotMuon
FROM dbo.sach s
JOIN dbo.theloai tl ON s.MaTheLoai = tl.MaTheLoai
JOIN dbo.chitietphieumuon ct ON s.MaSach = ct.MaSach
GROUP BY s.MaSach, s.TenSach, tl.TenTheLoai
ORDER BY TongLuotMuon DESC;
GO

-- 16. TAI KHOAN NHAN VIEN
CREATE TABLE dbo.taikhoannhanvien
(
    MaTK INT IDENTITY(1,1) PRIMARY KEY,
    MaNV INT NOT NULL UNIQUE,
    TenDangNhap VARCHAR(50) NOT NULL UNIQUE,
    MatKhau VARCHAR(255) NOT NULL,
    Quyen NVARCHAR(30) NOT NULL
        CHECK (Quyen IN (N'Admin', N'Thủ thư')),
    TrangThai NVARCHAR(20) NOT NULL
        CONSTRAINT DF_tknv_TrangThai DEFAULT N'Hoạt động'
        CHECK (TrangThai IN (N'Hoạt động', N'Bị khóa')),

    CONSTRAINT FK_tknv_nhanvien
        FOREIGN KEY (MaNV) REFERENCES dbo.nhanvien(MaNV)
);
GO

-- INDEX co ban ho tro tim kiem / JOIN
CREATE INDEX IX_sach_TenSach ON dbo.sach(TenSach);
CREATE INDEX IX_docgia_HoTen ON dbo.docgia(HoTen);
CREATE INDEX IX_phieumuon_MaDocGia_NgayMuon ON dbo.phieumuon(MaDocGia, NgayMuon);
CREATE INDEX IX_phieutra_MaPhieuMuon ON dbo.phieutra(MaPhieuMuon);
GO

-- 3. CHÈN DỮ LIỆU MẪU
INSERT INTO dbo.theloai (TenTheLoai, MoTa) VALUES 
(N'Văn học', N'Các tác phẩm tiểu thuyết, truyện ngắn'),
(N'Công nghệ thông tin', N'Sách lập trình, phần cứng, mạng'),
(N'Khoa học', N'Sách nghiên cứu tự nhiên, vũ trụ');

INSERT INTO dbo.nhaxuatban (TenNXB, DiaChi, SoDienThoai, Email) VALUES
(N'Nhà xuất bản Trẻ', N'TP. Hồ Chí Minh', '0281234567', 'tre@example.com'),
(N'Nhà xuất bản Kim Đồng', N'Hà Nội', '0241234567', 'kimdong@example.com');

INSERT INTO dbo.hangdocgia (TenHang, SoSachToiDa, SoNgayMuonToiDa) VALUES
(N'Thường', 3, 14),
(N'Bạc', 5, 21),
(N'Vàng', 7, 30);

INSERT INTO dbo.tacgia (HoTen, NgaySinh, QuocTich, TieuSu) VALUES
(N'Nguyễn Nhật Ánh', '1955-05-07', N'Việt Nam', N'Tác giả văn học thiếu nhi và tuổi mới lớn.'),
(N'Tô Hoài', '1920-09-27', N'Việt Nam', N'Nhà văn Việt Nam.');

INSERT INTO dbo.nhanvien (HoTen, ChucVu, SoDienThoai, Email, DiaChi) VALUES
(N'Nguyễn Văn A', N'Thủ thư', '0900000001', 'nva@example.com', N'Hà Nội'),
(N'Trần Thị B', N'Quản lý', '0900000002', 'ttb@example.com', N'Hà Nội');

INSERT INTO dbo.sach (TenSach, MaTheLoai, MaNXB, NamXuatBan, SoLuong) VALUES
(N'Mắt biếc', 1, 1, 1990, 10),
(N'Lập trình C# cơ bản', 2, 2, 2023, 8),
(N'Dế Mèn phiêu lưu ký', 1, 2, 1941, 5);

INSERT INTO dbo.docgia (HoTen, NgaySinh, GioiTinh, DiaChi, SoDienThoai, Email, MaHang) VALUES
(N'Lê Văn C', '2005-03-15', N'Nam', N'Hà Nội', '0911111111', 'lvc@example.com', 1),
(N'Phạm Thị D', '2004-08-21', N'Nữ', N'Hà Nội', '0922222222', 'ptd@example.com', 2);

INSERT INTO dbo.thedocgia (MaDocGia, NgayCap, NgayHetHan, TrangThai) VALUES
(1, CONVERT(DATE, GETDATE()), DATEADD(YEAR, 1, CONVERT(DATE, GETDATE())), N'Hoạt động'),
(2, CONVERT(DATE, GETDATE()), DATEADD(YEAR, 1, CONVERT(DATE, GETDATE())), N'Hoạt động');

INSERT INTO dbo.sach_tacgia (MaSach, MaTacGia) VALUES
(1, 1), (2, 1), (3, 2);
INSERT INTO dbo.phieumuon (MaDocGia, MaNV, NgayMuon) VALUES
(1, 8, '2026-10-01'),
(3, 9, '2026-10-10'); 
GO

INSERT INTO dbo.chitietphieumuon (MaPhieuMuon, MaSach, SoLuongMuon, HanTra) VALUES
(1, 3, 1, '2026-10-15'), 
(1, 2, 1, '2026-10-15'), 
(2, 4, 2, '2026-10-24');
GO

INSERT INTO dbo.phieutra (MaPhieuMuon, MaNV, NgayTra) VALUES
(1, 8, '2026-10-14'); 
GO

INSERT INTO dbo.chitietphieutra (MaPhieuTra, MaSach, SoLuongTra, TinhTrangSach, TienPhat) VALUES
(1, 3, 1, N'Tốt', 0),
(1, 2, 1, N'Tốt', 0);
GO

-- KIEM TRA CAC BANG
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
GO


