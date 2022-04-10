using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Parlivote.Core.Configurations;
using Parlivote.Shared.Models.Identity;

namespace Parlivote.Core.Services.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<User> userManager;
    private readonly JwtSettings jwtSettings;

    public IdentityService(UserManager<User> userManager, JwtSettings jwtSettings)
    {
        this.userManager = userManager;
        this.jwtSettings = jwtSettings;
    }

    public async Task<AuthenticationResult> RegisterAsync(string userRegistrationEmail, string userRegistrationPassword)
    {
        User existingUser = 
            await this.userManager.FindByEmailAsync(userRegistrationEmail);

        if (existingUser is not null)
        {
            return new AuthenticationResult
            {
                ErrorMessages = new[] {"User with this email address already exists"}
            };
        }

        var newUser = new User
        {
            Email = userRegistrationEmail,
            UserName = userRegistrationEmail
        };

        IdentityResult createdUserResult =
            await this.userManager.CreateAsync(newUser, userRegistrationPassword);

        if (!createdUserResult.Succeeded)
        {
            return new AuthenticationResult
            {
                ErrorMessages = createdUserResult.Errors.Select(x => x.Description)
            };
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        byte[] key = Encoding.ASCII.GetBytes(this.jwtSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, newUser.Email),
                new Claim(JwtRegisteredClaimNames.Email, newUser.Email),
                new Claim("id", newUser.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(16),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return new AuthenticationResult
        {
            Success = true,
            Token = tokenHandler.WriteToken(token)
        };

    }
}