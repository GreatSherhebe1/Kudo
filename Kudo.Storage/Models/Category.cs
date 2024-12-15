using Kudo.Storage.Enum;

namespace Kudo.Storage.Models;

public class Category
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; }

    public string Name { get; set; }

    public FinancialTransactionType TransactionType { get; set; }
}