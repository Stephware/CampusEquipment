using CampusEquipment.Core.Models.Database;
using CampusEquipment.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEquipment.Infrastructure.Repositories
{
    internal class DepartmentRepository : IDepartmentRepository
    {
        private readonly CampusEquipmentDbContext _context;

        public DepartmentRepository(CampusEquipmentDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Department>> GetAll()
        {
            return await _context.Department.ToListAsync();
        }

        public async Task<Department?> GetById(int id)
        {
            return await _context.Department
                .FirstOrDefaultAsync(d => d.DepartmentId == id);
        }
    }
}
