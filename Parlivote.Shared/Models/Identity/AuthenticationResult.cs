namespace Parlivote.Shared.Models.Identity;

public class AuthenticationResult
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public bool Success { get; set; }
    public List<string> ErrorMessages { get; set; } = new();
}