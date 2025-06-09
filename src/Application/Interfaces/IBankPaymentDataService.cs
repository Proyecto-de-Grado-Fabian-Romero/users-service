using UsersService.Src.Application.DTOs.BankPaymentData;

namespace UsersService.Src.Application.Interfaces;

public interface IBankPaymentDataService
{
    Task CreateAsync(CreateBankPaymentDataDto dto, Guid userPublicId);

    Task UpdateAsync(UpdateBankPaymentDataDto dto, Guid userPublicId);
}
