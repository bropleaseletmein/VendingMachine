namespace VendingMachine.Core.Domain.Entities;

public class Product
{
    private int _id;
    public int Id
    {
        get => _id;
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("Id продукта должен быть неотрицательным");
            }
            
            _id = value;
        }
    }
    
    private string _name;
    public required string Name
    {
        get => _name;

        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Имя товара не может быть пустым");
            }
            
            _name = value;
        }
    }

    private int _price;
    public required int Price
    {
        get => _price;

        set
        {
            if (value < 0)
            {
                throw new ArgumentException("Цена не может быть отрицательной");
            }

            if (value == 0)
            {
                throw new ArgumentException("Цена не может быть 0");
            }

            _price = value;
        }
    }

    private int _amount;
    public int Amount
    {
        get => _amount;
        set
        {
            if (_amount < 0)
            {
                throw new ArgumentException("Количество товара не может быть меньше 0");
            }
            
            _amount = value;    
        }
    }
}