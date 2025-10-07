using VendingMachine.Core.Application.Services;
using VendingMachine.Core.Infrastructure.Persistence.Repository;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var productRepository = new ProductRepository();
var moneyRepository = new MoneyRepository();
var service = new VendingMachineService(productRepository, moneyRepository);
var session = new VendingSession();

//да, это надо в конфиге держать, но я не успеваю это 
const string adminPassword = "veryStrongPasswordOutsideOfConfiguration)))";

while (true)
{
    Console.WriteLine("\n=== ВЕНДИНГОВЫЙ АВТОМАТ ===");
    Console.WriteLine($"Внесено: {session.InsertedMoney}");
    Console.WriteLine("1) Показать товары");
    Console.WriteLine("2) Внести деньги");
    Console.WriteLine("3) Купить товар");
    Console.WriteLine("4) Отмена (вернуть деньги)");
    Console.WriteLine("5) Админ режим");
    Console.WriteLine("0) Выход");
    Console.Write("Выбор: ");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            ShowProducts();
            break;
        
        case "2":
            InsertMoney();
            break;
        
        case "3":
            BuyProduct();
            break;
        
        case "4":
            var returned = session.InsertedMoney;
            session.Reset();
            Console.WriteLine($"Возвращено пользователю: {returned}");
            break;
        
        case "5":
            Console.Write("Введите пароль администратора: ");
            var pwd = Console.ReadLine();
            if (pwd == adminPassword)
            {
                AdminMenu();
            }
            else
            {
                Console.WriteLine("Неверный пароль.");
            }
            break;

        case "0":
            return;
        
        default:
            Console.WriteLine("Неизвестная команда.");
            break;
    }
}

void ShowProducts()
{
    Console.WriteLine("\nСписок товаров:");
    var products = productRepository.GetAll();
    foreach (var p in products)
    {
        Console.WriteLine($"ID={p.Id} | {p.Name} | Цена: {p.Price} | Остаток: {p.Amount}");
    }
}

void InsertMoney()
{
    Console.Write("Введите сумму для внесения: ");
    var input = Console.ReadLine();
    if (int.TryParse(input, out var money))
    {
        var r = session.TryInsertMoney(money);
        if (r.IsSuccess)
        {
            Console.WriteLine($"Внесено {money}. Всего: {session.InsertedMoney}");
        }
        else
        {
            Console.WriteLine(string.Join(", ", r.Messages));
        }
    }
    else
    {
        Console.WriteLine("Некорректный ввод.");
    }
}

void BuyProduct()
{
    Console.Write("Введите ID товара: ");
    var input = Console.ReadLine();
    if (!int.TryParse(input, out var id))
    {
        Console.WriteLine("Некорректный ID.");
        return;
    }

    var result = service.BuyProduct(id, session);
    if (!result.IsSuccess)
    {
        Console.WriteLine(string.Join(", ", result.Messages));
        return;
    }

    var p = result.Value!;
    Console.WriteLine($"Вы купили {p.Name} за {p.Price}. Остаток денег: {session.InsertedMoney}");
}

void AdminMenu()
{
    while (true)
    {
        Console.WriteLine("\n— Админ —");
        Console.WriteLine("1) Показать товары");
        Console.WriteLine("2) Добавить товар");
        Console.WriteLine("3) Обновить товар");
        Console.WriteLine("4) Пополнить остаток");
        Console.WriteLine("5) Удалить товар");
        Console.WriteLine("6) Деньги в кассе");
        Console.WriteLine("0) Назад");
        Console.Write("Выбор: ");

        var c = Console.ReadLine();

        switch (c)
        {
            case "1":
                var list = service.AdminListProducts();
                if (list.IsSuccess)
                {
                    foreach (var p in list.Value!)
                    {
                        Console.WriteLine($"ID={p.Id} | {p.Name} | Цена: {p.Price} | Остаток: {p.Amount}");
                    }
                }
                else
                {
                    Console.WriteLine(string.Join(", ", list.Messages));
                }
                break;
            case "2":
                Console.Write("Название: ");
                var name = Console.ReadLine() ?? string.Empty;
                Console.Write("Цена: ");
                var priceOk = int.TryParse(Console.ReadLine(), out var price);
                Console.Write("Количество: ");
                var amountOk = int.TryParse(Console.ReadLine(), out var amount);
                if (!priceOk || !amountOk)
                {
                    Console.WriteLine("Некорректный ввод.");
                    break;
                }
                var add = service.AdminAddProduct(name, price, amount);
                Console.WriteLine(add.IsSuccess ? $"Добавлен Id={add.Value}" : string.Join(", ", add.Messages));
                break;
            case "3":
                Console.Write("ID: ");
                var idOk = int.TryParse(Console.ReadLine(), out var id);
                Console.Write("Название: ");
                name = Console.ReadLine() ?? string.Empty;
                Console.Write("Цена: ");
                priceOk = int.TryParse(Console.ReadLine(), out price);
                Console.Write("Количество: ");
                amountOk = int.TryParse(Console.ReadLine(), out amount);
                if (!idOk || !priceOk || !amountOk)
                {
                    Console.WriteLine("Некорректный ввод.");
                    break;
                }
                var upd = service.AdminUpdateProduct(id, name, price, amount);
                Console.WriteLine(upd.IsSuccess ? "Ок" : string.Join(", ", upd.Messages));
                break;
            case "4":
                Console.Write("ID: ");
                idOk = int.TryParse(Console.ReadLine(), out id);
                Console.Write("Добавить: ");
                var amtOk = int.TryParse(Console.ReadLine(), out var addAmount);
                if (!idOk || !amtOk)
                {
                    Console.WriteLine("Некорректный ввод.");
                    break;
                }
                var rep = service.AdminReplenish(id, addAmount);
                Console.WriteLine(rep.IsSuccess ? "Ок" : string.Join(", ", rep.Messages));
                break;
            case "5":
                Console.Write("ID: ");
                idOk = int.TryParse(Console.ReadLine(), out id);
                if (!idOk)
                {
                    Console.WriteLine("Некорректный ввод.");
                    break;
                }
                var rem = service.AdminRemove(id);
                Console.WriteLine(rem.IsSuccess ? "Удалено" : string.Join(", ", rem.Messages));
                break;
            case "6":
                var cash = service.AdminGetCashTotal();
                Console.WriteLine(cash.IsSuccess ? $"В кассе: {cash.Value}" : string.Join(", ", cash.Messages));
                break;
            case "0":
                return;
            default:
                Console.WriteLine("Неизвестная команда.");
                break;
        }
    }
}
