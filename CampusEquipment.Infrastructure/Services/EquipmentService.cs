using CampusEquipment.Core.DTOs;
using CampusEquipment.Core.Repositories;
using CampusEquipment.Core.Services;
using Microsoft.Extensions.Logging;

namespace CampusEquipment.Infrastructure.Services;

public class EquipmentService : IEquipmentService
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogger<EquipmentService> _logger;

    private static readonly string[] ValidStatuses =
    {
        "Available",
        "Assigned",
        "UnderMaintenance",
        "Retired"
    };

    public EquipmentService(
        IEquipmentRepository equipmentRepository,
        IDepartmentRepository departmentRepository,
        ILogger<EquipmentService> logger)
    {
        _equipmentRepository = equipmentRepository;
        _departmentRepository = departmentRepository;
        _logger = logger;
    }

    public Task<IEnumerable<EquipmentDto>> GetAllEquipment()
    {
        return _equipmentRepository.GetAll();
    }

    public async Task<EquipmentDto?> GetEquipmentById(int id)
    {
        var equipment = await _equipmentRepository.GetById(id);

        if (equipment == null)
        {
            _logger.LogWarning("Equipment with id {EquipmentId} was not found.", id);
        }

        return equipment;
    }

    public async Task<IEnumerable<EquipmentDto>> SearchEquipment(
        string? search,
        string? category,
        string? status,
        int? departmentId)
    {
        var equipment = await _equipmentRepository.GetAll();
        var query = equipment.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(e =>
                e.AssetCode.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                e.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                (e.Brand != null && e.Brand.Contains(term, StringComparison.OrdinalIgnoreCase)));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            var categoryFilter = category.Trim();
            query = query.Where(e =>
                e.Category.Equals(categoryFilter, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var statusFilter = status.Trim();
            query = query.Where(e =>
                e.Status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase));
        }

        if (departmentId.HasValue)
        {
            query = query.Where(e => e.DepartmentId == departmentId.Value);
        }

        return query.ToList();
    }

    public async Task CreateEquipment(CreateEquipmentDto dto)
    {
        ValidateRequiredFields(dto.AssetCode, dto.Name, dto.Category, dto.Status, dto.DepartmentId);

        var status = NormalizeAndValidateStatus(dto.Status);
        await ValidateDepartment(dto.DepartmentId);

        var assetCode = dto.AssetCode.Trim();
        var equipment = await _equipmentRepository.GetAll();

        if (equipment.Any(e => e.AssetCode.Equals(assetCode, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Equipment creation failed because asset code {AssetCode} already exists.", assetCode);
            throw new InvalidOperationException("Asset code already exists.");
        }

        await _equipmentRepository.Add(new EquipmentDto
        {
            AssetCode = assetCode,
            Name = dto.Name.Trim(),
            Category = dto.Category.Trim(),
            Brand = CleanOptionalText(dto.Brand),
            Model = CleanOptionalText(dto.Model),
            PurchaseDate = dto.PurchaseDate,
            Status = status,
            DepartmentId = dto.DepartmentId
        });

        _logger.LogInformation("Equipment {AssetCode} was created.", assetCode);
    }

    public async Task UpdateEquipment(int id, UpdateEquipmentDto dto)
    {
        ValidateRequiredFields(dto.AssetCode, dto.Name, dto.Category, dto.Status, dto.DepartmentId);

        var equipment = await _equipmentRepository.GetById(id);
        if (equipment == null)
        {
            _logger.LogWarning("Equipment with id {EquipmentId} was not found for update.", id);
            throw new KeyNotFoundException("Equipment not found.");
        }

        var newStatus = NormalizeAndValidateStatus(dto.Status);
        await ValidateDepartment(dto.DepartmentId);

        var assetCode = dto.AssetCode.Trim();
        var allEquipment = await _equipmentRepository.GetAll();

        if (allEquipment.Any(e =>
            e.EquipmentId != id &&
            e.AssetCode.Equals(assetCode, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning(
                "Equipment update failed for id {EquipmentId} because asset code {AssetCode} already exists.",
                id,
                assetCode);
            throw new InvalidOperationException("Asset code already exists.");
        }

        ValidateAssignmentRule(equipment.Status, newStatus, id);

        equipment.AssetCode = assetCode;
        equipment.Name = dto.Name.Trim();
        equipment.Category = dto.Category.Trim();
        equipment.Brand = CleanOptionalText(dto.Brand);
        equipment.Model = CleanOptionalText(dto.Model);
        equipment.PurchaseDate = dto.PurchaseDate;
        equipment.Status = newStatus;
        equipment.DepartmentId = dto.DepartmentId;

        await _equipmentRepository.Update(equipment);
        _logger.LogInformation("Equipment with id {EquipmentId} was updated.", id);
    }

    public async Task RetireEquipment(int id)
    {
        var equipment = await _equipmentRepository.GetById(id);
        if (equipment == null)
        {
            _logger.LogWarning("Equipment with id {EquipmentId} was not found for retirement.", id);
            throw new KeyNotFoundException("Equipment not found.");
        }

        if (equipment.Status.Equals("Retired", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        equipment.Status = "Retired";
        await _equipmentRepository.Update(equipment);
        _logger.LogInformation("Equipment with id {EquipmentId} was retired.", id);
    }

    private async Task ValidateDepartment(int departmentId)
    {
        var department = await _departmentRepository.GetById(departmentId);
        if (department == null)
        {
            _logger.LogWarning("Equipment validation failed because department {DepartmentId} does not exist.", departmentId);
            throw new InvalidOperationException("Department does not exist.");
        }
    }

    private void ValidateAssignmentRule(string currentStatus, string newStatus, int equipmentId)
    {
        if (!newStatus.Equals("Assigned", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (currentStatus.Equals("Retired", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Retired equipment with id {EquipmentId} cannot be assigned.", equipmentId);
            throw new InvalidOperationException("Retired equipment cannot be assigned.");
        }

        if (currentStatus.Equals("UnderMaintenance", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Equipment under maintenance with id {EquipmentId} cannot be assigned.", equipmentId);
            throw new InvalidOperationException("Equipment under maintenance cannot be assigned.");
        }
    }

    private void ValidateRequiredFields(
        string assetCode,
        string name,
        string category,
        string status,
        int departmentId)
    {
        if (string.IsNullOrWhiteSpace(assetCode) ||
            string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(category) ||
            string.IsNullOrWhiteSpace(status) ||
            departmentId <= 0)
        {
            _logger.LogWarning("Equipment validation failed because one or more required fields are invalid.");
            throw new ArgumentException("Required equipment fields are invalid.");
        }
    }

    private string NormalizeAndValidateStatus(string status)
    {
        var normalizedStatus = status.Trim();
        var validStatus = ValidStatuses.FirstOrDefault(value =>
            value.Equals(normalizedStatus, StringComparison.OrdinalIgnoreCase));

        if (validStatus == null)
        {
            _logger.LogWarning("Equipment validation failed because status {Status} is invalid.", normalizedStatus);
            throw new ArgumentException("Status is invalid.");
        }

        return validStatus;
    }

    private static string? CleanOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
