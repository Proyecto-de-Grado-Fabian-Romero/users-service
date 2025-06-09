using UsersService.Src.Application.Commands.Data;
using UsersService.Src.Application.Commands.Interfaces;
using UsersService.Src.Domain.Entities;
using UsersService.Src.Domain.Interfaces;

namespace UsersService.Src.Application.Commands.Concretes.BankPayment;

public class CreateBankPaymentDataCommand(
    IUserRepository userRepository,
    IBankPaymentDataRepository bankRepository)
    : ICommand<BankPaymentInput, bool>
{
    public async Task<bool> ExecuteAsync(BankPaymentInput input)
    {
        if (input.CreateDto is null)
        {
            throw new ArgumentException("Create DTO is required.");
        }

        var user = await userRepository.GetByPublicIdAsync(input.UserPublicId)
                   ?? throw new Exception("Usuario no encontrado");

        var existing = await bankRepository.GetByUserPublicIdAsync(input.UserPublicId);
        if (existing is not null)
        {
            throw new Exception("Datos de pago ya existen");
        }

        var data = new BankPaymentData
        {
            UserId = user.Id,
            BankAccountNumber = input.CreateDto.BankAccountNumber,
            BankAccountHolder = input.CreateDto.BankAccountHolder,
            BankName = input.CreateDto.BankName,
        };

        await bankRepository.AddAsync(data);
        return true;
    }
}
