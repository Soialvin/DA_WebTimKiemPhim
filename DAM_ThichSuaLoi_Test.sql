CREATE DATABASE DAM_ThichSuaLoi_Test
GO

USE DAM_ThichSuaLoi_Test
GO

CREATE TABLE TaiKhoan(
    TenTK VARCHAR(50) PRIMARY KEY,
    MatKhau VARCHAR(15) NULL,
    HoVaTen NVARCHAR(50) NULL,
    HinhAnh VARCHAR(max),
    DiaChi NVARCHAR(256) NULL,
    SoDienThoai CHAR(10) NOT NULL UNIQUE,
    Email VARCHAR(30) NOT NULL UNIQUE,
    LoaiTK VARCHAR(5) NOT NULL,
    NgayDK DATE NOT NULL DEFAULT GETDATE()
);
GO

CREATE OR ALTER PROC InsertTK
    @TenTK VARCHAR(50),
    @MatKhau VARCHAR(15),
    @HoVaTen NVARCHAR(50),
    @HinhAnh VARCHAR(max),
    @DiaChi NVARCHAR(256),
    @SoDienThoai CHAR(10),
    @Email VARCHAR(30),
    @LoaiTK VARCHAR(5),
    @NgayDK DATE
AS
BEGIN
    IF @NgayDK IS NULL
        SET @NgayDK = GETDATE()
    INSERT INTO TaiKhoan(TenTK,MatKhau,HoVaTen,HinhAnh,DiaChi,SoDienThoai,Email,LoaiTK,NgayDK)
    VALUES(@TenTK,@MatKhau,@HoVaTen,@HinhAnh,@DiaChi,@SoDienThoai,@Email,@LoaiTK,@NgayDK)
END
GO
EXEC InsertTK 'khachhang01','khachhang01',N'Nguyễn Văn A',NULL,N'Buôn Ma Thuột','0368515813','a@gmail.com','User',NULL
EXEC InsertTK 'khachhang02','khachhang02',N'Nguyễn Văn B',NULL,N'Buôn Ma Thuột','0368515814','b@gmail.com','User',NULL
EXEC InsertTK 'khachhang03','khachhang03',N'Nguyễn Văn C',NULL,N'Buôn Ma Thuột','0368515815','c@gmail.com','User',NULL
EXEC InsertTK 'khachhang04','khachhang04',N'Nguyễn Văn D',NULL,N'Buôn Ma Thuột','0368515816','d@gmail.com','User',NULL
EXEC InsertTK 'khachhang05','khachhang05',N'Nguyễn Văn F',NULL,N'Buôn Ma Thuột','0368515817','e@gmail.com','User',NULL
EXEC InsertTK 'Admin','Admin',N'Nguyễn Văn G',NULL,N'Buôn Ma Thuột','0368515818','admin@gmail.com','Admin',NULL
GO

CREATE TABLE Rap(
    MaRap VARCHAR(10) NOT NULL PRIMARY KEY,
    TenRap NVARCHAR(50) NULL,
    DiaChi NVARCHAR(256) NULL
);
GO

CREATE OR ALTER PROC InsertRap
    @MaRap VARCHAR(10),
    @TenRap NVARCHAR(50),
    @DiaChi NVARCHAR(256)
AS
BEGIN
    INSERT INTO Rap(MaRap,TenRap,DiaChi)
    VALUES(@MaRap,@TenRap,@DiaChi)
END
GO

EXEC InsertRap 'R001', N'Galaxy Buôn Ma Thuột',N'Buôn Ma Thuột'
EXEC InsertRap 'R002', N'Cgv Buôn Ma Thuột',N'Buôn Ma Thuột'
EXEC InsertRap 'R003', N'StarLight Buôn Ma Thuộc',N'Buôn Ma Thuột'
EXEC InsertRap 'R004', N'Kim Đồng Buôn Ma Thuột',N'Buôn Ma Thuột'
EXEC InsertRap 'R005', N'Rạp Hưng Đạo',N'Buôn Ma Thuột'
GO

CREATE TABLE SuatChieu(
    MaSC VARCHAR(10) PRIMARY KEY NOT NULL,
    NgayChieu DATE DEFAULT '1990-01-01' NULL,
    KhungGio TIME DEFAULT '12:00:00' NULL,
    LoaiChieu VARCHAR(10) NULL
);
GO

CREATE OR ALTER PROC insertSuatChieu
    @MaSC VARCHAR(10),
    @NgayChieu DATE,
    @KhungGio TIME,
    @LoaiChieu VARCHAR(10) 
