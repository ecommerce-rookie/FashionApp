namespace StoreFront.Domain.Constants
{
    public class PageConstants
    {
        #region PrePath Page
        
        public const string PrePathComponent = "../Components";
        public const string PrePathMain = "/Main";

        #endregion

        #region Path Common Component

        public const string Header = PrePathComponent + "/Common/Header";
        public const string Footer = PrePathComponent + "/Common/Footer";
        public const string Navigation = PrePathComponent + "/Common/Navigation";
        public const string Breadcrumb = PrePathComponent + "/Common/Breadcrumb";
        public const string Pagination = PrePathComponent + "/Common/Pagination";

        #endregion

        #region Path Page

        public const string AuthenPage = PrePathMain + "/AuthenPage/Index";
        public const string CartPage = PrePathMain + "/CartPage/Index";
        public const string CheckoutPage = PrePathMain + "/CheckoutPage/Index";
        public const string HomePage = PrePathMain + "/HomePage/Index";
        public const string OrderPage = PrePathMain + "/OrderPage/Index";
        public const string ProductPage = PrePathMain + "/ProductPage/Index";
        public const string ShopPage = PrePathMain + "/ShopPage/Index";
        public const string LogoutPage = PrePathMain + "/LogoutPage/Index";

        #endregion

        #region Checkout component

        public const string CheckoutPaymentmethod = "../CheckoutPage/Components/CheckoutPaymentMethod";
        public const string CheckoutDetail = "../CheckoutPage/Components/CheckoutDetail";
        public const string CheckoutComplete = "../CheckoutPage/Components/CheckoutComplete";
        public const string CheckoutOverview = "../CheckoutPage/Components/CheckoutOverview";

        #endregion

        #region Shop component

        public const string ShopFilter = "../ShopPage/Components/ShopFilter";
        public const string ShopProductList = "../ShopPage/Components/ShopProductList";

        #endregion

        #region Product component

        public const string RecommendProduct = nameof(RecommendProduct);
        public const string BestSellerProduct = nameof(BestSellerProduct);
        public const string FeaturedProduct = nameof(FeaturedProduct);

        #endregion

        #region Dislay Product

        public const string ProductPreview = PrePathComponent + "/DisplayProduct/ProductPreview";

        #endregion

       

    }
}
