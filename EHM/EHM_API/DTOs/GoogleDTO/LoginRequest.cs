﻿namespace EHM_API.DTOs.GoogleDTO
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

public class LoginResponse
{
    public string Token { get; set; }
}