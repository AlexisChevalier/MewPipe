using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

namespace MewPipe.Accounts.OpenID
{
    /// <summary>
    /// Extension methods for using <see cref="OpenIDAuthenticationMiddleware"/>
    /// </summary>
    public static class OpenIDAuthenticationExtensions
    {
        /// <summary>
        /// Authenticate users using an OpenID provider
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/> passed to the configuration method</param>
        /// <param name="options">Middleware configuration options</param>
        /// <returns>The updated <see cref="IAppBuilder"/></returns>
        public static IAppBuilder UseOpenIDAuthentication(this IAppBuilder app, OpenIDAuthenticationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            app.Use(typeof(OpenIDAuthenticationMiddleware), app, options);
            return app;
        }

        /// <summary>
        /// Authenticate users using an OpenID provider
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/> passed to the configuration method</param>
        /// <param name="providerUri">The uri of the OpenID provider</param>
        /// <param name="providerName">Name of the OpenID provider</param>
        /// <returns>The updated <see cref="IAppBuilder"/></returns>
        public static IAppBuilder UseOpenIDAuthentication(this IAppBuilder app, string providerUri, string providerName)
        {
            return UseOpenIDAuthentication(app, providerUri, providerName, false);
        }

        /// <summary>
        /// Authenticate users using an OpenID provider
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/> passed to the configuration method</param>
        /// <param name="providerUri">The uri of the OpenID provider</param>
        /// <param name="providerName">Name of the OpenID provider</param>
        /// <param name="uriIsProviderLoginUri">True if the specified uri is the provider login uri and not the provider discovery uri</param>
        /// <returns>The updated <see cref="IAppBuilder"/></returns>
        public static IAppBuilder UseOpenIDAuthentication(this IAppBuilder app, string providerUri, string providerName, bool uriIsProviderLoginUri)
        {
            var authOptions = new OpenIDAuthenticationOptions
            {
                Caption = providerName,
                AuthenticationType = providerName,
                CallbackPath = new PathString("/signin-openid" + providerName.ToLowerInvariant())
            };
            if (uriIsProviderLoginUri)
            {
                authOptions.ProviderLoginUri = providerUri;
            }
            else
            {

                authOptions.ProviderDiscoveryUri = providerUri;
            }
            return UseOpenIDAuthentication(app, authOptions);
        }


        /// <summary>
        /// Gets the form body request parameters.
        /// </summary>
        /// <param name="request">Owin Request.</param>
        /// <returns>Dictionary of form body parameters.</returns>
        public static async Task<Dictionary<string, string>> GetBodyParameters(this IOwinRequest request)
        {
            var dictionary = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

            var formCollectionTask = await request.ReadFormAsync();

            foreach (var pair in formCollectionTask)
            {
                var value = GetJoinedValue(pair.Value);

                dictionary.Add(pair.Key, value);
            }

            return dictionary;
        }

        private static string GetJoinedValue(string[] value)
        {
            if (value != null)
                return string.Join(",", value);

            return null;
        }

        public static void Merge<TKey, TValue>(this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
        {
            if (second == null || first == null) return;
            foreach (var item in second)
                if (!first.ContainsKey(item.Key))
                    first.Add(item.Key, item.Value);
        }
    }
}
