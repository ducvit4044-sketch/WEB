using Microsoft.AspNetCore.Mvc;
using QLNhanVien2;
using System.Data.SqlClient;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    // 1. API Đăng nhập
    [HttpPost("login")]
    [HttpPost("dang-nhap")]
    public IActionResult Login([FromBody] LoginModel model)
    {
        try
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT tk.TenDangNhap, tk.MaNV, tk.MaQuyen, q.TenQuyen 
                               FROM TaiKhoan tk
                               LEFT JOIN Quyen q ON tk.MaQuyen = q.MaQuyen
                               WHERE tk.TenDangNhap = @TenDangNhap AND tk.MatKhau = @MatKhau AND tk.TrangThai = 1";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@TenDangNhap", model.TenDangNhap.Trim());
                    cmd.Parameters.AddWithValue("@MatKhau", model.MatKhau.Trim());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Ok(new
                            {
                                success = true,
                                TenDangNhap = reader["TenDangNhap"].ToString(),
                                MaNV = reader["MaNV"] != DBNull.Value ? reader["MaNV"].ToString() : null,
                                MaQuyen = reader["MaQuyen"].ToString().Trim(),
                                TenQuyen = reader["TenQuyen"] != DBNull.Value ? reader["TenQuyen"].ToString() : "Nhân viên",
                                message = "Đăng nhập thành công!"
                            });
                        }
                        return BadRequest(new { success = false, message = "Sai tài khoản hoặc mật khẩu!" });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Lỗi Server: " + ex.Message });
        }
    }

    // 2. API Đổi mật khẩu
    [HttpPost("doi-mat-khau")]
    public IActionResult DoiMatKhau([FromBody] DoiMatKhauModel model)
    {
        try
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string checkSql = "SELECT COUNT(*) FROM TaiKhoan WHERE TenDangNhap = @User AND MatKhau = @Pass";
                using (SqlCommand cmdCheck = new SqlCommand(checkSql, conn))
                {
                    cmdCheck.Parameters.AddWithValue("@User", model.TenDangNhap.Trim());
                    cmdCheck.Parameters.AddWithValue("@Pass", model.MatKhauCu.Trim());
                    if ((int)cmdCheck.ExecuteScalar() == 0)
                    {
                        return BadRequest(new { success = false, message = "Mật khẩu cũ không chính xác!" });
                    }
                }

                string updateSql = "UPDATE TaiKhoan SET MatKhau = @NewPass WHERE TenDangNhap = @User";
                using (SqlCommand cmdUpdate = new SqlCommand(updateSql, conn))
                {
                    cmdUpdate.Parameters.AddWithValue("@NewPass", model.MatKhauMoi.Trim());
                    cmdUpdate.Parameters.AddWithValue("@User", model.TenDangNhap.Trim());
                    cmdUpdate.ExecuteNonQuery();
                }
            }
            return Ok(new { success = true, message = "Đổi mật khẩu thành công!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Lỗi Server: " + ex.Message });
        }
    }
}

// Các Model dữ liệu đi kèm
public class LoginModel
{
    public string TenDangNhap { get; set; }
    public string MatKhau { get; set; }
}

public class DoiMatKhauModel
{
    public string TenDangNhap { get; set; }
    public string MatKhauCu { get; set; }
    public string MatKhauMoi { get; set; }
}