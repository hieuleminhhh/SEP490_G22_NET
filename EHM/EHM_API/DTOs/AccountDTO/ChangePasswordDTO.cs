﻿namespace EHM_API.DTOs.AccountDTO
{
	public class ChangePasswordDTO
	{
		public string? CurrentPassword { get; set; }
		public string? NewPassword { get; set; }
		public string? ConfirmPassword { get; set; }

	}
}
