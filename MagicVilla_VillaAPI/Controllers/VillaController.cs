using System.Net;
using AutoMapper;
using magicvilla_villaapi.Models;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTOs;
using MagicVilla_VillaAPI.Repositories.IRepositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers;

[ApiController]
[Route("api/villa")]
public class VillaController(IVillaRepository dbVilla, IMapper mapper) : ControllerBase
{

    protected ApiResponse _response = new();
    private readonly IVillaRepository _dbVilla = dbVilla;
    private readonly IMapper _mapper = mapper;


    // Get All Villas
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetVillas()
    {
        try
        {
            var villaList = await _dbVilla.GetAllAsync();
            _response.Result = _mapper.Map<List<VillaDTO>>(villaList);
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            return _response;
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
    public async Task<ActionResult<ApiResponse>> GetVilla(int id)
    {
        try
        {
            // Validations
            if (id <= 0)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return _response;
            }
            // retrieve
            var villa = await _dbVilla.GetAsync(x => x.Id == id);

            if (villa == null)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = ["Not found"];
                _response.StatusCode = HttpStatusCode.NotFound;
                return _response;
            }
            _response.Result = _mapper.Map<VillaDTO>(villa);
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            return _response;
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
    public async Task<ActionResult<ApiResponse>> CreateVilla([FromBody] CreateVillaDTO createDTO)
    {
        // Validations
        if (createDTO == null)
            return BadRequest();

        //Mapping
        var villa = _mapper.Map<Villa>(createDTO);

        await _dbVilla.CreateAsync(villa);
        _response.Result = villa;
        _response.StatusCode = HttpStatusCode.Created;
        _response.IsSuccess = true;
        await _dbVilla.SaveAsync();
        return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);
    }

    // Update specific villa all prop by prop (PUT)
    [HttpPut("{id:int}", Name = "UpdateVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> UpdateVilla(int id, [FromBody] UpdateVillaDTO updateDTO)
    {
        if (id <= 0)
            return BadRequest();

        if (id != updateDTO.Id || updateDTO == null)
            return BadRequest();

        var villa = await _dbVilla.GetAsync(x => x.Id == id, false);

        if (villa == null)
        {
            ModelState.AddModelError("Error: ", "Villa not found!");
            return NotFound();
        }

        villa = _mapper.Map<Villa>(updateDTO);

        await _dbVilla.UpdateAsync(villa);
        await _dbVilla.SaveAsync();
        _response.Result = villa;
        _response.StatusCode = HttpStatusCode.NoContent;
        _response.IsSuccess = true;
        //return Ok(_response);
        return _response;

    }

    // Update Partially a Villa
    [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> UpdatePartialVilla(int id, JsonPatchDocument<UpdateVillaDTO> patchDTO)
    {
        try
        {
            // Retrieve the existing villa entity from the database (tracking enabled)
            var villa = await _dbVilla.GetAsync(x => x.Id == id, false);

            if (villa == null)
                return NotFound();  // Return 404 if villa not found

            var villa2 = _mapper.Map<UpdateVillaDTO>(villa);
            // Apply the patch directly to the entity instead of the DTO
            patchDTO.ApplyTo(villa2, ModelState);

            // Check if the patch operation resulted in a valid state
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            villa = _mapper.Map<Villa>(villa2);

            // Update the entity in the database (tracked entity is already being monitored)
            await _dbVilla.UpdateAsync(villa);
            await _dbVilla.SaveAsync();  // Save the changes to the database
            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSuccess = true;
            return _response;  // Return 204 No Content on success
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
    public async Task<ActionResult<ApiResponse>> DeleteVilla(int? id)
    {
        try
        {
            if (id <= 0 || id == null)
                return BadRequest();

            var villa = await _dbVilla.GetAsync(x => x.Id == id);

            if (villa == null)
                return NotFound();

            await _dbVilla.RemoveAsync(villa);
            await _dbVilla.SaveAsync();
            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSuccess = true;
            return _response;
        }
        catch (System.Exception error)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = [error.Message.ToString()];
            return _response;
        }
    }
}
