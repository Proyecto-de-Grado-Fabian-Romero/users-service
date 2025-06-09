using UsersService.Src.Application.DTOs.BankPaymentData;

namespace UsersService.Src.Application.Commands.Data;

public class BankPaymentDataInput
{
    public Guid UserPublicId { get; set; }

    public CreateBankPaymentDataDto? CreateDto { get; set; }

    public UpdateBankPaymentDataDto? UpdateDto { get; set; }
}