using System;
using Xeptions;

namespace Parlivote.Shared.Models.Identity.Exceptions;

public class EmailNotConfirmedException : Xeption
{
    public EmailNotConfirmedException()
        : base(message: "The email and confirm email field are not identical")
    {
    }

    
}