using System.Data;
using Dapper;
using notepad_backend.Entities;
using notepad_backend.Repsitory.Interface;

namespace notepad_backend.Repsitory;

public class AuthRepository : IAuthInterface
{
    private IDbConnection _db;
    public AuthRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<AppUserEntity?> GetuserbyId(int id)
    {
        try
        {
            var sql = """"
            SELECT * FROM app_user WHERE id = @id
            """";
            var user = await _db.QueryFirstAsync<AppUserEntity>(sql, new { id });

            return user;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to get user with id {id}: {ex.Message}", ex);
        }
    }

    public async Task<AppUserEntity?> GetUserByUsername(string username)
    {
        try
        {
            var sql = """"
            SELECT * FROM app_user WHERE user_name = @username
            """";
            var user = await _db.QueryFirstOrDefaultAsync<AppUserEntity>(sql, new { username });

            return user;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to get user by username '{username}': {ex.Message}", ex);
        }
    }

    public async Task<(IEnumerable<AppUserEntity>, int)> GetUsers(int page, int pagesize, string? search = null)
    {
        try
        {
            var sql = """"
        SELECT COUNT(*)
        FROM app_user
        WHERE (@search IS NULL OR user_name LIKE '%' || @search || '%');

        SELECT *
        FROM app_user
        WHERE (@search IS NULL OR user_name LIKE '%' || @search || '%')
        ORDER BY id
        LIMIT @pageSize OFFSET (@page - 1) * @pageSize;
        """";
            var param = new DynamicParameters();
            param.Add("page", page);
            param.Add("pageSize", pagesize);

            param.Add("search", search);

            using var multi = await _db.QueryMultipleAsync(sql, param);

            int totalCount = await multi.ReadSingleAsync<int>();
            var users = (await multi.ReadAsync<AppUserEntity>()).ToList();

            return (users, totalCount);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to get users (page {page}, pageSize {pagesize}, search '{search}'): {ex.Message}", ex);
        }
    }

    public async Task<AppUserEntity?> CreateUser(AppUserEntity users)
    {
        try
        {
            var sql1 = """"
                SELECT * FROM app_user WHERE user_name = @username;
            """";

            var existingUser = await _db.QueryFirstOrDefaultAsync<AppUserEntity>(sql1, new { username = users.Username });

            if (existingUser != null)
            {
                throw new InvalidOperationException($"A user with username '{users.Username}' already exists.");
            }

            var sql2 = """
            INSERT INTO app_user
                (user_name, email, password_hash, password_salt, first_name, last_name, phone_number,
                 is_google_sign_in, is_email_verified, is_active, failed_attempts, created_at, updated_at)
            VALUES
                (@Username, @Email, @PasswordHash, @PasswordSalt, @FirstName, @LastName, @PhoneNumber,
                 @IsGoogleSignIn, @IsEmailVerified, @IsActive, 0, NOW(), NOW())
            RETURNING *;
            """;

            var createdUser = await _db.QueryFirstAsync<AppUserEntity>(sql2, users);

            return createdUser;
        }

        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create user '{users.Username}': {ex.Message}", ex);
        }
    }


    public async Task<AppUserEntity?> UpdateUser(AppUserEntity users)
    {
        try
        {
            var sql1 = """"
            SELECT * FROM app_user WHERE id = @id;
        """";

            var existingUser = await _db.QueryFirstOrDefaultAsync<AppUserEntity>(sql1, new { id = users.Id });

            if (existingUser is null)
            {
                throw new InvalidOperationException($"User with id {users.Id} doesn't exist.");
            }

            var setClauses = new List<string>();
            var param = new DynamicParameters();
            param.Add("id", users.Id);

            if (users.Username is not null)
            {
                setClauses.Add("user_name = @Username");
                param.Add("Username", users.Username);
            }
            if (users.Email is not null)
            {
                setClauses.Add("email = @Email");
                param.Add("Email", users.Email);
            }
            if (users.PasswordHash is not null)
            {
                setClauses.Add("password_hash = @PasswordHash");
                param.Add("PasswordHash", users.PasswordHash);
            }
            if (users.PasswordSalt is not null)
            {
                setClauses.Add("password_salt = @PasswordSalt");
                param.Add("PasswordSalt", users.PasswordSalt);
            }
            if (users.FirstName is not null)
            {
                setClauses.Add("first_name = @FirstName");
                param.Add("FirstName", users.FirstName);
            }
            if (users.LastName is not null)
            {
                setClauses.Add("last_name = @LastName");
                param.Add("LastName", users.LastName);
            }
            if (users.PhoneNumber is not null)
            {
                setClauses.Add("phone_number = @PhoneNumber");
                param.Add("PhoneNumber", users.PhoneNumber);
            }

            if (setClauses.Count == 0)
            {
                return existingUser;
            }

            setClauses.Add("updated_at = NOW()");

            var sql2 = $"""
        UPDATE app_user
        SET {string.Join(", ", setClauses)}
        WHERE id = @id
        RETURNING *;
        """;

            var updatedUser = await _db.QueryFirstAsync<AppUserEntity>(sql2, param);

            return updatedUser;
        }

        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to update user with id {users.Id}: {ex.Message}", ex);
        }
    }

    public async Task<AppUserEntity?> SoftRemoveUser(int id)
    {
        try
        {
            var sql1 = """"
                SELECT * FROM app_user WHERE id = @id;
            """";

            var existingUser = await _db.QueryFirstOrDefaultAsync<AppUserEntity>(sql1, new { id });

            if (existingUser is null)
            {
                throw new InvalidOperationException($"User with id {id} doesn't exist.");
            }

            var sql2 = """
            UPDATE app_user
            SET
            is_active = FALSE, deleted_at = NOW()
            WHERE id = @id
            RETURNING *;
            """;
            var deletedUser = await _db.QueryFirstAsync<AppUserEntity>(sql2, new { id });

            return deletedUser;
        }

        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to soft-remove user with id {id}: {ex.Message}", ex);
        }
    }


}