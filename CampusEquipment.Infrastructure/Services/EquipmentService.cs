using CampusEquipment.Core.DTOs;
using CampusEquipment.Core.Models.Database;
using CampusEquipment.Core.Repositories;
using CampusEquipment.Core.Services;

namespace CampusEquipment.Infrastructure.Services;

public class EquipmentService : IEquipmentService
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IDepartmentRepository _departmentRepository;

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
            query = query.Where(e =>
                e.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(e =>
                e.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
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

        await ValidateDepartment(dto.DepartmentId);

        var equipment = await _equipmentRepository.GetAll();
        if (equipment.Any(e =>
            e.AssetCode.Equals(dto.AssetCode, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Asset code already exists.");
        }

        var newEquipment = new Equipment
        {
            AssetCode = dto.AssetCode,
            Name = dto.Name,
            Category = dto.Category,
            Brand = dto.Brand,
            Model = dto.Model,
            PurchaseDate = dto.PurchaseDate,
            Status = dto.Status,
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

        await ValidateDepartment(dto.DepartmentId);

        var allEquipment = await _equipmentRepository.GetAll();
        if (allEquipment.Any(e =>
            e.EquipmentId != id &&
            e.AssetCode.Equals(dto.AssetCode, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Asset code already exists.");
        }

        if ((equipment.Status.Equals("Retired", StringComparison.OrdinalIgnoreCase) ||
             equipment.Status.Equals("UnderMaintenance", StringComparison.OrdinalIgnoreCase)) &&
            dto.Status.Equals("Assigned", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Retired or under maintenance equipment cannot be assigned.");
        }

        equipment.AssetCode = dto.AssetCode;
        equipment.Name = dto.Name;
        equipment.Category = dto.Category;
        equipment.Brand = dto.Brand;
        equipment.Model = dto.Model;
        equipment.PurchaseDate = dto.PurchaseDate;
        equipment.Status = dto.Status;
        equipment.DepartmentId = dto.DepartmentId;

        await _equipmentRepository.Update(equipment);
    }

    public async Task RetireEquipment(int id)
    {
        var equipment = await _equipmentRepository.GetById(id)
            ?? throw new KeyNotFoundException("Equipment not found.");

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
