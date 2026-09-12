using CampusEquipment.API.Common;
using CampusEquipment.Core.DTOs;
using CampusEquipment.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusEquipment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipmentController : ControllerBase
{
    private readonly IEquipmentService _equipmentService;
    private readonly ILogger<EquipmentController> _logger;

    public EquipmentController(
        IEquipmentService equipmentService,
        ILogger<EquipmentController> logger)
    {
        _equipmentService = equipmentService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? category,
        [FromQuery] string? status,
        [FromQuery] int? departmentId)
    {
        var hasFilters =
            !string.IsNullOrWhiteSpace(search) ||
            !string.IsNullOrWhiteSpace(category) ||
            !string.IsNullOrWhiteSpace(status) ||
            departmentId.HasValue;

        var equipment = hasFilters
            ? await _equipmentService.SearchEquipment(search, category, status, departmentId)
            : await _equipmentService.GetAllEquipment();

        return Ok(ApiResponse<IEnumerable<EquipmentDto>>.SuccessResponse(
            equipment,
            "Equipment retrieved successfully."));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var equipment = await _equipmentService.GetEquipmentById(id);

        if (equipment == null)
        {
            _logger.LogWarning("Equipment with id {EquipmentId} was not found.", id);
            return NotFound(ApiResponse<object?>.FailResponse("Equipment not found."));
        }

        return Ok(ApiResponse<EquipmentDto>.SuccessResponse(
            equipment,
            "Equipment retrieved successfully."));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEquipmentDto dto)
    {
        try
        {
            await _equipmentService.CreateEquipment(dto);

            var createdEquipment = await FindByAssetCode(dto.AssetCode);
            _logger.LogInformation("Equipment {AssetCode} was created.", dto.AssetCode);

            if (createdEquipment == null)
            {
                return StatusCode(
                    StatusCodes.Status201Created,
                    ApiResponse<object?>.SuccessResponse(null, "Equipment created successfully."));
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdEquipment.EquipmentId },
                ApiResponse<EquipmentDto>.SuccessResponse(
                    createdEquipment,
                    "Equipment created successfully."));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Equipment creation failed validation.");
            return BadRequest(ApiResponse<object?>.FailResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Equipment creation failed business validation.");
            return ToBusinessErrorResponse(ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating equipment.");
            return ToUnexpectedErrorResponse();
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEquipmentDto dto)
    {
        try
        {
            await _equipmentService.UpdateEquipment(id, dto);

            _logger.LogInformation("Equipment with id {EquipmentId} was updated.", id);
            return Ok(ApiResponse<object?>.SuccessResponse(
                null,
                "Equipment updated successfully."));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Equipment with id {EquipmentId} was not found.", id);
            return NotFound(ApiResponse<object?>.FailResponse(ex.Message));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Equipment update failed validation for id {EquipmentId}.", id);
            return BadRequest(ApiResponse<object?>.FailResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Equipment update failed business validation for id {EquipmentId}.", id);
            return ToBusinessErrorResponse(ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while updating equipment with id {EquipmentId}.", id);
            return ToUnexpectedErrorResponse();
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Retire(int id)
    {
        try
        {
            await _equipmentService.RetireEquipment(id);

            _logger.LogInformation("Equipment with id {EquipmentId} was retired.", id);
            return Ok(ApiResponse<object?>.SuccessResponse(
                null,
                "Equipment retired successfully."));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Equipment with id {EquipmentId} was not found.", id);
            return NotFound(ApiResponse<object?>.FailResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retiring equipment with id {EquipmentId}.", id);
            return ToUnexpectedErrorResponse();
        }
    }

    private async Task<EquipmentDto?> FindByAssetCode(string assetCode)
    {
        var equipment = await _equipmentService.SearchEquipment(
            assetCode,
            category: null,
            status: null,
            departmentId: null);

        return equipment.FirstOrDefault(item =>
            item.AssetCode.Equals(assetCode, StringComparison.OrdinalIgnoreCase));
    }

    private static IActionResult ToBusinessErrorResponse(InvalidOperationException ex)
    {
        var response = ApiResponse<object?>.FailResponse(ex.Message);

        if (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase) ||
            ex.Message.Contains("cannot be assigned", StringComparison.OrdinalIgnoreCase))
        {
            return new ConflictObjectResult(response);
        }

        return new BadRequestObjectResult(response);
    }

    private static IActionResult ToUnexpectedErrorResponse()
    {
        return new ObjectResult(ApiResponse<object?>.FailResponse(
            "An unexpected error occurred while processing the request."))
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }
}
