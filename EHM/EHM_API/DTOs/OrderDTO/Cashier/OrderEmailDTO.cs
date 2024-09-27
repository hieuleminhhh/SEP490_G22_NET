namespace EHM_API.DTOs.OrderDTO.Cashier
{
    public class OrderEmailDTO
    {
        public int OrderId { get; set; }
        public string? Email{ get; set; }
        public string? ConsigneeName { get; set; }
        public string? EateryName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? SettingEmail { get; set; }
        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }
        public string? Qrcode { get; set; }
        public string? Logo { get; set; }
        public string? LinkContact { get; set; }
        public DateTime? ShipTime { get; set; }
    }
}
