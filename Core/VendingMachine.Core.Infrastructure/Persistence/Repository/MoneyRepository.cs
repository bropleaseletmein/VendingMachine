using VendingMachine.Core.Domain.Interfaces.Repository;

namespace VendingMachine.Core.Infrastructure.Persistence.Repository;

public class MoneyRepository : IMoneyRepository
{
    private int _money;

    public int GetMoney(int money)
    {
        return _money;
    }

    public void SaveMoney(int money)
    {
        if (money <= 0)
            throw new ArgumentException("Сумма должна быть положительной");

        _money += money;
    }
}