using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sellers;
using MediatR;

namespace BazaR.Backend.Application.Sellers.Commands.CreateSeller;

public sealed class CreateSellerCommandHandler : IRequestHandler<CreateSellerCommand, Result<Guid>>
{
    private readonly ISellerRepository _sellers;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _current;

    public CreateSellerCommandHandler(ISellerRepository sellers, IUnitOfWork uow, ICurrentUser current)
    {
        _sellers = sellers;
        _uow = uow;
        _current = current;
    }

    public async Task<Result<Guid>> Handle(CreateSellerCommand request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result<Guid>.Failure(new Error("Auth.Required", "Authentication required."));

        // MVP правило: 1 user -> 1 seller
        var existing = await _sellers.GetByOwnerUserIdAsync(_current.UserId, ct);
        if (existing is not null)
            return Result<Guid>.Failure(new Error("Seller.AlreadyExists", "Seller already exists for this user."));

        // 1) Валидируем/нормализуем slug через VO
        var slugRes = SellerSlug.Create(request.Slug);
        if (slugRes.IsFailure)
            return Result<Guid>.Failure(slugRes.Error);

        // 2) Проверяем уникальность slug
        var slugTaken = await _sellers.SlugExistsAsync(slugRes.Value!, excludeSellerId: null, ct: ct);
        if (slugTaken)
            return Result<Guid>.Failure(new Error("Seller.SlugTaken", "Slug is already taken."));

        // 3) Создаём seller (Draft для Regular)
        var sellerRes = Seller.Create(
            name: request.Name,
            slug: request.Slug,
            type: request.Type,
            ownerUserId: _current.UserId,
            countryCode: request.CountryCode
        );

        if (sellerRes.IsFailure)
            return Result<Guid>.Failure(sellerRes.Error);

        var seller = sellerRes.Value!;

       

        var profileRes = seller.UpdateProfile(request.Description, request.LogoUrl);
        if (profileRes.IsFailure)
            return Result<Guid>.Failure(profileRes.Error);

        var legalRes = seller.UpdateLegal(request.LegalName, request.TaxNumber, request.CountryCode);
        if (legalRes.IsFailure)
            return Result<Guid>.Failure(legalRes.Error);

        var contactsRes = seller.UpdateSupportContacts(request.SupportEmail, request.SupportPhone);
        if (contactsRes.IsFailure)
            return Result<Guid>.Failure(contactsRes.Error);

       
        if (request.SubmitForApproval)
        {
            var submitRes = seller.SubmitForApproval();
            if (submitRes.IsFailure)
                return Result<Guid>.Failure(submitRes.Error);
        }

        _sellers.Add(seller);

        try
        {
            await _uow.SaveChangesAsync(ct);
        }
        catch (Exception)
        {
        
            return Result<Guid>.Failure(new Error("Seller.SlugTaken", "Slug is already taken."));
        }

        return Result<Guid>.Success(seller.Id.Value);
    }
}

