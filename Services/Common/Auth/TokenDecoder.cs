using System.Text;
using Newtonsoft.Json;

namespace Peers.Moderno.Services.Common.Auth;

public class TokenDecoder : ITokenDecoder
{
    public UserInfo DecodeIdToken(string idToken)
    {
        if (string.IsNullOrEmpty(idToken))
            throw new ArgumentException("Token não pode ser nulo ou vazio", nameof(idToken));

        var parts = idToken.Split('.');
        if (parts.Length != 3)
            throw new ArgumentException("Token JWT inválido", nameof(idToken));

        var payload = parts[1];

        // Adicionar padding se necessário
        switch (payload.Length % 4)
        {
            case 2: payload += "=="; break;
            case 3: payload += "="; break;
        }

        var jsonBytes = Convert.FromBase64String(payload);
        var jsonString = Encoding.UTF8.GetString(jsonBytes);

        dynamic data = JsonConvert.DeserializeObject(jsonString) ?? throw new InvalidOperationException("Falha ao decodificar token");

        return new UserInfo
        {
            Email = data.preferred_username ?? data.email ?? data.upn ?? string.Empty,
            Name = data.name ?? $"{data.given_name} {data.family_name}" ?? string.Empty,
            Id = data.oid ?? data.sub ?? string.Empty,
            PreferredUsername = data.preferred_username ?? string.Empty,
            GivenName = data.given_name ?? string.Empty,
            FamilyName = data.family_name ?? string.Empty
        };
    }

    public T DecodeToken<T>(string token) where T : class
    {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentException("Token não pode ser nulo ou vazio", nameof(token));

        var parts = token.Split('.');
        if (parts.Length != 3)
            throw new ArgumentException("Token JWT inválido", nameof(token));

        var payload = parts[1];

        switch (payload.Length % 4)
        {
            case 2: payload += "=="; break;
            case 3: payload += "="; break;
        }

        var jsonBytes = Convert.FromBase64String(payload);
        var jsonString = Encoding.UTF8.GetString(jsonBytes);

        return JsonConvert.DeserializeObject<T>(jsonString) ?? throw new InvalidOperationException("Falha ao decodificar token");
    }

    public bool ValidateTokenStructure(string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        var parts = token.Split('.');
        return parts.Length == 3;
    }
}