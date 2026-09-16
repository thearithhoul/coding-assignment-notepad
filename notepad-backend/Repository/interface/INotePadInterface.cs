using Microsoft.AspNetCore.Mvc;
using notepad_backend.Entities;

namespace notepad_backend.Repsitory.Interface;

public interface INotePadInterface
{
    // CURD
    Task<(IEnumerable<NotePadEntity>, int)> GetNotePadsAsync(int userId, int page, int pagesize, string? search = null, int sorting = 0, IDictionary<string, object>? filers = null);

    Task<NotePadEntity> GetNotePadDetailAsync(int userId, int notepadId);
    Task<NotePadEntity> CreateNotePadAsync(NotePadEntity notepad);
    Task<NotePadEntity> UpdateNotePadAsync(NotePadEntity notepad);
    Task<NotePadEntity> SoftRemoveNotePadAsync(int userId, int notepadId);
}

