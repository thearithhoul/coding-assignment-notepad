using System.Security.Cryptography;

namespace notepad_backend.Func;

public class HashFunc
{
    private const int SaltSizeBytes = 16;
    private const int HashSizeBytes = 32;
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    public static string GenerateSalt()
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSizeBytes);
        return Convert.ToBase64String(salt);
    }

    public static string HashPassword(string password, string saltBase64)
    {
        byte[] salt = Convert.FromBase64String(saltBase64);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSizeBytes);
        return Convert.ToBase64String(hash);
    }

    public static bool VerifyPassword(string password, string saltBase64, string hashBase64)
    {
        string computedHash = HashPassword(password, saltBase64);
        byte[] computedHashBytes = Convert.FromBase64String(computedHash);
        byte[] expectedHashBytes = Convert.FromBase64String(hashBase64);

        return CryptographicOperations.FixedTimeEquals(computedHashBytes, expectedHashBytes);
    }
}
