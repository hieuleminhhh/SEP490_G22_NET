﻿using EHM_API.DTOs.GoogleDTO;
using EHM_API.Models;

namespace EHM_API.Services
{
    public interface IGoogleService
    {
        Account GetByEmail(string email);
        string GenerateJwtToken(Account account);
        Task<Account> RegisterGoogleAccountAsync(string email);
        bool UpdatePassword(UpdatePasswordDTO dto);
    }
}
