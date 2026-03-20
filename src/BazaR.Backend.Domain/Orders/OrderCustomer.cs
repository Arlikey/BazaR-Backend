using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Orders;

public sealed class OrderCustomer : ValueObject
{
    public string FirstName { get; }
    public string LastName { get; }
    public string Email { get; }
    public string? Phone { get; }

    private OrderCustomer(
        string firstName,
        string lastName,
        string email,
        string? phone)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
    }

    public static Result<OrderCustomer> Create(
        string firstName,
        string lastName,
        string email,
        string? phone)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Result<OrderCustomer>.Failure(
                new Error("OrderCustomer.FirstName.Invalid", "First name is required."));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            return Result<OrderCustomer>.Failure(
                new Error("OrderCustomer.LastName.Invalid", "Last name is required."));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return Result<OrderCustomer>.Failure(
                new Error("OrderCustomer.Email.Invalid", "Email is required."));
        }

        return Result<OrderCustomer>.Success(new OrderCustomer(
            firstName.Trim(),
            lastName.Trim(),
            email.Trim(),
            string.IsNullOrWhiteSpace(phone) ? null : phone.Trim()
        ));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
        yield return Email;
        yield return Phone;
    }
}