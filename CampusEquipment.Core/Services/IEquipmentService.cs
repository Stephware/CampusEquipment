using CampusEquipment.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEquipment.Core.Services
{
    public interface IEquipmentService
    {
        IEnumerable<EquipmentDto> GetAllEquipment();

        EquipmentDto? GetEquipmentById(int id);

        IEnumerable<EquipmentDto> SearchEquipment(
            string? search,
            string? category,
            string? status,
            int? departmentId);

        bool CreateEquipment(CreateEquipmentDto dto, out string message);

        bool UpdateEquipment(int id, UpdateEquipmentDto dto, out string message);

        bool RetireEquipment(int id, out string message);
    }
}
