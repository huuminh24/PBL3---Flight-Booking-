namespace AirlineBookingApi.Helpers;

public static class PasswordHasherHelper
{
    /// <summary>
    /// Hash mật khẩu bằng BCrypt (tự động sinh salt).
    /// Kết quả bao gồm salt + hash → mỗi lần hash cùng password ra kết quả khác nhau.
    /// </summary>
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// So sánh password nhập vào với hash đã lưu trong database.
    /// BCrypt tự extract salt từ hash để verify.
    /// </summary>
    public static bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
