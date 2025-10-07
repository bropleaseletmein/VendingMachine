using VendingMachine.Core.Domain.Entities;
using VendingMachine.Core.Domain.Interfaces.Repository;
using VendingMachine.Core.Infrastructure.Persistence.Seeders;

namespace VendingMachine.Core.Infrastructure.Persistence.Repository;

public class ProductRepository : IProductRepository
{
    private readonly Dictionary<int, Product> _products = new();
    
    public ProductRepository()
    {
        var defaults = ProductSeeders.GetDefaultProducts();
        foreach (var product in defaults)
        {
            _products[product.Id] = product;
        }
    }

    public Product? GetById(int id)
    {
        _products.TryGetValue(id, out var product);
        return product;
    }

    public List<Product> GetAll()
    {
        return _products.Values
            .Select(p => new Product
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Amount = p.Amount
            })
            .ToList();
    }

    public int Add(Product product)
    {
        if (_products.ContainsKey(product.Id))
            throw new InvalidOperationException($"Товар с Id={product.Id} уже существует.");

        int newId = product.Id != 0 ? product.Id :
            (_products.Count == 0 ? 1 : _products.Keys.Max() + 1);

        product.Id = newId;
        _products[newId] = product;

        return newId;
    }

    public bool UpdateById(int id, Product product)
    {
        if (!_products.ContainsKey(id))
            return false;

        product.Id = id;
        _products[id] = product;
        return true;
    }

    public bool RemoveById(int id)
    {
        return _products.Remove(id);
    }

    public void Clear()
    {
        _products.Clear();
    }
}