using CampusEquipment.Core.DTOs;
using CampusEquipment.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEquipment.Infrastructure.Services
{
    public class EquipmentService : IEquipmentService
    {
        public bool CreateEquipment(CreateEquipmentDto dto, out string message)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EquipmentDto> GetAllEquipment()
        {
            throw new NotImplementedException();
        }

        public EquipmentDto? GetEquipmentById(int id)
        {
            throw new NotImplementedException();
        }

        public bool RetireEquipment(int id, out string message)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EquipmentDto> SearchEquipment(string? search, string? category, string? status, int? departmentId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateEquipment(int id, UpdateEquipmentDto dto, out string message)
        {
            throw new NotImplementedException();
        }
    }
}
