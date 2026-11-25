using HerosPath.Requests;
using HerosPath.Requests;

namespace HerosPath.Interfaces
{
    public interface IHerosPathService
    {
        Task<object> Register(RegisterRequest request);
        Task<object> Login(LoginRequest request);
        Task<object> Logout(string token);
        Task<object> GetUser(string token);
        Task<object> GetLevels(string token);
        Task<object> GetProgress(string token);
        Task<object> SaveProgress(SaveProgressRequest request);
    }
}