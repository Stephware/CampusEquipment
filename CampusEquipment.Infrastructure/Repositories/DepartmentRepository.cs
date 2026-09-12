using CampusEquipment.Core.DTOs;
using CampusEquipment.Core.Repositories;
using CampusEquipment.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusEquipment.Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly AppDbContext _context;

    public DepartmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DepartmentDto>> GetAll()
    {
        return await _context.Departments
            .AsNoTracking()
            .Select(d => new DepartmentDto
            {
                DepartmentId = d.DepartmentId,
                Name = d.Name,
                Description = d.Description
            })
            .ToListAsync();
    }

    public async Task<DepartmentDto?> GetById(int id)
    {
        return await _context.Departments
            .AsNoTracking()
            .Where(d => d.DepartmentId == id)
            .Select(d => new DepartmentDto
            {
                DepartmentId = d.DepartmentId,
                Name = d.Name,
                Description = d.Description
            })
            .FirstOrDefaultAsync();
    }
}
