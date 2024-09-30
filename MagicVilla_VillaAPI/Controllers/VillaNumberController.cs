using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using magicvilla_villaapi.Models;
using MagicVilla_VillaAPI.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTOs.VillaNumberDTOs;

namespace MagicVilla_VillaAPI.Controllers
{
    [ApiController]
    [Route("api/VillaNumber")]
    public class VillaNumberController(IVillaNumberRepository repo) : ControllerBase
    {
        private readonly IVillaNumberRepository _repo = repo;
        protected ApiResponse _response = new();

        //GetAll
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse>> GetVillas()
        {
            _response.Result = await _repo.GetAllAsync();
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        // Add new Villa Number
        [HttpPost(Name = "CreateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> CreateVillaNumber([FromBody] VillaNumber villaNumber)
        {
            try
            {
                if (villaNumber == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                await _repo.CreateAsync(villaNumber);
                await _repo.SaveAsync();
                _response.Result = villaNumber;
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetVillaNumber", new { VillaNo = villaNumber.VillaNo }, _response);

            }
            catch (Exception error)
            {
                _response.ErrorMessages = [error.Message.ToString()];
                _response.IsSuccess = false;
                return _response;
            }


        }
        [HttpGet("{id:int}", Name = "GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> GetVillaNumber(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _response.ErrorMessages = ["Invalid Id"];
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var villaNumber = await _repo.GetAsync(x => x.VillaNo == id);

                if (villaNumber == null)
                {
                    _response.ErrorMessages = [$"Villa Number {id} Not found"];
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);

                }
                _response.Result = villaNumber;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception error)
            {
                _response.ErrorMessages = [error.Message.ToString()];
                _response.IsSuccess = false;
                return _response;
            }
        }

    }
}