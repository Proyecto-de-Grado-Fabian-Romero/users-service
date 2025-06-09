using UsersService.Src.Application.Commands.Data;
using UsersService.Src.Application.Commands.Interfaces;
using UsersService.Src.Domain.Interfaces;

namespace UsersService.Src.Application.Commands.Concretes.BankPayment;

public class UpdateBankPaymentDataCommand(
    IBankPaymentDataRepository bankRepository)
    : ICommand<BankPaymentInput, bool>
{
    public async Task<bool> ExecuteAsync(BankPaymentInput input)
    {
        if (input.UpdateDto is null)
        {
            throw new ArgumentException("Update DTO is required.");
        }

        var data = await bankRepository.GetByUserPublicIdAsync(input.UserPublicId)
                   ?? throw new Exception("Datos bancarios no encontrados");

        data.BankAccountNumber = input.UpdateDto.BankAccountNumber;
        data.BankAccountHolder = input.UpdateDto.BankAccountHolder;
        data.BankName = input.UpdateDto.BankName;

        await bankRepository.UpdateAsync(data);
        return true;
    }
}
