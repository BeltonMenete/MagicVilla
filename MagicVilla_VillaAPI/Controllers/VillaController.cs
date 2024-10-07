using System.Net;
using AutoMapper;
using magicvilla_villaapi.Models;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTOs;
using MagicVilla_VillaAPI.Models.DTOs.VillaDTOs;
using MagicVilla_VillaAPI.Repositories.IRepositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers;

[ApiController]
[Route("api/villa")]
public class VillaController(IVillaRepository villas, IMapper mapper) : ControllerBase
{

    protected APIResponse _response = new();
    private readonly IVillaRepository _villas = villas;
    private readonly IMapper _mapper = mapper;


    // Get All Villas
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse>> GetVillas()
    {
        try
        {
            var villaList = await _villas.GetAllAsync();
            _response.Result = _mapper.Map<List<VillaDTO>>(villaList);
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
        catch (Exception error)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = [error.Message.ToString()];
            return _response;
        }
    }

    // Get a Specific Villa by ID
    [HttpGet("{id:int}", Name = "GetVilla")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<APIResponse>> GetVilla(int id)
    {
        try
        {
            // Validations
            if (id <= 0)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = ["Invalid Id"];
                return BadRequest(_response);
            }
            // retrieve
            var villa = await _villas.GetAsync(x => x.Id == id);

            if (villa == null)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = ["Not found"];
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }
            _response.Result = _mapper.Map<VillaDTO>(villa);
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
        catch (Exception Error)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = [Error.Message.ToString()];
            return _response;
        }
    }
    // Add New Villa To DataBase

    [HttpPost(Name = "CreateVilla")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] CreateVillaDTO createDTO)
    {
        try
        {
            // Validations
            if (createDTO == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            //Mapping
            var villa = _mapper.Map<Villa>(createDTO);

            await _villas.CreateAsync(villa);
            await _villas.SaveAsync();
            _response.Result = villa;
            _response.StatusCode = HttpStatusCode.Created;
            return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);
        }
        catch (Exception error)
        {
            _response.ErrorMessages = [error.Message.ToString()];
            _response.IsSuccess = false;
            return _response;
        }

    }

    // Update specific villa all prop by prop (PUT)
    [HttpPut("{id:int}", Name = "UpdateVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] UpdateVillaDTO updateDTO)
    {
        try
        {
            if (id != updateDTO.Id || id <= 0 || updateDTO == null)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = ["Invalid Object or Id"];
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            var villa = await _villas.GetAsync(x => x.Id == id, false);

            if (villa == null)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = ["Villa not found"];
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }

            villa = _mapper.Map<Villa>(updateDTO);

            await _villas.UpdateAsync(villa);
            await _villas.SaveAsync();
            _response.Result = villa;
            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSuccess = true;
            return NoContent();
        }
        catch (System.Exception error)
        {

            _response.IsSuccess = false;
            _response.ErrorMessages = [error.Message.ToString()];
            return _response;
        }

    }

    // Update Partially a Villa
    [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<APIResponse>> UpdatePartialVilla(int id, JsonPatchDocument<UpdateVillaDTO> patchDTO)
    {
        try
        {
            // Retrieve the existing villa entity from the database (tracking disabled)
            var villa = await _villas.GetAsync(x => x.Id == id, false);

            if (villa == null)
            {
                _response.ErrorMessages = [$"Villa with the id {id} not found"];
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }  // Return 404 if villa not found

            var villa2 = _mapper.Map<UpdateVillaDTO>(villa);
            // Apply the patch directly to the entity instead of the DTO
            patchDTO.ApplyTo(villa2, ModelState);

            // Check if the patch operation resulted in a valid state
            if (!ModelState.IsValid)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = ["Invalid object"];
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }


            villa = _mapper.Map<Villa>(villa2);

            // Update the entity in the database (tracked entity is already being monitored)
            await _villas.UpdateAsync(villa);
            await _villas.SaveAsync();  // Save the changes to the database
            _response.StatusCode = HttpStatusCode.NoContent;
            return NoContent();  // Return 204 No Content on success
        }
        catch (System.Exception error)
        {

            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSuccess = false;
            _response.ErrorMessages = [error.Message.ToString()];
            return _response;  // Return 204 No Content on success
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<APIResponse>> DeleteVilla(int? id)
    {
        try
        {
            if (id <= 0 || id == null)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = ["Invalid Id"];
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            var villa = await _villas.GetAsync(x => x.Id == id);

            if (villa == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages = ["Villa not found"];
                return NotFound(_response);
            }

            await _villas.RemoveAsync(villa);
            await _villas.SaveAsync();
            _response.StatusCode = HttpStatusCode.NoContent;
            return NoContent();
        }
        catch (System.Exception error)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = [error.Message.ToString()];
            return _response;
        }
    }
}
