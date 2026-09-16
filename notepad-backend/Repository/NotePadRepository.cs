using System.Data;
using Dapper;
using notepad_backend.Entities;
using notepad_backend.Repsitory.Interface;

namespace notepad_backend.Repsitory;

public class NotePadRepository : INotePadInterface
{
    private static readonly IReadOnlyDictionary<string, string> FilterColumnMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["isPinned"] = "is_pinned",
        ["isArchived"] = "is_archived",
        ["isDeleted"] = "is_deleted",
    };

    private IDbConnection _db;
    public NotePadRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<(IEnumerable<NotePadEntity>, int)> GetNotePadsAsync(int userId, int page, int pagesize, string? search = null, int sorting = 0, IDictionary<string, object>? filers = null)
    {
        try
        {
            var whereClauses = new List<string>
            {
                "user_id = @userId",
                "(@search IS NULL OR title ILIKE '%' || @search || '%')",
            };

            var param = new DynamicParameters();
            param.Add("userId", userId);
            param.Add("page", page);
            param.Add("pageSize", pagesize);
            param.Add("search", search, DbType.String);

            if (filers is not null)
            {
                foreach (var (key, value) in filers)
                {
                    if (!FilterColumnMap.TryGetValue(key, out var column))
                    {
                        throw new ArgumentException($"Unsupported filter key '{key}'.", nameof(filers));
                    }

                    var paramName = $"filter_{column}";
                    whereClauses.Add($"{column} = @{paramName}");
                    param.Add(paramName, value);
                }
            }

            var where = string.Join(" AND ", whereClauses);
            var orderBy = sorting switch
            {
                1 => "created_at DESC",
                2 => "title ASC",
                _ => "updated_at DESC",
            };

            var sql = $"""
            SELECT COUNT(*)
            FROM noted_pads
            WHERE {where};

            SELECT *
            FROM noted_pads
            WHERE {where}
            ORDER BY {orderBy}
            LIMIT @pageSize OFFSET (@page - 1) * @pageSize;
            """;

            using var multi = await _db.QueryMultipleAsync(sql, param);

            var totalCount = await multi.ReadSingleAsync<int>();
            var notepads = (await multi.ReadAsync<NotePadEntity>()).ToList();

            return (notepads, totalCount);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to get notepads for user {userId} (page {page}, pageSize {pagesize}, search '{search}'): {ex.Message}", ex);
        }
    }

    public async Task<NotePadEntity> GetNotePadDetailAsync(int userId, int notepadId)
    {
        try
        {
            var sql = """
            SELECT * FROM noted_pads WHERE id = @notepadId AND user_id = @userId;

            SELECT * FROM noted_pad_detail WHERE note_id = @notepadId;
            """;

            using var multi = await _db.QueryMultipleAsync(sql, new { notepadId, userId });

            var notepad = await multi.ReadFirstOrDefaultAsync<NotePadEntity>();
            if (notepad is null)
            {
                throw new InvalidOperationException($"NotePad with id {notepadId} doesn't exist.");
            }

            notepad.Detail = await multi.ReadFirstOrDefaultAsync<NotePedDetailEntity>();

            return notepad;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to get notepad detail for id {notepadId}: {ex.Message}", ex);
        }
    }



    public async Task<NotePadEntity> CreateNotePadAsync(NotePadEntity notepad)
    {
        try
        {
            var sql1 = """
            INSERT INTO noted_pads
                (user_id, title, sub_title, is_pinned, is_deleted, created_at, updated_at)
            VALUES
                (@UserId, @Title, @SubTitle, @IsPinned, FALSE, NOW(), NOW())
            RETURNING *;
            """;

            var createdNote = await _db.QueryFirstAsync<NotePadEntity>(sql1, notepad);

            var sql2 = """
            INSERT INTO noted_pad_detail (note_id, content)
            VALUES (@NoteId, @Content)
            RETURNING *;
            """;

            createdNote.Detail = await _db.QueryFirstAsync<NotePedDetailEntity>(sql2, new
            {
                NoteId = createdNote.Id,
                Content = notepad.Detail?.Content ?? string.Empty,
            });

            return createdNote;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create notepad '{notepad.Title}': {ex.Message}", ex);
        }
    }

    public async Task<NotePadEntity> UpdateNotePadAsync(NotePadEntity notepad)
    {
        try
        {
            var sql1 = """
            SELECT * FROM noted_pads WHERE id = @Id AND user_id = @UserId;
            """;

            var existing = await _db.QueryFirstOrDefaultAsync<NotePadEntity>(sql1, new { notepad.Id, notepad.UserId });
            if (existing is null)
            {
                throw new InvalidOperationException($"NotePad with id {notepad.Id} doesn't exist.");
            }

            var setClauses = new List<string> { "is_pinned = @IsPinned", "updated_at = NOW()" };
            var param = new DynamicParameters();
            param.Add("Id", notepad.Id);
            param.Add("UserId", notepad.UserId);
            param.Add("IsPinned", notepad.IsPinned);

            if (notepad.Title is not null)
            {
                setClauses.Add("title = @Title");
                param.Add("Title", notepad.Title);
            }
            if (notepad.SubTitle is not null)
            {
                setClauses.Add("sub_title = @SubTitle");
                param.Add("SubTitle", notepad.SubTitle);
            }

            var sql2 = $"""
            UPDATE noted_pads
            SET {string.Join(", ", setClauses)}
            WHERE id = @Id AND user_id = @UserId
            RETURNING *;
            """;

            var updatedNote = await _db.QueryFirstAsync<NotePadEntity>(sql2, param);

            if (notepad.Detail?.Content is not null)
            {
                var sql3 = """
                UPDATE noted_pad_detail
                SET content = @Content
                WHERE note_id = @Id
                RETURNING *;
                """;

                updatedNote.Detail = await _db.QueryFirstOrDefaultAsync<NotePedDetailEntity>(sql3, new { notepad.Id, notepad.Detail.Content });
            }

            return updatedNote;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to update notepad with id {notepad.Id}: {ex.Message}", ex);
        }
    }

    public async Task<NotePadEntity> SoftRemoveNotePadAsync(int userId, int notepadId)
    {
        try
        {
            var sql1 = """
            SELECT * FROM noted_pads WHERE id = @notepadId AND user_id = @userId;
            """;

            var existing = await _db.QueryFirstOrDefaultAsync<NotePadEntity>(sql1, new { notepadId, userId });
            if (existing is null)
            {
                throw new InvalidOperationException($"NotePad with id {notepadId} doesn't exist.");
            }

            var sql2 = """
            UPDATE noted_pads
            SET is_deleted = TRUE, updated_at = NOW()
            WHERE id = @notepadId AND user_id = @userId
            RETURNING *;
            """;

            var removedNote = await _db.QueryFirstAsync<NotePadEntity>(sql2, new { notepadId, userId });

            return removedNote;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to soft-remove notepad with id {notepadId}: {ex.Message}", ex);
        }
    }


}