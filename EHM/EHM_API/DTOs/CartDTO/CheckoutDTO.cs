using EHM_API.DTOs.CartDTO;

public class CheckoutDTO
{
    public string GuestPhone { get; set; } = null!;
    public string? Email { get; set; }
    public int? AddressId { get; set; }
    public string? GuestAddress { get; set; }
    public string? ConsigneeName { get; set; }
    public DateTime? OrderDate { get; set; }
    public int? Status { get; set; }
    public DateTime? RecevingOrder { get; set; }
    public decimal? TotalAmount { get; set; }
    public decimal Deposits { get; set; }

    public List<CartOrderDetailsDTO> CartItems { get; set; }

}