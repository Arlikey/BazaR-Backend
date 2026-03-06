namespace BazaR.Backend.Domain.Catalog.Products;


public enum ProductStatus
{
    Draft = 0,      // создан, но не показываем
    Published = 1,  // можно показывать
    Hidden = 2,     // забанен админом
    Archived = 3    // снят навсегда
}