AS
BEGIN
    INSERT INTO SuatChieu(MaSC,NgayChieu,KhungGio,LoaiChieu)
    VALUES(@MaSC,@NgayChieu,@KhungGio,@LoaiChieu)
END
GO

EXEC insertSuatChieu 'SC001','2021-02-01','12:00:00','2D'
EXEC insertSuatChieu 'SC002','2021-02-01','02:00:00','2D'
EXEC insertSuatChieu 'SC003','2021-02-01','10:00:00','2D'
EXEC insertSuatChieu 'SC004','2021-02-01','08:00:00','2D'
EXEC insertSuatChieu 'SC005','2021-02-01','18:00:00','2D'
GO

CREATE TABLE Phim(
    MaPhim VARCHAR(10) NOT NULL PRIMARY KEY,
    TenPhim NVARCHAR(50) NULL,
    HinhAnh NVARCHAR(256) NULL,
    ThongTinPhim NVARCHAR(MAX) NULL,
    NoiDung NVARCHAR(Max) NULL,
    ThoiLuong int NULL,
    TrangThai NVARCHAR(50) NULL,
    NgayChieu DATE DEFAULT '1990-01-01' NULL
);
GO

CREATE OR ALTER PROC InsertPhim
    @MaPhim VARCHAR(10),
    @TenPhim NVARCHAR(50),
    @HinhAnh NVARCHAR(256),
    @ThongTinPhim NVARCHAR(MAX),
    @NoiDung NVARCHAR(MAX),
    @ThoiLuong INT,
    @TrangThai NVARCHAR(50),
    @NgayChieu DATE
AS
BEGIN
    INSERT INTO Phim(MaPhim,TenPhim,HinhAnh,ThongTinPhim,NoiDung,ThoiLuong,TrangThai,NgayChieu)
    VALUES(@MaPhim,@TenPhim,@HinhAnh,@ThongTinPhim,@NoiDung,@ThoiLuong,@TrangThai,@NgayChieu)
END
GO
EXEC InsertPhim 'P001',N'Nhà Bà Nữ',NULL,N'NSX: Trấn Thành, Diễn Viên: Uyên An, Trấn Thành,','abcxyz',120,N'Đã Chiếu','2021-02-01'
EXEC InsertPhim 'P002',N'Nhà Bà Nữ',NULL,N'NSX: Marvel, Diễn Viên: Stephen Strange,','abcxyz',120,N'Đang Chiếu','2021-12-04'
EXEC InsertPhim 'P003',N'Đất Rừng Phương Nam',NULL,N'NSX: Trần Văn Nam, Diễn Viên: Nguyễn Hoàn, Trấn Thành,','abcxyz',120,N'Sắp Chiếu','2023-10-20'
EXEC InsertPhim 'P004',N'Shazam: rising of the god',NULL,N'NSX: DC, Diễn Viên: The Rock, Black Adam,','abcxyz',120,N'Đã Chiếu','2021-02-01'
EXEC InsertPhim 'P005',N'Ant-Man: Quantumania',NULL,N'NSX: Marvel, Diễn Viên: Anthony William','abcxyz',120,N'Đã Chiếu','2021-02-01'
GO

CREATE TABLE Rap_Phim(
    ID INT IDENTITY(1,1) PRIMARY KEY,
    MaRap VARCHAR(10) NULL,
    MaPhim VARCHAR(10) NULL,
    FOREIGN KEY(MaRap) REFERENCES Rap(MaRap),
    FOREIGN KEY (MaPhim) REFERENCES Phim(MaPhim)
);
GO

CREATE OR ALTER PROC InsertRap_Phim
    @MaRap VARCHAR(10),
    @MaPhim VARCHAR(10)
AS
BEGIN
    INSERT INTO RAP_PHIM(MaRap,MaPhim)
    VALUES (@MaRap,@MaPhim)
END
GO

EXEC InsertRap_Phim 'R001','P001'
EXEC InsertRap_Phim 'R002','P001'
EXEC InsertRap_Phim 'R003','P002'
EXEC InsertRap_Phim 'R004','P003'
EXEC InsertRap_Phim 'R005','P004'
GO

CREATE TABLE SuatChieu_Phim(
    ID INT IDENTITY(1,1) PRIMARY KEY,
    MaSC VARCHAR(10),
    MaPhim VARCHAR(10),
    FOREIGN KEY (MaSC) REFERENCES SuatChieu(MaSC),
    FOREIGN KEY (MaPhim) REFERENCES Phim(MaPhim)
);
GO
CREATE OR ALTER PROC insertSuatChieu_Phim 
    
    @MaSC VARCHAR(10),
    @MaPhim VARCHAR(10)
