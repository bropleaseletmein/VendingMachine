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
    
        public Result<List<Product>> AdminListProducts()
    {
        var all = _productRepository.GetAll();
        return Result<List<Product>>.Success(all);
    }

    public Result<int> AdminAddProduct(string name, int price, int amount)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<int>.Failure(["Имя товара не должно быть пустым"]);
        }

        if (price <= 0)
        {
            return Result<int>.Failure(["Цена должна быть > 0"]);
        }

        if (amount < 0)
        {
            return Result<int>.Failure(["Количество не может быть отрицательным"]);
        }

        var id = _productRepository.Add(new Product
        {
            Id = 0,
            Name = name.Trim(),
            Price = price,
            Amount = amount
        });

        return Result<int>.Success(id);
    }

    public Result AdminUpdateProduct(int id, string name, int price, int amount)
    {
        var existing = _productRepository.GetById(id);
        if (existing is null)
        {
            return Result.Failure([$"Товар с Id={id} не найден"]);
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(["Имя товара не должно быть пустым"]);
        }

        if (price <= 0)
        {
            return Result.Failure(["Цена должна быть > 0"]);
        }

        if (amount < 0)
        {
            return Result.Failure(["Количество не может быть отрицательным"]);
        }

        var ok = _productRepository.UpdateById(id, new Product
        {
            Id = id,
            Name = name.Trim(),
            Price = price,
            Amount = amount
        });

        return ok ? Result.Success() : Result.Failure(["Не удалось обновить товар"]);
    }

    public Result AdminReplenish(int id, int amountToAdd)
    {
        if (amountToAdd <= 0)
        {
            return Result.Failure(["Количество должно быть > 0"]);
        }

        var p = _productRepository.GetById(id);
        if (p is null)
        {
            return Result.Failure([$"Товар с Id={id} не найден"]);
        }

        var ok = _productRepository.UpdateById(id, new Product
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Amount = p.Amount + amountToAdd
        });

        return ok ? Result.Success() : Result.Failure(["Не удалось пополнить остаток"]);
    }

    public Result AdminRemove(int id)
    {
        var removed = _productRepository.RemoveById(id);
        return removed ? Result.Success() : Result.Failure([$"Товар с Id={id} не найден"]);
    }

    public Result<int> AdminGetCashTotal()
    {
        var total = _moneyRepository.GetMoney(0);
        return Result<int>.Success(total);
    }

}