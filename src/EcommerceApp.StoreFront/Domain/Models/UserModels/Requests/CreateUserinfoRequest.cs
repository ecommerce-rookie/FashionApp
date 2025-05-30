﻿namespace StoreFront.Domain.Models.UserModels.Requests
{
    public class CreateUserinfoRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public IFormFile? Avatar { get; set; }
    }
}
