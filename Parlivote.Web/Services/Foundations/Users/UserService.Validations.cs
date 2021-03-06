using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Parlivote.Shared.Extensions;
using Parlivote.Shared.Models;
using Parlivote.Shared.Models.Identity.Users;
using Parlivote.Shared.Models.Identity.Users.Exceptions;

namespace Parlivote.Web.Services.Foundations.Users;

public partial class UserService
{
    
    private static dynamic IsInvalidUserId(Guid userId) => new
    {
        Condition = (userId == Guid.Empty),
        Message = ExceptionMessages.INVALID_ID
    };

    private static void ValidateUserId(Guid userId) =>
        Validate((IsInvalidUserId(userId), nameof(User.Id)));

    private static void Validate(params (dynamic Rule, string Parameter)[] validations)
    {
        var invalidPostException = new InvalidUserException();

        foreach ((dynamic rule, string parameter) in validations)
        {
            if (rule.Condition)
            {
                invalidPostException.UpsertDataList(
                    key: parameter,
                    value: rule.Message);
            }
        }

        invalidPostException.ThrowIfContainsErrors();
    }
}