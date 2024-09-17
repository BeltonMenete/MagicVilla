using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.DTOs;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repositories.IRepositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace MagicVilla_VillaAPI.Controllers;

[ApiController]
[Route("api/villa")]
public class VillaController(ILogger<VillaController> logger, IVillaRepository dbVilla, IMapper mapper) : ControllerBase
{
    private readonly ILogger<VillaController> _logger = logger;
    private readonly IVillaRepository _dbVilla = dbVilla;
    private readonly IMapper _mapper = mapper;

    // Get All Villas
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<VillaDTO>> GetVillas()
    {
        _logger.LogInformation("____ Getting All Vilas _____");
        var villaList = await _dbVilla.GetAllAsync();

        return Ok(_mapper.Map<List<VillaDTO>>(villaList));
    }

    // Get a Specific Villa by ID
    [HttpGet("{id:int}", Name = "GetVilla")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VillaDTO>> GetVilla(int id)
    {
        // Validations
        if (id <= 0)
        {
            _logger.LogError($"-----  Villa: {id} BadRequest --------");
            return BadRequest();
        }

        // retrieve
        var villa = await _dbVilla.GetAsync(x => x.Id == id);

        if (villa == null)
        {
            _logger.LogWarning($"----- c Villa{id} not found --------");
            ModelState.AddModelError("ERROR: ", $"Villa with Id: {id} not found");
            return NotFound(ModelState);
        }

        _logger.LogInformation($"----- Getting specific Villa {id} --------");
        return Ok(_mapper.Map<VillaDTO>(villa));

    }
    // Add New Villa To DataBase

    [HttpPost(Name = "CreateVilla")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Villa>> CreateVilla([FromBody] CreateVillaDTO createDTO)
    {
        // Validations
        if (createDTO == null)
            return BadRequest();

        //Mapping
        var model = _mapper.Map<Villa>(createDTO);

        await _dbVilla.CreateAsync(model);
        await _dbVilla.SaveAsync();
        return CreatedAtRoute("GetVilla", new { id = model.Id }, model);
    }

    // Update specific villa all prop by prop (PUT)
    [HttpPut("{id:int}", Name = "UpdateVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateVilla(int id, [FromBody] UpdateVillaDTO updateDTO)
    {
        if (id <= 0)
            return BadRequest();

        if (id != updateDTO.Id || updateDTO == null)
            return BadRequest();

        var villa = await _dbVilla.GetAsync(x => x.Id == id);

        if (villa == null)
        {
            ModelState.AddModelError("Error: ", "Villa not found!");
            return NotFound();
        }

        villa = _mapper.Map<Villa>(updateDTO);
        await _dbVilla.UpdateAsync(villa);
        await _dbVilla.SaveAsync();
        return NoContent();

    }

    // Update Partially a Villa
    [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdatePartialVilla(int id, JsonPatchDocument<UpdateVillaDTO> patchDTO)
    {
        // Retrieve the existing villa entity from the database (tracking enabled)
        var villa = await _dbVilla.GetAsync(x => x.Id == id, true);

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

        return NoContent();  // Return 204 No Content on success

    }
    const string projects = "fullOfNothing";
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteVilla(int? id)
    {
        if (id <= 0 || id == null)
            return BadRequest();

        var villa = await _dbVilla.GetAsync(x => x.Id == id);

        if (villa == null)
            return NotFound();

        await _dbVilla.RemoveAsync(villa);
        await _dbVilla.SaveAsync();
        return NoContent();
    }

}
