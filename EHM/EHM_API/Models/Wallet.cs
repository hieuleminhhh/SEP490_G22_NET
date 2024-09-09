using System;
using System.Collections.Generic;

namespace EHM_API.Models
{
    public partial class Wallet
    {
        public int WalletId { get; set; }
        public string GuestPhone { get; set; } = null!;
        public decimal Balance { get; set; }
        public DateTime LastUpdated { get; set; }

        public virtual Guest GuestPhoneNavigation { get; set; } = null!;
    }
}
