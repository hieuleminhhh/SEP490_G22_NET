namespace EHM_API.Enums
{
    namespace EHM_API.Models
    {
        public enum SortField
        {
            Name,
            Price,
            OrderQuantity
        }

        public enum SortOrder
        {
            Ascending,
            Descending
        }
        public enum AccountRole
        {
            User = 1,
            OrderStaff = 2,
            Cashier = 3,
            Chef = 4
        }

    }
}
