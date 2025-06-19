using UsersService.Src.Domain.Entities;

namespace UsersService.Src.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByPublicIdAsync(Guid publicId);

    Task<User?> GetByIdAsync(Guid subId);

    Task PromoteToOwnerAsync(Guid publicId);
}
