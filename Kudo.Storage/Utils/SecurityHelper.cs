using System.Text.RegularExpressions;

namespace Kudo.Storage.Utils;

public static partial class SecurityHelper
{
    [GeneratedRegex("^[a-zA-Z0-9][a-zA-Z0-9_-]{3,32}$")]
    private static partial Regex LoginValidatorRegex();

    [GeneratedRegex("^(?=.*[a-zA-Zа-яА-ЯёЁ0-9])(?!.*\\s).{5,64}$")]
    private static partial Regex PasswordValidatorRegex();

    public static bool IsLoginValid(string login)
    {
        if (string.IsNullOrWhiteSpace(login))
        {
            return false;
        }

        var regex = LoginValidatorRegex();

        return regex.IsMatch(login);
    }

    public static bool IsPasswordValid(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        var regex = PasswordValidatorRegex();

        return regex.IsMatch(password);
    }

    public static string GeneratePasswordHash(string login, string password, DateTime createdDate)
    {
        return HashGenerator.GetHashSequence(
            password,
            GetCustomSalt(login, createdDate)
        );
    }

    private static string GetCustomSalt(string login, DateTime createdAt)
    {
        return $"{login.ToLower()}_at_{createdAt:yyyy-MM-dd HH:mm:ss}";
    }
}