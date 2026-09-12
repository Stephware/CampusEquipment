using CampusEquipment.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEquipment.Core.Services
{
    public interface IDepartmentService
    {
        IEnumerable<DepartmentDto> GetAllDepartments();

        DepartmentDto? GetDepartmentById(int id);
    }
}
