using UsersService.Src.Domain.Entities;

namespace UsersService.Src.Domain.Interfaces;

public interface IBankPaymentDataRepository
{
    Task<BankPaymentData?> GetByUserPublicIdAsync(Guid userPublicId);

    Task AddAsync(BankPaymentData data);

    Task UpdateAsync(BankPaymentData data);
}
