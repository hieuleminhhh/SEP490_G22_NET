namespace EHM_API.DTOs.Email
{
    public class EmailSettings
    {
        public string SMTPHost { get; set; }
        public int SMTPPort { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string From { get; set; }
    }

}
