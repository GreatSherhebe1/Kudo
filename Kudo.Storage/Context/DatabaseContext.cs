using Kudo.Storage.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualBasic;
using System.Diagnostics.Metrics;

namespace Kudo.Storage.Context;

public partial class DatabaseContext : DbContext, IDatabaseContext
{
    private readonly string _connectionString = string.Empty;

    private Dictionary<IProperty, int>? _maxLengthMetadata;

    private const string sourceLocale = "en-u-ks-primary";

    private const string targetCollation = "icu_en-u-ks-primary";

    private DatabaseContext()
    {
        // Npgsql 6.0 brings some major breaking changes and is not a simple in-place upgrade.
        // Carefully read the breaking change notes below and upgrade with care.
        // https://www.npgsql.org/efcore/release-notes/6.0.html?tabs=annotations#timestamp-rationalization-and-improvements
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    private DatabaseContext(string connectionString) : this()
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        this._connectionString = connectionString;
    }

    public DbSet<Category> Categories { get; set; }

    public DbSet<FinancialTransaction> FinancialTransactions { get; set; }

    public DbSet<User> Users { get; set; }

    public static IDatabaseContext Create(string connectionString)
    {
        return new DatabaseContext(connectionString);
    }

    public void EnsureMigratedDatabase()
    {
        Database.Migrate();
    }

    public async Task EnsureMigratedDatabaseAsync(CancellationToken cancellationToken)
    {
        await Database.MigrateAsync(cancellationToken);
    }

    public void EnsureDeletedDatabase()
    {
        Database.EnsureDeleted();
    }

    public async Task EnsureDeletedDatabaseAsync(CancellationToken cancellationToken)
    {
        await Database.EnsureDeletedAsync(cancellationToken);
    }

    public IDbContextTransaction BeginTransaction()
    {
        return Database.BeginTransaction();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        return await Database.BeginTransactionAsync(cancellationToken);
    }

    public int SaveAllChanges()
    {
        TruncateStringProperties();

        return SaveChanges();
    }

    public async Task<int> SaveAllChangesAsync(CancellationToken cancellationToken)
    {
        TruncateStringProperties();

        return await SaveChangesAsync(cancellationToken);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(_connectionString);
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasCollation(targetCollation, locale: sourceLocale, provider: "icu", deterministic: false);

        modelBuilder.Entity<User>(entity =>
        {
            entity.Metadata.SetTableName(nameof(Users));

            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasIdentityOptions();
            entity.Property(x => x.Login).HasMaxLength(32);
            entity.Property(x => x.Login).UseCollation(targetCollation);
            entity.Property(x => x.PasswordHash).HasMaxLength(512);
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Metadata.SetTableName(nameof(Categories));

            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasIdentityOptions();
            entity.Property(x => x.Name).HasMaxLength(32);
            entity.Property(x => x.Name).UseCollation(targetCollation);
        });

        modelBuilder.Entity<FinancialTransaction>(entity =>
        {
            entity.Metadata.SetTableName(nameof(FinancialTransactions));

            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasIdentityOptions();
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
        });
    }

    private void TruncateStringProperties()
    {
        _maxLengthMetadata ??= GetMaxLengthMetadata();

        foreach (var entry in ChangeTracker.Entries())
        {
            foreach (var property in entry.CurrentValues.Properties.Where(x => x.ClrType == typeof(string)))
            {
                if (entry.CurrentValues[property.Name] == null)
                {
                    continue;
                }

                var value = entry.CurrentValues[property.Name]?.ToString();

                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                if (_maxLengthMetadata.TryGetValue(property, out var value1))
                {
                    entry.CurrentValues[property.Name] = TruncateString(value, value1);
                }
            }
        }
    }

    private Dictionary<IProperty, int> GetMaxLengthMetadata()
    {
        var metadata = new Dictionary<IProperty, int>();

        foreach (var entityType in Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                var annotation = property.GetAnnotations().FirstOrDefault(x => x.Name == "MaxLength");

                if (annotation == null)
                {
                    continue;
                }

                var length = Convert.ToInt32(annotation.Value);

                if (length > 0)
                {
                    metadata[property] = length;
                }
            }
        }

        return metadata;
    }

    private static string TruncateString(string value, int length)
    {
        return string.IsNullOrEmpty(value)
            ? value
            : value.Length > length
                ? value[..length]
                : value;
    }
}