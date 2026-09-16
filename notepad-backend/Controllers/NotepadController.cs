
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using notepad_backend.Dto;
using notepad_backend.Entities;
using notepad_backend.Repsitory.Interface;

namespace notepad_backend.Controllers;

[ApiController]
[Route("api/v1/note")]
[Authorize]
public class NotePadController(INotePadInterface notePadRepo) : ControllerBase
{
    [HttpGet("list")]
    public async Task<ActionResult<NotedPadsPagedResponceDto>> GetNotedPadsList([FromQuery] NotedPadsRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        IDictionary<string, object>? filers;
        try
        {
            filers = ParseFiler(request.Filter);
        }
        catch (FormatException ex)
        {
            return BadRequest(ex.Message);
        }

        var (notepads, totalCount) = await notePadRepo.GetNotePadsAsync(
            userId.Value, request.Page, request.PageSize, request.Search, request.Sorting, filers);

        return Ok(new NotedPadsPagedResponceDto
        {
            Items = notepads.Select(ToListDto),
            TotalCount = totalCount,
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<NotedPadResponceDto>> GetNotedPadsDetail(int id)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            var notepad = await notePadRepo.GetNotePadDetailAsync(userId.Value, id);
            return Ok(ToDetailDto(notepad));
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    [HttpPost("create")]
    public async Task<ActionResult<NotedPadResponceDto>> CreateNewNotedPad([FromBody] NotedPadsCreateRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest("Title is required.");
        }

        var notepad = new NotePadEntity
        {
            UserId = userId.Value,
            Title = request.Title,
            SubTitle = request.SubTitle,
            IsPinned = request.IsPinned,
            Detail = new NotePedDetailEntity { Content = request.Detail?.Content ?? string.Empty },
        };

        var created = await notePadRepo.CreateNotePadAsync(notepad);

        return CreatedAtAction(nameof(GetNotedPadsDetail), new { id = created.Id }, ToDetailDto(created));
    }

    [HttpPost("update/{id:int}")]
    public async Task<ActionResult<NotedPadResponceDto>> UpdateNotedPad(int id, [FromBody] NotedPadsCreateRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var notepad = new NotePadEntity
        {
            Id = id,
            UserId = userId.Value,
            Title = request.Title,
            SubTitle = request.SubTitle,
            IsPinned = request.IsPinned,
            Detail = new NotePedDetailEntity { Content = request.Detail?.Content ?? string.Empty },
        };

        try
        {
            var updated = await notePadRepo.UpdateNotePadAsync(notepad);
            return Ok(ToDetailDto(updated));
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    [HttpGet("remove/{id:int}")]
    public async Task<ActionResult> RemoveNotedPad(int id)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            await notePadRepo.SoftRemoveNotePadAsync(userId.Value, id);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    private int? GetUserId()
    {
        var userIdClaim = User.FindFirst("user_id")?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }


    // Parses "ispin=true;isdelete=false" into { isPinned: true, isDeleted: false }.
    private static IDictionary<string, object>? ParseFiler(string? filer)
    {
        if (string.IsNullOrWhiteSpace(filer))
        {
            return null;
        }

        var result = new Dictionary<string, object>();

        foreach (var segment in filer.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var parts = segment.Split('=', 2);
            if (parts.Length != 2)
            {
                throw new FormatException($"Malformed filter segment '{segment}'. Expected 'key=value'.");
            }

            var key = parts[0].Trim();
            var value = parts[1].Trim();

            result[key] = value;
        }

        return result;
    }

    private static NotedPadListResponceDto ToListDto(NotePadEntity notepad) => new()
    {
        Id = notepad.Id,
        Title = notepad.Title,
        SubTitle = notepad.SubTitle,
        IsPinned = notepad.IsPinned,
        IsDeleted = notepad.IsDeleted,
        CreatedAt = notepad.CreatedAt,
        UpdatedAt = notepad.UpdatedAt,
    };

    private static NotedPadResponceDto ToDetailDto(NotePadEntity notepad) => new()
    {
        Id = notepad.Id,
        Title = notepad.Title,
        SubTitle = notepad.SubTitle,
        IsPinned = notepad.IsPinned,
        IsDeleted = notepad.IsDeleted,
        CreatedAt = notepad.CreatedAt,
        UpdatedAt = notepad.UpdatedAt,
        Detail = new NotedPadDetailResponceDto
        {
            Id = notepad.Detail?.Id ?? 0,
            Content = notepad.Detail?.Content ?? string.Empty,
        },
    };
}