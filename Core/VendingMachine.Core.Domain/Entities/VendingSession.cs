using VendingMachine.Core.Domain.Dto.Response;

public class VendingSession
{
    private int _insertedMoney;
    public int InsertedMoney => _insertedMoney; 

    public Result TryInsertMoney(int amount)
    {
        if (amount <= 0)
        {
            return Result.Failure(["Нельзя вставить отрицательное число денег или 0"]);
        }

        _insertedMoney += amount;

        return Result.Success();
    }

    public Result TrySpendMoney(int amount)
    {
        if (amount <= 0)
        {
            return Result.Failure(["Нельзя списать отрицательное число денег или 0"]);
        }

        if (_insertedMoney < amount)
        {
            return Result.Failure(["Недостаточно денег"]);
        }

        _insertedMoney -= amount;
        
        return Result.Success();
    }

    public void Reset() => _insertedMoney = 0;
}