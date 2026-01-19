using System.Threading.Tasks;
using GestPipe.Backend.Models.Setting;
using GestPipe.Backend.Services.IServices;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;

namespace GestPipe.Backend.Services
{
    public class GoogleTokenValidator : IGoogleTokenValidator
    {
        private readonly GoogleSettings _googleSettings;

        public GoogleTokenValidator(IOptions<GoogleSettings> googleSettings)
        {
            _googleSettings = googleSettings?.Value
                ?? throw new System.ArgumentNullException(nameof(googleSettings));
        }

        public async Task<GoogleJsonWebSignature.Payload> ValidateAsync(string idToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _googleSettings.ClientId }
            };

            // Nếu token invalid, method này sẽ ném exception (InvalidJwtException ...)
            return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        }
    }
}



