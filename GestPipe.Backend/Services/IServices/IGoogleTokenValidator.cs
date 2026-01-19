using System.Threading.Tasks;
using Google.Apis.Auth;
namespace GestPipe.Backend.Services.IServices
{
    /// <summary>
    /// Abstraction để validate Google ID token.
    /// Cho phép mock trong unit test và dùng GoogleJsonWebSignature ở production.
    /// </summary>
    public interface IGoogleTokenValidator
    {
        Task<GoogleJsonWebSignature.Payload> ValidateAsync(string idToken);
    }
}


