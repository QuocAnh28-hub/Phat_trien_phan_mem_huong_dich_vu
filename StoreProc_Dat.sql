-------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetKhachHang
AS
BEGIN
    SELECT MaKH, TenKH, SDT, DiaChi
    FROM KHACHHANG
    WHERE 
        MaKH IS NOT NULL AND MaKH <> ''
END
EXEC sp_GetKhachHang
-------------------------------------------------------------------------------------------------
CREATE OR ALTER PROC sp_GetByIDKhachHang(@MAKH CHAR(15))
AS
BEGIN
    SELECT MaKH, TenKH, SDT, DiaChi
    FROM KHACHHANG
    WHERE 
        MaKH = @MaKH
END
EXEC sp_GetByIDKhachHang'KH006'
-------------------------------------------------------------------------------------------------
--XÓA THÔNG TIN CỦA KH CÓ MÃ BẤT KỲ
CREATE OR ALTER PROC SP_XOAKH(@MAKH CHAR(15))
AS
BEGIN
	IF(NOT EXISTS(SELECT * FROM KHACHHANG K WHERE K.MAKH = @MAKH)) 
		RETURN -1
	IF(EXISTS(SELECT * FROM KHACHHANG K WHERE K.MAKH = @MAKH))
	DELETE FROM KHACHHANG
	WHERE MAKH = @MAKH
END
EXEC SP_XOAKH'SV100'--KHÔNG XÓA ĐƯỢC
EXEC SP_XOAKH'KH005'--XÓA ĐƯỢC
-------------------------------------------------------------------------------------------------
--THÊM THÔNG TIN CỦA KH
CREATE OR ALTER PROC SP_THEMKH
	@MAKH CHAR(15),
	@TENKH NVARCHAR(100),
	@SDT CHAR(10),
	@DIACHI NVARCHAR(300)
AS
BEGIN
	IF(EXISTS(SELECT * FROM KHACHHANG K WHERE K.MAKH = @MAKH)) 
		RETURN -1
	IF(NOT EXISTS(SELECT * FROM KHACHHANG K WHERE K.MAKH = @MAKH))
		INSERT INTO KHACHHANG VALUES (@MAKH, @TENKH, @SDT, @DIACHI)
END
EXEC SP_THEMKH'KH006', N'Đỗ Tiến Đạt', '0905555555', N'Hưng Yên'
EXEC SP_THEMKH'KH007', N'Đỗ Hữu Quốc Anh', '0905555555', N'Hưng Yên'
-------------------------------------------------------------------------------------------------
--SỬA THÔNG TIN CỦA KH CÓ MÃ BẤT KỲ
CREATE OR ALTER PROC SP_SUAKH(@MAKH CHAR(15),
								@TENKH NVARCHAR(100),
								@SDT CHAR(10),
								@DIACHI NVARCHAR(300))
AS
BEGIN
	IF(NOT EXISTS(SELECT * FROM KHACHHANG K WHERE K.MAKH = @MAKH)) 
		RETURN -1
	IF(EXISTS(SELECT * FROM KHACHHANG K WHERE K.MAKH = @MAKH))
	UPDATE KHACHHANG SET TENKH = @TENKH, SDT = @SDT, DIACHI = @DIACHI 
	WHERE MAKH = @MAKH
END
EXEC SP_SUAKH'KH006', N'Đỗ Hữu Quốc Anh', '0905555553', N'Hà Nội'
EXEC SP_SUAKH'KH007', N'Đỗ Tiến Đạt', '0905555554', N'Thái Nguyên'

-------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetNhaCungCap
AS
BEGIN
    SELECT MANCC, TENNCC, DIACHI, SDT, EMAIL
    FROM NHACUNGCAP
    WHERE 
        MANCC IS NOT NULL AND MANCC <> ''
END
EXEC sp_GetNhaCungCap
-------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetByIDNhaCungCap(@MANCC CHAR(15))
AS
BEGIN
    SELECT MaNCC, TENNCC, DiaChi, SDT, EMAIL
    FROM NHACUNGCAP
    WHERE 
        MANCC = @MANCC
END
EXEC sp_GetByIDNhaCungCap'NCC001'
-------------------------------------------------------------------------------------------------
--XÓA THÔNG TIN CỦA NCC CÓ MÃ BẤT KỲ
CREATE OR ALTER PROC SP_XOANCC(@MANCC CHAR(15))
AS
BEGIN
	IF(NOT EXISTS(SELECT * FROM NHACUNGCAP WHERE MANCC = @MANCC)) 
		RETURN -1
	IF(EXISTS(SELECT * FROM NHACUNGCAP WHERE MANCC = @MANCC))
	DELETE FROM NHACUNGCAP
	WHERE MANCC = @MANCC
