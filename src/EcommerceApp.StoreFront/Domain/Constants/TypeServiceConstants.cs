using StoreFront.Application.Services;

namespace StoreFront.Domain.Constants
{
    public class TypeServiceConstants
    {
        public static Type[] TypeServices =
            [
                typeof(ICategoryService),
                typeof(IProductService),
                typeof(IUserService),
                typeof(IOrderService),
                typeof(IOrderDetailService),
                typeof(IFeedbackService),
            ];
    }
}
