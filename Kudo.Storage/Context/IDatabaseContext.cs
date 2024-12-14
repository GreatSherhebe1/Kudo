using Kudo.Storage.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Kudo.Storage.Context;

public interface IDatabaseContext : IDisposable
{
    DbSet<Category> Categories { get; }

    DbSet<FinancialTransaction> FinancialTransactions { get; }

    DbSet<User> Users { get; }

    void EnsureMigratedDatabase();

    Task EnsureMigratedDatabaseAsync(CancellationToken cancellationToken);

    void EnsureDeletedDatabase();

    Task EnsureDeletedDatabaseAsync(CancellationToken cancellationToken);

    IDbContextTransaction BeginTransaction();

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);

    int SaveAllChanges();

    Task<int> SaveAllChangesAsync(CancellationToken cancellationToken);
}