namespace StoreFront.Domain.Models.UserModels.Responses
{
    public class AuthorResponse
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Avatar { get; set; }
        public string? Status { get; set; }
    }
}
