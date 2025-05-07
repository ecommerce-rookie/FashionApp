namespace StoreFront.Domain.Models.ProductModels.Request
{
    public class ProductIdsRequest
    {
        public IEnumerable<Guid> ProductIds { get; set; } = new List<Guid>();
    }
}
