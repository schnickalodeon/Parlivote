using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Core.Brokers.UserManagements;
using Parlivote.Core.Configurations;
using Parlivote.Shared.Models.Identity;
using Parlivote.Shared.Models.Identity.Users;

namespace Parlivote.Core.Services.Identity;

public partial class IdentityService : IIdentityService
{
    private readonly UserManager<User> userManager;
    private readonly RoleManager<Role> roleManager;
    private readonly JwtSettings jwtSettings;
    private readonly TokenValidationParameters tokenValidationParameters;
    private readonly IStorageBroker storageBroker;
    private readonly ILoggingBroker loggingBroker;
    private readonly IUserManagementBroker userManagementBroker;
    

    public IdentityService(
        UserManager<User> userManager, 
        JwtSettings jwtSettings, 
        TokenValidationParameters tokenValidationParameters, 
        IStorageBroker storageBroker,
        IUserManagementBroker userManagementBroker, 
        RoleManager<Role> roleManager)
    {
        this.userManager = userManager;
        this.jwtSettings = jwtSettings;
        this.tokenValidationParameters = tokenValidationParameters;
        this.storageBroker = storageBroker;
        this.userManagementBroker = userManagementBroker;
        this.roleManager = roleManager;
    }

    public async Task<AuthSuccessResponse> RegisterAsync(UserRegistration registration)
    {
        string email = registration.Email;
        string password = registration.Password;

        ValidateEmailAddress(email);

        await ValidateUserDoesNotExist(email);

        var newUser = new User
        {
            FirstName = registration.FirstName,
            LastName = registration.LastName,
            Email = email,
            UserName = email
        };

        IdentityResult createdUserResult =
            await this.userManager.CreateAsync(newUser, password);

        await this.userManager.AddToRoleAsync(newUser, registration.Role);

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

        ValidateEmailPasswordCombination(user);

        await ValidatePassword(user, password);

        AuthenticationResult result = await GenerateAuthenticationResultForUserAsync(user);

        if (result.Success)
        {
            user.IsLoggedIn = true;
            await this.storageBroker.UpdateUserAsync(user);
        }

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

    public async Task<bool> LogOutAsync(Guid userId)
    {
        try
        {
            ValidateUserId(userId);

            User userToLogout =
                await this.userManagementBroker.SelectUserByIdAsync(userId);

            ValidateStorageUser(userToLogout);

            userToLogout.IsLoggedIn = false;

            await this.storageBroker.UpdateUserAsync(userToLogout);
            return true;
        }
        catch (Exception exception)
        {
            this.loggingBroker.LogError(exception);
            return false;
        }
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

        IList<string> roles = await this.userManager.GetRolesAsync(user);
        string role = roles.FirstOrDefault(string.Empty);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Role, role),
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