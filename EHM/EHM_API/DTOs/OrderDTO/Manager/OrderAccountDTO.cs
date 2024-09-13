namespace EHM_API.DTOs.OrderDTO.Manager
{
    public class OrderAccountDTO
    {
        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? Status { get; set; }
        public int? AccountId { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? StaffId { get; set; }
    }
}
