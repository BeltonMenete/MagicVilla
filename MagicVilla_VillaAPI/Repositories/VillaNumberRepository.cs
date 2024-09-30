using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using magicvilla_villaapi.Repositories;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repositories.IRepositories;

namespace MagicVilla_VillaAPI.Repositories
{
    public class VillaNumberRepository(ApplicationDbContext context) : Repository<VillaNumber>(context), IVillaNumberRepository
    {
        private readonly ApplicationDbContext _context = context;

        public new async Task CreateAsync(VillaNumber entity)
        {
            entity.CreateDate = DateTime.Now;
            await _context.VillaNumbers.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<VillaNumber> UpdateAsync(VillaNumber entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _context.VillaNumbers.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}