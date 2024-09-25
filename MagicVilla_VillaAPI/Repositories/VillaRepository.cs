using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using magicvilla_villaapi.Repositories;
using magicvilla_villaapi.Repositories.IRepositories;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Repositories;

public class VillaRepository : Repository<Villa>, IVillaRepository
{
    private readonly ApplicationDbContext _context;
    public VillaRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Villa> UpdateAsync(Villa entity)
    {
        entity.UpdatedDate = DateTime.Now;
        _context.Villas.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}
