using Kudo.Storage.Enum;
using Kudo.Storage.Models;

namespace Kudo.Storage.Repositories;

public interface IStorageRepository
{
    Task<bool> CreateStorageDefaultDataAsync(CancellationToken cancellationToken);

    Task CreateCategoryAsync(
        string userLogin,
        string name,
        FinancialTransactionType transactionType,
        CancellationToken cancellationToken);

    Task RemoveCategoryAsync(
        Guid categoryId, 
        CancellationToken cancellationToken);

    Task<List<Category>> GetCategoriesByUser(
        string userLogin,
        FinancialTransactionType transactionType,
        CancellationToken cancellationToken);

    Task CreateUserAsync(
        string login,
        string password,
        CancellationToken cancellationToken);

    Task UpdateUserAsync(
        Guid id,
        string? login,
        string? password,
        CancellationToken cancellationToken);

    Task<User> GetUserById(
        Guid userId,
        CancellationToken cancellationToken);

    Task<User> GetUserByLogin(
        string login,
        CancellationToken cancellationToken);

    Task CreateFinancialTransactionAsync(
        string userLogin,
        FinancialTransactionType transactionType,
        Category category,
        decimal amount,
        CancellationToken cancellationToken);

    Task UpdateFinancialTransactionAsync(
        Guid transactionId,
        FinancialTransactionType? transactionType,
        Category? categories,
        decimal? amount,
        CancellationToken cancellationToken);

    Task RemoveFinancialTransactionAsync(
        Guid transactionId,
        CancellationToken cancellationToken);

    Task<List<FinancialTransaction>> GetFinancialTransactions(
        string userLogin,
        FinancialTransactionType transactionType,
        CancellationToken cancellationToken);
}