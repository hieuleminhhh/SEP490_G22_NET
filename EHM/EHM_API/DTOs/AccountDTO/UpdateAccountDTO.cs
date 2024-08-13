using Microsoft.AspNetCore.Mvc;

namespace EHM_API.DTOs.AccountDTO
{
    public class UpdateAccountDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
    }
}
