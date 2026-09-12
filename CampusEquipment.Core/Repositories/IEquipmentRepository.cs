using CampusEquipment.Core.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEquipment.Core.Repositories
{
    public interface IEquipmentRepository
    {
        Task<IEnumerable<Equipment>> GetAll();

        Task<Equipment?> GetById(int id);

        Task Add(Equipment equipment);

        Task Update(Equipment equipment);

        Task Delete(Equipment equipment);
    }
}
