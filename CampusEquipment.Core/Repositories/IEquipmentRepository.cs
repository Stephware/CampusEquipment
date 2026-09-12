using CampusEquipment.Core.DTOs;

namespace CampusEquipment.Core.Repositories;

public interface IEquipmentRepository
{
    Task<IEnumerable<EquipmentDto>> GetAll();
    Task<EquipmentDto?> GetById(int id);
    Task Add(EquipmentDto equipment);
    Task Update(EquipmentDto equipment);
    Task Delete(EquipmentDto equipment);
}
