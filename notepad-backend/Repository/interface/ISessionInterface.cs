

using notepad_backend.Entities;

namespace notepad_backend.Repsitory.Interface;

public interface ISessionInterface
{
    // Session
    Task<SessionEntity?> InsertSession(SessionEntity sessionEntity);
    Task<SessionEntity?> GetSessionByRefreshToken(string refreshToken);
    Task<IEnumerable<SessionEntity>> GetSessionsByUserId(int userId);
    Task RevokeSessionById(int sessionId);
    Task RevokeAllSessionsByUserId(int userId);
}