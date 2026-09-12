using CampusEquipment.Core.DTOs;

namespace CampusEquipment.Web.Models;

public class EquipmentIndexViewModel
{
    public IEnumerable<EquipmentDto> Equipment { get; set; } = [];
    public IEnumerable<DepartmentDto> Departments { get; set; } = [];
    public string? Search { get; set; }
    public string? Category { get; set; }
    public string? Status { get; set; }
    public int? DepartmentId { get; set; }
}
