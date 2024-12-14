using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace Kudo.Storage.Utils;

public static class HashGenerator
{
    public static string GetHashSequence(string password, string customSalt)
    {
        return Convert.ToBase64String(
            KeyDerivation.Pbkdf2(
                password,
                Encoding.UTF8.GetBytes($"Kudo.Security.Cryptography:{customSalt}"),
                KeyDerivationPrf.HMACSHA512,
                4096,
                72));
    }

    public static string GetMd5Hash(string input)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();

        var inputBytes = Encoding.ASCII.GetBytes(input);
        var hashBytes = md5.ComputeHash(inputBytes);

        return Convert.ToHexString(hashBytes);
    }
}