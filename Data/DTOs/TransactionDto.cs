public class TransactionDto
{
    public int AccountId { get; set; }
    public decimal Amount { get; set; }
    public string TransactionType { get; set; } // Puede ser "Withdraw" o "Deposit"
}
