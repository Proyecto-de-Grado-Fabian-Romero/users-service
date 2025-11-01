namespace UsersService.Src.Domain.Entities;

public class BankPaymentData
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }

    public string BankAccountNumber { get; set; } = string.Empty;

    public string BankAccountHolder { get; set; } = string.Empty;

    public string BankName { get; set; } = string.Empty;

    public User User { get; set; } = null!;
}
