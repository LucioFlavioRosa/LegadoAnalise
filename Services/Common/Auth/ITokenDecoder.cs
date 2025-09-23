namespace Peers.Moderno.Services.Common.Auth;

public interface ITokenDecoder
{
    UserInfo DecodeIdToken(string idToken);
    T DecodeToken<T>(string token) where T : class;
    bool ValidateTokenStructure(string token);
}

public class UserInfo
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string PreferredUsername { get; set; } = string.Empty;
    public string GivenName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
}