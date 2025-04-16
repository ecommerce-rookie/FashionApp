using Duende.IdentityModel;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using EcommerceApp.IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace IdentityService
{
    //public class CustomProfileService : UserClaimsPrincipalFactory<ApplicationUser>
    //{
    //    public CustomProfileService(UserManager<ApplicationUser> userManager, IOptions<IdentityOptions> optionsAccessor)
    //        : base(userManager, optionsAccessor)
    //    {
    //    }

    //    public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
    //    {
    //        var principal = await base.CreateAsync(user);

    //        var identity = (ClaimsIdentity)principal.Identity!;

    //        identity.AddClaim(new Claim(JwtClaimTypes.Confirmation, user.EmailConfirmed.ToString()));

    //        return principal;
    //    }
    //}

    public class CustomProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // Phương thức này sẽ được gọi khi token được tạo ra
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            // Lấy userId từ subject
            var subjectId = context.Subject?.FindFirst(JwtClaimTypes.Subject)?.Value;
            if (subjectId == null)
            {
                return;
            }

            // Lấy thông tin người dùng
            var user = await _userManager.FindByIdAsync(subjectId);
            if (user == null)
            {
                return;
            }

            // Lấy danh sách các claim gốc của người dùng
            var claims = context.Subject.Claims.ToList();

            // Nếu bạn có các claim khác cần thêm thì có thể bổ sung thêm ở đây
            // Thêm claim "cnf" nếu có thể (ví dụ, dựa trên trạng thái email)
            claims.Add(new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed.ToString()));

            // Lọc các claim theo scope đã yêu cầu (context.RequestedClaimTypes)
            context.IssuedClaims = claims
                .Where(claim => context.RequestedClaimTypes.Contains(claim.Type))
                .ToList();

            
        }

        // Kiểm tra xem người dùng còn hoạt động hay không
        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subjectId = context.Subject?.FindFirst(JwtClaimTypes.Subject)?.Value;
            if (subjectId == null)
            {
                context.IsActive = false;
                return;
            }

            var user = await _userManager.FindByIdAsync(subjectId);
            context.IsActive = (user != null);
        }
    }
}
