using CampusEquipment.Core.DTOs;

namespace CampusEquipment.Core.Services;

public interface IEquipmentService
{
    Task<IEnumerable<EquipmentDto>> GetAllEquipment();
    Task<EquipmentDto?> GetEquipmentById(int id);
    Task<IEnumerable<EquipmentDto>> SearchEquipment(
        string? search,
        string? category,
        string? status,
        int? departmentId);
    Task CreateEquipment(CreateEquipmentDto dto);
    Task UpdateEquipment(int id, UpdateEquipmentDto dto);
    Task RetireEquipment(int id);
}
