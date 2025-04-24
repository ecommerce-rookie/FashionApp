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
        public const string Toast = "_Toast";

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
        public const string OnboardingPage = PrePathMain + "/OnboardingPage/Index";
        public const string ProfilePage = PrePathMain + "/ProfilePage/Index";

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

        #region Onboarding

        public const string Step1 = "../OnboardingPage/Step1";
        public const string Step2 = "../OnboardingPage/Step2";
        public const string Step3 = "../OnboardingPage/Step3";
        public const string Step4 = "../OnboardingPage/Step4";
        public const string StepCompletion = "../OnboardingPage/StepCompletion";

        #endregion

        #region Profile User

        public const string UserInformation = "../ProfilePage/Components/UserInformation";
        public const string WidgetProfile = "../ProfilePage/Components/WidgetProfile";

        public const string UserProfile = nameof(UserProfile);
        public const string UserOrder = nameof(UserOrder);

        #endregion

        #region Feedback component

        public const string ReviewProduct = nameof(ReviewProduct);
        public const string FeedbackProduct = nameof(FeedbackProduct);

        #endregion
    }
}
