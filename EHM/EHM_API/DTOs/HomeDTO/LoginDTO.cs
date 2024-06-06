using EHM_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace EHM_API.DTOs.HomeDTO
{
    public class LoginDTO
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
