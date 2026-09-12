using CampusEquipment.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusEquipment.Web.Controllers;

public class EquipmentController : Controller
{
    private readonly IEquipmentService _equipmentService;

    public EquipmentController(IEquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
    }

    public async Task<IActionResult> Index()
    {
        var equipment = await _equipmentService.GetAllEquipment();
        return View(equipment);
    }
}
