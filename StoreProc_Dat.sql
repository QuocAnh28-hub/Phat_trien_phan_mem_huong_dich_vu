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
CREATE OR ALTER PROCEDURE sp_GetByIDKhachHang(@MAKH CHAR(15))
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