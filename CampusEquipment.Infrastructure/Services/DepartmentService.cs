using CampusEquipment.Core.DTOs;
using CampusEquipment.Core.Models.Database;
using CampusEquipment.Core.Repositories;
using CampusEquipment.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEquipment.Infrastructure.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService(
            IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        // GET ALL DEPARTMENTS
        public async Task<IEnumerable<DepartmentDto>> GetAllDepartments()
        {
            var departments = await _departmentRepository.GetAll();

            return departments.Select(MapToDto);
        }

        // GET DEPARTMENT BY ID
        public async Task<DepartmentDto?> GetDepartmentById(int id)
        {
            var department = await _departmentRepository.GetById(id);

            if (department == null)
                return null;

            return MapToDto(department);
        }

        // ENTITY → DTO
        private DepartmentDto MapToDto(Department department)
        {
            return new DepartmentDto
            {
                DepartmentId = department.DepartmentId,
                Name = department.Name,
                Description = department.Description
            };
        }
    }
}
