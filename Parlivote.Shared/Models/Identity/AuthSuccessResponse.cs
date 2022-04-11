namespace Parlivote.Shared.Models.Identity;

public class AuthSuccessResponse : AuthenticationResult
{
    public AuthSuccessResponse(string token, DateTime tokenExpiration, string refreshToken)
    {
        this.Success = true;
        this.Token = token;
        this.RefreshToken = refreshToken;
        this.Token_Expiration = tokenExpiration;
    }
}