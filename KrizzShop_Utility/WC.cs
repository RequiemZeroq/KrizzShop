using System.Collections.ObjectModel;

namespace KrizzShop_Utility
{
    public static class WC
    {
        public const string ImagePath = @"\images\product\";
        public const string SessionCart = "ShoppingCardSession";
        public const string SessionInquiryId = "InquirySession";

        public const string AdminRole = "Admin";
        public const string CustomerRole = "Customer";

        public const string EmailAdmin = "oxtonreina@gmail.com";

        public const string Success = "Success";
        public const string Error = "Error";

        #region OrderStatus
        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "Processing";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";

        public static readonly IEnumerable<string> listStatus = new ReadOnlyCollection<string>(
            new List<string>()
            {
                StatusPending, StatusApproved, StatusInProcess, StatusShipped, StatusCancelled, StatusRefunded
            });

        #endregion OrderStatus

        #region DB
        public const string CategoryName = "Category";
        public const string ApplicationTypeName = "ApplicationType";
        public const string ProductName = "Product";
        #endregion DB
    }
}
