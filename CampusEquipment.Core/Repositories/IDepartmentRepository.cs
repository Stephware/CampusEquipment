using CampusEquipment.Core.DTOs;

namespace CampusEquipment.Core.Repositories;

public interface IDepartmentRepository
{
    Task<IEnumerable<DepartmentDto>> GetAll();
    Task<DepartmentDto?> GetById(int id);
}