AS
BEGIN
    INSERT INTO SuatChieu_Phim(MaSC,MaPhim)
    VALUES (@MaSC,@MaPhim)
END
GO
EXEC insertSuatChieu_Phim 'SC001','P002'
EXEC insertSuatChieu_Phim 'SC001','P003'
EXEC insertSuatChieu_Phim 'SC002','P004'
EXEC insertSuatChieu_Phim 'SC003','P002'
EXEC insertSuatChieu_Phim 'SC004','P005'
GO

CREATE TABLE TheLoai(
    MaTL VARCHAR(10) PRIMARY KEY NOT NULL,
    TenTL NVARCHAR(50) NULL
);
GO
CREATE OR ALTER PROC InsertTheLoai
    @MaTL VARCHAR(10),
    @TenTL NVARCHAR(50)
AS
BEGIN
    INSERT INTO TheLoai(MaTL,TenTL)
    VALUES(@MaTL,@TenTL)
END
GO
EXEC InsertTheLoai 'TL001',N'Lãng Mạng'
EXEC InsertTheLoai 'TL002',N'Trinh thám'
EXEC InsertTheLoai 'TL003',N'Kinh Dị'
EXEC InsertTheLoai 'TL004',N'Khoa Học Viễn tưởng'
EXEC InsertTheLoai 'TL005',N'Lịch Sử'
EXEC InsertTheLoai 'TL006',N'Hoạt hình'
EXEC InsertTheLoai 'TL007',N'Hài'
GO
CREATE TABLE Phim_TheLoai(
    ID INT IDENTITY(1,1) PRIMARY KEY,
    MaTL VARCHAR(10) NULL,
    MaPhim VARCHAR(10) NULL
    FOREIGN KEY (MaTL) REFERENCES TheLoai(MaTL),
    FOREIGN KEY (MaPhim) REFERENCES Phim(MaPhim)
);
GO
CREATE OR ALTER PROC InsertPhim_TheLoai
    @MaTL VARCHAR(10),
    @MaPhim VARCHAR(10)
AS
BEGIN
    INSERT INTO Phim_TheLoai(MaTL,MaPhim)
    VALUES(@MaTL,@MaPhim)
END
GO
EXEC InsertPhim_TheLoai 'TL006','P001'
EXEC InsertPhim_TheLoai 'TL004','P002'
EXEC InsertPhim_TheLoai 'TL005','P003'
EXEC InsertPhim_TheLoai 'TL004','P004'
EXEC InsertPhim_TheLoai 'TL004','P005'
GO

CREATE OR ALTER TRIGGER xoaLQRap on Rap INSTEAD OF DELETE
AS
    BEGIN
        DELETE FROM RAP_PHIM WHERE MaRap IN (SELECT MaRap FROM deleted)
        DELETE FROM Rap WHERE MaRap IN (SELECT MaRap FROM deleted)
    END
GO
CREATE OR ALTER TRIGGER xoaLQSC on SuatChieu INSTEAD OF DELETE
AS
    BEGIN
        DELETE FROM SuatChieu_Phim WHERE MaSC IN (SELECT MaSC FROM deleted)
        DELETE FROM SuatChieu WHERE MaSC IN (SELECT MaSC FROM deleted)
    END
GO
CREATE OR ALTER TRIGGER xoaLQP on PHIM INSTEAD OF DELETE
AS
    BEGIN 
        DELETE FROM SuatChieu_Phim WHERE MaPhim IN (SELECT MaPhim FROM deleted)
        DELETE FROM Rap_Phim WHERE MaPhim IN (SELECT MaPhim FROM deleted)
        DELETE FROM Phim_TheLoai WHERE MaPhim IN (SELECT MaPhim FROM deleted)
        DELETE FROM Phim WHERE MaPhim IN (SELECT MaPhim FROM deleted)
    END
GO
CREATE OR ALTER PROC ĐếmTàiKhoảnTheoThángNăm AS
BEGIN
    SELECT
        YEAR(NgayDK) AS Năm,
        MONTH(NgayDK) AS Tháng,
        COUNT(*) AS SốTàiKhoản
    FROM TaiKhoan
    GROUP BY YEAR(NgayDK), MONTH(NgayDK)
    ORDER BY Năm, Tháng;
END
GO


EXEC ĐếmTàiKhoảnTheoThángNăm
SELECT * FROM TheLoai
SELECT * FROM Phim_TheLoai
SELECT * FROM Phim
SELECT * FROM RAP_PHIM
SELECT * FROM SuatChieu_Phim
SELECT * FROM TaiKhoan