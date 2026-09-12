using CampusEquipment.Core.DTOs;
using CampusEquipment.Core.Repositories;
using CampusEquipment.Core.Services;

namespace CampusEquipment.Infrastructure.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;

    public DepartmentService(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public Task<IEnumerable<DepartmentDto>> GetAllDepartments()
    {
        return _departmentRepository.GetAll();
    }

    public Task<DepartmentDto?> GetDepartmentById(int id)
    {
        return _departmentRepository.GetById(id);
    }
}
