using CampusEquipment.Core.DTOs;
using CampusEquipment.Core.Models.Database;
using CampusEquipment.Core.Repositories;
using CampusEquipment.Core.Services;

namespace CampusEquipment.Infrastructure.Services;

public class EquipmentService : IEquipmentService
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IDepartmentRepository _departmentRepository;

    private static readonly string[] ValidStatuses =
    {
        "Available",
        "Assigned",
        "UnderMaintenance",
        "Retired"
    };

    public EquipmentService(
        IEquipmentRepository equipmentRepository,
        IDepartmentRepository departmentRepository)
    {
        _equipmentRepository = equipmentRepository;
        _departmentRepository = departmentRepository;
    }

    public async Task<IEnumerable<EquipmentDto>> GetAllEquipment()
    {
        var equipment = await _equipmentRepository.GetAll();
        return equipment.Select(ToDto).ToList();
    }

    public async Task<EquipmentDto?> GetEquipmentById(int id)
    {
        var equipment = await _equipmentRepository.GetById(id);
        return equipment == null ? null : ToDto(equipment);
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

        return query.Select(ToDto).ToList();
    }

    public async Task CreateEquipment(CreateEquipmentDto dto)
    {
        ValidateRequiredFields(
            dto.AssetCode,
            dto.Name,
            dto.Category,
            dto.Status,
            dto.DepartmentId);

        var status = NormalizeAndValidateStatus(dto.Status);
        await ValidateDepartment(dto.DepartmentId);

        var assetCode = dto.AssetCode.Trim();
        var equipment = await _equipmentRepository.GetAll();

        if (equipment.Any(e =>
            e.AssetCode.Equals(assetCode, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Asset code already exists.");
        }

        var newEquipment = new Equipment
        {
            AssetCode = assetCode,
            Name = dto.Name.Trim(),
            Category = dto.Category.Trim(),
            Brand = CleanOptionalText(dto.Brand),
            Model = CleanOptionalText(dto.Model),
            PurchaseDate = dto.PurchaseDate,
            Status = status,
            DepartmentId = dto.DepartmentId
        };

        await _equipmentRepository.Add(newEquipment);
    }

    public async Task UpdateEquipment(int id, UpdateEquipmentDto dto)
    {
        ValidateRequiredFields(
            dto.AssetCode,
            dto.Name,
            dto.Category,
            dto.Status,
            dto.DepartmentId);

        var equipment = await _equipmentRepository.GetById(id)
            ?? throw new KeyNotFoundException("Equipment not found.");

        var newStatus = NormalizeAndValidateStatus(dto.Status);
        await ValidateDepartment(dto.DepartmentId);

        var assetCode = dto.AssetCode.Trim();
        var allEquipment = await _equipmentRepository.GetAll();

        if (allEquipment.Any(e =>
            e.EquipmentId != id &&
            e.AssetCode.Equals(assetCode, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Asset code already exists.");
        }

        ValidateAssignmentRule(equipment.Status, newStatus);

        equipment.AssetCode = assetCode;
        equipment.Name = dto.Name.Trim();
        equipment.Category = dto.Category.Trim();
        equipment.Brand = CleanOptionalText(dto.Brand);
        equipment.Model = CleanOptionalText(dto.Model);
        equipment.PurchaseDate = dto.PurchaseDate;
        equipment.Status = newStatus;
        equipment.DepartmentId = dto.DepartmentId;

        await _equipmentRepository.Update(equipment);
    }

    public async Task RetireEquipment(int id)
    {
        var equipment = await _equipmentRepository.GetById(id)
            ?? throw new KeyNotFoundException("Equipment not found.");

        if (equipment.Status.Equals("Retired", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        equipment.Status = "Retired";
        await _equipmentRepository.Update(equipment);
    }

    private async Task ValidateDepartment(int departmentId)
    {
        var department = await _departmentRepository.GetById(departmentId);

        if (department == null)
        {
            throw new InvalidOperationException("Department does not exist.");
        }
    }

    private static void ValidateAssignmentRule(string currentStatus, string newStatus)
    {
        if (!newStatus.Equals("Assigned", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (currentStatus.Equals("Retired", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Retired equipment cannot be assigned.");
        }

        if (currentStatus.Equals("UnderMaintenance", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Equipment under maintenance cannot be assigned.");
        }
    }

    private static void ValidateRequiredFields(
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
            throw new ArgumentException("Required equipment fields are invalid.");
        }
    }

    private static string NormalizeAndValidateStatus(string status)
    {
        var normalizedStatus = status.Trim();

        var validStatus = ValidStatuses.FirstOrDefault(value =>
            value.Equals(normalizedStatus, StringComparison.OrdinalIgnoreCase));

        if (validStatus == null)
        {
            throw new ArgumentException("Status is invalid.");
        }

        return validStatus;
    }

    private static string? CleanOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static EquipmentDto ToDto(Equipment equipment)
    {
        return new EquipmentDto
        {
            EquipmentId = equipment.EquipmentId,
            AssetCode = equipment.AssetCode,
            Name = equipment.Name,
            Category = equipment.Category,
            Brand = equipment.Brand,
            Model = equipment.Model,
            PurchaseDate = equipment.PurchaseDate,
            Status = equipment.Status,
            DepartmentId = equipment.DepartmentId,
            DepartmentName = equipment.Department?.Name ?? "Unknown"
        };
    }
}
