namespace Parlivote.Shared.Models.Identity;

public class AuthFailedResponse : AuthenticationResult
{
    public AuthFailedResponse()
    {
        
    }

    public AuthFailedResponse(string error)
    {
        Success = false;
        this.RefreshToken = "";
        this.Token = "";
        this.ErrorMessages.Add(error);
    }
    public AuthFailedResponse(List<string> errors)
    {
        Success = false;
        this.RefreshToken = "";
        this.Token = "";
        this.ErrorMessages = errors;
    }
}