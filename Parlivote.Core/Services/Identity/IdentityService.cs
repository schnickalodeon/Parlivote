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

public partial class IdentityService : IIdentityService
{
    private readonly UserManager<User> userManager;
    private readonly JwtSettings jwtSettings;

    public IdentityService(UserManager<User> userManager, JwtSettings jwtSettings)
    {
        this.userManager = userManager;
        this.jwtSettings = jwtSettings;
    }

    public async Task<AuthSuccessResponse> RegisterAsync(string email, string password)
    {
        ValidateEmailAddress(email);

        await ValidateUserDoesNotExist(email);

        var newUser = new User
        {
            Email = email,
            UserName = email
        };

        IdentityResult createdUserResult =
            await this.userManager.CreateAsync(newUser, password);

        ValidateCreatedUser(createdUserResult);

        AuthenticationResult authenticationResult =
            GenerateAuthenticationResultForUser(newUser);

        return new AuthSuccessResponse
        {
            Token = authenticationResult.Token
        };
    }

    public async Task<AuthSuccessResponse> LoginAsync(string email, string password)
    {
        ValidateEmailAddress(email);

        User user =
            await this.userManager.FindByEmailAsync(email);

        ValidateStorageUser(user);

        await ValidatePassword(user, password);

        AuthenticationResult result = GenerateAuthenticationResultForUser(user);

        return new AuthSuccessResponse
        {
            Token = result.Token
        };
    }

    private AuthenticationResult GenerateAuthenticationResultForUser(User newUser)
    {
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