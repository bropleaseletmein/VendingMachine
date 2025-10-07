namespace VendingMachine.Core.Domain.Interfaces.Repository;

public interface IMoneyRepository
{ 
    int GetMoney(int money);
    void SaveMoney(int money);
}