END
EXEC SP_XOANCC'SV100'--KHÔNG XÓA ĐƯỢC
EXEC SP_XOANCC'KH005'--XÓA ĐƯỢC
-------------------------------------------------------------------------------------------------
--THÊM THÔNG TIN CỦA NCC
CREATE OR ALTER PROC SP_THEMNCC
	@MANCC CHAR(15),
	@TENNCC NVARCHAR(300),
	@DIACHI NVARCHAR(500),
	@SDT VARCHAR(12),
	@EMAIL NVARCHAR(320)
AS
BEGIN
	IF(EXISTS(SELECT * FROM NHACUNGCAP WHERE MANCC = @MANCC)) 
		RETURN -1
	IF(NOT EXISTS(SELECT * FROM NHACUNGCAP WHERE MANCC = @MANCC))
		INSERT INTO NHACUNGCAP VALUES (@MANCC, @TENNCC, @DIACHI, @SDT, @EMAIL)
END
EXEC SP_THEMNCC'NCC005', N'Việt Tiến', N'Hà Nội', '0905678901', 'viettien@gmail.com'
EXEC SP_THEMNCC'NCC006', N'Việt Tiến 2', N'Hà Nội', '0905678901', 'viettien@gmail.com'
-------------------------------------------------------------------------------------------------
--SỬA THÔNG TIN CỦA NCC CÓ MÃ BẤT KỲ
CREATE OR ALTER PROC SP_SUANCC(@MANCC CHAR(15),
	@TENNCC NVARCHAR(300),
	@DIACHI NVARCHAR(500),
	@SDT VARCHAR(12),
	@EMAIL NVARCHAR(320))
AS
BEGIN
	IF(NOT EXISTS(SELECT * FROM NHACUNGCAP WHERE MANCC = @MANCC)) 
		RETURN -1
	IF(EXISTS(SELECT * FROM NHACUNGCAP WHERE MANCC = @MANCC))
	UPDATE NHACUNGCAP SET TENNCC = @TENNCC, SDT = @SDT, DIACHI = @DIACHI, EMAIL = @EMAIL 
	WHERE MANCC = @MANCC
END
EXEC SP_SUANCC'NCC006', N'Việt Tiến 2', N'Hà Nội', '0905678901', 'viettien@gmail.com'
EXEC SP_SUANCC'NCC007', N'Việt Tiến 2', N'Hà Nội', '0905678901', 'viettien@gmail.com'



-------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetKhuyenMai
AS
BEGIN
    SELECT * FROM KHUYENMAI
    WHERE 
        MAKM IS NOT NULL AND MAKM <> ''
END
EXEC sp_GetKhuyenMai
-------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetByIDKM(@MAkm CHAR(15))
AS
BEGIN
    SELECT * FROM KHUYENMAI WHERE 
        MAkm = @MAkm
END
EXEC sp_GetByIDKM'km001'
-------------------------------------------------------------------------------------------------
--XÓA THÔNG TIN CỦA KM CÓ MÃ BẤT KỲ
CREATE OR ALTER PROC SP_XOAKM(@MAKM CHAR(15))
AS
BEGIN
	IF(NOT EXISTS(SELECT * FROM KHUYENMAI WHERE MAKM= @MAKM)) 
		RETURN -1
	IF(EXISTS(SELECT * FROM KHUYENMAI WHERE MAKM= @MAKM))
	DELETE FROM KHUYENMAI
	WHERE MAKM = @MAKM
END
EXEC SP_XOAKM'KM100'
-------------------------------------------------------------------------------------------------
--THÊM THÔNG TIN CỦA KM
CREATE OR ALTER PROC SP_THEMKM
	@MAKM CHAR(15),
    @TENKM NVARCHAR(300),
    @MASP CHAR(15),
    @NGAYBATDAU DATE,
    @NGAYKETTHUC DATE
AS
BEGIN
	IF(EXISTS(SELECT * FROM KHUYENMAI WHERE MAKM= @MAKM)) 
		RETURN -1
	IF(NOT EXISTS(SELECT * FROM KHUYENMAI WHERE MAKM= @MAKM))
		INSERT INTO KHUYENMAI VALUES (@MAKM, @TENKM, @MASP, @NGAYBATDAU, @NGAYKETTHUC)
