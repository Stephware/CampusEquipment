using CampusEquipment.Core.DTOs;
using CampusEquipment.Core.Repositories;
using CampusEquipment.Infrastructure.Data;
using CampusEquipment.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace CampusEquipment.Infrastructure.Repositories;

public class EquipmentRepository : IEquipmentRepository
{
    private readonly AppDbContext _context;

    public EquipmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EquipmentDto>> GetAll()
    {
        return await _context.Equipment
            .AsNoTracking()
            .Include(e => e.Department)
            .Select(e => new EquipmentDto
            {
                EquipmentId = e.EquipmentId,
                AssetCode = e.AssetCode,
                Name = e.Name,
                Category = e.Category,
                Brand = e.Brand,
                Model = e.Model,
                PurchaseDate = e.PurchaseDate,
                Status = e.Status,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department.Name
            })
            .ToListAsync();
    }

    public async Task<EquipmentDto?> GetById(int id)
    {
        return await _context.Equipment
            .AsNoTracking()
            .Include(e => e.Department)
            .Where(e => e.EquipmentId == id)
            .Select(e => new EquipmentDto
            {
                EquipmentId = e.EquipmentId,
                AssetCode = e.AssetCode,
                Name = e.Name,
                Category = e.Category,
                Brand = e.Brand,
                Model = e.Model,
                PurchaseDate = e.PurchaseDate,
                Status = e.Status,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department.Name
            })
            .FirstOrDefaultAsync();
    }

    public async Task Add(EquipmentDto equipment)
    {
        var entity = new Equipment
        {
            AssetCode = equipment.AssetCode,
            Name = equipment.Name,
            Category = equipment.Category,
            Brand = equipment.Brand,
            Model = equipment.Model,
            PurchaseDate = equipment.PurchaseDate,
            Status = equipment.Status,
            DepartmentId = equipment.DepartmentId
        };

        await _context.Equipment.AddAsync(entity);
        await _context.SaveChangesAsync();
        equipment.EquipmentId = entity.EquipmentId;
    }

    public async Task Update(EquipmentDto equipment)
    {
        var entity = await _context.Equipment.FindAsync(equipment.EquipmentId)
            ?? throw new KeyNotFoundException("Equipment not found.");

        entity.AssetCode = equipment.AssetCode;
        entity.Name = equipment.Name;
        entity.Category = equipment.Category;
        entity.Brand = equipment.Brand;
        entity.Model = equipment.Model;
        entity.PurchaseDate = equipment.PurchaseDate;
        entity.Status = equipment.Status;
        entity.DepartmentId = equipment.DepartmentId;

        await _context.SaveChangesAsync();
    }

    public async Task Delete(EquipmentDto equipment)
    {
        var entity = await _context.Equipment.FindAsync(equipment.EquipmentId);
        if (entity == null)
        {
            return;
        }

        _context.Equipment.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
