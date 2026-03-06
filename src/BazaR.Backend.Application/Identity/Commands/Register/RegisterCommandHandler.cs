using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions.Security;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Identity;
using BazaR.Backend.Domain.Users;
using MediatR;

namespace BazaR.Backend.Application.Identity.Commands.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? Phone = null
) : IRequest<Result<Guid>>;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<Guid>>
{
    private readonly IUserRepository _users;
    private readonly IAuthUserRepository _authUsers;
    private readonly IPasswordHasher _hasher;
    private readonly IUnitOfWork _uow;

    public RegisterCommandHandler(
        IUserRepository users,
        IAuthUserRepository authUsers,
        IPasswordHasher hasher,
        IUnitOfWork uow)
    {
        _users = users;
        _authUsers = authUsers;
        _hasher = hasher;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(RegisterCommand request, CancellationToken ct)
    {
        // 1. Валидация и нормализация email через доменный value object
        var emailRes = IdentityEmail.Create(request.Email);
        if (emailRes.IsFailure)
            return Result<Guid>.Failure(emailRes.Error);

        var normalizedEmail = emailRes.Value!.Value;

        // 2. Проверка уникальности email в таблице аутентификации
        var existingAuth = await _authUsers.GetByEmailAsync(normalizedEmail, ct);
        if (existingAuth is not null)
            return Result<Guid>.Failure(new Error("Identity.EmailTaken", "Email is already taken."));

        // 3. Дополнительная проверка уникальности email в таблице пользователей (бизнес-данные)
        var existingUser = await _users.GetByEmailAsync(normalizedEmail, ct);
        if (existingUser is not null)
            return Result<Guid>.Failure(new Error("User.EmailTaken", "Email is already taken."));

        // 4. Проверка уникальности телефона, если он указан
        if (!string.IsNullOrWhiteSpace(request.Phone))
        {
            var existingByPhone = await _users.GetByPhoneAsync(request.Phone!, ct);
            if (existingByPhone is not null)
                return Result<Guid>.Failure(new Error("User.PhoneTaken", "Phone is already taken."));
        }

        // 5. Создание бизнес-пользователя (доменная сущность User)
        var userId = Guid.NewGuid();
        var userRes = User.Create(
            identityUserId: userId,
            email: request.Email,
            firstName: request.FirstName,
            lastName: request.LastName,
            phone: request.Phone
        );
        if (userRes.IsFailure)
            return Result<Guid>.Failure(userRes.Error);
        var user = userRes.Value!;

        // 6. Создание учетной записи аутентификации (AuthUser) с хешированием пароля
        var hash = _hasher.Hash(request.Password);
        var authRes = AuthUser.Register(
            userId: userId,
            email: request.Email,
            passwordHash: hash
        );
        if (authRes.IsFailure)
            return Result<Guid>.Failure(authRes.Error);

        // 7. Сохранение обеих сущностей в БД
        _users.Add(user);
        _authUsers.Add(authRes.Value!);
        await _uow.SaveChangesAsync(ct);

        // 8. Возврат идентификатора созданного пользователя
        return Result<Guid>.Success(user.Id.Value);
    }
}