using Kudo.Storage.Enum;

namespace Kudo.Storage.Models;

public class FinancialTransaction
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; }

    public FinancialTransactionType TransactionType { get; set; }

    public Guid CategoryId { get; set; }

    public Category Category { get; set; }

    public decimal Amount { get; set; }

    public DateTime CreatedAt { get; set; }
}