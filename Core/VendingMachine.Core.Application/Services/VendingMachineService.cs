using VendingMachine.Core.Domain.Dto.Response;
using VendingMachine.Core.Domain.Entities;
using VendingMachine.Core.Domain.Interfaces.Repository;
using VendingMachine.Core.Application.Dto;

namespace VendingMachine.Core.Application.Services;

public class VendingMachineService
{
    private readonly IProductRepository _productRepository;
    private readonly IMoneyRepository _moneyRepository;

    public VendingMachineService(
        IProductRepository productRepository,
        IMoneyRepository moneyRepository)
    {
        _productRepository = productRepository;
        _moneyRepository = moneyRepository;
    }

    public Result<ReturnedProduct> BuyProduct(int productId, VendingSession vendingSession)
    {
        var product = _productRepository.GetById(productId);
        if (product is null)
        {
            return Result<ReturnedProduct>.Failure([$"Не найден товар с Id={productId}"]);
        }

        if (product.Amount == 0)
        {
            return Result<ReturnedProduct>.Failure([$"Товар с Id={productId} закончился"]);
        }

        if (vendingSession.InsertedMoney < product.Price)
        {
            return Result<ReturnedProduct>.Failure(
                [$"Недостаточно денег ({vendingSession.InsertedMoney}) для покупки товара стоимостью {product.Price}"]);
        }

        var spendingResult = vendingSession.TrySpendMoney(product.Price);

        if (!spendingResult.IsSuccess)
        {
            return Result<ReturnedProduct>.Failure(spendingResult.Messages);
        }
        
        _moneyRepository.SaveMoney(product.Price);

        _productRepository.UpdateById(productId, new Product
        {
            Id = product.Id,
            Name = product.Name,
            Amount = product.Amount - 1,
            Price = product.Price,
        });
        
        var returned = new ReturnedProduct
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price, 
        };

        return Result<ReturnedProduct>.Success(returned);
    }
}