namespace EHM_API.Repositories
{
    public static class PasswordGenerator
    {
        private static readonly Random random = new Random();
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public static string GeneratePassword()
        {
            // Chọn độ dài ngẫu nhiên từ 6 đến 8
            int length = random.Next(6, 9);

            // Random các ký tự
            var passwordChars = Enumerable.Repeat(chars, length - 2)
                                          .Select(s => s[random.Next(s.Length)]).ToList();

            // Thêm ký tự đặc biệt và số vào mật khẩu

            passwordChars.Add(chars[random.Next(10)]); // Number

            // Trộn ngẫu nhiên các ký tự
            return new string(passwordChars.OrderBy(_ => random.Next()).ToArray());
        }
    }
}
