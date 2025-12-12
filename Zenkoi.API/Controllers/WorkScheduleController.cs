using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.WorkScheduleDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WorkScheduleController : BaseAPIController
{
    private readonly IWorkScheduleService _workScheduleService;

    public WorkScheduleController(IWorkScheduleService workScheduleService)
    {
        _workScheduleService = workScheduleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllWorkSchedules(
        [FromQuery] WorkScheduleFilterRequestDTO? filter)
    {
        try
        {
            var result = await _workScheduleService.GetAllWorkSchedulesAsync(
                filter ?? new WorkScheduleFilterRequestDTO());
            return Success(result, "Work schedules retrieved successfully");
        }
        catch (Exception ex)
        {
            return GetError($"Error retrieving work schedules: {ex.Message}");
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetWorkScheduleById(int id)
    {
        try
        {
            var result = await _workScheduleService.GetWorkScheduleByIdAsync(id);
            return GetSuccess(result);
        }
        catch (ArgumentException ex)
        {
            return GetNotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return GetError($"Error retrieving work schedule: {ex.Message}");
        }
    }

    [HttpGet("staff/{staffId:int}")]
    public async Task<IActionResult> GetWorkSchedulesByStaffId(
        int staffId,
        [FromQuery] WorkScheduleFilterRequestDTO? filter,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _workScheduleService.GetWorkSchedulesByStaffIdAsync(
                staffId,
                filter ?? new WorkScheduleFilterRequestDTO(),
                pageIndex,
                pageSize);
            return GetPagedSuccess(result);
        }
        catch (Exception ex)
        {
            return GetError($"Error retrieving work schedules for staff: {ex.Message}");
        }
    }

    [HttpGet("pond/{pondId:int}")]
    public async Task<IActionResult> GetWorkSchedulesByPondId(
        int pondId,
        [FromQuery] WorkScheduleFilterRequestDTO? filter,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _workScheduleService.GetWorkSchedulesByPondIdAsync(
                pondId,
                filter ?? new WorkScheduleFilterRequestDTO(),
                pageIndex,
                pageSize);
            return GetPagedSuccess(result);
        }
        catch (Exception ex)
        {
            return GetError($"Error retrieving work schedules for pond: {ex.Message}");
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetWorkSchedulesByUserId(
        [FromQuery] WorkScheduleFilterRequestDTO? filter)
    {
        try
        {
            var result = await _workScheduleService.GetWorkSchedulesByUserIdAsync(
                UserId,
                filter ?? new WorkScheduleFilterRequestDTO());
            return Success(result, "Work schedules retrieved successfully");
        }
        catch (Exception ex)
        {
            return GetError($"Error retrieving work schedules for user: {ex.Message}");
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateWorkSchedule([FromBody] WorkScheduleRequestDTO dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var result = await _workScheduleService.CreateWorkScheduleAsync(dto, UserId);
            return SaveSuccess(result, "Work schedule created successfully");
        }
        catch (ArgumentException ex)
        {
            return GetError(ex.Message);
        }
        catch (Exception ex)
        {
            return GetError($"Error creating work schedule: {ex.Message}");
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateWorkSchedule(int id, [FromBody] WorkScheduleRequestDTO dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var result = await _workScheduleService.UpdateWorkScheduleAsync(id, dto);
            return Success(result, "Work schedule updated successfully");
        }
        catch (ArgumentException ex)
        {
            return GetNotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return GetError($"Error updating work schedule: {ex.Message}");
        }
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateWorkScheduleStatus(int id, [FromBody] UpdateWorkScheduleStatusDTO dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var result = await _workScheduleService.UpdateWorkScheduleStatusAsync(id, dto);
            return Success(result, "Work schedule status updated successfully");
        }
        catch (ArgumentException ex)
        {
            return GetNotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return GetError($"Error updating work schedule status: {ex.Message}");
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteWorkSchedule(int id)
    {
        try
        {
            var result = await _workScheduleService.DeleteWorkScheduleAsync(id);
            if (result)
                return SaveSuccess(new { message = "Work schedule deleted successfully" }, "Work schedule deleted successfully");
            return GetNotFound("Work schedule not found");
        }
        catch (Exception ex)
        {
            return GetError($"Error deleting work schedule: {ex.Message}");
        }
    }

    [HttpPost("bulk-assign")]
    public async Task<IActionResult> BulkAssignStaff([FromBody] BulkAssignStaffDTO dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var result = await _workScheduleService.BulkAssignStaffAsync(dto);

            if (result.FailedAssignments > 0)
            {
                return Success(result, $"Bulk assignment completed with {result.SuccessfulAssignments} successful and {result.FailedAssignments} failed assignments");
            }

            return SaveSuccess(result, $"Successfully assigned {result.SuccessfulAssignments} staff to work schedules");
        }
        catch (ArgumentException ex)
        {
            return GetError(ex.Message);
        }
        catch (Exception ex)
        {
            return GetError($"Error during bulk assignment: {ex.Message}");
        }
    }

    [HttpPost("{id:int}/complete-my-assignment")]
    [Authorize]
    public async Task<IActionResult> CompleteMyAssignment(int id, [FromBody] CompleteStaffAssignmentDTO dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var result = await _workScheduleService.CompleteStaffAssignmentAsync(id, UserId, dto);
            return SaveSuccess(result, "Assignment marked as completed successfully");
        }
        catch (ArgumentException ex)
        {
            return GetNotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return GetError(ex.Message);
        }
        catch (Exception ex)
        {
            return GetError($"Error completing assignment: {ex.Message}");
        }
    }

    [HttpGet("my-assignments")]
    [Authorize]
    public async Task<IActionResult> GetMyAssignments([FromQuery] WorkScheduleFilterRequestDTO? filter)
    {
        try
        {
            var result = await _workScheduleService.GetStaffAssignmentsAsync(
                UserId,
                filter ?? new WorkScheduleFilterRequestDTO());
            return Success(result, "Your assignments retrieved successfully");
        }
        catch (Exception ex)
        {
            return GetError($"Error retrieving your assignments: {ex.Message}");
        }
    }
}
