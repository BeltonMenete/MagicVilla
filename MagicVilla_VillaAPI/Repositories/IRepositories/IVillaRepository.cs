using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using magicvilla_villaapi.Repositories.IRepositories;
using MagicVilla_VillaAPI.Models;

namespace MagicVilla_VillaAPI.Repositories.IRepositories;

public interface IVillaRepository : IRepository<Villa>
{
    Task<Villa> UpdateAsync(Villa entity);
}
