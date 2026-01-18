using BazaR.Backend.Domain.Users;
namespace BazaR.Backend.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetById(UserId id);
    Task<User?> GetByEmail(Email email);

    Task Add(User user);
}
