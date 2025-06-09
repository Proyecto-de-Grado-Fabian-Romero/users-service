using Microsoft.EntityFrameworkCore;
using UsersService.Src.Domain.Entities;
using UsersService.Src.Domain.Interfaces;
using UsersService.Src.Infraestructure.Data;

namespace UsersService.Src.Infraestructure.Repositories;

public class BankPaymentDataRepository(AppDbContext context) : IBankPaymentDataRepository
{
    private readonly AppDbContext _context = context;

    public async Task<BankPaymentData?> GetByUserPublicIdAsync(Guid userPublicId)
    {
        var user = await _context.Users
            .Include(u => u.BankPaymentData)
            .FirstOrDefaultAsync(u => u.PublicId == userPublicId);

        return user?.BankPaymentData;
    }

    public async Task AddAsync(BankPaymentData data)
    {
        _context.BankPaymentData.Add(data);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(BankPaymentData data)
    {
        _context.BankPaymentData.Update(data);
        await _context.SaveChangesAsync();
    }
}
