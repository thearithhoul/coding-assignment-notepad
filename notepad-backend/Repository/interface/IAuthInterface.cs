
using notepad_backend.Entities;

namespace notepad_backend.Repsitory.Interface;

public interface IAuthInterface
{

    // User CURD 
    Task<(IEnumerable<AppUserEntity>, int)> GetUsers(int page, int pagesize, string? search = null);
    Task<AppUserEntity?> GetuserbyId(int id);
    Task<AppUserEntity?> GetUserByUsername(string username);
    Task<AppUserEntity?> CreateUser(AppUserEntity users);
    Task<AppUserEntity?> UpdateUser(AppUserEntity users);
    Task<AppUserEntity?> SoftRemoveUser(int id);

}