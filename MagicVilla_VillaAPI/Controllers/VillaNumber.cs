using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using magicvilla_villaapi.Models;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VillaNumber : ControllerBase
    {

        private readonly
        //GetAll
        public Task<ActionResult<ApiResponse>> GetAll()
        {
            return Ok();
        }
        //GetById
        // UpdateAllById
        // UpdatePartsId
        // DeleteById
    }
}