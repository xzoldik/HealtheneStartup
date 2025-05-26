using Domain.Models;
using Microsoft.Extensions.Options;
using Twilio.Jwt.AccessToken;

namespace BusinessLogic.Services
{

    // using YourApp.Models; // For TwilioSettings

    public class TwilioVideoService
    {
        private readonly TwilioSettings _twilioSettings;

        public TwilioVideoService(IOptions<TwilioSettings> twilioSettings)
        {
            // Use IOptions to safely access configured settings
            _twilioSettings = twilioSettings.Value;
        }

        public string GenerateTwilioToken(string identity, string roomName)
        {
            // The identity is a unique identifier for the user in the video room (e.g., "user-123").
            // The roomName is the unique name of the video room (e.g., "session-45").

            var grant = new VideoGrant { Room = roomName };

            var token = new Token(
                _twilioSettings.AccountSid,
                _twilioSettings.ApiKey,
                _twilioSettings.ApiSecret,
                identity: identity,
                grants: new HashSet<IGrant> { grant }
            );

            return token.ToJwt();
        }
    }
}