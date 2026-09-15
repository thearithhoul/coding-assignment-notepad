
using System.Data;
using Dapper;
using notepad_backend.Entities;
using notepad_backend.Repsitory.Interface;

namespace notepad_backend.Repsitory;

public class SessionRepsitory : ISessionInterface
{
    private IDbConnection _db;

    public SessionRepsitory(IDbConnection db)
    {
        _db = db;
    }


    public async Task<IEnumerable<SessionEntity>> GetSessionsByUserId(int userId)
    {
        try
        {
            var sql = """"
            SELECT * 
            FROM session
            WHERE user_id = @userid;
            """";

            var sessions = await _db.QueryAsync<SessionEntity>(sql, new { userid = userId });
            return sessions;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to get session with userid {userId}: {ex.Message}", ex);

        }
    }

    public async Task<SessionEntity?> GetSessionByRefreshToken(string refreshToken)
    {
        try
        {
            var sql = """"
            SELECT * 
            FROM session
            WHERE refresh_token = @refreshToken;
            """";
            var session = await _db.QueryFirstAsync<SessionEntity>(sql, new { refreshToken = refreshToken });

            return session;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to get Session by refreshToken: {ex.Message}", ex);
        }
    }



    public async Task<SessionEntity?> InsertSession(SessionEntity sessionEntity)
    {
        try
        {
            var sql = """
            INSERT INTO session
                (user_id, session_token, refresh_token, refresh_token_expiry, created_at)
            VALUES
                (@UserId, @SessionToken, @RefreshToken, @RefreshTokenExpiry, NOW())
            RETURNING *;
            """;

            var createdSession = await _db.QueryFirstAsync<SessionEntity>(sql, sessionEntity);

            return createdSession;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to insert session for user id {sessionEntity.UserId}: {ex.Message}", ex);
        }
    }


    public async Task RevokeSessionById(int sessionId)
    {
        try
        {
            var sql = """
            DELETE FROM session WHERE id = @sessionId;
            """;

            await _db.ExecuteAsync(sql, new { sessionId });
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to revoke session with id {sessionId}: {ex.Message}", ex);
        }
    }

    public async Task RevokeAllSessionsByUserId(int userId)
    {
        try
        {
            var sql = """
            DELETE FROM session WHERE user_id = @userId;
            """;

            await _db.ExecuteAsync(sql, new { userId });
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to revoke sessions for user id {userId}: {ex.Message}", ex);
        }
    }

}