using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs;
using Zenkoi.BLL.DTOs.Response;
using Zenkoi.BLL.DTOs.WeeklyScheduleTemplateDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WeeklyScheduleTemplateController : ControllerBase
{
    private readonly IWeeklyScheduleTemplateService _templateService;

    public WeeklyScheduleTemplateController(IWeeklyScheduleTemplateService templateService)
    {
        _templateService = templateService;
    }

    [HttpPost]
    public async Task<ActionResult<ResponseApiDTO>> CreateTemplate([FromBody] WeeklyScheduleTemplateRequestDTO dto)
    {
        try
        {
            var result = await _templateService.CreateTemplateAsync(dto);
            return Ok(new ResponseApiDTO
            {
                IsSuccess = true,
                Message = "Weekly schedule template created successfully",
                Result = result
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ResponseApiDTO
            {
                IsSuccess = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseApiDTO
            {
                IsSuccess = false,
                Message = $"An error occurred: {ex.Message}"
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ResponseApiDTO>> GetTemplateById(int id)
    {
        try
        {
            var result = await _templateService.GetTemplateByIdAsync(id);
            return Ok(new ResponseApiDTO
            {
                IsSuccess = true,
                Message = "Weekly schedule template retrieved successfully",
                Result = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ResponseApiDTO
            {
                IsSuccess = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseApiDTO
            {
                IsSuccess = false,
                Message = $"An error occurred: {ex.Message}"
            });
        }
    }

    [HttpGet]
    public async Task<ActionResult<ResponseApiDTO>> GetAllTemplates()
    {
        try
        {
            var result = await _templateService.GetAllTemplatesAsync();
            return Ok(new ResponseApiDTO
            {
                IsSuccess = true,
                Message = "Weekly schedule templates retrieved successfully",
                Result = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseApiDTO
            {
                IsSuccess = false,
                Message = $"An error occurred: {ex.Message}"
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ResponseApiDTO>> UpdateTemplate(int id, [FromBody] WeeklyScheduleTemplateRequestDTO dto)
    {
        try
        {
            var result = await _templateService.UpdateTemplateAsync(id, dto);
            return Ok(new ResponseApiDTO
            {
                IsSuccess = true,
                Message = "Weekly schedule template updated successfully",
                Result = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ResponseApiDTO
            {
                IsSuccess = false,
                Message = ex.Message
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ResponseApiDTO
            {
                IsSuccess = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseApiDTO
            {
                IsSuccess = false,
                Message = $"An error occurred: {ex.Message}"
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ResponseApiDTO>> DeleteTemplate(int id)
    {
        try
        {
            await _templateService.DeleteTemplateAsync(id);
            return Ok(new ResponseApiDTO
            {
                IsSuccess = true,
                Message = "Weekly schedule template deleted successfully"
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ResponseApiDTO
            {
                IsSuccess = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseApiDTO
            {
                IsSuccess = false,
                Message = $"An error occurred: {ex.Message}"
            });
        }
    }

    [HttpPost("generate")]
    public async Task<ActionResult<ResponseApiDTO>> GenerateSchedules([FromBody] GenerateScheduleRequestDTO request)
    {
        try
        {
            var result = await _templateService.GenerateWorkSchedulesFromTemplateAsync(request);
            return Ok(new ResponseApiDTO
            {
                IsSuccess = true,
                Message = $"{result.Count} work schedules generated successfully",
                Result = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ResponseApiDTO
            {
                IsSuccess = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseApiDTO
            {
                IsSuccess = false,
                Message = $"An error occurred: {ex.Message}"
            });
        }
    }
}
