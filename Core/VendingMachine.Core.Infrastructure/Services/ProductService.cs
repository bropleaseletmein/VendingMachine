using VendingMachine.Core.Domain.Dto.Response;
using VendingMachine.Core.Domain.Entities;

namespace VendingMachine.Core.Infrastructure.Services;

public class ProductService
{
    private VendingMachineState _vendingMachineState;
    
    public ProductService(
        VendingMachineState vendingMachineState)
    {
        _vendingMachineState = vendingMachineState; 
    }

    public Result<Product> GetProductById(int productId)
    {
        if (!_vendingMachineState.Products.TryGetValue(productId, out var product))
        {
            return Result<Product>.Failure([$"Не найден товар с Id={productId}"]);
        }
        
        return Result<Product>.Success(product);
    }
    
}