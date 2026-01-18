using BazaR.Backend.Domain.Catalog;

namespace BazaR.Backend.Domain.Repositories;

public interface IProductRepository
{
    Task<Product?> GetById(ProductId id);

    Task Add(Product product);
    Task Update(Product product);
}
