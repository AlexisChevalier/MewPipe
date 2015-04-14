using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;

namespace MewPipe.Accounts.OpenID.Provider
{
    /// <summary>
    /// Provides context information to middleware providers.
    /// </summary>
    public class OpenIDReturnEndpointContext : ReturnEndpointContext
    {
        /// <summary>
        /// Initializes a <see cref="OpenIDReturnEndpointContext"/>
        /// </summary>
        /// <param name="context">OWIN environment</param>
        /// <param name="ticket">The authentication ticket</param>
        public OpenIDReturnEndpointContext(IOwinContext context, AuthenticationTicket ticket)
            : base(context, ticket)
        { }
    }
}
