using System;
using System.Collections.Generic;

namespace CampusEquipment.Core.Models.Database;

public partial class Equipment
{
    public int EquipmentId { get; set; }

    public string AssetCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Category { get; set; } = null!;

    public string? Brand { get; set; }

    public string? Model { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    public string Status { get; set; } = null!;

    public int DepartmentId { get; set; }

    public virtual Department Department { get; set; } = null!;
}
