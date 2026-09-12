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
    public class EquipmentRepository : IEquipmentRepository
    {
        private readonly CampusEquipmentDbContext _context;

        public EquipmentRepository(CampusEquipmentDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Equipment>> GetAll()
        {
            return await _context.Equipment
                .Include(e => e.Department)
                .ToListAsync();
        }

        public async Task<Equipment?> GetById(int id)
        {
            return await _context.Equipment
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.EquipmentId == id);
        }

        public async Task Add(Equipment equipment)
        {
            await _context.Equipment.AddAsync(equipment);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Equipment equipment)
        {
            _context.Equipment.Update(equipment);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Equipment equipment)
        {
            _context.Equipment.Remove(equipment);
            await _context.SaveChangesAsync();
        }
    }
}
