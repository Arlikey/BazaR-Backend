using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Users;

public static class UserErrors
{
    public static readonly Error EmailInvalid =
        new("user.email.invalid", "Email is invalid.", ErrorType.Validation);

    public static readonly Error PasswordHashRequired =
        new("user.passwordHash.required", "Password hash is required.", ErrorType.Validation);

    public static readonly Error AlreadyBlocked =
        new("user.status.alreadyBlocked", "User is already blocked.", ErrorType.Conflict);

    public static readonly Error AlreadyActive =
        new("user.status.alreadyActive", "User is already active.", ErrorType.Conflict);

    public static readonly Error CannotRemoveLastRole =
        new("user.roles.cannotRemoveLast", "User must have at least one role.", ErrorType.Validation);

    public static readonly Error RoleAlreadyGranted =
        new("user.roles.alreadyGranted", "Role is already granted.", ErrorType.Conflict);

    public static readonly Error RoleNotAssigned =
        new("user.roles.notAssigned", "Role is not assigned.", ErrorType.NotFound);

    public static Error IdentityIdRequired => new(
       "User.IdentityIdRequired",
       "Identity user ID is required.");

    public static readonly Error FirstNameRequired =
        new("User.FirstNameRequired", "First name is required.");

    public static readonly Error LastNameRequired =
        new("User.LastNameRequired", "Last name is required.");

    public static readonly Error FirstNameTooLong =
        new("User.FirstNameTooLong", "First name is too long.");

    public static readonly Error LastNameTooLong =
        new("User.LastNameTooLong", "Last name is too long.");

    public static readonly Error PhoneInvalid =
        new("User.PhoneInvalid", "Phone number is invalid.");

    public static readonly Error PhoneTooLong =
        new("User.PhoneTooLong", "Phone number is too long.");

    public static readonly Error BlockedCannotBeModified =
        new("User.BlockedCannotBeModified", "Blocked user cannot be modified.");
}