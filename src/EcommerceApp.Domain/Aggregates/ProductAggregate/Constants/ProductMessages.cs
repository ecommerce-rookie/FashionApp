namespace Domain.Aggregates.ProductAggregate.Constants
{
    public class ProductMessages
    {
        #region Url Message

        public const string ImageUrlCannotEmpty = "Image URL cannot be empty.";
        public const string ImageUrlInvalid = "Image URL is not valid.";

        #endregion

        #region Money Message

        public const string AmountCannotNegative = "Amount cannot be negative.";
        public const string CurrencyMustBeProvided = "Currency must be provided.";
        
        public const string UnitPriceCannotNegative = "UnitPrice must be greater than or equal to 0";
        public const string PurchasePriceCannotNegative = "PurchasePrice must be greater than or equal to 0";
        public const string PurchasePriceCannotGreaterThanUnitPrice = "PurchasePrice must be less than or equal to UnitPrice";

        #endregion

    }
}
