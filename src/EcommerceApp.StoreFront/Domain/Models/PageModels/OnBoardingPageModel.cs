namespace StoreFront.Domain.Models.PageModels
{
    public class OnBoardingPageModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public string? StreetAddress { get; set; }
        public IFormFile? Avatar { get; set; }
    }
}
