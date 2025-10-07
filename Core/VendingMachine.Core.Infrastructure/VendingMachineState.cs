using VendingMachine.Core.Domain.Entities;

namespace VendingMachine.Core.Infrastructure;

public class VendingMachineState
{
    /// <summary>
    /// общее количество денег в автомате
    /// </summary>
    public int MoneyAvailable { get; set; }

    /// <summary>
    /// сумма, внесенная пользователем для покупки
    /// </summary>
    public int DepositedMoney { get; set; } = 0;
    
    /// <summary>
    /// словарь: айди товара -> товар
    /// </summary>
    public Dictionary<int, Product> Products { get; set; } = new Dictionary<int, Product>();
    
    public VendingMachineState()
    {
        
    }
}