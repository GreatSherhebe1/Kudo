using Kudo.Storage.Context;
using Kudo.Storage.Enum;
using Kudo.Storage.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Kudo.Storage.Utils;

namespace Kudo.Storage.Repositories;

public class StorageRepository : IStorageRepository
{
    private readonly IDatabaseContext _context;

    private async Task<bool> IsStorageFullOfData(CancellationToken cancellationToken)
    {
        var r = await _context.Categories.AnyAsync(cancellationToken)
                || await _context.Users.AnyAsync(cancellationToken)
                || await _context.FinancialTransactions.AnyAsync(cancellationToken);

        return r;
    }

    private StorageRepository(IDatabaseContext context)
    {
        _context = context;
    }

    public static IStorageRepository Create(IDatabaseContext databaseContext)
    {
        return new StorageRepository(databaseContext);
    }

    public async Task<bool> CreateStorageDefaultDataAsync(CancellationToken cancellationToken)
    {
        if (await IsStorageFullOfData(cancellationToken))
        {
            return false;
        }

        await CreateUsersAsync(cancellationToken);
        await CreateCategoriesAsync(cancellationToken);
        await CreateFinancialTransactionAsync(cancellationToken);

        return true;
    }

    public async Task CreateCategoryAsync(
        string userLogin, 
        string name, 
        FinancialTransactionType transactionType,
        CancellationToken cancellationToken)
    {
        var user = await GetUserByLogin(userLogin, cancellationToken);

        var category = new Category
        {
            Name = name,
            TransactionType = transactionType,
            UserId = user.Id,
        };

        await _context.Categories.AddAsync(category, cancellationToken);
        await _context.SaveAllChangesAsync(cancellationToken);
    }

    public async Task RemoveCategoryAsync(
        Guid categoryId, 
        CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(category => categoryId == category.Id, cancellationToken);

        if (category is not null)
        {
            _context.Categories.Remove(category);
            await _context.SaveAllChangesAsync(cancellationToken);
        }
    }