END
EXEC SP_THEMKM'KM005', N'Giảm 10% nồi cơm điện', 'SP005', '2025-05-01', '2025-05-15'
-------------------------------------------------------------------------------------------------
--SỬA THÔNG TIN CỦA KM CÓ MÃ BẤT KỲ
CREATE OR ALTER PROC SP_SUAKM(
	@MAKM CHAR(15),
    @TENKM NVARCHAR(300),
    @MASP CHAR(15),
    @NGAYBATDAU DATE,
    @NGAYKETTHUC DATE)
AS
BEGIN
	IF(NOT EXISTS(SELECT * FROM KHUYENMAI WHERE MAKM = @MAKM)) 
		RETURN -1
	IF(EXISTS(SELECT * FROM KHUYENMAI WHERE MAKM= @MAKM))
	UPDATE KHUYENMAI SET TENKM = @TENKM, MASP = @MASP, NGAYBATDAU = @NGAYBATDAU, NGAYKETTHUC = @NGAYKETTHUC 
	WHERE MAKM = @MAKM
END
EXEC SP_SUAKM'KM005', N'Giảm 10% nồi cơm điện', 'SP005', '2025-05-01', '2025-05-15'
EXEC SP_SUAKM'KM006', N'Giảm 10% nồi cơm điện', 'SP005', '2025-05-01', '2025-05-15'



-------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetThanhToan
AS
BEGIN
    SELECT * FROM THANHTOAN
    WHERE 
        MATHANHTOAN IS NOT NULL AND MATHANHTOAN <> ''
END
EXEC sp_GetThanhToan
-------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetByIDTT(@MATHANHTOAN CHAR(15))
AS
BEGIN
    SELECT * FROM THANHTOAN WHERE 
        MATHANHTOAN = @MATHANHTOAN
END
EXEC sp_GetByIDTT'TT007'
-------------------------------------------------------------------------------------------------
--XÓA THÔNG TIN CỦA TT CÓ MÃ BẤT KỲ
CREATE OR ALTER PROC SP_XOATT(@MATHANHTOAN CHAR(15))
AS
BEGIN
	IF(NOT EXISTS(SELECT * FROM THANHTOAN WHERE MATHANHTOAN= @MATHANHTOAN)) 
		RETURN -1
	IF(EXISTS(SELECT * FROM THANHTOAN WHERE MATHANHTOAN= @MATHANHTOAN))
	DELETE FROM THANHTOAN
	WHERE MATHANHTOAN = @MATHANHTOAN
END
EXEC SP_XOATT'TT100'
-------------------------------------------------------------------------------------------------
--THÊM THÔNG TIN CỦA TT
CREATE OR ALTER PROC SP_THEMTT
	@MATHANHTOAN CHAR(15),
	@MAHDBAN CHAR(15),
	@PHUONGTHUC NVARCHAR(50),
	@SOTIENTHANHTOAN FLOAT,
	@NGAYTHANHTOAN DATETIME,
	@TRANGTHAI NVARCHAR(50)
AS
BEGIN
	IF(EXISTS(SELECT * FROM THANHTOAN WHERE MATHANHTOAN= @MATHANHTOAN)) 
		RETURN -1
	IF(NOT EXISTS(SELECT * FROM THANHTOAN WHERE MATHANHTOAN= @MATHANHTOAN))
		INSERT INTO THANHTOAN VALUES (@MATHANHTOAN, @MAHDBAN, @PHUONGTHUC, @SOTIENTHANHTOAN, @NGAYTHANHTOAN, @TRANGTHAI)
END
EXEC SP_THEMTT'TT005', 'HD005', N'Chuyển khoản', 1400000, '2025-01-14', N'Chưa thanh toán'
-------------------------------------------------------------------------------------------------
--SỬA PHƯƠNG THỨC TT, TRẠNG THÁI CỦA TT CÓ MÃ BẤT KỲ
CREATE OR ALTER PROC SP_SUATT(
	@MATHANHTOAN CHAR(15),
	@PHUONGTHUC NVARCHAR(50),
	@TRANGTHAI NVARCHAR(50))
AS
BEGIN
	IF(NOT EXISTS(SELECT * FROM THANHTOAN WHERE MATHANHTOAN= @MATHANHTOAN)) 
		RETURN -1
	IF(EXISTS(SELECT * FROM THANHTOAN WHERE MATHANHTOAN= @MATHANHTOAN))
	UPDATE THANHTOAN SET PHUONGTHUC = @PHUONGTHUC, TRANGTHAI = @TRANGTHAI
	WHERE MATHANHTOAN = @MATHANHTOAN
END
EXEC SP_SUATT'TT007', N'Chuyển khoản', N'Đã thanh toán'