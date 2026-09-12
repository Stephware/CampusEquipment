using System.ComponentModel.DataAnnotations;

namespace CampusEquipment.Core.DTOs;

public class UpdateEquipmentDto
{
    [Required, StringLength(50, MinimumLength = 2)]
    public string AssetCode { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 2)]
    public string Category { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Brand { get; set; }

    [StringLength(100)]
    public string? Model { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    [Required, StringLength(50, MinimumLength = 2)]
    public string Status { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "DepartmentId must be greater than 0")]
    public int DepartmentId { get; set; }
}