    public async Task<List<Category>> GetCategoriesByUser(
        string userLogin, 
        FinancialTransactionType transactionType, 
        CancellationToken cancellationToken)
    {
        var user = await GetUserByLogin(userLogin, cancellationToken);

        return await _context.Categories
            .Where(category => category.UserId == user.Id && category.TransactionType == transactionType)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task CreateUserAsync(
        string login, 
        string password, 
        CancellationToken cancellationToken)
    {
        if (!SecurityHelper.IsLoginValid(login))
        {
            throw new Exception("Логин не удовлетворяет требованиям безопасности. Должен содержать латинские буквы или цифры; длина не менее 4 символов");
        }

        if (!SecurityHelper.IsPasswordValid(password))
        {
            throw new Exception("Пароль не удовлетворяет требованиям безопасности: должны присутствовать буквы или цифры без пробелов; длина не менее 5 символов.");
        }

        var now = DateTime.Now;
        var hash = SecurityHelper.GeneratePasswordHash(login, password, now);

        var user = new User
        {
            Login = login,
            PasswordHash = hash,
            CreatedAt = now
        };
        
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveAllChangesAsync(cancellationToken);
    }

    public Task UpdateUserAsync(
        Guid id, 
        string? login, 
        string? password, 
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<User> GetUserById(
        Guid userId, 
        CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .SingleAsync(user => user.Id == userId, cancellationToken);
    }

    public async Task<User> GetUserByLogin(
        string login, 
        CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .SingleAsync(user => user.Login == login, cancellationToken);
    }

    public async Task CreateFinancialTransactionAsync(
        string userLogin, 
        FinancialTransactionType transactionType, 
        Category category,
        decimal amount, 
        CancellationToken cancellationToken)
    {
        var user = await GetUserByLogin(userLogin, cancellationToken);

        var financialTransaction = new FinancialTransaction
        {
            CategoryId = category.Id,
            UserId = user.Id,
            Amount = amount,
            CreatedAt = DateTime.Now
        };

        await _context.FinancialTransactions.AddAsync(financialTransaction, cancellationToken);
        await _context.SaveAllChangesAsync(cancellationToken);
    }

    public async Task UpdateFinancialTransactionAsync(
        Guid transactionId, 
        FinancialTransactionType? transactionType, 
        Category? category,
        decimal? amount, 
        CancellationToken cancellationToken)
    {
        var financialTransaction = await _context.FinancialTransactions
            .Include(ft => ft.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(ft => ft.Id == transactionId, cancellationToken);

        if (financialTransaction == null)
        {
            throw new ArgumentException("Транзакция не найдена");
        }

        if (transactionType.HasValue)
        {
            financialTransaction.TransactionType = transactionType.Value;
        }

        if (category != null)
        {
            financialTransaction.Category = category;
        }

        if (amount.HasValue)
        {
            financialTransaction.Amount = amount.Value;
        }

        await _context.SaveAllChangesAsync(cancellationToken);
    }

    public async Task RemoveFinancialTransactionAsync(
        Guid transactionId, 
        CancellationToken cancellationToken)
    {
        var financialTransaction = await _context.FinancialTransactions
            .FirstOrDefaultAsync(financialTransaction => transactionId == financialTransaction.Id, cancellationToken);

        if (financialTransaction is not null)
        {
            _context.FinancialTransactions.Remove(financialTransaction);
            await _context.SaveAllChangesAsync(cancellationToken);
        }
    }

    public async Task<List<FinancialTransaction>> GetFinancialTransactions(
        string userLogin, 
        FinancialTransactionType transactionType,
        CancellationToken cancellationToken)
    {
        return await _context.FinancialTransactions
            .AsNoTracking()
            .Where(financialTransaction => financialTransaction.User.Login == userLogin)
            .ToListAsync(cancellationToken);
    }

    private async Task CreateCategoriesAsync(CancellationToken cancellationToken)
    {
        await CreateCategoryAsync(
            "ivan",
            "Фрукты",
            FinancialTransactionType.Expanse,
            cancellationToken);

        await CreateCategoryAsync(
            "ivan",
            "Кварплата",
            FinancialTransactionType.Expanse,
            cancellationToken);

        await CreateCategoryAsync(
            "petr",
            "Пиво",
            FinancialTransactionType.Expanse,
            cancellationToken);

        await CreateCategoryAsync(
            "petr",
            "Кварплата",
            FinancialTransactionType.Expanse,
            cancellationToken);

        await CreateCategoryAsync(
            "ivan",
            "Зарплата",
            FinancialTransactionType.Income,
            cancellationToken);

        await CreateCategoryAsync(
            "petr",
            "Получка",
            FinancialTransactionType.Income,
            cancellationToken);
    }

    private async Task CreateUsersAsync(CancellationToken cancellationToken)
    {
        await CreateUserAsync(
            "ivan",
            "kudo1",
            cancellationToken);

        await CreateUserAsync(
            "petr",
            "kudo2",
            cancellationToken);
    }

    private async Task CreateFinancialTransactionAsync(CancellationToken cancellationToken)
    {
        var expanseCategoriesByIvan = await GetCategoriesByUser("ivan", FinancialTransactionType.Expanse, cancellationToken);
        var incomeCategoriesByIvan = await GetCategoriesByUser("ivan", FinancialTransactionType.Income, cancellationToken);
        var expanseCategoriesByPetr = await GetCategoriesByUser("petr", FinancialTransactionType.Expanse, cancellationToken);
        var incomeCategoriesByPetr = await GetCategoriesByUser("petr", FinancialTransactionType.Income, cancellationToken);

        await CreateFinancialTransactionAsync(
            "ivan",
            FinancialTransactionType.Expanse,
            expanseCategoriesByIvan[0],
            500.00m,
            cancellationToken);

        await CreateFinancialTransactionAsync(
            "ivan",
            FinancialTransactionType.Expanse,
            expanseCategoriesByIvan[0],
            1000.00m,
            cancellationToken);

        await CreateFinancialTransactionAsync(
            "ivan",
            FinancialTransactionType.Expanse,
            expanseCategoriesByIvan[1],
            7000.00m,
            cancellationToken);

        await CreateFinancialTransactionAsync(
            "petr",
            FinancialTransactionType.Expanse,
            expanseCategoriesByPetr[0],
            5000.00m,
            cancellationToken);

        await CreateFinancialTransactionAsync(
            "petr",
            FinancialTransactionType.Expanse,
            expanseCategoriesByPetr[0],
            2000.00m,
            cancellationToken);

        await CreateFinancialTransactionAsync(
            "petr",
            FinancialTransactionType.Expanse,
            expanseCategoriesByPetr[1],
            7000.00m,
            cancellationToken);


        await CreateFinancialTransactionAsync(
            "ivan",
            FinancialTransactionType.Income,
            incomeCategoriesByIvan[0],
            20000.00m,
            cancellationToken);

        await CreateFinancialTransactionAsync(
            "petr",
            FinancialTransactionType.Income,
            incomeCategoriesByPetr[0],
            70000.00m,
            cancellationToken);
    }
}