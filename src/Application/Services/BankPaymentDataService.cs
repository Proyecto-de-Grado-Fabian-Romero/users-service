using UsersService.Src.Application.Commands.Data;
using UsersService.Src.Application.Commands.Interfaces;
using UsersService.Src.Application.DTOs.BankPaymentData;
using UsersService.Src.Application.Interfaces;
using UsersService.Src.Domain.Interfaces;

namespace UsersService.Src.Application.Services;

public class BankPaymentDataService(
    ICommand<BankPaymentInput, bool> createCommand,
    ICommand<BankPaymentInput, bool> updateCommand,
    IUserRepository userRepository)
    : IBankPaymentDataService
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task CreateAsync(CreateBankPaymentDataDto dto, Guid userPublicId)
    {
        var input = new BankPaymentInput
        {
            UserPublicId = userPublicId,
            CreateDto = dto,
        };

        await createCommand.ExecuteAsync(input);

        await _userRepository.PromoteToOwnerAsync(userPublicId);
    }

    public async Task UpdateAsync(UpdateBankPaymentDataDto dto, Guid userPublicId)
    {
        var input = new BankPaymentInput
        {
            UserPublicId = userPublicId,
            UpdateDto = dto,
        };

        await updateCommand.ExecuteAsync(input);
    }
}
