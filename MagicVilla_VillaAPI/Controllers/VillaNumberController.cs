using System.Net;
using magicvilla_villaapi.Models;
using MagicVilla_VillaAPI.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTOs.VillaNumberDTOs;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;

namespace MagicVilla_VillaAPI.Controllers;
[ApiController]
[Route("api/VillaNumber")]
public class VillaNumberController(IVillaNumberRepository villaNumbers, IVillaRepository villas, IMapper mapper) : ControllerBase
{
    private readonly IMapper _mapper = mapper;
    private readonly IVillaRepository _villas = villas;
    private readonly IVillaNumberRepository _villaNumbers = villaNumbers;
    protected APIResponse _response = new();

    //GetAll
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse>> GetVillas()
    {
        _response.Result = await _villaNumbers.GetAllAsync();
        _response.StatusCode = HttpStatusCode.OK;
        return Ok(_response);
    }

    // Get one
    [HttpGet("{villaNo:int}", Name = "GetVillaNumber")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<APIResponse>> GetVillaNumber(int villaNo)
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

            var villaNumber = await _villaNumbers.GetAsync(x => x.VillaNo == villaNo);

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
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] CreateVillaNumberDTO createVillaNumber)
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

            var exists = await _villaNumbers.GetAsync(x => x.VillaNo == createVillaNumber.VillaNo) != null;

            if (exists)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = ["Villa Number already exists"];
                return Conflict(_response);
            }
            // Check if the referenced Villa exists
            var villaExists = await _villas.GetAsync(x => x.Id == createVillaNumber.VillaID) != null;
            if (!villaExists)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = [$"Villa ID {createVillaNumber.VillaID} not found"];
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }


            if (createVillaNumber == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = ["Something is missing"];
                return BadRequest(_response);
            }

            var villaNumber = _mapper.Map<VillaNumber>(createVillaNumber);
            villaNumber.UpdatedDate = DateTime.UtcNow;

            await _villaNumbers.CreateAsync(villaNumber);
            await _villaNumbers.SaveAsync();
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
    [HttpPut("{villaNo:int}", Name = "UpdateVillaNumber")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int villaNo, [FromBody] UpdateVillaNumberDTO updateVillaNumber)
    {
        try
        {
            if (updateVillaNumber.VillaNo != villaNo)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = ["VillaNo cannot be updated"];
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            if (updateVillaNumber.VillaNo <= 0 || updateVillaNumber == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = ["Invalid Id or Villa Number"];
                return BadRequest(_response);
            }
            var villaNumber = await _villaNumbers.GetAsync(x => x.VillaNo == villaNo, false);

            if (villaNumber == null)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = ["Villa Number not found"];
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }
            bool hasMatchingVilla = await _villas.GetAsync(x => x.Id == updateVillaNumber.VillaID) != null;

            if (!hasMatchingVilla)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = [$"Villa ID {updateVillaNumber.VillaID} not found"];
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }


            villaNumber = _mapper.Map<VillaNumber>(updateVillaNumber);
            await _villaNumbers.UpdateAsync(villaNumber);
            await _villaNumbers.SaveAsync();
            _response.StatusCode = HttpStatusCode.NoContent;
            return NoContent();
        }
        catch (Exception error)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = [error.Message.ToString()];
            return _response;
        }
    }
    // Update parts of the existing Villa Number
    [HttpPatch("{villaNo:int}", Name = "VillaNumberPartial")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<APIResponse>> VillaNumberPartial(int villaNo, [FromBody] JsonPatchDocument<UpdateVillaNumberDTO> patchDoc)
    {

        try
        {
            // Retrieve the existing villa entity from the database (tracking disabled)
            var villaNumber = await _villaNumbers.GetAsync(x => x.VillaNo == villaNo, false);

            if (villaNumber == null)
            {
                _response.ErrorMessages = [$"Villa with the id {villaNo} not found"];
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }  // Return 404 if villa not found

            var villaNumber2 = _mapper.Map<UpdateVillaNumberDTO>(villaNumber);
            // Apply the patch directly to the entity instead of the DTO
            if (villaNo != villaNumber.VillaNo)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = ["VillaNo cannot be updated"];
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            patchDoc.ApplyTo(villaNumber2, ModelState);

            // Check if the patch operation resulted in a valid state
            if (!ModelState.IsValid)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = ["Invalid object"];
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            villaNumber = _mapper.Map<VillaNumber>(villaNumber2);

            bool hasMatchingVilla = await _villas.GetAsync(x => x.Id == villaNumber.VillaID) != null;
            if (!hasMatchingVilla)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = [$"Villa ID {villaNumber.VillaID} not found"];
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }

            // Update the entity in the database (tracked entity is already being monitored)
            await _villaNumbers.UpdateAsync(villaNumber);
            await _villaNumbers.SaveAsync();  // Save the changes to the database
            _response.StatusCode = HttpStatusCode.NoContent;
            return NoContent();  // Return 204 No Content on success
        }
        catch (Exception error)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = [error.Message.ToString()];
            return _response;
        }

    }
    // Deleting existing VillaNumber
    [HttpDelete("{villaNo:int}", Name = "DeleteVillaNumber")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int villaNo)
    {
        try
        {
            if (villaNo <= 0)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = ["Invalid ID"];
                return BadRequest(_response);
            }

            var villaNumber = await _villaNumbers.GetAsync(x => x.VillaNo == villaNo);
            if (villaNumber == null)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = ["Villa NotFound"];
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }
            else
            {
                await _villaNumbers.RemoveAsync(villaNumber);
                return NoContent();
            }
        }
        catch (Exception error)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = [error.Message.ToString()];
            return _response;
        }
    }
}