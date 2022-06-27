namespace Parlivote.Shared.Models.Identity;

public class UserRegistration
{
    public string FirstName { get; set; }
    public string LastName { get; set; } = String.Empty;
    public string Email { get; set; }
    public string EmailConfirmation { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}