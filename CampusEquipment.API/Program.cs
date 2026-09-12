using CampusEquipment.API.Common;
using CampusEquipment.Core.Models.Database;
using CampusEquipment.Core.Repositories;
using CampusEquipment.Core.Services;
using CampusEquipment.Infrastructure.Repositories;
using CampusEquipment.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState.Values
                .SelectMany(value => value.Errors)
                .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                    ? "Invalid request value."
                    : error.ErrorMessage)
                .ToList();

            return new BadRequestObjectResult(
                ApiResponse<object?>.FailResponse("Invalid request data.", errors));
        };
    });

builder.Services.AddOpenApi();
builder.Services.AddDbContext<CampusEquipmentDbContext>();

builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
