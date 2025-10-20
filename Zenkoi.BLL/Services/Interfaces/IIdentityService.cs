using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Zenkoi.DAL.Entities;

namespace Zenkoi.BLL.Services.Interfaces
{
	public interface IIdentityService
	{
		Task<ApplicationUser> GetByEmailAsync(string email);
		Task<ApplicationUser> GetByUserNameAsync(string userName);
		Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
		Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);
		Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
		Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string token);
		Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
		Task<IList<string>> GetRolesAsync(ApplicationUser user);
		Task<ApplicationUser> GetByEmailOrUserNameAsync(string input);
		Task SignOutAsync();
		Task<SignInResult> PasswordSignInAsync(ApplicationUser user, string password, bool isPersistent, bool lockOutOnFailure);
		Task<string> GenerateTwoFactorTokenAsync(ApplicationUser user, string tokenProvider);
		Task<SignInResult> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberClient);
		Task<bool> VerifyTwoFactorTokenAsync(ApplicationUser user, string tokenProvider, string token);
		Task<bool> IsLockedOutAsync(ApplicationUser user);
		Task<bool> IsEmailConfirmedAsync(ApplicationUser user);
		Task ResetAccessFailedCountAsync(ApplicationUser user);
		Task<ApplicationUser> GetUserAsync(ClaimsPrincipal principal);
		Task<IdentityResult> SetTwoFactorEnabledAsync(ApplicationUser user, bool enable2Fa);
		Task<SignInResult> CheckPasswordSignInAsync(ApplicationUser user, string password, bool lockOutOnFailure);
		Task<ApplicationUser> GetByIdAsync(int id);
		Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
		Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPass);
	}
}
