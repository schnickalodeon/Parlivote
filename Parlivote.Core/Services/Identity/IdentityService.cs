using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Core.Configurations;
using Parlivote.Shared.Models.Identity;

namespace Parlivote.Core.Services.Identity;

public partial class IdentityService : IIdentityService
{
    private readonly UserManager<User> userManager;
    private readonly JwtSettings jwtSettings;
    private readonly TokenValidationParameters tokenValidationParameters;
    private readonly IStorageBroker storageBroker;
    

    public IdentityService(
        UserManager<User> userManager, 
        JwtSettings jwtSettings, 
        TokenValidationParameters tokenValidationParameters, 
        IStorageBroker storageBroker)
    {
        this.userManager = userManager;
        this.jwtSettings = jwtSettings;
        this.tokenValidationParameters = tokenValidationParameters;
        this.storageBroker = storageBroker;
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

        AuthenticationResult authResult =
            await GenerateAuthenticationResultForUserAsync(newUser);

        return new AuthSuccessResponse(
            authResult.Token, 
            authResult.Token_Expiration, 
            authResult.RefreshToken);
    }

    public async Task<AuthSuccessResponse> LoginAsync(string email, string password)
    {
        ValidateEmailAddress(email);

        User user =
            await this.userManager.FindByEmailAsync(email);

        ValidateStorageUser(user);

        await ValidatePassword(user, password);

        AuthenticationResult result = await GenerateAuthenticationResultForUserAsync(user);

        return new AuthSuccessResponse(
            result.Token,
            result.Token_Expiration,
            result.RefreshToken);
    }

    public async Task<AuthSuccessResponse> RefreshTokenAsync(string token, string refreshToken)
    {
        var validatedToken = GetPrincipalFromToken(token);

        if (validatedToken == null)
        {
            throw new Exception("Invalid Token");
        }

        long expiryDateUnix = 
            long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

        DateTime expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            .AddSeconds(expiryDateUnix);

        if (expiryDateTimeUtc > DateTime.UtcNow)
        {
            throw new Exception("Token has not expired yet");
        }

        string jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

        RefreshToken storedRefreshToken = 
            await this.storageBroker.SelectRefreshTokenByToken(refreshToken);

        if (storedRefreshToken is null)
        {
            throw new Exception("This refreshToken does not exists");
        }

        if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
        {
            throw new Exception("This refreshToken has expired");
        }

        if (storedRefreshToken.Invalidated)
        {
            throw new Exception("This refreshToken is invalid");
        }

        if (storedRefreshToken.Used)
        {
            throw new Exception("This refreshToken is already used");
        }

        if (storedRefreshToken.JwtId != jti)
        {
            throw new Exception("This refreshToken is invalid id");
        }

        storedRefreshToken.Used = true;
        await this.storageBroker.UpdateRefreshTokenAsync(storedRefreshToken);

        string userId = validatedToken.Claims.Single(x => x.Type == "id").Value;
        User user = await this.userManager.FindByIdAsync(userId);

        AuthenticationResult authenticationResult =
            await GenerateAuthenticationResultForUserAsync(user);

        return new AuthSuccessResponse(
            authenticationResult.Token,
            authenticationResult.Token_Expiration,
            authenticationResult.RefreshToken);
    }

    private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
    {
        return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
               jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
    }

    private ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            ClaimsPrincipal principal = 
                tokenHandler.ValidateToken(token, this.tokenValidationParameters, out SecurityToken validatedToken);
            
            return IsJwtWithValidSecurityAlgorithm(validatedToken) 
                ? principal
                : null;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        byte[] key = Encoding.ASCII.GetBytes(this.jwtSettings.Secret);

        DateTime expiration = DateTime.UtcNow.Add(this.jwtSettings.TokenLifeTime);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("id", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
            Expires = expiration,
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        var refreshToken = new RefreshToken
        {
            JwtId = token.Id,
            UserId = user.Id,
            CreationDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddMonths(6)
        };

        await this.storageBroker.InsertRefreshTokenAsync(refreshToken);

        return new AuthenticationResult
        {
            Success = true,
            Token = tokenHandler.WriteToken(token),
            Token_Expiration = expiration,
            RefreshToken = refreshToken.Token
        };
    }

   
}