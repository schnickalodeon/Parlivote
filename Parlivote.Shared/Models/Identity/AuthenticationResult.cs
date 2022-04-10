namespace Parlivote.Shared.Models.Identity;

public class AuthenticationResult
{
    public string Token { get; set; }
    public bool Success { get; set; }
    public IEnumerable<string> ErrorMessages { get; set; }
}