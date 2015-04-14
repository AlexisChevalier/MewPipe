using System.Threading.Tasks;
using MewPipe.Accounts.OpenID.Infrastructure;
using Microsoft.Owin.Security;

namespace MewPipe.Accounts.OpenID
{
    public interface IOpenIDProtocolExtension
    {

        /// <summary>
        /// Adds the required information in the authorization endpoint URL.
        /// </summary>
        Task OnChallengeAsync(AuthenticationResponseChallenge challenge, OpenIDAuthorizationEndpointInfo endpoint);

        /// <summary>
        /// Performs additional authentication response message validations.
        /// </summary>
        Task<bool> OnValidateMessageAsync(Message message);

        /// <summary>
        /// Extracts the data form the authentication response message and returns them.
        /// </summary>
        Task<object> OnExtractResultsAsync(System.Security.Claims.ClaimsIdentity identity, string claimedId, Message message);
    }
}