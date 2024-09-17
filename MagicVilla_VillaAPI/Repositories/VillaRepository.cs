using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Repositories;

public class VillaRepository(ApplicationDbContext context) : IVillaRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task CreateAsync(Villa entity)
    {
        await _context.Villas.AddAsync(entity);
        await SaveAsync();
    }

    public async Task<Villa> GetAsync(Expression<Func<Villa, bool>>? filter = null, bool tracked = true)
    {
        IQueryable<Villa> query = _context.Villas;

        if (!tracked)
            query = query.AsNoTracking();

        if (filter != null)
            query = query.Where(filter);

        return await query.FirstOrDefaultAsync();

    }

    public async Task<List<Villa>> GetAllAsync(Expression<Func<Villa, bool>>? filter = null)
    {
        IQueryable<Villa> query = _context.Villas;

        if (filter != null)
            query = query.Where(filter);
        return await query.ToListAsync();
    }

    public async Task UpdateAsync(Villa entity)
    {
        _context.Villas.Update(entity);
        await SaveAsync();

    }

    public async Task RemoveAsync(Villa entity)
    {
        _context.Villas.Remove(entity);
        await SaveAsync();
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }


}
