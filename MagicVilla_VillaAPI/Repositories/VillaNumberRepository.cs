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

        public override async Task CreateAsync(VillaNumber entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            await _context.AddAsync(entity);
            await SaveAsync();
        }
        public async Task<VillaNumber> UpdateAsync(VillaNumber entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            _context.VillaNumbers.Update(entity);
            await SaveAsync();
            return entity;
        }
    }
}