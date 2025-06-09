namespace UsersService.Src.Application.DTOs.BankPaymentData;

public class CreateBankPaymentDataDto
{
    public string BankAccountNumber { get; set; } = string.Empty;

    public string BankAccountHolder { get; set; } = string.Empty;

    public string BankName { get; set; } = string.Empty;
}