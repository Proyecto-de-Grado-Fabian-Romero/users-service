using UsersService.Src.Application.DTOs.BankPaymentData;

namespace UsersService.Src.Application.Commands.Data;

public class BankPaymentInput
{
    public Guid UserPublicId { get; set; }

    required public UpdateBankPaymentDataDto Dto { get; set; }
}