using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using magicvilla_villaapi.Repositories.IRepositories;
using MagicVilla_VillaAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace magicvilla_villaapi.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    internal DbSet<T> DbSet;
    public Repository(ApplicationDbContext context)
    {
        _context = context;
        this.DbSet = _context.Set<T>();
    }
    public async virtual Task CreateAsync(T entity)
    {
        await _context.AddAsync(entity);
        await SaveAsync();
    }

    public async Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true)
    {
        IQueryable<T> query = DbSet;

        if (!tracked)
            query = query.AsNoTracking();

        if (filter != null)
            query = query.Where(filter);

        return await query.FirstOrDefaultAsync();

    }

    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null)
    {
        IQueryable<T> query = DbSet;

        if (filter != null)
            query = query.Where(filter);
        return await query.ToListAsync();
    }


    public async Task RemoveAsync(T entity)
    {
        DbSet.Remove(entity);
        await SaveAsync();
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
