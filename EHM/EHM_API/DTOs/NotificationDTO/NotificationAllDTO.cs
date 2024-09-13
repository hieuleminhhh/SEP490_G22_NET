namespace EHM_API.DTOs.NotificationDTO
{
    public class NotificationAllDTO
    {
        public int NotificationId { get; set; }
        public string? Description { get; set; }
        public int? AccountId { get; set; }
        public int? OrderId { get; set; }
        public int? Type { get; set; }
        public DateTime? Time { get; set; }
        public bool? IsView { get; set; }
    }
    public class NotificationCreateDTO
    {
        public string? Description { get; set; }
        public int? AccountId { get; set; }
        public int? OrderId { get; set; }
        public int? Type { get; set; }
    }
}
