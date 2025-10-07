using VendingMachine.Core.Domain.Entities;

namespace VendingMachine.Core.Domain.Interfaces.Repository;

public interface IProductRepository
{
    Product? GetById(int id);
    bool UpdateById(int id, Product product);
    int Add(Product product);
    bool RemoveById(int id);
    List<Product> GetAll();
    void Clear();
}