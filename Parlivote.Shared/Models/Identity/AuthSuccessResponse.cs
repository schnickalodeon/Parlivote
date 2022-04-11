namespace Parlivote.Shared.Models.Identity;

public class AuthSuccessResponse : AuthenticationResult
{
    public AuthSuccessResponse(string token, string refreshToken)
    {
        this.Success = true;
        this.Token = token;
    }
}