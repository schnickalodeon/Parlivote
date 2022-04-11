namespace Parlivote.Shared.Models.Identity;

public class RefreshTokenRequest
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}