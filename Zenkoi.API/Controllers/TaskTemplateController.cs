using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.TaskTemplateDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TaskTemplateController : BaseAPIController
{
    private readonly ITaskTemplateService _taskTemplateService;

    public TaskTemplateController(ITaskTemplateService taskTemplateService)
    {
        _taskTemplateService = taskTemplateService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTaskTemplates(
        [FromQuery] TaskTemplateFilterRequestDTO? filter,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _taskTemplateService.GetAllTaskTemplatesAsync(
                filter ?? new TaskTemplateFilterRequestDTO(),
                pageIndex,
                pageSize);
            return GetPagedSuccess(result);
        }
        catch (Exception ex)
        {
            return GetError($"Error retrieving task templates: {ex.Message}");
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTaskTemplateById(int id)
    {
        try
        {
            var result = await _taskTemplateService.GetTaskTemplateByIdAsync(id);
            return GetSuccess(result);
        }
        catch (ArgumentException ex)
        {
            return GetNotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return GetError($"Error retrieving task template: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateTaskTemplate([FromBody] TaskTemplateRequestDTO dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var result = await _taskTemplateService.CreateTaskTemplateAsync(dto);
            return SaveSuccess(result, "Task template created successfully");
        }
        catch (Exception ex)
        {
            return GetError($"Error creating task template: {ex.Message}");
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTaskTemplate(int id, [FromBody] TaskTemplateRequestDTO dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var result = await _taskTemplateService.UpdateTaskTemplateAsync(id, dto);
            return Success(result, "Task template updated successfully");
        }
        catch (ArgumentException ex)
        {
            return GetNotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return GetError($"Error updating task template: {ex.Message}");
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTaskTemplate(int id)
    {
        try
        {
            var result = await _taskTemplateService.DeleteTaskTemplateAsync(id);
            return Success(result, result ? "Task template deleted successfully" : "Task template not found");
        }
        catch (Exception ex)
        {
            return GetError($"Error deleting task template: {ex.Message}");
        }
    }
}
