using Microsoft.EntityFrameworkCore;
using UsersService.Src.Domain.Entities;
using UsersService.Src.Domain.Interfaces;
using UsersService.Src.Infraestructure.Data;

namespace UsersService.Src.Infraestructure.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    private readonly AppDbContext _context = context;

    public async Task<User?> GetByPublicIdAsync(Guid publicId)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.PublicId == publicId);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }
}