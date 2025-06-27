namespace JBook.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }

        // 简化处理，仅内存保存用户数据（可替换为数据库）
        public static List<User> Users = new List<User>();
    }
}
