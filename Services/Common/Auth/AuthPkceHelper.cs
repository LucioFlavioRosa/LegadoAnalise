using System.Security.Cryptography;
using System.Text;

namespace Peers.Moderno.Services.Common.Auth;

public static class AuthPkceHelper
{
    public static string GenerateCodeVerifier()
    {
        var bytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    public static string GenerateCodeChallenge(string codeVerifier)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(codeVerifier);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }

    public static string GenerateState()
    {
        return Guid.NewGuid().ToString();
    }

    public static bool ValidateState(string? receivedState, string? sessionState)
    {
        return !string.IsNullOrEmpty(receivedState) &&
               !string.IsNullOrEmpty(sessionState) &&
               receivedState == sessionState;
    }
}