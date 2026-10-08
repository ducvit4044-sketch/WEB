using Microsoft.AspNetCore.Mvc;
using QLNhanVien2;
using System;
using System.Data.SqlClient;

[Route("api/[controller]")]
[ApiController]
public class NhanVienController : ControllerBase
{
    // 1. API Lấy danh sách nhân viên (Đã đầy đủ tất cả các cột)
    [HttpGet("danh-sach")]
    public IActionResult GetDanhSach()
    {
        var list = new System.Collections.Generic.List<object>();
        try
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT nv.MaNV, nv.HoTen, nv.NTNS, nv.Phai, nv.Luong, nv.SDT, nv.Email, nv.DiaChi, 
                                      nv.MaCV, cv.TenCV, nv.MaPB, pb.TenPB, nv.TrangThai 
                               FROM NhanVien nv 
                               LEFT JOIN ChucVu cv ON nv.MaCV = cv.MaCV 
                               LEFT JOIN PhongBan pb ON nv.MaPB = pb.MaPB";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new
                        {
                            MaNV = reader["MaNV"].ToString(),
                            HoTen = reader["HoTen"].ToString(),
                            NgaySinh = reader["NTNS"] != DBNull.Value ? Convert.ToDateTime(reader["NTNS"]).ToString("yyyy-MM-dd") : "",
                            Phai = reader["Phai"] != DBNull.Value ? reader["Phai"].ToString() : "",
                            Luong = reader["Luong"] != DBNull.Value ? reader["Luong"].ToString() : "",
                            SDT = reader["SDT"] != DBNull.Value ? reader["SDT"].ToString() : "",
                            Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "",
                            DiaChi = reader["DiaChi"] != DBNull.Value ? reader["DiaChi"].ToString() : "",
                            MaCV = reader["MaCV"] != DBNull.Value ? reader["MaCV"].ToString() : "",
                            ChucVu = reader["TenCV"] != DBNull.Value ? reader["TenCV"].ToString() : "",
                            MaPB = reader["MaPB"] != DBNull.Value ? reader["MaPB"].ToString() : "",
                            PhongBan = reader["TenPB"] != DBNull.Value ? reader["TenPB"].ToString() : "",
                            TrangThai = reader["TrangThai"] != DBNull.Value && Convert.ToBoolean(reader["TrangThai"])
                        });
                    }
                }
            }
            return Ok(list);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // 2. API Lấy thông tin cá nhân theo tên đăng nhập
    [HttpGet("thong-tin/{username}")]
    public IActionResult GetThongTin(string username)
    {
        try
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT nv.MaNV, nv.HoTen, nv.NTNS, nv.Phai, nv.Luong, nv.SDT, nv.Email, nv.DiaChi, cv.TenCV, pb.TenPB, nv.TrangThai
                               FROM TaiKhoan tk
                               INNER JOIN NhanVien nv ON tk.MaNV = nv.MaNV
                               LEFT JOIN ChucVu cv ON nv.MaCV = cv.MaCV
                               LEFT JOIN PhongBan pb ON nv.MaPB = pb.MaPB
                               WHERE tk.TenDangNhap = @TenDangNhap";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@TenDangNhap", username.Trim());
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var info = new
                            {
                                MaNV = reader["MaNV"].ToString(),
                                HoTen = reader["HoTen"].ToString(),
                                NgaySinh = reader["NTNS"] != DBNull.Value ? Convert.ToDateTime(reader["NTNS"]).ToString("dd/MM/yyyy") : "",
                                Phai = reader["Phai"] != DBNull.Value ? reader["Phai"].ToString() : "",
                                Luong = reader["Luong"] != DBNull.Value ? reader["Luong"].ToString() : "",
                                SDT = reader["SDT"] != DBNull.Value ? reader["SDT"].ToString() : "",
                                Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "",
                                DiaChi = reader["DiaChi"] != DBNull.Value ? reader["DiaChi"].ToString() : "",
                                ChucVu = reader["TenCV"] != DBNull.Value ? reader["TenCV"].ToString() : "",
                                PhongBan = reader["TenPB"] != DBNull.Value ? reader["TenPB"].ToString() : "",
                                TrangThai = reader["TrangThai"] != DBNull.Value && Convert.ToBoolean(reader["TrangThai"])
                            };
                            return Ok(info);
                        }
                        return NotFound(new { message = "Không tìm thấy nhân viên!" });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // 3. API Thêm nhân viên mới (Hỗ trợ đầy đủ các trường)
    [HttpPost("them")]
    public IActionResult ThemMoi([FromBody] NhanVienModel model)
    {
        try
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = "INSERT INTO NhanVien (MaNV, HoTen, Phai, Luong, NTNS, SDT, Email, DiaChi, MaCV, MaPB, TrangThai) VALUES (@MaNV, @HoTen, @Phai, @Luong, @NTNS, @SDT, @Email, @DiaChi, @MaCV, @MaPB, @TrangThai)";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaNV", model.MaNV);
                    cmd.Parameters.AddWithValue("@HoTen", model.HoTen);
                    cmd.Parameters.AddWithValue("@Phai", model.Phai ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Luong", model.Luong.HasValue ? (object)model.Luong.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@NTNS", model.NgaySinh.HasValue ? (object)model.NgaySinh.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@SDT", model.SDT ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", model.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DiaChi", model.DiaChi ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MaCV", model.MaCV ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MaPB", model.MaPB ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TrangThai", model.TrangThai);
                    cmd.ExecuteNonQuery();
                }
            }
            return Ok(new { Success = true, Message = "Thêm nhân viên thành công!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // 4. API Sửa thông tin nhân viên (Hỗ trợ đầy đủ các trường)
    [HttpPut("sua")]
    public IActionResult SuaNhanVien([FromBody] NhanVienModel model)
    {
        try
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = "UPDATE NhanVien SET HoTen = @HoTen, Phai = @Phai, Luong = @Luong, NTNS = @NTNS, SDT = @SDT, Email = @Email, DiaChi = @DiaChi, MaCV = @MaCV, MaPB = @MaPB, TrangThai = @TrangThai WHERE MaNV = @MaNV";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaNV", model.MaNV);
                    cmd.Parameters.AddWithValue("@HoTen", model.HoTen);
                    cmd.Parameters.AddWithValue("@Phai", model.Phai ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Luong", model.Luong.HasValue ? (object)model.Luong.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@NTNS", model.NgaySinh.HasValue ? (object)model.NgaySinh.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@SDT", model.SDT ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", model.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DiaChi", model.DiaChi ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MaCV", model.MaCV ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MaPB", model.MaPB ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TrangThai", model.TrangThai);
                    cmd.ExecuteNonQuery();
                }
            }
            return Ok(new { Success = true, Message = "Cập nhật thành công!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // 5. API Xóa nhân viên
    [HttpDelete("xoa/{maNV}")]
    public IActionResult XoaNhanVien(string maNV)
    {
        try
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = "DELETE FROM NhanVien WHERE MaNV = @MaNV";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaNV", maNV);
                    cmd.ExecuteNonQuery();
                }
            }
            return Ok(new { Success = true, Message = "Xóa thành công!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // 6. API Chấm công
    [HttpPost("cham-cong")]
    public IActionResult ChamCong([FromBody] ChamCongModel model)
    {
        try
        {
            if (model == null || string.IsNullOrEmpty(model.TenDangNhap))
            {
                return BadRequest(new { success = false, message = "Thiếu thông tin tài khoản chấm công!" });
            }

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                // 1. Lấy MaNV từ bảng TaiKhoan dựa vào TenDangNhap
                string getMaNVSql = "SELECT MaNV FROM TaiKhoan WHERE TenDangNhap = @TenDangNhap";
                string maNV = null;
                using (SqlCommand cmdGet = new SqlCommand(getMaNVSql, conn))
                {
                    cmdGet.Parameters.AddWithValue("@TenDangNhap", model.TenDangNhap.Trim());
                    object result = cmdGet.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        maNV = result.ToString();
                    }
                }

                if (string.IsNullOrEmpty(maNV))
                {
                    return BadRequest(new { success = false, message = "Tài khoản này chưa được liên kết với mã nhân viên nào!" });
                }

                // 2. Kiểm tra xem hôm nay nhân viên này đã chấm công chưa (Tùy chọn: mỗi ngày 1 lần)
                string checkSql = @"SELECT COUNT(*) FROM ChamCong 
                                WHERE MaNV = @MaNV AND CAST(ThoiGianChamCong AS DATE) = CAST(GETDATE() AS DATE)";
                using (SqlCommand cmdCheck = new SqlCommand(checkSql, conn))
                {
                    cmdCheck.Parameters.AddWithValue("@MaNV", maNV);
                    int count = (int)cmdCheck.ExecuteScalar();
                    if (count > 0)
                    {
                        return BadRequest(new { success = false, message = "Hôm nay bạn đã chấm công rồi, không thể chấm công lại!" });
                    }
                }

                // 3. Thực hiện thêm bản ghi chấm công vào bảng ChamCong
                string insertSql = "INSERT INTO ChamCong (MaNV, ThoiGianChamCong, TrangThai) VALUES (@MaNV, GETDATE(), N'Đúng giờ')";
                using (SqlCommand cmdInsert = new SqlCommand(insertSql, conn))
                {
                    cmdInsert.Parameters.AddWithValue("@MaNV", maNV);
                    cmdInsert.ExecuteNonQuery();
                }

                return Ok(new { success = true, message = "Chấm công thành công!" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Lỗi Server khi chấm công: " + ex.Message });
        }
    }

    // Model nhận dữ liệu chấm công đi kèm
    public class ChamCongModel
    {
        public string TenDangNhap { get; set; }
    }
}

// Các class mô hình dữ liệu (DTO) phục vụ nhận/gửi JSON
public class NhanVienModel
{
    public string MaNV { get; set; }
    public string HoTen { get; set; }
    public string Phai { get; set; }
    public double? Luong { get; set; }
    public DateTime? NgaySinh { get; set; }
    public string SDT { get; set; }
    public string Email { get; set; }
    public string DiaChi { get; set; }
    public string MaCV { get; set; }
    public string MaPB { get; set; }
    public bool TrangThai { get; set; }
}

public class ChamCongModel
{
    public string TenDangNhap { get; set; }
}