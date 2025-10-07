using VendingMachine.Core.Domain.Entities;

namespace VendingMachine.Core.Infrastructure.Persistence.Seeders;

public static class ProductSeeders
{
    public static List<Product> GetDefaultProducts() =>
    [
        new Product { Id = 1, Name = "Вода", Price = 50, Amount = 10 },
        new Product { Id = 2, Name = "Сок", Price = 80, Amount = 5 },
        new Product { Id = 3, Name = "Шоколад", Price = 120, Amount = 3 },
        new Product { Id = 4, Name = "Чипсы", Price = 90, Amount = 7 }
    ];
}