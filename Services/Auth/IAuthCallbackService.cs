using System.Threading.Tasks;

namespace Services.Auth
{
    public interface IAuthCallbackService
    {
        Task<AuthResult> ProcessCallbackAsync(string code, string state, string sessionState, string codeVerifier);
        Task<UserInfo> GetUserInfoFromTokenAsync(string idToken);
    }

    public class AuthResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public UserInfo UserInfo { get; set; }
    }

    public class UserInfo
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public IDictionary<string, string> Claims { get; set; }
    }
}
