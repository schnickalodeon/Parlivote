﻿namespace Parlivote.Shared.Models.Identity;

public class UserLogin
{
    public UserLogin()
    {
        
    }

    public UserLogin(string email, string password)
    {
        Email = email;
        Password = password;
    }
    public string Email { get; set; }
    public string Password { get; set; }
}