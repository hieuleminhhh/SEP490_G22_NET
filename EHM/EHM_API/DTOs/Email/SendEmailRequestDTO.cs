namespace EHM_API.DTOs.Email
{
    public class SendEmailRequestDTO
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
