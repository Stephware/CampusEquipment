using CampusEquipment.Core.DTOs;
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

    public async Task<IActionResult> Create()
    {
        await LoadDepartments();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEquipmentDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadDepartments();
            return View(dto);
        }

        try
        {
            await _equipmentService.CreateEquipment(dto);
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        await LoadDepartments();
        return View(dto);
    }

    public async Task<IActionResult> Details(int id)
    {
        var equipment = await _equipmentService.GetEquipmentById(id);

        if (equipment == null)
        {
            return NotFound();
        }

        return View(equipment);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var equipment = await _equipmentService.GetEquipmentById(id);

        if (equipment == null)
        {
            return NotFound();
        }

        await LoadDepartments();

        var model = new UpdateEquipmentDto
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

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateEquipmentDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadDepartments();
            return View(dto);
        }

        try
        {
            await _equipmentService.UpdateEquipment(id, dto);
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        await LoadDepartments();
        return View(dto);
    }

    public async Task<IActionResult> Retire(int id)
    {
        var equipment = await _equipmentService.GetEquipmentById(id);

        if (equipment == null)
        {
            return NotFound();
        }

        return View(equipment);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Retire")]
    public async Task<IActionResult> RetireConfirmed(int id)
    {
        try
        {
            await _equipmentService.RetireEquipment(id);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDepartments()
    {
        ViewBag.Departments = await _departmentService.GetAllDepartments();
    }
}
