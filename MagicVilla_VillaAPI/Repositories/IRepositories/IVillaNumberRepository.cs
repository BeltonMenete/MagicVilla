using magicvilla_villaapi.Repositories.IRepositories;
using MagicVilla_VillaAPI.Models;

namespace MagicVilla_VillaAPI.Repositories.IRepositories
{
    public interface IVillaNumberRepository : IRepository<VillaNumber>
    {
        public Task<VillaNumber> UpdateAsync(VillaNumber entity);
    }
}