using UsersService.Src.Application.DTOs.BankPaymentData;

namespace UsersService.Src.Application.Interfaces;

public interface IBankPaymentDataService
{
    Task CreateAsync(UpdateBankPaymentDataDto dto, Guid userPublicId);

    Task UpdateAsync(UpdateBankPaymentDataDto dto, Guid userPublicId);
}
