using CampusEquipment.Core.Services;
using CampusEquipment.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace CampusEquipment.Web.Controllers;

public class EquipmentController : Controller
{
    private readonly IEquipmentService _equipmentService;
    private readonly IDepartmentService _departmentService;

    public EquipmentController(
        IEquipmentService equipmentService,
        IDepartmentService departmentService)
    {
        _equipmentService = equipmentService;
        _departmentService = departmentService;
    }

    public async Task<IActionResult> Index(
        string? search,
        string? category,
        string? status,
        int? departmentId)
    {
        var equipment = await _equipmentService.SearchEquipment(
            search,
            category,
            status,
            departmentId);

        var departments = await _departmentService.GetAllDepartments();

        var model = new EquipmentIndexViewModel
        {
            Equipment = equipment,
            Departments = departments,
            Search = search,
            Category = category,
            Status = status,
            DepartmentId = departmentId
        };

        return View(model);
    }
}
