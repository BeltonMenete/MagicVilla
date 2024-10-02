using System.Net;
using magicvilla_villaapi.Models;
using MagicVilla_VillaAPI.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTOs.VillaNumberDTOs;
using AutoMapper;

namespace MagicVilla_VillaAPI.Controllers
{
    [ApiController]
    [Route("api/VillaNumber")]
    public class VillaNumberController(IVillaNumberRepository repo, IMapper mapper) : ControllerBase
    {
        private readonly IMapper _mapper = mapper;
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

        // Get one
        [HttpGet("{villaNo:int}", Name = "GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> GetVillaNumber(int villaNo)
        {
            try
            {

                if (villaNo <= 0)
                {
                    _response.ErrorMessages = ["Invalid Id"];
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var villaNumber = await _repo.GetAsync(x => x.VillaNo == villaNo);

                if (villaNumber == null)
                {
                    _response.ErrorMessages = [$"Villa Number {villaNo} Not found"];
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
        // Add new Villa Number
        [HttpPost(Name = "CreateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> CreateVillaNumber([FromBody] CreateVillaNumberDTO createVillaNumber)
        {
            try
            {
                if (createVillaNumber.VillaNo <= 0)
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessages = [$"This {createVillaNumber.VillaNo} is invalid"];
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var exists = await _repo.GetAsync(x => x.VillaNo == createVillaNumber.VillaNo) != null;

                if (exists)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = ["Villa Number already exists"];
                    return BadRequest(_response);
                }

                if (createVillaNumber == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = ["Something is missing"];
                    return BadRequest(_response);
                }

                var villaNumber = _mapper.Map<VillaNumber>(createVillaNumber);

                villaNumber.CreateDate = DateTime.UtcNow;
                villaNumber.UpdatedDate = DateTime.UtcNow;

                await _repo.CreateAsync(villaNumber);
                await _repo.SaveAsync();
                _response.Result = villaNumber;
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetVillaNumber", new { villaNo = villaNumber.VillaNo }, _response);
            }
            catch (Exception error)
            {
                _response.ErrorMessages = [error.Message.ToString()];
                _response.IsSuccess = false;
                return _response;
            }
        }

        // Update existing Villa Number
        [HttpPut]
    }
}